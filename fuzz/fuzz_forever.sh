#!/usr/bin/env bash
#
# Persistent fuzzing runner for neo-devpack-dotnet.
# Runs all .NET fuzz targets in parallel and restarts them after crashes
# so the campaign can run for days or weeks.

set -euo pipefail

FUZZ_ROOT="$(cd "$(dirname "$0")" && pwd)"
PROJECT="$FUZZ_ROOT/Neo.DevPack.Fuzz/Neo.DevPack.Fuzz.csproj"
HARNESS_DLL="$FUZZ_ROOT/Neo.DevPack.Fuzz/bin/Release/net10.0/Neo.DevPack.Fuzz.dll"
LOGDIR="$FUZZ_ROOT/logs"
PIDDIR="$FUZZ_ROOT/pids"
RUNNER="$FUZZ_ROOT/run_target_forever.sh"
LAUNCHER_PID_FILE="$PIDDIR/launcher.pid"
ACTIVE_TARGETS_FILE="$PIDDIR/targets.txt"

ALL_TARGETS=(
    fuzz_compile
    fuzz_structured_compile
    fuzz_template_projects
    fuzz_differential
    fuzz_devpack_runtime
)

if [ "$#" -eq 0 ]; then
    TARGET_SPECS=("${ALL_TARGETS[@]}")
else
    TARGET_SPECS=("$@")
fi

mkdir -p "$LOGDIR" "$PIDDIR"
PIDS=()
STOPPING=0
declare -A SEEN_RUN_IDS=()

is_known_target() {
    local target="$1"

    for known_target in "${ALL_TARGETS[@]}"; do
        if [ "$known_target" = "$target" ]; then
            return 0
        fi
    done

    return 1
}

parse_target_spec() {
    local spec="$1"
    local target instance run_id

    if [[ "$spec" == *"@"* ]]; then
        target="${spec%@*}"
        instance="${spec#*@}"
        if [ -z "$target" ] || [ -z "$instance" ]; then
            echo "Invalid target spec '$spec'. Use <target> or <target>@<instance>." >&2
            return 1
        fi
        run_id="$target@$instance"
    else
        target="$spec"
        run_id="$spec"
    fi

    if ! is_known_target "$target"; then
        echo "Unknown fuzz target '$target' in spec '$spec'." >&2
        return 1
    fi

    printf '%s\t%s\n' "$target" "$run_id"
}

cleanup() {
    STOPPING=1

    for pid in "${PIDS[@]}"; do
        kill "$pid" 2>/dev/null || true
    done

    for pid in "${PIDS[@]}"; do
        wait "$pid" 2>/dev/null || true
    done

    rm -f "$FUZZ_ROOT/fuzz.pids" "$LAUNCHER_PID_FILE" "$ACTIVE_TARGETS_FILE"
    find "$PIDDIR" -maxdepth 1 -type f -name '*.pid' -delete 2>/dev/null || true
}

trap cleanup EXIT
trap 'exit 0' INT TERM

if [ ! -x "$RUNNER" ]; then
    echo "Missing target runner script: $RUNNER" >&2
    exit 1
fi

if [ -f "$LAUNCHER_PID_FILE" ]; then
    EXISTING_LAUNCHER="$(cat "$LAUNCHER_PID_FILE")"
    if [ -n "$EXISTING_LAUNCHER" ] && kill -0 "$EXISTING_LAUNCHER" 2>/dev/null; then
        echo "A fuzz launcher is already running with PID $EXISTING_LAUNCHER." >&2
        exit 1
    fi

    rm -f "$LAUNCHER_PID_FILE"
fi

echo "=== neo-devpack-dotnet persistent fuzzer ==="
echo "Targets: ${TARGET_SPECS[*]}"
echo "Logs:    $LOGDIR/"
echo "Started: $(date)"
echo ""

if [ ! -f "$HARNESS_DLL" ]; then
    echo "Release harness not found. Building once..."
    dotnet build "$PROJECT" -c Release >/dev/null
fi

if [ ! -f "$HARNESS_DLL" ]; then
    echo "Missing harness assembly after build: $HARNESS_DLL" >&2
    exit 1
fi

printf '%s\n' "$$" > "$LAUNCHER_PID_FILE"
: > "$ACTIVE_TARGETS_FILE"

for target_spec in "${TARGET_SPECS[@]}"; do
    IFS=$'\t' read -r target run_id <<< "$(parse_target_spec "$target_spec")"

    if [ -n "${SEEN_RUN_IDS[$run_id]:-}" ]; then
        echo "Duplicate fuzz instance '$run_id'." >&2
        exit 1
    fi
    SEEN_RUN_IDS["$run_id"]=1

    CORPUS="$FUZZ_ROOT/corpus/$run_id"
    ARTIFACTS="$FUZZ_ROOT/artifacts/$run_id"
    LOGFILE="$LOGDIR/${run_id}.log"
    PIDFILE="$PIDDIR/${run_id}.pid"

    mkdir -p "$CORPUS" "$ARTIFACTS"

    if [ -f "$PIDFILE" ]; then
        EXISTING_PID="$(cat "$PIDFILE")"
        if [ -n "$EXISTING_PID" ] && kill -0 "$EXISTING_PID" 2>/dev/null; then
            echo "Target loop '$run_id' is already running with PID $EXISTING_PID." >&2
            exit 1
        fi

        rm -f "$PIDFILE"
    fi

    printf '%s\t%s\n' "$run_id" "$target" >> "$ACTIVE_TARGETS_FILE"

    echo "[$run_id] Starting persistent fuzz loop for $target..."
    "$RUNNER" \
        "$target" \
        "$HARNESS_DLL" \
        "$CORPUS" \
        "$ARTIFACTS" \
        "$FUZZ_ROOT/seeds" \
        "$FUZZ_ROOT/dotnet.dict" \
        "$PIDFILE" >> "$LOGFILE" 2>&1 &

    PIDS+=("$!")
    echo "[$run_id] PID=${PIDS[-1]} running in background"
done

echo ""
echo "Launcher PID: $$"
echo "All targets launched. PIDs:"
printf '%s\n' "${PIDS[@]}"
echo ""
echo "To monitor: tail -f $LOGDIR/<target>.log"
echo "To stop:    ./fuzz_stop.sh"
echo "To check:   ./fuzz_status.sh"

printf '%s\n' "${PIDS[@]}" > "$FUZZ_ROOT/fuzz.pids"
echo "PIDs saved to $FUZZ_ROOT/fuzz.pids"

while true; do
    set +e
    wait -n "${PIDS[@]}"
    STATUS=$?
    set -e

    if [ "$STOPPING" -eq 1 ]; then
        exit 0
    fi

    echo "A target loop exited unexpectedly with status $STATUS. Stopping the campaign." >&2
    exit "$STATUS"
done
