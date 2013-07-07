# Nitrate main script

# stop on errors
$ErrorActionPreference='Stop'

# Initialize root paths
$scriptPath = Split-Path -parent $MyInvocation.MyCommand.Definition

$rootPath = ""
Push-Location
do
{
	if (Test-Path ".\NitrateConfig.ps1")
	{
		$rootPath = (Get-Location -PSProvider FileSystem).ProviderPath
	}
	else
	{
		cd ..
	}
}
while (($rootPath -eq "") -and -not((Get-Location -PSProvider FileSystem).ProviderPath.length -eq 3))
Pop-Location

if ($args[0] -eq "init")
{
	if (-not($rootPath -eq ""))
	{
		Write-Host
		CON_WriteInfo "A configuration file was found at [$rootPath], you may not initialize a new environment." $true
		Write-Host
		exit
	}
	
	copy "$scriptPath\DefaultData.ps1" "NitrateConfig.ps1"
	
	Write-Host
	CON_WriteInfo "Your environment is initialized." $true
	Write-Host "You should edit NitrateConfig.ps1 to configure the settings."
	Write-Host "Then setup Orchard by calling Nitrate.ps1 setup"
	Write-Host
	exit
}
elseif ($rootPath -eq "")
{
	Write-Host
	CON_WriteInfo "Configuration file NitrateConfig.ps1 could not be found." $true
	Write-Host "Use Nitrate.ps1 init to initialize a new environment"
	Write-Host
	exit
}

# load all additional modules, -Force ensures they get reloaded in case they get changed
Import-Module -Force -Name "$scriptPath\Console"
Import-Module -Force -Name "$scriptPath\FileSystem"
Import-Module -Force -Name "$scriptPath\Net"
Import-Module -Force -Name "$scriptPath\IIS"
Import-Module -Force -Name "$scriptPath\SQL"

# load the default and custom data
. "$scriptPath\DefaultData.ps1"
. "$rootPath\NitrateConfig.ps1"

# initialize some values
$backupPath = "$rootPath\db\$DAT_SqlFileName.bak"
$backupArchive = "$rootPath\db\$DAT_SqlFileName.zip"
$resetPasswordFile = "$scriptPath\ResetAdmin.sql"

# Ensure source folders exist
FS_EnsureDir("$rootPath\db")
FS_EnsureDir("$rootPath\source")
FS_EnsureDir("$rootPath\source\modules")
FS_EnsureDir("$rootPath\source\themes")
FS_EnsureDir("$rootPath\source\media")

# utility functions
function UnZip($source, $destination)
{
	CON_WriteInfo "Unzipping $source... "
	$pathExists = Test-Path "$destination"
	if ($pathExists -eq $false){New-Item -ItemType directory -Path "$destination"}
	. "$scriptPath\tools\unzip.exe" -q "$source" -d $destination
	CON_WriteDone
}

###########################################################################################################
#
#  cleans existing environment
#
function Clean
{
	CON_WriteInfo "Cleaning up the site..." $true

	IIS_RemoveApplication $DAT_websiteName $DAT_virtualDirectoryName
	FS_UnlinkFolders "$rootPath\source\themes" "$rootPath\orchard\src\orchard.web\themes"
	FS_UnlinkFolders "$rootPath\source\modules" "$rootPath\orchard\src\orchard.web\modules"
	FS_UnlinkFolder "$rootPath\orchard\src\orchard.web\Media"
	FS_EmptyDir "$rootPath\orchard"
	SQL_DeleteDb $DAT_SqlServer $DAT_SqlInstance $DAT_SqlDatabase
	SQL_DeleteDbUser $DAT_SqlServer $DAT_SqlInstance $DAT_SqlUser

	CON_WriteGood "All done!"
}

###########################################################################################################
#
#  builds a new environment
#
function Setup
{
	Clean

	CON_WriteInfo "Retrieving Orchard source for branch $DAT_CodeBranch" $true
	. hg clone https://hg.codeplex.com/orchard "$rootPath\orchard" -b $DAT_CodeBranch
	CON_WriteDone
	
	CON_WriteInfo "Building Orchard" $true
	. msbuild "$rootPath\orchard\src\orchard.sln"
	CON_WriteDone

	FS_LinkFolders "$rootPath\source\themes" "$rootPath\orchard\src\orchard.web\themes"
	FS_LinkFolders "$rootPath\source\modules" "$rootPath\orchard\src\orchard.web\modules"
	Remove-Item "$rootPath\orchard\src\orchard.web\Media" -Force -Recurse
	FS_LinkFolder "$rootPath\source\media\" "$rootPath\orchard\src\orchard.web\Media"
	SQL_CreateDb $DAT_SqlServer $DAT_SqlInstance $DAT_SqlDatabase
	SQL_CreateDbUser $DAT_SqlServer $DAT_SqlInstance $DAT_SqlDatabase $DAT_SqlUser $DAT_SqlPassword

	CON_WriteInfo "Creating Orchard site... (this takes a while as Orchard downloads additional modules)" $true
	Push-Location
	cd "$rootPath\orchard\src\orchard.web\bin\"
	$connectionString = SQL_MakeConnectionString $DAT_SqlServer $DAT_SqlInstance $DAT_SqlDatabase $DAT_SqlUser $DAT_SqlPassword
	Write-Host $connectionString
	. ".\orchard.exe" setup /sitename:$DAT_OrchardSiteName /adminusername:$DAT_OrchardAdminUser /adminpassword:$DAT_OrchardAdminPassword /databaseprovider:SQLServer /DatabaseConnectionString:"$connectionString" /recipe:Default
	. ".\orchard.exe" feature enable Orchard.CodeGeneration
	Pop-Location
	CON_WriteDone

	RestoreDb

	IIS_CreateApplication $DAT_websiteName $DAT_virtualDirectoryName "$rootPath\orchard\src\orchard.web"

	CON_WriteGood "Your development environment is ready!"
	Write-Host "User name : $DAT_OrchardAdminUser"
	Write-Host "Password  : $DAT_OrchardAdminPassword"
}

