# Copyright (c) Microsoft Corporation.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

param(
    [Parameter(Mandatory=$true)]
    [string]$Command,
    [string]$Browser = "chromium"
)

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

# Resolve the dotnet command
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    Write-Error "Could not find 'dotnet' command. Please ensure .NET SDK is installed and available in PATH."
    exit 1
}

# Resolve the project directory
$projectDir = $PSScriptRoot
if (-not $projectDir) {
    $projectDir = Get-Location
}

# Resolve the project file
$projectFile = Join-Path $projectDir "CRMSystemUITests.csproj"
if (-not (Test-Path $projectFile)) {
    Write-Error "Could not find project file at $projectFile"
    exit 1
}

# Resolve the output directory
$outputDir = Join-Path $projectDir "bin\Debug\net8.0"
if (-not (Test-Path $outputDir)) {
    Write-Error "Could not find output directory at $outputDir. Please build the project first."
    exit 1
}

# Resolve the playwright CLI
$playwrightCli = Join-Path $outputDir ".playwright\package\lib\cli\cli.js"
if (-not (Test-Path $playwrightCli)) {
    Write-Error "Could not find Playwright CLI at $playwrightCli. Please ensure Microsoft.Playwright NuGet package is installed."
    exit 1
}

# Resolve Node.js
$node = Get-Command node -ErrorAction SilentlyContinue
if (-not $node) {
    Write-Error "Could not find 'node' command. Please ensure Node.js is installed and available in PATH."
    exit 1
}

# Execute the command
switch ($Command) {
    "install" {
        & $node $playwrightCli install $Browser
    }
    "install-deps" {
        & $node $playwrightCli install-deps $Browser
    }
    default {
        Write-Error "Unknown command: $Command"
        exit 1
    }
} 