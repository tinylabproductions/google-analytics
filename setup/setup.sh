#!/bin/sh

set -e

source "`dirname $0`/functions.sh"
name="Google Analytics"

echo "Setting up $name."

dirlink Assets/Vendor/GoogleAnalytics

setup_gitignore

echo "Done with $name."
