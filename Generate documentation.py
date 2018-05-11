import json
import urllib.request
import zipfile
from pathlib import Path
import time
import os


def installPackages():
    import importlib
    try:
        importlib.import_module("progressbar")
    except ImportError:
        import pip
        pip.main(['install', 'progressbar2'])
    finally:
        globals()["progressbar"] = importlib.import_module("progressbar")


def incrementProgressbar(count, blockSize, totalSize):
    bar.max_value = totalSize
    if count * blockSize < totalSize:
        bar.update(count * blockSize)
    else:
        bar.update(totalSize)


def yes_or_no(question):
    while "the answer is invalid":
        reply = str(input(question + ' (y/n): ')).lower().strip()
        if reply[0] == 'y':
            return True
        elif reply[0] == 'n':
            return False
        else:
            print("incorrect answer, please press y or n\n")

# Install necessary packages
installPackages()

# Check if docfx is already generated, otherwise exit.
if not Path("./Documentation/docfx.json").exists:
    raise FileNotFoundError("docfx.json was not found. Are you sure you are" +
                            " running this script form the root directory and" +
                            " have pulled the right files?")

# Download tools.
print("Downloading necessary tools...")

with urllib.request.urlopen("https://api.github.com/repos/dotnet/docfx/releases/latest") as url:
    latestReleaseResponse = json.loads(url.read().decode())
    url = latestReleaseResponse["assets"][0]["browser_download_url"]

    import progressbar
    bar = progressbar.ProgressBar()
    urllib.request.urlretrieve(url, filename="./Documentation/Tools/docfx.zip", reporthook=incrementProgressbar)

# Unzip tools.
time.sleep(1)
print("\nUnzipping tools...")

with zipfile.ZipFile("./Documentation/Tools/docfx.zip", 'r') as docfxzip:
    docfxzip.extractall("./Documentation/Tools/docfx")

# Initialize docfx
print("--------------------------\nInitializing framework...\n")
os.system(".\Documentation\Tools\docfx\docfx.exe init -q -o Documentation")

# Force build the metadata
print("--------------------------\nForce building the metadata...\n")
os.system(".\Documentation\Tools\docfx\docfx.exe metadata .\Documentation\docfx.json -f")

# Force build the documentation
print("--------------------------\nForce building the documentaion...\n")
os.system(".\Documentation\Tools\docfx\docfx.exe build .\Documentation\docfx.json -f")

# Serve the documentation if wanted by the user
print("--------------------------\n")
if yes_or_no("The documentation can be served on http://localhost:8080. Do you want this to happen?"):
    os.system(".\Documentation\Tools\docfx\docfx.exe serve .\Documentation\_site")
