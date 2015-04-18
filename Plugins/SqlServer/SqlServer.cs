using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;

// Help: http://www.codeproject.com/Articles/23823/Create-a-SQL-Database-Programmatically

namespace Nitrate.Plugins.SqlServer
{
	public class SqlServerConfig
	{
		public string Server { get; set; }
		public string Instance { get; set; }
		public string Database { get; set; }
		public string DbUser { get; set; }
		public string DbPass { get; set; }
		public string BackupFile { get; set; }
		public string UserSchema { get; set; }
	}

	[Export(typeof(IPlugin))]
	public class SqlServer : BasePlugin<SqlServerConfig>
	{
		private static class Commands
		{
			public const string Create = "create";
			public const string Backup = "backup";
			public const string Restore = "restore";
			public const string Drop = "drop";
		}

		private static class CreateArguments
		{
			public const string Force = "f";
		}

		private static class BackupArguments
		{
			public const string Force = "f";
		}

		public override string Description
		{
			get { return "Manages SQL Server databases"; }
		}

		private IDictionary<string, SubCommand> _subCommands;
		public override IDictionary<string, SubCommand> SubCommands
		{
			get
			{
				if (_subCommands == null)
				{
					_subCommands = new Dictionary<string, SubCommand>() {
						{
							Commands.Create,
							new SubCommand() {
								Description = "Creates the database",
								Example = "[-f]",
								Arguments = new List<Argument>() {
									new Argument() {
										Name = CreateArguments.Force,
										Description = "Forces recreation of the databsase if it already exists",
										Regex = "^-f$"
									}
								}
							}
						},
						{
							Commands.Backup,
							new SubCommand() {
								Description = "Backs up the database",
								Example = "[-f]",
								Arguments = new List<Argument>() {
									new Argument() {
										Name = CreateArguments.Force,
										Description = "Forces deletion of the previous backup if any",
										Regex = "^-f$"
									}
								}
							}
						},
						{ Commands.Restore, new SubCommand() { Description = "Restores a database backup" } },
						{ Commands.Drop, new SubCommand() { Description = "Removes the database" } }
					};
				}
				return _subCommands;
			}
		}

		protected override Dictionary<string, SqlServerConfig> SampleConfiguration()
		{
			return new Dictionary<string, SqlServerConfig>()
			{
				{
					"Orchard", new SqlServerConfig
					{
						BackupFile = @"db\orchard.bak",
						Database = "orchard",
						DbUser = "orchard_user",
						DbPass = "ocd_1234"
					}
				}
			};
		}

		public override bool IsAvailable()
		{
			return true;
		}

		public override void Execute(string configName, SqlServerConfig config, string subCommand, Dictionary<string, string> args)
		{
			Server server;
			if (string.IsNullOrWhiteSpace(config.Server))
				server = new Server();
			else
				server = new Server(config.Server);

			switch (subCommand)
			{
				case Commands.Create:
					CreateDatabase(config, server, args.ContainsKey(CreateArguments.Force));
					break;
				case Commands.Drop:
					DropDatabase(config, server);
					break;
				case Commands.Backup:
					BackupDatabase(config, server, args.ContainsKey(CreateArguments.Force));
					break;
				case Commands.Restore:
					RestoreDatabase(config, server);
					break;
			}
		}

