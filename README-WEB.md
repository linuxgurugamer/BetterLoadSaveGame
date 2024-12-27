Originally written by @jefftimlin, original thread here: https://forum.kerbalspaceprogram.com/index.php?/topic/162619-14-17-betterloadsavegame-24/

An addon for Kerbal Space Program designed to improve the user interface for loading save games.

I was always frustrated with the in-game GUI for loading saved games, as it lists them in alphabetical order making it really hard to find the save you want when there's lots in the list. It also gets really slow to open once you have a lot of saves.

This mod attempts to fix that by:

Listing the saves by creation time (descending)
Including thumbnail screenshots
Looks like this:

https://github.com/jefftimlin/BetterLoadSaveGame/raw/master/screenshot.jpg

Dependencies

Click Through Blocker
ToolbarController
SpaceTuxLibrary
Availability

Source: https://github.com/linuxgurugamer/BetterLoadSaveGame/
Download: https://spacedock.info/mod/2326/Better Load Save Game Renewed
License: MIT
Installation
Available for install via CKAN.

Alternatively, download the latest release and extract the contents of the GameData folder into the GameData folder inside your KSP install.

Usage
Note for best screenshot quality, ensure "Texture Quality" is set to "Full Res" in your game options (under "Graphics").

Settings are available in the stock Settings pages.  The following options are available:

In the General column:

Replace stock Load Game dialog on F9    This replaces the Load Game dialog, if disabled, then F7 opens the window
Don't show the persistent.sfs file  
Use alternate skin                                          A more compact skin which a number of mods use
In the Deletion/Archive column:

Archive old saves               Archive and Delete are exclusive, only one can be active.
Delete old saves                 Both can be disabled to disable this functionality
Unit: Hours                          Hours and Days are mutually exclusive
Unit: Days
File age in specified units     The number of Hours/Days a save has to be to be either archived or deleted
The BLSG window has a number of toggles and buttons:
At the top are:

Use Archives    A toggle which has the mod show all the archived saves
Sort                    One of three different ways to sort the saves in the list
At the bottom are the following buttons:

Delete       Delete selected save
Archive     Move the selected save into the Archive directory
Cancel      Close the dialog
Load         Load the selected save
Recommended mods
Highly recommend also using a mod like Dated Quick Saves or Kerbal Improved Save System to address my other favourite problem in the game of only having one quicksave slot when you press F5.

Current state
Currently has basic features including screenshots, summary info, sorting and filtering.

