# This script checks for changes to the csx files and clears the scriptcs cache if needed
# Then it runs Nitrate
$curPath = Get-Location
$scriptPath = Split-Path -parent $MyInvocation.MyCommand.Definition
Push-Location $scriptPath

if (test-path ".\.cache\Nitrate.dll")
{
   $csxTime = (gci .\*.csx -recurse | sort LastWriteTime | select -last 1).lastwritetime
   $cacheTime = (get-item ".cache\Nitrate.dll").lastwritetime

   if ($cacheTime -lt $csxTime)
   {
     remove-item "$scriptPath\.cache" -Force -Recurse
   }
}
. scriptcs ".\Nitrate.csx" -cache -- "$curPath" $args[0] $args[1] $args[2] $args[3] $args[4] $args[5] $args[6] $args[7]
Pop-Location
