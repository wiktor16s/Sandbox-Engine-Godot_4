#!/bin/sh
echo -ne '\033c\033]0;Sandbox-Engine\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/debug.x86_64" "$@"
