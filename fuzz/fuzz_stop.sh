#!/usr/bin/env bash
#
# Stop all running fuzz targets.

set -euo pipefail

FUZZ_ROOT="$(cd "$(dirname "$0")" && pwd)"
PIDDIR="$FUZZ_ROOT/pids"
LAUNCHER_PID_FILE="$PIDDIR/launcher.pid"
PIDS_FILE="$FUZZ_ROOT/fuzz.pids"

echo "Stopping neo-devpack-dotnet fuzzers..."

if [ -f "$LAUNCHER_PID_FILE" ]; then
    kill "$(cat "$LAUNCHER_PID_FILE")" 2>/dev/null || true
fi

if [ -f "$PIDS_FILE" ]; then
    while read -r pid; do
        kill "$pid" 2>/dev/null || true
    done < "$PIDS_FILE"
    rm -f "$PIDS_FILE"
fi

if [ -d "$PIDDIR" ]; then
    while read -r pidfile; do
        kill "$(cat "$pidfile")" 2>/dev/null || true
    done < <(find "$PIDDIR" -maxdepth 1 -type f -name '*.pid' -print)
fi

pkill -f "$FUZZ_ROOT/run_target_forever.sh" 2>/dev/null || true
pkill -f "$FUZZ_ROOT/fuzz_forever.sh" 2>/dev/null || true
pkill -f "bash ./fuzz_forever.sh" 2>/dev/null || true
pkill -f "Neo.DevPack.Fuzz.dll" 2>/dev/null || true

for _ in $(seq 1 20); do
    if ! pgrep -f "Neo.DevPack.Fuzz.dll|run_target_forever.sh|fuzz_forever.sh" >/dev/null 2>&1; then
        break
    fi

    sleep 0.5
done

pkill -9 -f "$FUZZ_ROOT/run_target_forever.sh" 2>/dev/null || true
pkill -9 -f "$FUZZ_ROOT/fuzz_forever.sh" 2>/dev/null || true
pkill -9 -f "bash ./fuzz_forever.sh" 2>/dev/null || true
pkill -9 -f "Neo.DevPack.Fuzz.dll" 2>/dev/null || true

rm -f "$LAUNCHER_PID_FILE"
rm -rf "$PIDDIR"

echo "All fuzzers stopped."
