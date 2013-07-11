Nitrate - Grow your Orchard
===

Nitrate is a set of scripts aimed at making your life easier when developing with the Orchard CMS.

Features
---

* Commit only your code to source control.
* Easily build and keep a consistent environment across a team.
* Automatically configure IIS and SQL Server.
* Easy database backup and restore (useful to test migrations or 3rd party modules).
* Git style commands work from anywhere in your environment path.
* Synchronize your site with FTP (experimental)
* Anything you contribute!

Requirements
---

* .NET Framework 4.5
* IIS with ASP.NET support
* Mercurial client
* PowerShell
* Visual Studio 2010 or 2012
* A Microsoft SQL Server instance accessible
	* Use Mixed Mode Authentication
* The SQL Server SMO:
	* [32bits package for SQL Server 2012](http://go.microsoft.com/fwlink/?LinkID=239658&clcid=0x409)
	* [64bits package for SQL Server 2012](http://go.microsoft.com/fwlink/?LinkID=239659&clcid=0x409)
* The SQL Server provider for PowerShell:
	* [32bits package for SQL Server 2012](http://go.microsoft.com/fwlink/?LinkID=239655&clcid=0x409)
	* [64bits package for SQL Server 2012](http://go.microsoft.com/fwlink/?LinkID=239656&clcid=0x409)
* [SQL Server PowerShell Extensions](http://sqlpsx.codeplex.com/)

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

It will take a little while because the Orchard source code is downloaded and compiled. But once done, you'll have an Orchard environment ready for setup.

File Organization
---

One goal of Nitrate is to separate your code from Orchard's so that you don't need to commit all of Orchard into your source control system. A standard Nitrate setup contains the following folders:

* **orchard**: the Orchard source code, make sure your source control client ignores that folder.
* **source**: should contain your source code.
	* **modules**: contains your modules. Each folder in there is symlinked to the Orchard modules folder by the **setup** command.
	* **modules**: contains your themes. Each folder in there is symlinked to the Orchard themes folder by the **setup** command.
	* **media**: this folder is symlinked to the Orchard media folder by the **setup** command.
* **db**: contains your database backup

If you wish to create a separate solution for your themes and modules, I recommend saving the .sln file under the source folder. Also when you use the create-module command, you will need to adjust the location of the referenced Orchard assemblies.   

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
* **shell**: runs the Orchard command line.
* **build-recipe**: creates a recipe file that contains the list of modules and themes downloaded from the gallery.

License
---

Copyright 2013 André Rieussec

Licensed under the Apache License, Version 2.0 (the "License"); you may not use any file in this project except in compliance with the License. You may obtain a copy of the License at

<http://www.apache.org/licenses/LICENSE-2.0>

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.