# neo-devpack-dotnet fuzzing

This mirrors the operational shape of `~/git/neo-solidity/fuzz`:

- multiple focused targets instead of one monolithic runner
- a shared corpus and mutation dictionary
- per-target crash artifacts and logs
- a `fuzz_forever.sh` loop that restarts crashed targets automatically

## Targets

- `fuzz_compile`: compiles arbitrary mutated source text or wrapped code snippets
- `fuzz_structured_compile`: generates valid smart contracts and compiles them end to end
- `fuzz_template_projects`: runs `TemplateManager`, patches generated projects to the local framework, and compiles them
- `fuzz_differential`: compiles the same generated input twice and crashes on output mismatches
- `fuzz_devpack_runtime`: compiles generated contracts, deploys them into `TestEngine`, and invokes stable methods

## Layout

- `Neo.DevPack.Fuzz/`: .NET harness project
- `seeds/`: tracked seed corpus used to bootstrap empty target corpora
- `corpus/`: live mutated corpus, ignored by git
- `artifacts/`: crash repros, ignored by git
- `logs/`: per-target logs, ignored by git
- `state/`: persisted fingerprints that prevent the same semantic outcome from being re-added after restarts
- `dotnet.dict`: mutation dictionary

## Commands

Build the harness:

```bash
dotnet build fuzz/Neo.DevPack.Fuzz/Neo.DevPack.Fuzz.csproj -c Release
```

List available targets:

```bash
dotnet run --project fuzz/Neo.DevPack.Fuzz/Neo.DevPack.Fuzz.csproj -- list
```

Run one target for a short smoke cycle:

```bash
dotnet run --project fuzz/Neo.DevPack.Fuzz/Neo.DevPack.Fuzz.csproj -c Release --no-build -- \
  run fuzz_structured_compile \
  --max-total-time-seconds 30 \
  --iterations 200
```

Reproduce a crash input:

```bash
dotnet run --project fuzz/Neo.DevPack.Fuzz/Neo.DevPack.Fuzz.csproj -c Release --no-build -- \
  repro fuzz_differential fuzz/artifacts/fuzz_differential/crash-.../input.bin
```

Run all targets continuously:

```bash
nohup ./fuzz/fuzz_forever.sh >> fuzz/logs/launcher.log 2>&1 &
```

Run only a subset:

```bash
nohup ./fuzz/fuzz_forever.sh fuzz_structured_compile fuzz_differential >> fuzz/logs/launcher.log 2>&1 &
```

Check status:

```bash
./fuzz/fuzz_status.sh
```

Report newly found crashes:

```bash
./fuzz/report_crashes.sh
```

Show all crash artifacts, including ones you already reviewed:

```bash
./fuzz/report_crashes.sh --all --no-mark-seen
```

Reset the seen-crash state if you want to review everything again as new:

```bash
./fuzz/report_crashes.sh --clear-seen
```

Stop everything:

```bash
./fuzz/fuzz_stop.sh
```

Run housekeeping once:

```bash
./fuzz/fuzz_housekeeping.sh
```

## Long-running operation

For a quick unattended run, use `tmux`, `screen`, or `nohup`.

For a multi-week or multi-month campaign, a user service is cleaner:

```bash
mkdir -p ~/.config/systemd/user
cp fuzz/neo-devpack-fuzz.service.example ~/.config/systemd/user/neo-devpack-fuzz.service
systemctl --user daemon-reload
systemctl --user enable --now neo-devpack-fuzz.service
```

If `systemctl --user` is being called from a non-login shell and cannot find the user bus, export these first:

```bash
export XDG_RUNTIME_DIR="/run/user/$(id -u)"
export DBUS_SESSION_BUS_ADDRESS="unix:path=$XDG_RUNTIME_DIR/bus"
```

Then monitor with:

```bash
systemctl --user status neo-devpack-fuzz.service
tail -f fuzz/logs/fuzz_structured_compile.log
```

To stop a service-managed campaign cleanly:

```bash
systemctl --user stop neo-devpack-fuzz.service
```

Use `./fuzz/fuzz_stop.sh` for non-service runs or for cleanup while debugging the launcher locally.

For multi-month campaigns, enable hourly housekeeping too:

```bash
cp fuzz/neo-devpack-fuzz-housekeeping.service.example ~/.config/systemd/user/neo-devpack-fuzz-housekeeping.service
cp fuzz/neo-devpack-fuzz-housekeeping.timer.example ~/.config/systemd/user/neo-devpack-fuzz-housekeeping.timer
systemctl --user daemon-reload
systemctl --user enable --now neo-devpack-fuzz-housekeeping.timer
```

The housekeeping job truncates active logs after archiving them to `fuzz/logs/archive/` and removes stale compiler temp directories under `/tmp/Neo.Compiler`.

Only one launcher instance should be active at a time. If `fuzz_forever.sh` detects a live launcher PID, it refuses to start another copy.

The service uses `Restart=always`, `fuzz_forever.sh` restarts each individual target after crashes, and the launcher exits if one of the long-lived target loops dies unexpectedly. That gives you both target-level restarts and service-level recovery for multi-week campaigns.
