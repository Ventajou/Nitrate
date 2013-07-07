#
# Console module
#
# Place all console related functions here
#

# Writes an informational message on the console
function CON_WriteInfo($message, $newLine)
{
	if ($newLine)
	{
		Write-Host -f Blue "$message"
	}
	else
	{
		Write-Host -f Blue -NoNewline "$message"
	}
}

# Writes a success message on the console
function CON_WriteDone
{
	Write-Host -f Green 'Done!'
}

function CON_WriteGood($message)
{
	Write-Host -f Green "$message"
}

Export-ModuleMember CON_WriteInfo, CON_WriteDone, CON_WriteGood