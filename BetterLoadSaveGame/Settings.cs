
using System.Collections;
using System.Reflection;

namespace BetterLoadSaveGame
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    // HighLogic.CurrentGame.Parameters.CustomParams<BLSG1>().useAlternateSkin
    public class BLSG1 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "General"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "BetterLoadSaveGame"; } }
        public override string DisplaySection { get { return "Better Load Save Game"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return true; } }


        [GameParameters.CustomParameterUI("Replace stock Load Game dialog on F9",
            toolTip = "If enabled, the F7 key won't be used to open BLSG")]
        public bool replaceStock = true;


        [GameParameters.CustomParameterUI("Don't show the persistent.sfs file")]
        public bool hidePersistent = false;

        [GameParameters.CustomParameterUI("Use alternate skin",
            toolTip = "Use a more minimiliast skin")]
        public bool useAlternateSkin = false;


        public override void SetDifficultyPreset(GameParameters.Preset preset) { }
        public override bool Enabled(MemberInfo member, GameParameters parameters) { return true; }
        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }
        public override IList ValidValues(MemberInfo member) { return null; }
    }

    public class BLSG2 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Deletion/Archive"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "BetterLoadSaveGame"; } }
        public override string DisplaySection { get { return "Better Load Save Game"; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets { get { return false; } }

        bool oldArchiveSaves, oldDeleteSaves;
        bool initted = false;

        [GameParameters.CustomParameterUI("Archive old saves",
            toolTip = "Automatically archive saves older than the specified age")]
        public bool archiveSaves = true;

        [GameParameters.CustomParameterUI("Delete old saves",
            toolTip = "Automatically delete saves older than the specified age")]
        public bool deleteSaves = false;


        bool oldHourUnit, oldDayUnit;

        [GameParameters.CustomParameterUI("Unit:  Hours",
            toolTip = "Age of the save will be measured in Hours")]
        public bool hourUnit = false;
        [GameParameters.CustomParameterUI("Unit:  Days",
            toolTip = "Age of the save will be measured in Days")]
        public bool dayUnit = true;

        [GameParameters.CustomIntParameterUI("File age in specified units", minValue = 1, maxValue = 1440)]
        public int fileAge = 5;

        public override void SetDifficultyPreset(GameParameters.Preset preset) { }

        public override bool Enabled(MemberInfo member, GameParameters parameters) { return true; }


        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            if (!initted)
            {
                initted = true;
                oldArchiveSaves = archiveSaves;
                oldDeleteSaves = deleteSaves;
                oldHourUnit = hourUnit;
                oldDayUnit = dayUnit;
            }

            if (archiveSaves && !oldArchiveSaves)
                deleteSaves = false;
            if (deleteSaves && !oldDeleteSaves)
                archiveSaves = false;

            oldDeleteSaves = deleteSaves;
            oldArchiveSaves = archiveSaves;
            if (hourUnit != oldHourUnit)
                dayUnit = !hourUnit;
            if (dayUnit != oldDayUnit)
                hourUnit = !dayUnit;

            oldHourUnit = hourUnit;
            oldDayUnit = dayUnit;

            return true;
        }

        public override IList ValidValues(MemberInfo member) { return null; }
    }
}