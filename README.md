# BetterLoadSaveGame

An addon for Kerbal Space Program designed to improve the user interface for loading save games.

I was always frustrated with the in-game GUI for loading saved games, as it lists them in alphabetical order making it really hard to find the save you want when there's lots in the list. It also gets really slow to open once you have a lot of saves.

This mod attempts to fix that by:
* Listing the saves by creation time (descending)
* Including thumbnail screenshots

Looks like this:

![screenshot](screenshot.jpg)

## Installation

Available for install via CKAN.

Alternatively, download the latest release and extract the contents of the GameData folder into the GameData folder inside your KSP install.

## Usage

Note for best screenshot quality, ensure "Texture Quality" is set to "Full Res" in your game options (under "Graphics").

Usage

Settings are available in the stock Settings pages.  The following options are available:

In the General column:
    Replace stock Load Game dialog on F9    This replaces the Load Game dialog, if disabled, then F7 opens the window
    Don't show the persistent.sfs file  
    Use alternate skin                      A more compact skin which a number of mods use

In the Deletion/Archive column:
    Archive old saves               Archive and Delete are exclusive, only one can be active.
    Delete old saves                Both can be disabled to disable this functionality
    Unit: Hours                     Hours and Days are mutually exclusive
    Unit: Days
    File age in specified units     The number of Hours/Days a save has to be to be either archived or deleted

The BLSG window has a number of toggles and buttons:
At the top are:

    Use Archives    A toggle which has the mod show all the archived saves
    Sort            One of three different ways to sort the saves in the list

At the bottom are the following buttons:

    Delete      Delete selected save
    Archive     Move the selected save into the Archive directory
    Cancel      Close the dialog
    Load        Load the selected save



## Recommended mods

Highly recommend also using a mod like [Dated Quick Saves](http://forum.kerbalspaceprogram.com/index.php?/topic/97033-13122-magico13s-modlets-sensible-screenshot-dated-quicksaves-etc/) or [Kerbal Improved Save System](http://forum.kerbalspaceprogram.com/index.php?/topic/138001-130-kiss-kerbal-improved-save-system/) to address my other favourite problem in the game of only having one quicksave slot when you press F5.

## Current state

Currently has basic features including screenshots, summary info, sorting and filtering.

I'm keen to make further improvements to it. If you have any ideas for new features or improvements, or if you find any bugs, please let me know!

In particular I'm not aware of any way to replace the official "Load Game" feature in the game menu, which is why you need to press a key (F7) to open the new dialog. If you have any idea how to fix this, please let me know.

## Change log

2.4
 - Rebuilt for KSP 1.7 (also tested working with 1.4, 1.5, 1.6)
 - Improved error checking when resizing screenshots (thanks linuxgurugamer)

2.3
 - Fixed a bug where loading games that were saved at the space center would just load the "persistent" save instead.

2.2
 - Fixed duplicate named saves appearing in the list
 - Fixed missing screenshots when multiple saves made at the same time

2.1
 - Fix scroll position not being reset when showing window

2.0
 - Major rewrite to improve performance. Now opens and scrolls much faster, and uses less space on disk for screenshots.

1.4
 - Rebuilt for KSP 1.4.1

1.3
 - Reduced lag when loading screenshots while scrolling
 - Reset scroll position when showing window
 - Fix screenshots not being displayed when filtering
 - Fix broken funds display when loadmeta file missing

1.2
 - Increased screenshot and window size
 - Reduced memory usage
 - New placeholder image

1.1
 - Added KSP 1.3 support

1.0
 - Initial release
