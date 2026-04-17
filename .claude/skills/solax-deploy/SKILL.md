---
name: solax-deploy
description: >
  Build and deploy the SolaxHub .NET worker service to the user's Raspberry Pi
  over SSH. Invoke this skill whenever the user asks to deploy SolaxHub, ship
  the app to the Pi, update the inverter service, push a new build, or any
  similar phrasing. Always executes the complete sequence: build linux-arm64 →
  stop systemd service → upload binaries → merge remote appsettings (existing
  server values are NEVER overwritten; any new config keys require the user to
  supply values interactively) → upload merged appsettings → restart service →
  verify health. Never skip steps, never auto-fill config values.
---

# SolaxHub Deploy

This skill automates deploying the SolaxHub .NET worker to a Raspberry Pi.
The sequence is strict and must always be followed in order.

---

## Configuration file

Read `deploy.config.json` (in the same directory as this SKILL.md) before
doing anything else.

If `ssh.host`, `ssh.user`, or `remote.service_name` are empty strings, stop
immediately with:

> Deploy config is incomplete. Please edit
> `.claude/skills/solax-deploy/deploy.config.json` and fill in
> `ssh.host`, `ssh.user`, and `remote.service_name`, then try again.

Build the SSH base command from the config:
```
ssh -o StrictHostKeyChecking=accept-new [-i <identity_file>] [-p <port>] <user>@<host>
```
Omit `-i` if `identity_file` is empty. Omit `-p` if `port` is 22.

---

## Step 1 — Build

From `local.repo_root`, run:

```
dotnet publish SolaxHub/SolaxHub.csproj -c Release -r linux-arm64 --no-self-contained
```

Stop if exit code is non-zero. Publish output lands at:
`<local.repo_root>/<local.publish_dir_relative>/`

---

## Step 2 — Fetch remote appsettings

Run:
```
<ssh> "cat <remote.appsettings_path>"
```

Capture the output as the remote JSON. If the command fails (first deploy,
file not found), treat remote as an empty object `{}` and continue.

---

## Step 3 — Detect and handle new config keys

Read both the local canonical appsettings (`<local.canonical_appsettings_relative>`)
and the remote JSON from Step 2.

Recursively compare every key in the local file against the remote JSON. For
each key path:

- If it already exists in remote → **leave it alone. Remote value wins, always.**
- If it is absent from remote → it is new and needs a value from the user.

For every new key found, ask the user what value to use. Show the full
dot-separated key path and the local default value as a hint. Never fill it
in yourself. The user may explicitly choose to leave a key empty — that is
fine, but they must confirm it.

If there are no new keys, skip to Step 4.

---

## Step 4 — Build the merged appsettings

Construct the merged JSON yourself:
- Start from a deep copy of the remote JSON.
- Insert only the new keys with the values the user provided in Step 3.
- Every value that existed on the server must be identical in the output —
  do not reformat, reorder, or touch existing values.

Write the merged JSON to a local temp file (e.g. `/tmp/solaxhub_appsettings_merged.json`).

---

## Step 5 — Stop the service

```
<ssh> "sudo systemctl stop <remote.service_name>"
```

Confirm it stopped:
```
<ssh> "systemctl is-active <remote.service_name>"
```
Expected: `inactive` or `failed`. If still `active` after 10 seconds, stop
and tell the user.

---

## Step 6 — Upload binaries

Use `rsync` if available, otherwise `scp`.

**rsync (preferred):**
```
rsync -az --delete \
  --exclude='appsettings.json' \
  --exclude='appsettings.*.json' \
  -e "ssh [-i <identity_file>] [-p <port>]" \
  <publish_dir>/ \
  <user>@<host>:<remote.install_dir>/
```

**scp fallback:**
```
scp [-i <identity_file>] [-P <port>] -r <publish_dir>/. \
  <user>@<host>:<remote.install_dir>/
```
Then remove any appsettings scp may have copied over:
```
<ssh> "rm -f <remote.install_dir>/appsettings.json <remote.install_dir>/appsettings.*.json"
```

---

## Step 7 — Upload merged appsettings

```
scp [-i <identity_file>] [-P <port>] /tmp/solaxhub_appsettings_merged.json \
  <user>@<host>:<remote.appsettings_path>
```

---

## Step 8 — Start the service

```
<ssh> "sudo systemctl start <remote.service_name>"
```

---

## Step 9 — Verify health

Wait `health_check.wait_seconds_before_check` seconds, then:

**Service state:**
```
<ssh> "systemctl is-active <remote.service_name>"
```
Must return `active`. If not, fetch 50 lines of the journal, show them, and
stop.

**Log tail:**
```
<ssh> "journalctl -u <remote.service_name> -n <health_check.journalctl_tail_lines> --no-pager"
```
Search for `health_check.modbus_log_pattern` (regex). At least one match
confirms the app reached the inverter.

**Error scan:**
Search the same tail (case-insensitive) for `error`, `exception`, `fatal`,
`unhandled`. Surface any matches to the user even if the service is active.

---

## Step 10 — Report

```
Deploy complete.
  Built:             linux-arm64, net10.0
  Binaries:          <remote.install_dir>
  Config keys added: <N>  (server keys preserved: <M>)
  Service:           active
  Inverter:          [first matching log line, or "no Modbus log line found"]
  [Warnings from error scan, or "No errors detected"]
```

---

## Error handling

On any failure: print the failed command, exit code, and stderr, then stop.
If the failure occurred after Step 5 (service was stopped), warn the user they
may need to manually restart: `sudo systemctl start <service_name>`.
Never skip a step or silently continue after a failure.
