#
# FileSystem module
#
# Place all file system related functions here
#

# Creates a symlink for each child folder of $source into $destination
function FS_LinkFolders($source, $destination)
{
	CON_WriteInfo "Adding symlinks in $destination... " $true
	$sourceFolders = Get-Item "$source\*" | Where {$_.psIsContainer -eq $true }
	foreach($sourceFolder in $sourceFolders)
	{
	    $linkSource = $sourceFolder.FullName + "\"
	    $linkDestination = "$destination\" + $sourceFolder.Name
	    . "cmd" /c mklink "$linkDestination" "$linkSource" /D
	}
	CON_WriteDone
}

function FS_LinkFolder($source, $destination)
{
	CON_WriteInfo "Adding symlink for $source in $destination... " $true
	. "cmd" /c mklink "$destination" "$source" /D
	CON_WriteDone
}

function FS_LinkFile($source, $destination)
{
	CON_WriteInfo "Adding symlink for $source in $destination... " $true
	. "cmd" /c mklink "$destination" "$source"
	CON_WriteDone
}

# Removes all symlinks for child folders of $source present in $destination
function FS_UnlinkFolders($source, $destination)
{
	CON_WriteInfo "Removing symlinks in $destination... " $true
	$sourceFolders = Get-Item "$source\*" | Where {$_.psIsContainer -eq $true }
	foreach($sourceFolder in $sourceFolders)
	{
	    $destinationFolder = "$destination\" + $sourceFolder.Name
	    if (Test-Path $destinationFolder) { . "cmd" /c rmdir "$destinationFolder" }
	}
	CON_WriteDone
}

function FS_UnlinkFolder($destination)
{
	CON_WriteInfo "Removing symlink $destination... " $true
    if (Test-Path $destination) { . "cmd" /c rmdir "$destination" }
	CON_WriteDone
}

function FS_UnlinkFile($destination)
{
	CON_WriteInfo "Removing symlink $destination... " $true
    if (Test-Path $destination) { . "cmd" /c del "$destination" }
	CON_WriteDone
}

function FS_EnsureDir($path)
{
	if (-not(Test-Path $path)){New-Item -ItemType directory -Path "$path"}
}

function FS_EmptyDir($path)
{
	CON_WriteInfo "Deleting content of $path... "
	if (Test-Path $path)
	{
		Remove-Item "$path\*" -Force -Recurse
	}
	CON_WriteDone
}

function FS_RemoveDir($path)
{
	CON_WriteInfo "Deleting $path... "
	if (Test-Path $path)
	{
		Remove-Item "$path" -Force -Recurse
	}
	CON_WriteDone
}

function FS_CombinePath($left, $right)
{
	$combined = "$left\$right"
	while($combined -match '\\\\')
	{
		$combined = $combined -replace '\\\\', '\'
	}
	$combined
}


Export-ModuleMember FS_LinkFolders, FS_LinkFolder, FS_LinkFile, FS_UnlinkFolders, FS_UnlinkFolder, FS_UnlinkFile, FS_EmptyDir, FS_CombinePath, FS_EnsureDir, FS_RemoveDir