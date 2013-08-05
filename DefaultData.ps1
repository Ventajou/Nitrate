#
# This is the default data for Nitrate, edit the values in this file to configure your environment
# Any value deleted will use the default value from Nitrate
#
# You can also create a file alongside this one called NitrateLocal.ps1.
# It will override the values from this file, so if you exclude it from source control 
# you can have a computer specific setup, useful for working in a team.	
#

###############
# IIS Settings
$DAT_useIIS			  		= $true			# set to $false if you'd rather use IIS Express for example
$DAT_websiteName      		= "Default Web Site"
$DAT_appPoolName      		= ".Net v4.5"
$DAT_webAppName       		= "Orchard"		# set to "" if you want orchard to be at the site's root
$DAT_protocol         		= "http"		# required if webAppName is ""
$DAT_bindingInfo      		= "*:80:"		# required if webAppName is ""
$DAT_canDeleteWebside 		= $false		# only set to $true if you're not sharing the site with other apps

###################
# Orchard Settings
$DAT_OrchardSiteName 		= "Orchard Test"
$DAT_OrchardAdminUser 		= "admin"
$DAT_OrchardAdminPassword 	= "password"
$DAT_CopySolution 			= $false		# this will copy Orchard.sln to the \source folder and create a symlink to it in \orchard\src

######################
# SQL Server settings
$DAT_SqlServer 				= "localhost"
$DAT_SqlInstance 			= "DEFAULT"
$DAT_SqlDatabase 			= "orchard"
$DAT_SqlUser 				= "orchard_user"
$DAT_SqlPassword 			= "password"
$DAT_SqlFileName 			= "orchard"

####################
# FTP sync settings
$DAT_FtpUrl 				= ""
$DAT_FtpRoot 				= ""

#########################################
# Name of the Orchard code branch to use
$DAT_CodeBranch 			= "master"  	# this is the branch on Codeplex
