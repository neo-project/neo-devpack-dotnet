#!/usr/bin/env bash
#
# Summarize fuzz crash artifacts and keep track of which ones have already
# been reviewed.

set -euo pipefail

FUZZ_ROOT="${FUZZ_ROOT_OVERRIDE:-$(cd "$(dirname "$0")" && pwd)}"
ARTIFACTS_ROOT="$FUZZ_ROOT/artifacts"
STATE_DIR="$FUZZ_ROOT/state/reports"
SEEN_FILE="$STATE_DIR/seen_crashes.txt"
PROJECT_PATH="fuzz/Neo.DevPack.Fuzz/Neo.DevPack.Fuzz.csproj"

MODE="new"
MARK_SEEN=1

usage() {
    cat <<'EOF'
Usage:
  ./fuzz/report_crashes.sh [--new|--all] [--mark-seen|--no-mark-seen] [--clear-seen]

Options:
  --new           Show only crashes that have not been reported before (default)
  --all           Show all crash artifacts, including already seen ones
  --mark-seen     Record reported crashes in fuzz/state/reports/seen_crashes.txt (default)
  --no-mark-seen  Do not update the seen set
  --clear-seen    Remove the seen set and exit
  -h, --help      Show this help text
EOF
}

while [ "$#" -gt 0 ]; do
    case "$1" in
        --new)
            MODE="new"
            ;;
        --all)
            MODE="all"
            ;;
        --mark-seen)
            MARK_SEEN=1
            ;;
        --no-mark-seen)
            MARK_SEEN=0
            ;;
        --clear-seen)
            mkdir -p "$STATE_DIR"
            rm -f "$SEEN_FILE"
            echo "Cleared seen crash state: $SEEN_FILE"
            exit 0
            ;;
        -h|--help)
            usage
            exit 0
            ;;
        *)
            echo "Unknown option: $1" >&2
            usage >&2
            exit 1
            ;;
    esac
    shift
done

mkdir -p "$STATE_DIR"

declare -A SEEN=()
if [ -f "$SEEN_FILE" ]; then
    while IFS= read -r line; do
        if [ -n "$line" ]; then
            SEEN["$line"]=1
        fi
    done < "$SEEN_FILE"
fi

mapfile -d '' CRASH_PATHS < <(
    if [ -d "$ARTIFACTS_ROOT" ]; then
        find "$ARTIFACTS_ROOT" -mindepth 2 -maxdepth 2 -type d -name 'crash-*' -print0 | sort -z
    fi
)

echo "=== neo-devpack-dotnet crash report ==="
echo "Date: $(date)"
echo "Mode: $MODE"
echo ""

if [ "${#CRASH_PATHS[@]}" -eq 0 ]; then
    echo "No crash artifacts found under $ARTIFACTS_ROOT"
    exit 0
fi

reported=0
new_count=0
mark_count=0
declare -a TO_MARK=()

print_field() {
    local label="$1"
    local value="$2"
    printf '  %-10s %s\n' "$label" "$value"
}

for crash_path in "${CRASH_PATHS[@]}"; do
    crash_path="${crash_path%$'\0'}"
    rel="${crash_path#"$ARTIFACTS_ROOT"/}"
    target="${rel%%/*}"
    status="seen"

    if [ -z "${SEEN[$rel]:-}" ]; then
        status="NEW"
    fi

    if [ "$MODE" = "new" ] && [ "$status" != "NEW" ]; then
        continue
    fi

    summary_file="$crash_path/summary.txt"
    input_file="$crash_path/input.bin"
    crash_time="(missing)"
    summary_line="(missing)"
    exception_line="(missing)"

    if [ -f "$summary_file" ]; then
        parsed_time="$(sed -n 's/^time:[[:space:]]*//p' "$summary_file" | head -n 1)"
        parsed_summary="$(sed -n '4p' "$summary_file")"
        parsed_exception="$(awk 'NR >= 6 && NF { print; exit }' "$summary_file")"

        if [ -n "$parsed_time" ]; then
            crash_time="$parsed_time"
        fi

        if [ -n "$parsed_summary" ]; then
            summary_line="$parsed_summary"
        fi

        if [ -n "$parsed_exception" ]; then
            exception_line="$parsed_exception"
        fi
    fi

    echo "[$status] $rel"
    print_field "Target:" "$target"
    print_field "Time:" "$crash_time"
    print_field "Summary:" "$summary_line"
    print_field "Exception:" "$exception_line"
    print_field "Path:" "$crash_path"

    if [ -f "$input_file" ]; then
        print_field "Repro:" "dotnet run --project $PROJECT_PATH -c Release --no-build -- repro $target $input_file"
    else
        print_field "Repro:" "(missing input.bin)"
    fi

    echo ""

    reported=$((reported + 1))
    if [ "$status" = "NEW" ]; then
        new_count=$((new_count + 1))
        if [ "$MARK_SEEN" -eq 1 ]; then
            TO_MARK+=("$rel")
        fi
    fi
done

if [ "$reported" -eq 0 ]; then
    echo "No new crashes to report."
    exit 0
fi

if [ "$MARK_SEEN" -eq 1 ] && [ "${#TO_MARK[@]}" -gt 0 ]; then
    touch "$SEEN_FILE"
    for rel in "${TO_MARK[@]}"; do
        printf '%s\n' "$rel" >> "$SEEN_FILE"
        mark_count=$((mark_count + 1))
    done
fi

echo "Reported: $reported crash(es)"
echo "New:      $new_count"

if [ "$MARK_SEEN" -eq 1 ]; then
    echo "Marked:   $mark_count"
    echo "Seen set: $SEEN_FILE"
fi