		private static void RestoreDatabase(SqlServerConfig config, Server server)
		{
			var backupPath = Path.Combine(Config.Current.Path, config.BackupFile);
			var restore = new Restore()
			{
				Database = config.Database,
				Action = RestoreActionType.Database,
				ReplaceDatabase = true,
				NoRecovery = false
			};
			restore.Devices.AddDevice(backupPath, DeviceType.File);
			restore.PercentComplete += CompletionStatusInPercent;
			restore.Complete += Restore_Completed;

			var fileList = restore.ReadFileList(server);
			var relocationTable = new Dictionary<string, string>();
			foreach (DataRow row in fileList.Rows)
			{
				var logicalName = row["LogicalName"].ToString();
				var physicalName = Path.GetFileName(row["PhysicalName"].ToString());
				if (row["Type"].ToString() == "L")
				{
					relocationTable.Add(logicalName, Path.Combine(server.Settings.DefaultLog, physicalName));
				}
				else
				{
					relocationTable.Add(logicalName, Path.Combine(server.Settings.DefaultFile, physicalName));
				}
			}

			foreach (KeyValuePair<string, string> pair in relocationTable)
			{
				restore.RelocateFiles.Add(new RelocateFile(pair.Key, pair.Value));
			}

			server.KillAllProcesses(restore.Database);

			var db = server.Databases[config.Database];
			if (db != null)
			{
				db.DatabaseOptions.UserAccess = DatabaseUserAccess.Single;
				db.Alter(TerminationClause.RollbackTransactionsImmediately);
				server.DetachDatabase(restore.Database, false);
			}

			restore.Action = RestoreActionType.Database;
			restore.ReplaceDatabase = true;

			restore.SqlRestore(server);
			db = server.Databases[config.Database];
			db.SetOnline();
			server.Refresh();
			db.DatabaseOptions.UserAccess = DatabaseUserAccess.Multiple;

			EnsureUser(config, server, db);
			//           db.SetOwner(config.DbUser, true);
		}

		private static void BackupDatabase(SqlServerConfig config, Server server, bool force)
		{
			var backupPath = Path.Combine(Config.Current.Path, config.BackupFile);

			if (File.Exists(backupPath))
			{
				if (force)
					File.Delete(backupPath);
				else
				{
					Shell.Error("The backup file already exists, use -f to delete it.");
					return;
				}
			}

			var backupDirectory = Path.GetDirectoryName(backupPath);
			if (!Directory.Exists(backupDirectory)) Directory.CreateDirectory(backupDirectory);

			Backup backup = new Backup()
			{
				Action = BackupActionType.Database,
				Database = config.Database,
				BackupSetName = "Nitrate Backup",
				Initialize = true
			};
			backup.Devices.AddDevice(backupPath, DeviceType.File);
			backup.PercentComplete += CompletionStatusInPercent;
			backup.Complete += Backup_Completed;

			backup.SqlBackup(server);
		}

		private static void DropDatabase(SqlServerConfig config, Server server)
		{
			Shell.Info("Dropping database " + config.Database + "...");
			var db = server.Databases[config.Database];
			if (db != null)
			{
				db.Drop();
				Shell.Success("Done.");
			}
			else Shell.Info("The database does not exist.");
		}

		private static void CreateDatabase(SqlServerConfig config, Server server, bool force)
		{
			Shell.Info("Creating database " + config.Database + "...");
			var db = server.Databases[config.Database];
			if (db != null)
			{
				if (force)
				{
					Shell.Info("Database already exists, dropping it first.");
					db.Drop();
				}
				else
				{
					Shell.Error("The database already exists, use -f to recreate it.");
					return;
				}
			}
			db = new Database(server, config.Database);
			db.Create();

			EnsureUser(config, server, db);
			db.SetOwner(config.DbUser, true);

			Shell.Success(config.Database + " was created successfully.");
		}

		private static void EnsureUser(SqlServerConfig config, Server server, Database db)
		{
			Login login;
			if (server.Logins.Contains(config.DbUser))
			{
				login = server.Logins[config.DbUser];
				// Ensure the password is correct
				login.ChangePassword(config.DbPass);
			}
			else
			{
				login = new Login(server, config.DbUser);
				login.LoginType = LoginType.SqlLogin;
				login.Create(config.DbPass);
			}

			var user = new User(db, config.DbUser);
			user.Login = config.DbUser;
			user.Create();
			user.AddToRole("db_owner");
			if (!string.IsNullOrWhiteSpace(config.UserSchema))
			{
				user.DefaultSchema = config.UserSchema;
				user.Alter();
			}
		}

		private static void CompletionStatusInPercent(object sender, PercentCompleteEventArgs args)
		{
			Shell.Write(String.Format("\r{0}%.    ", args.Percent), false);
		}

		private static void Backup_Completed(object sender, ServerMessageEventArgs args)
		{
			Shell.Write("\r", false);
			Shell.Success("Backup completed.");
			Shell.Success(args.Error.Message);
		}

		private static void Restore_Completed(object sender, ServerMessageEventArgs args)
		{
			Shell.Write("\r", false);
			Shell.Success("Restore completed.");
			Shell.Success(args.Error.Message);
		}
	}
}
