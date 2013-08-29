Push-Location
Import-Module SQLPS -DisableNameChecking
import-module sqlserver -force
Pop-Location

function Get-SqlDefaultDir
{
    param(
    [Parameter(Position=0, Mandatory=$true)] $sqlserver,
    [ValidateSet("Data", "Log")]
    [Parameter(Position=1, Mandatory=$true)] [string]$dirtype
    )

    switch ($sqlserver.GetType().Name)
    {
        'String' { $server = Get-SqlServer $sqlserver }
        'Server' { $server = $sqlserver }
        default { throw 'Get-SqlDefaultDir:Param sqlserver must be a String or Server object.' }
    }

    Write-Verbose "Get-SqlDefaultDir $($server.Name)"

    #I thought about adding this properties to the server object in Get-SqlServer, but felt it was important
    #not to mask whether the default directories had been set or not. You should always set the default directories as
    #a configuration task
    switch ($dirtype)
    {
        'Data'  { if ($server.DefaultFile) { $server.DefaultFile } else { $server.InstallDataDirectory + '\' + 'Data' } }
        'Log'   { if ($server.DefaultLog) { $server.DefaultLog } else { $server.InstallDataDirectory + '\' + 'Data' } }
    }
}

function SQL_CreateDb($server, $instance, $database)
{
	SQL_DeleteDb $server $instance $database
	CON_WriteInfo "Creating database [$database] on $server\$instance... "
	$db = New-Object Microsoft.SqlServer.Management.SMO.Database
    $db.Parent = Get-Item "SQLSERVER:\SQL\$server\$instance"
    $db.Name = $database
    $db.Create()
	CON_WriteDone
}

function SQL_DeleteDb($server, $instance, $database)
{
	$dbPath = "SQLSERVER:\SQL\$server\$instance\Databases\$database"
	if (Test-Path $dbPath)
	{
		CON_WriteInfo "Database [$database] found on $server\$instance, deleting... "

		$serverName = "$server\$instance"
		if ($instance -eq "DEFAULT")
		{
			$serverName = $server
		}
		$dbServer = get-sqlserver $serverName
		$dbServer.KillAllProcesses($database)

		Remove-Item $dbPath
		CON_WriteDone
	}
}

function SQL_CreateDbUser($server, $instance, $database, $user, $password)
{
	SQL_DeleteDbUser $server $instance $user

	CON_WriteInfo "Creating login [$user] as owner of [$database] on $server\$instance... "
	$db = Get-Item "SQLSERVER:\SQL\$server\$instance\Databases\$database"

	$login = New-Object Microsoft.SqlServer.Management.SMO.Login
    $login.Parent = Get-Item "SQLSERVER:\SQL\$server\$instance"
    $login.Name = $user
	$login.LoginType = 2
	$login.DefaultDatabase = $database
	$login.Create("$password")

  $schema = New-Object Microsoft.SqlServer.Management.SMO.Schema $db, "$user"
  $schema.create()

  $userObject = New-Object Microsoft.SqlServer.Management.SMO.User $db, "$user"
  $userObject.Login = "$user"
  $userObject.DefaultSchema = "dbo"
  $userObject.create()

  $userObject.AddToRole("db_owner")

	CON_WriteDone
}

function SQL_DeleteDbUser($server, $instance, $user)
{
	$dbPath = "SQLSERVER:\SQL\$server\$instance\Logins\$user"
	if (Test-Path $dbPath)
	{
		CON_WriteInfo "Login [$user] found on $server\$instance, deleting... "
		Remove-Item $dbPath
		CON_WriteDone
	}
}

function SQL_MakeConnectionString($server, $instance, $database, $user, $password)
{
	if ($instance -eq "DEFAULT")
	{
		"Data Source=$server;Initial Catalog=$database;User Id=$user; Password=$password;"
	}
	else
	{
		"Data Source=$server\$instance;Initial Catalog=$database;User Id=$user; Password=$password;"
	}
}

function SQL_BackupDb($server, $instance, $database, $backupFile)
{
	CON_WriteInfo "Backing up database [$database] on $server\$instance... "
	$serverName = "$server\$instance"
	if ($instance -eq "DEFAULT")
	{
		$serverName = $server
	}
	Backup-SqlDatabase -serverInstance $serverName -Database $database -BackupFile $backupFile
	CON_WriteDone
}

function SQL_RestoreDb($server, $instance, $database, $backupFile, $user, $resetPasswordFile)
{
	CON_WriteInfo "Restoring database [$database] on $server\$instance... "

	$serverName = "$server\$instance"
	if ($instance -eq "DEFAULT")
	{
		$serverName = $server
	}

	$dbServer = get-sqlserver $serverName

	$filepath = Resolve-Path $backupFile | select -ExpandProperty Path
	$dbname = $database

	$dataPath = Get-SqlDefaultDir -sqlserver $dbServer -dirtype Data
	$logPath = Get-SqlDefaultDir -sqlserver $dbServer -dirtype Log

	$relocateFiles = @{}
	Invoke-SqlRestore -sqlserver $dbServer  -filepath $filepath -fileListOnly | foreach { `
		if ($_.Type -eq 'L')
		{ $physicalName = "$logPath\{0}" -f [system.io.path]::GetFileName("$($_.PhysicalName)") }
		else
		{ $physicalName = "$dataPath\{0}" -f [system.io.path]::GetFileName("$($_.PhysicalName)") }
		$relocateFiles.Add("$($_.LogicalName)", "$physicalName")
	}

	$dbServer.KillAllProcesses($dbname)
	Invoke-SqlRestore -sqlserver $dbServer -dbname $dbname -filepath $filepath -relocatefiles $relocateFiles -force

	$db = Get-Item "SQLSERVER:\SQL\$server\$instance\Databases\$database"
	$db.SetOwner("$user")

  invoke-sqlcmd -inputfile "$resetPasswordFile" -serverinstance "$serverName" -database "$database"

	CON_WriteDone
}

Export-ModuleMember SQL_CreateDb, SQL_DeleteDb, SQL_CreateDbUser, SQL_DeleteDbUser, SQL_DeleteDbUser, SQL_MakeConnectionString, SQL_BackupDb, SQL_RestoreDb
