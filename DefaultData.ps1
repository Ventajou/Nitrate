#
# This is the default data for Nitrate, edit the values in this file to configure your environment
#

###############
# IIS Settings
$DAT_websiteName = "Default Web Site"
$DAT_virtualDirectoryName = "Orchard"

###################
# Orchard Settings
$DAT_OrchardSiteName = "Orchard Test"
$DAT_OrchardAdminUser = "admin"
$DAT_OrchardAdminPassword = "password"

######################
# SQL Server settings
$DAT_SqlServer = "localhost"
$DAT_SqlInstance = "DEFAULT"
$DAT_SqlDatabase = "orchard"
$DAT_SqlUser = "orchard_user"
$DAT_SqlPassword = "password"
$DAT_SqlFileName = "orchard"

####################
# FTP sync settings
$DAT_FtpUrl = ""
$DAT_FtpRoot = ""

#########################################
# Name of the Orchard code branch to use
$DAT_CodeBranch = "1.x"  # this is the branch on Codeplex, at the moment it should probably remain at 1.x for everyone, unless you use an old version of Orchard
$DAT_CodeTag	= "tip"  # this is the tag or commit hash, if you work with 1.6, then put "1.6" there, I didn't see a 1.6.1 tag
