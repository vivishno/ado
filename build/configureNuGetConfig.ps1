$ErrorActionPreference = "Stop"

# this is a CDP service account PAT that has access to many Microsoft interal feeds
if (-not $env:CDP_DEFAULT_CLIENT_PACKAGE_PAT)
{
    throw "No PAT to access NuGet feeds found"
}

# MSEng requires a different PAT from the VSTS accounts
# if (-not $env:CDP_DEFAULT_CLIENT_PACKAGE_MSENG_PAT)
# {
#     throw "No PAT to access MSEng NuGet feeds found"
# }

# location of nuget config to reconfigure with passwords
$configPath = "$PSScriptRoot\..\nuget.config"

write-host "Updating $configPath"

([xml](Get-Content $configPath)).configuration.packageSources.add | % {
    $key = $_.key
    $value = $_.value

    write-host "Setting password for $key"
    &nuget sources update -Name $key -Source $value -UserName "VSTS" -Password $env:CDP_DEFAULT_CLIENT_PACKAGE_PAT -Verbosity detailed -NonInteractive -ConfigFile $configPath -StorePasswordInClearText

    if ($LASTEXITCODE -ne 0)
    {
        throw "Failed to set Nuget password for $key (ExitCode: $LASTEXITCODE)"
    }
}
