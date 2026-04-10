#!/usr/bin/env bash
#
# Quick status check for running fuzz targets.

set -euo pipefail

FUZZ_ROOT="$(cd "$(dirname "$0")" && pwd)"
LOGDIR="$FUZZ_ROOT/logs"
PIDDIR="$FUZZ_ROOT/pids"
LAUNCHER_PID_FILE="$PIDDIR/launcher.pid"
ACTIVE_TARGETS_FILE="$PIDDIR/targets.txt"
DEFAULT_TARGETS=(
    fuzz_compile
    fuzz_structured_compile
    fuzz_template_projects
    fuzz_differential
    fuzz_devpack_runtime
)

declare -a TARGETS=()
declare -A TARGET_LABELS=()

load_targets() {
    if [ -s "$ACTIVE_TARGETS_FILE" ]; then
        while IFS=$'\t' read -r run_id target_name; do
            [ -n "$run_id" ] || continue
            TARGETS+=("$run_id")
            TARGET_LABELS["$run_id"]="$target_name"
        done < "$ACTIVE_TARGETS_FILE"
        return
    fi

    for target in "${DEFAULT_TARGETS[@]}"; do
        TARGETS+=("$target")
        TARGET_LABELS["$target"]="$target"
    done
}

find_worker_pid() {
    local loop_pid="$1"
    local supervisor_pid worker_pid

    if [ -z "$loop_pid" ] || ! kill -0 "$loop_pid" 2>/dev/null; then
        return 0
    fi

    supervisor_pid="$(pgrep -P "$loop_pid" | head -n 1 || true)"
    if [ -z "$supervisor_pid" ]; then
        return 0
    fi

    worker_pid="$(pgrep -P "$supervisor_pid" | head -n 1 || true)"
    printf '%s\n' "${worker_pid:-$supervisor_pid}"
}

load_targets

echo "=== neo-devpack-dotnet fuzzer status ==="
echo "Date: $(date)"
echo ""

if [ -f "$LAUNCHER_PID_FILE" ]; then
    LAUNCHER_PID="$(cat "$LAUNCHER_PID_FILE")"
    if [ -n "$LAUNCHER_PID" ] && kill -0 "$LAUNCHER_PID" 2>/dev/null; then
        echo "Launcher: running (PID $LAUNCHER_PID)"
    else
        echo "Launcher: stale PID file ($LAUNCHER_PID)"
    fi
else
    echo "Launcher: not running"
fi

echo ""

for target in "${TARGETS[@]}"; do
    CORPUS="$FUZZ_ROOT/corpus/$target"
    ARTIFACTS="$FUZZ_ROOT/artifacts/$target"
    LOGFILE="$LOGDIR/${target}.log"
    PIDFILE="$PIDDIR/${target}.pid"
    TARGET_NAME="${TARGET_LABELS[$target]:-$target}"

    CORPUS_COUNT=$(find "$CORPUS" -type f 2>/dev/null | wc -l)
    CRASHES=$(find "$ARTIFACTS" -maxdepth 1 -type d -name "crash-*" 2>/dev/null | wc -l)

    LAST=""
    if [ -f "$LOGFILE" ]; then
        LAST=$(tail -1 "$LOGFILE" | head -c 160)
    fi

    LOOP_STATUS="not running"
    LOOP_PID=""
    STATUS_ICON=" "
    if [ -f "$PIDFILE" ]; then
        LOOP_PID="$(cat "$PIDFILE")"
        if [ -n "$LOOP_PID" ] && kill -0 "$LOOP_PID" 2>/dev/null; then
            LOOP_STATUS="running (PID $LOOP_PID)"
            STATUS_ICON="*"
        else
            LOOP_STATUS="stale PID file ($LOOP_PID)"
            STATUS_ICON="!"
        fi
    fi

    WORKER_PID=""
    if [ -n "$LOOP_PID" ] && kill -0 "$LOOP_PID" 2>/dev/null; then
        WORKER_PID="$(find_worker_pid "$LOOP_PID")"
    fi
    if [ -n "$WORKER_PID" ]; then
        STATUS_ICON="*"
    fi

    echo "[$STATUS_ICON] $target"
    if [ "$TARGET_NAME" != "$target" ]; then
        echo "    Target: $TARGET_NAME"
    fi
    echo "    Loop:   $LOOP_STATUS"
    if [ -n "$WORKER_PID" ]; then
        echo "    Worker: running (PID $WORKER_PID)"
    else
        echo "    Worker: idle"
    fi
    echo "    Corpus: $CORPUS_COUNT files | Crashes: $CRASHES"
    if [ -n "$LAST" ]; then
        echo "    Last:   $LAST"
    fi
    echo ""
done
