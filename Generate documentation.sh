#!/bin/bash

# Warning about needing unzip
echo "========================================================"
echo "| IMPORTANT: You need unzip for this script to work!!! |"
echo "|            Force quit if this isn't the case!        |"
echo "========================================================"

sleep 5


# Check if docfx is already generated, otherwise exit
if [! -f ./Documentation/docfx.json]; then
    echo "docfx.json was not found. Are you sure you are running this script from the root directory and have pulled the right files?"
    read
    exit 1
fi


# Download tools
echo "Downloading necessary tools..."

url="https://github.com/dotnet/docfx/releases/download/v2.34/docfx.zip"
outputdir="./Documentation/Tools/"

wget $url -P $outputdir


# Unzip tools
echo "--------------------------"
echo ""
echo "Unpacking tools..."
echo ""
zipfile="./Documentation/Tools/docfx.zip"
outputdir="./Documentation/Tools/docfx"

mkdir -p $outputdir
unzip $zipfile -d $outputdir


# Set an alias for the tool
alias docfx=mono ./Documentation/Tools/docfx/docfx.exe


# Initialize docfx
echo "--------------------------"
echo ""
echo "Initializeing docfx framework..."
echo ""
docfx init -q -o Documentation


# Force build the metadata
echo "--------------------------"
echo ""
echo "Force building the metadata..."
echo ""
docfx metadata ./Documentation/docfx.json -f


# Force build the documentation
echo "--------------------------"
echo ""
echo "Force building the documentation..."
echo ""
docfx build ./Documentation/docfx.json -f


# Serve the documentation if wanted by the user
echo "--------------------------"

read -p "The documentation can be served on http://localhost:8080. Do you want this to happen? " 1 -r
if [[! $REPLY =~ ^[Yy]$]]; then
    docfx serve ./Documentation/_site
fi
