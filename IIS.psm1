#
# IIS module
#
# Place all IIS related functions here
#

Import-Module webadministration

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

Export-ModuleMember IIS_CreateApplication, IIS_RemoveApplication