#!/usr/bin/env bash
#
# Lightweight maintenance for long-running fuzz campaigns.
# - truncates oversized active logs after copying them to a compressed archive
# - removes stale temporary compiler work directories

set -euo pipefail

FUZZ_ROOT="$(cd "$(dirname "$0")" && pwd)"
LOGDIR="$FUZZ_ROOT/logs"
ARCHIVEDIR="$LOGDIR/archive"
TMP_ROOT="/tmp/Neo.Compiler"
MAX_LOG_SIZE_MB="${MAX_LOG_SIZE_MB:-256}"
STALE_TMP_DAYS="${STALE_TMP_DAYS:-2}"
MAX_CORPUS_FILES_PER_TARGET="${MAX_CORPUS_FILES_PER_TARGET:-20480}"
MAX_CRASHES_PER_TARGET="${MAX_CRASHES_PER_TARGET:-512}"

mkdir -p "$ARCHIVEDIR"

timestamp() {
    date "+%Y-%m-%d %H:%M:%S"
}

rotate_log() {
    local logfile="$1"
    local name size_bytes size_limit archive

    name="$(basename "$logfile")"
    size_bytes="$(stat -c '%s' "$logfile")"
    size_limit=$((MAX_LOG_SIZE_MB * 1024 * 1024))

    if [ "$size_bytes" -lt "$size_limit" ]; then
        return 0
    fi

    archive="$ARCHIVEDIR/${name%.*}-$(date '+%Y%m%d-%H%M%S').log"
    cp "$logfile" "$archive"
    gzip -f "$archive"
    : > "$logfile"

    echo "[$(timestamp)] rotated $name (size=${size_bytes} bytes)"
}

cleanup_tmp_tree() {
    local root="$1"

    if [ ! -d "$root" ]; then
        return 0
    fi

    find "$root" -mindepth 1 -maxdepth 1 -type d -mtime "+$STALE_TMP_DAYS" -print0 |
        while IFS= read -r -d '' path; do
            rm -rf "$path"
            echo "[$(timestamp)] removed stale temp directory $path"
        done
}

prune_corpus_dir() {
    local dir="$1"
    local count remove

    if [ ! -d "$dir" ]; then
        return 0
    fi

    count="$(find "$dir" -type f | wc -l)"
    if [ "$count" -le "$MAX_CORPUS_FILES_PER_TARGET" ]; then
        return 0
    fi

    remove=$((count - MAX_CORPUS_FILES_PER_TARGET))
    find "$dir" -type f | tail -n "+$((MAX_CORPUS_FILES_PER_TARGET + 1))" | xargs -r rm -f
    echo "[$(timestamp)] pruned corpus $(basename "$dir") removed=$remove kept=$MAX_CORPUS_FILES_PER_TARGET"
}

prune_artifact_dir() {
    local dir="$1"
    local count remove

    if [ ! -d "$dir" ]; then
        return 0
    fi

    count="$(find "$dir" -maxdepth 1 -type d -name 'crash-*' | wc -l)"
    if [ "$count" -le "$MAX_CRASHES_PER_TARGET" ]; then
        return 0
    fi

    remove=$((count - MAX_CRASHES_PER_TARGET))
    find "$dir" -maxdepth 1 -type d -name 'crash-*' | sort | head -n "$remove" | xargs -r rm -rf
    echo "[$(timestamp)] pruned artifacts $(basename "$dir") removed=$remove kept=$MAX_CRASHES_PER_TARGET"
}

main() {
    echo "[$(timestamp)] housekeeping start"

    find "$LOGDIR" -maxdepth 1 -type f -name '*.log' -print0 |
        while IFS= read -r -d '' logfile; do
            rotate_log "$logfile"
        done

    cleanup_tmp_tree "$TMP_ROOT/CompileSources"
    cleanup_tmp_tree "$TMP_ROOT/Fuzz"

    find "$FUZZ_ROOT/corpus" -mindepth 1 -maxdepth 1 -type d |
        while read -r dir; do
            prune_corpus_dir "$dir"
        done

    find "$FUZZ_ROOT/artifacts" -mindepth 1 -maxdepth 1 -type d |
        while read -r dir; do
            prune_artifact_dir "$dir"
        done

    du -sh "$FUZZ_ROOT/corpus" "$FUZZ_ROOT/artifacts" "$FUZZ_ROOT/logs" "$TMP_ROOT" 2>/dev/null |
        while read -r size path; do
            echo "[$(timestamp)] usage $path $size"
        done

    echo "[$(timestamp)] housekeeping done"
}

main "$@"
