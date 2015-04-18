# Nitrate
[![Build status](https://ci.appveyor.com/api/projects/status/3otmy0mo3yw2qmpl?svg=true)](https://ci.appveyor.com/project/Ventajou71375/nitrate)

A command line tool to help Windows developers quickly configure an environment.

## Introduction

Have you ever spent hours setting up your development environment after installing a new computer? 
Have you ever tried to help a coworker troubleshoot an issue on their computer and been caught in how different their setup
is from yours? Have you ever been annoyed at how much time you spend doing repetitive tasks and wished they should be simple?

Nitrate was designed with such situations in mind. With a single configuration file, you can define what your software project
needs to run properly on a developer machine.

## Getting started

Install Nitrate using [Chocolatey](https://chocolatey.org/):

    choco install nitrate

Go to your project's directory and type:

    no3 init

This will initialize a default configuration suitable to work with the Orchard CMS. You can open the ```nitrate.json``` file and
configure it to your liking. By adding it to source control alongside your project, you can make your configuration settings available
to your teammates.

## Plugins

The following plugins are available out of the box:
- Git: clones git repositories into your project and keeps them up to date.
- IIS Express: starts and stops IIS Express instances on a given path.
- MSBuild: runs the MSBuild tool to compile things or do whatever else MSBuild does.
- Orchard: creates Orchard CMS sites.
- Run: runs a sequence of Nitrate commands.
- SQL Server: creates, backs up and restores SQL Server databases.
- Symlinks: adds or removes NTFS symbolic links.

Plugins support multiple configurations in the ```nitrate.json``` file. For example, you can configure several
SQL Server databases.

## Running commands

Nitrate commands use the following pattern:

    no3 <plugin>[:<config>] [<subcommand>] [<arguments>]

- ```plugin``` is the name of the plugin.
- ```config``` is the name of the plugin's configuration as defined in ```nitrate.json```. When omitted, all configurations are run.
- ```subcommand``` is a plugin specific subcommand, when supported by the plugin.
- ```arguments``` some plugins support arguments.

There are two special commands that do not follow this syntax:

- ```init``` is used to create the default configuration file in the current directory.
- ```help``` shows the help screen. You can optionally provide a plugin name for more details.

The Run plugin can be used to combine multiple commands into one.

## Overriding configuration

You can create a ```nitrate.local.json``` file alongside ```nitrate.json```, anything defined there will override the main configuration.
This file is intended to be kept out of source control and allow you do tweak settings specific to your machine.

## Extending Nitrate

Take a look at the code, plugins are for the most part rather simple and self explanatory.
You should be able to write your own plugins in no time.
