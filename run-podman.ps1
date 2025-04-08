Set-StrictMode -Version Latest
Set-Variable $ErrorActionPreference='Stop'

podman run --rm --privileged $(podman build -q .)