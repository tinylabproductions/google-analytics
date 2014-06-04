#!/bin/sh

set -e

source "`dirname $0`/functions.sh"
name="Google Analytics"

echo "Setting up $name."

dirlink Assets/Vendor/GoogleAnalytics

echo "Done with $name."
