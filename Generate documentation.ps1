# Download tools
echo "Downloading necessary tools..."

$url = "https://github.com/dotnet/docfx/releases/download/v2.34/docfx.zip"
$outputdir = ".\Documentation\Tools\"
$output = ".\Documentation\Tools\docfx.zip"

if (!(Test-Path $outputdir)) {
    New-Item -ItemType Directory -Force -Path $path
}
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest -Uri $url -OutFile $output


# Unzip tools
echo "" "Unpacking tools..."

Add-Type -AssemblyName System.IO.Compression.FileSystem
$zipfile = $output
$output = ".\Documentation\Tools\docfx\"

Expand-Archive -Force $zipfile -DestinationPath $output


# Set an alias for the tool
Set-Alias -Name docfx -Value "$((Get-Item -Path ./).FullName)\Documentation\Tools\docfx\docfx.exe"


# Check if docfx is already generated, otherwise generate
if (!(Test-Path .\Documentation\docfx.json)) {
    Write-Error "docfx.json was not found. Are you sure you are running this script from the root directory and have pulled the right files?"
    pause
    [void](Read-Host 'Press Enter to exit...')
}

# Initialize docfx
echo "--------------------------" "" "Initializing docfx framework" ""
docfx init -q -o Documentation

# Force build the metadata
echo "--------------------------" "" "Force building the metadata..." ""
docfx metadata .\Documentation\docfx.json -f


# Force build the documentation
echo "--------------------------" "" "Force building the documentation..." ""
docfx build .\Documentation\docfx.json -f

# Serve the documentation if wanted by the user
echo "--------------------------"

$choices = New-Object Collections.ObjectModel.Collection[Management.Automation.Host.ChoiceDescription]
$choices.Add((New-Object Management.Automation.Host.ChoiceDescription -ArgumentList '&Yes'))
$choices.Add((New-Object Management.Automation.Host.ChoiceDescription -ArgumentList '&No'))

$answer = $Host.UI.PromptForChoice("The documentation can be served on http://localhost:8080.", "Do you want this to happen?", $choices, 1)
if ($answer -eq 0) {
    docfx serve .\Documentation\_site
}
