#
# Net module
#
# Place all network related functions here
#

function NET_DownloadFile($source, $destination)
{
	CON_WriteInfo "Downloading file... "
	if (Test-Path "$destination")
	{
		CON_WriteGood "File found at [$destination], no need to download it."
	}
	else
	{
	    Invoke-WebRequest "$source" -OutFile "$destination"
		CON_WriteDone
	}
}

Export-ModuleMember NET_DownloadFile