#
# IIS module
#
# Place all IIS related functions here
#

Import-Module webadministration

function IIS_CreateSite($site, $protocol, $bindingInfo, $path, $appPool)
{
	$sitePath = "IIS:\Sites\$site"
	if (-not(Test-Path $sitePath))
	{
		CON_WriteInfo "Creating $site... " $true
		New-Item $sitePath -physicalPath "$path" -bindings @{protocol="$protocol"; bindingInformation="$bindingInfo"}
		CON_WriteDone
	}
}

function IIS_RemoveSite($site)
{
	$sitePath = "IIS:\Sites\$site"
	if (Test-Path $sitePath)
	{
		CON_WriteInfo "Removing $site... " $true
	    Remove-Item $sitePath -Recurse -Force
		# Tries to give IIS time to shut down so the file deletion below occurs without errors
		Start-Sleep -s 2
		CON_WriteDone
	}
}

function IIS_CreateApplication($site, $name, $path, $appPool)
{
	CON_WriteInfo "Creating $site\$name... " $true
	$webAppPath = "IIS:\Sites\$site\$name"
	New-Item $webAppPath -physicalPath "$path" -type Application
	Set-ItemProperty $webAppPath -name applicationPool -value $appPool
	CON_WriteDone
}

function IIS_RemoveApplication($site, $name)
{
	$webAppPath = "IIS:\Sites\$site\$name"
	if (Test-Path $webAppPath)
	{
		CON_WriteInfo "Removing $site\$name... " $true
	    Remove-Item $webAppPath -Recurse -Force
		# Tries to give IIS time to shut down so the file deletion below occurs without errors
		Start-Sleep -s 2
		CON_WriteDone
	}
}

Export-ModuleMember IIS_CreateApplication, IIS_RemoveApplication, IIS_CreateSite, IIS_RemoveSite