###########################################################################################################
#
#  backs up the site's database
#
function BackupDb
{
	if (Test-Path $backupPath)
	{
		Remove-Item $backupPath
	}

	if (Test-Path $backupArchive)
	{
		Remove-Item $backupArchive
	}

	SQL_BackupDb $DAT_SqlServer $DAT_SqlInstance $DAT_SqlDatabase $backupPath

	Push-Location
	cd "$rootPath\db"
	. "$scriptPath\Tools\zip.exe" -9 -m "$DAT_SqlFileName.zip" "$DAT_SqlFileName.bak"
	Pop-Location
}

###########################################################################################################
#
#  restores the site's database
#
function RestoreDb
{
	if (Test-Path $backupArchive)
	{
		UnZip $backupArchive "$rootPath\db\"

		if (Test-Path $backupPath)
		{
			SQL_DeleteDb $DAT_SqlServer $DAT_SqlInstance $DAT_SqlDatabase
			SQL_RestoreDb $DAT_SqlServer $DAT_SqlInstance $DAT_SqlDatabase $backupPath $DAT_SqlUser $resetPasswordFile
			Remove-Item $backupPath
		}
	}
}

###########################################################################################################
#
#  Synchronizes the orchard site on an ftp server
#
function FtpSync
{
	CON_WriteInfo "Synchronizing site... "
	. "$scriptPath\tools\winscp.exe" /console /script="$scriptPath\winscp.txt" /parameter "$DAT_FtpUrl" "$rootPath\orchard\src\orchard.web" "$DAT_FtpRoot" "$rootPath\orchard\src\orchard.web\app_data\Sites\Default\Settings.txt"
	CON_WriteDone
}

###########################################################################################################
#
#  creates a new module, adds it to Orchard and the source folder.
#
function CreateModule($name)
{
	Push-Location
	CON_WriteInfo "Generating module with Orchard... " $true
	cd "$rootPath\orchard\src\orchard.web\bin\"
	. ".\orchard.exe" codegen module $name
	Pop-Location
	CON_WriteDone
	CON_WriteInfo "Moving files to source folder... "
	Move-Item "$rootPath\orchard\src\orchard.web\modules\$name" "$rootPath\source\modules\"
	CON_WriteDone
	FS_LinkFolder "$rootPath\source\modules\$name" "$rootPath\orchard\src\orchard.web\modules\$name"
}

###########################################################################################################
#
#  creates a new theme, adds it to Orchard and the source folder.
#
function CreateTheme($name)
{
	Push-Location
	CON_WriteInfo "Generating theme with Orchard... " $true
	cd "$rootPath\orchard\src\orchard.web\bin\"
	. ".\orchard.exe" codegen theme $name
	Pop-Location
	CON_WriteDone
	CON_WriteInfo "Moving files to source folder... "
	Move-Item "$rootPath\orchard\src\orchard.web\themes\$name" "$rootPath\source\themes\"
	CON_WriteDone
	FS_LinkFolder "$rootPath\source\themes\$name" "$rootPath\orchard\src\orchard.web\themes\$name"
}

###########################################################################################################
#
#  Shell
#
function Shell
{
	Push-Location
	cd "$rootPath\orchard\src\orchard.web\bin\"
	. ".\orchard.exe"
	Pop-Location
}

###########################################################################################################
#
#  Main
#
switch ($args[0])
{
	"clean" {
		Clean
	}
	"setup" {
		Setup
	}
	"backup-db" {
		BackupDb
	}
	"load-db" {
		RestoreDb
	}
	"ftp-sync" {
		FtpSync
	}
	"create-module" {
		if ($args[1] -match "^[\w\.]+$")
		{
			CreateModule $args[1]
		}
		else
		{
			Write-Host
			Write-Host "Invalid module name!"
			Write-Host
			Write-Host "Syntax: Nitrate.ps1 create-module <module_name>"
			Write-Host
		}
	}
	"create-theme" {
		if ($args[1] -match "^[\w\.]+$")
		{
			CreateTheme $args[1]
		}
		else
		{
			Write-Host
			Write-Host "Invalid theme name!"
			Write-Host
			Write-Host "Syntax: Nitrate.ps1 create-theme <theme_name>"
			Write-Host
		}
	}
	"shell" {
	  Shell
	}
	default {
		Write-Host
		Write-Host "Usage: Nitrate.ps1 <command>"
		Write-Host
		Write-Host "Available commands:"
		Write-Host " - clean:         cleans existing environment."
		Write-Host " - setup:         builds a new environment."
		Write-Host " - backup-db:     backs up the site's database."
		Write-Host " - load-db:       restores the site's database."
		Write-Host " - ftp-sync:      synchronizes the orchard site on an ftp server."
		Write-Host " - create-module: creates a new module, adds it to Orchard and the source folder."
		Write-Host " - create-theme:  creates a new theme, adds it to Orchard and the source folder."
		Write-Host " - shell:         runs the Orchard command line."
		Write-Host
	}
}
