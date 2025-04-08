Set-StrictMode -Version Latest
Set-Variable $ErrorActionPreference='Stop'

docker run --rm --privileged $(docker build -q .)