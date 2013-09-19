Nitrate - Grow your Orchard faster
===

Nitrate is a set of scripts aimed at making your life easier when developing with the Orchard CMS.

Features
---

* Commit only your code to source control.
* Easily build and keep a consistent environment across a team.
* Automatically configure IIS and SQL Server.
* Easy database backup and restore (useful to test migrations or 3rd party modules).
* Git style commands work from anywhere in your repository path.
* Only adds one small configuration file to your repository. 
* Synchronize your site with FTP (experimental)
* Anything you contribute!

Requirements
---

Nitrate helps grow your Orchard but it's also a pollutant, andit requires a number of applications and tools which you may not yet have installed.

* .NET Framework 4.5
* IIS with ASP.NET support
* Git command line client (to retrieve Orchard and also for some tools it includes)
* PowerShell 3.0 (2.0 reported to work but not officially supported)
* Visual Studio 2010 or 2012
* A Microsoft SQL Server instance accessible
	* Use Mixed Mode Authentication
* The SQL Server SMO:
	* [32bits package for SQL Server 2012](http://go.microsoft.com/fwlink/?LinkID=239658&clcid=0x409)
	* [64bits package for SQL Server 2012](http://go.microsoft.com/fwlink/?LinkID=239659&clcid=0x409)
* The SQL Server provider for PowerShell:
	* [32bits package for SQL Server 2012](http://go.microsoft.com/fwlink/?LinkID=239655&clcid=0x409)
	* [64bits package for SQL Server 2012](http://go.microsoft.com/fwlink/?LinkID=239656&clcid=0x409)

Installation
---

Simply copy the content of the Nitrate repo somewhere, for example:

	:::powershell
    git clone git@bitbucket.org:Ventajou/nitrate.git

To make your life easier, I strongly recommend creating an alias to Nitrate in your PowerShell profile. To do that, create or edit **Microsoft.PowerShell_profile.ps1** in your Windows Documents folder and add the following line:

	:::powershell
	New-Alias no3 <path to nitrate>\nitrate.ps1

Quick Start
---

You will need a PowerShell command line which provides you with the Visual Studio environment settings, an example of how do do that can be found [on my blog](http://ventajou.com/all-in-one-command-prompt-for-windows-developers).

Once installed, open a new PowerShell session (so it picks up the alias) and go into an empty directory, then run:

	no3 init

This will simply create a configuration file for Nitrate in this folder, the file is called NitrateConfig.ps1 and it contains the default settings. Feel free to edit any of those settings to match your environment and preferences.

Once you're ready to set up Orchard, type:

	no3 setup

It will take a little while because the Orchard source code is downloaded and compiled. But once done, you'll have an Orchard environment ready for customization.

Using Visual Studio
---

If you set the `$DAT_CopySolution` setting to `$true` in your configuration file, Nitrate will move the Orchard solution file to the Source directory and create a symbolic link to it from `.\Orchard\src\Orchard.sln`. To create modules and themes, use the Nitrate commands `create-modules` and `create-theme`, they call the corresponding Orchard codegen commands but they also move the files into your source folder and automatically create symlinks. The main issue with this method is you will have some manual tweaking to do if you want to update Orchard later.

The default setting does not perform that copy, the idea being that you may want to create your own solution file and just add the Orchard core solution inside a solution folder by using Visual Studio's _add existing project_ feature. It will be easier to upgrade Orchard, but you will have to manually add the new projects you create from the command line. 

File Organization
---

One goal of Nitrate is to separate your code from Orchard's so that you don't need to commit all of Orchard into your source control system. A standard Nitrate setup contains the following folders:

* **orchard**: the Orchard source code, make sure your source control client ignores that folder.
* **source**: should contain your source code.
	* **modules**: contains your modules. Each folder in there is symlinked to the Orchard modules folder by the **setup** command.
	* **themes**: contains your themes. Each folder in there is symlinked to the Orchard themes folder by the **setup** command.
	* **media**: this folder is symlinked to the Orchard media folder by the **setup** command.
* **db**: contains your database backup

Source Control
---

Make sure you do not commit the Orchard folder to source control. If you use Git, add it to the .gitignore file.

Team Environment
---

When working in a team, developers may want to have a slightly different setup from each other. Nitrate supports a local configuration file: `NitrateLocal.ps1`, when placed alongside `NitrateConfig.ps1` it will be automatically loaded and override the project settings. The idea being to only commit `NitrateConfig.ps1` to source control as a baseline configuration.

Command Reference
---

* **init**: initializes a new Nitrate environment.
* **clean**: cleans existing environment.
* **setup**: builds a new environment.
* **backup-db**: backs up the site's database.
* **load-db**: restores the site's database.
* **ftp-sync**: synchronizes the orchard site on an ftp server.
* **create-module**: creates a new module, adds it to Orchard and the source folder.
* **create-theme**: creates a new theme, adds it to Orchard and the source folder.
* **shell**: runs the Orchard command line, with optional parameters.
* **build-recipe**: creates a recipe file that contains the list of modules and themes downloaded from the gallery.
* **rebuild-links**: removes and restores all the symlinks to your source folders and files.
* **log**: tails the log file.

License
---

Copyright 2013 André Rieussec

Licensed under the Apache License, Version 2.0 (the "License"); you may not use any file in this project except in compliance with the License. You may obtain a copy of the License at

<http://www.apache.org/licenses/LICENSE-2.0>

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.