#!/usr/bin/env bash
#
# Internal helper used by fuzz_forever.sh to run one target indefinitely.

set -euo pipefail

if [ "$#" -ne 7 ]; then
    echo "usage: $0 <target> <harness-dll> <corpus-dir> <artifacts-dir> <seed-dir> <dictionary> <pid-file>" >&2
    exit 1
fi

TARGET="$1"
HARNESS_DLL="$2"
CORPUS="$3"
ARTIFACTS="$4"
SEEDS="$5"
DICT="$6"
PIDFILE="$7"
MAX_CORPUS_IN_MEMORY="${MAX_CORPUS_IN_MEMORY:-4096}"
MAX_CORPUS_FILES="${MAX_CORPUS_FILES:-20480}"
STATUS_INTERVAL_SECONDS="${STATUS_INTERVAL_SECONDS:-30}"

mkdir -p "$(dirname "$PIDFILE")" "$CORPUS" "$ARTIFACTS"

CHILD_PID=""

cleanup() {
    if [ -n "$CHILD_PID" ] && kill -0 "$CHILD_PID" 2>/dev/null; then
        kill "$CHILD_PID" 2>/dev/null || true
        wait "$CHILD_PID" 2>/dev/null || true
    fi
}

trap 'cleanup; exit 0' INT TERM
trap 'rm -f "$PIDFILE"' EXIT

printf '%s\n' "$$" > "$PIDFILE"

iteration=0
while true; do
    iteration=$((iteration + 1))
    echo "[$(date "+%Y-%m-%d %H:%M:%S")] $TARGET iteration $iteration starting..."

    timeout --signal=TERM --kill-after=30 86500 \
        dotnet "$HARNESS_DLL" \
            run "$TARGET" \
            --corpus-dir "$CORPUS" \
            --artifacts-dir "$ARTIFACTS" \
            --seed-dir "$SEEDS" \
            --dictionary "$DICT" \
            --max-total-time-seconds 86400 \
            --max-input-size 16384 \
            --max-corpus-in-memory "$MAX_CORPUS_IN_MEMORY" \
            --max-corpus-files "$MAX_CORPUS_FILES" \
            --status-interval-seconds "$STATUS_INTERVAL_SECONDS" &
    CHILD_PID=$!

    set +e
    wait "$CHILD_PID"
    STATUS=$?
    set -e
    CHILD_PID=""

    case "$STATUS" in
        0)
            echo "[$(date "+%Y-%m-%d %H:%M:%S")] $TARGET iteration $iteration completed."
            sleep 2
            ;;
        124|137)
            echo "[$(date "+%Y-%m-%d %H:%M:%S")] $TARGET iteration $iteration hit the timeout guard, restarting in 5s..."
            sleep 5
            ;;
        *)
            echo "[$(date "+%Y-%m-%d %H:%M:%S")] $TARGET iteration $iteration exited with status $STATUS, restarting in 5s..."
            sleep 5
            ;;
    esac
done
