
using System.Collections;
using System.Reflection;

namespace BetterLoadSaveGame
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    // HighLogic.CurrentGame.Parameters.CustomParams<BLSG1>().useAlternateSkin
    public class BLSG1 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "#LOC_BLSG_GENERAL"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "BetterLoadSaveGame"; } }
        public override string DisplaySection { get { return "Better Load Save Game"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return true; } }


        [GameParameters.CustomParameterUI("#LOC_BLSG_REPLACE_STOCK",
            toolTip = "#LOC_BLSG_REPLACE_STOCK_TIP")]
        public bool replaceStock = true;


        [GameParameters.CustomParameterUI("#LOC_BLSG_NO_PERSISTENT")]
        public bool hidePersistent = false;

        [GameParameters.CustomParameterUI("#LOC_BLSG_ALT_SKIN",
            toolTip = "#LOC_BLSG_ALT_SKIN_TIP")]
        public bool useAlternateSkin = false;


        public override void SetDifficultyPreset(GameParameters.Preset preset) { }
        public override bool Enabled(MemberInfo member, GameParameters parameters) { return true; }
        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }
        public override IList ValidValues(MemberInfo member) { return null; }
    }

    public class BLSG2 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "#LOC_BLSG_DELETION_ARCHIVE"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "BetterLoadSaveGame"; } }
        public override string DisplaySection { get { return "Better Load Save Game"; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets { get { return false; } }

        bool oldArchiveSaves, oldDeleteSaves;
        bool initted = false;

        [GameParameters.CustomParameterUI("#LOC_BLSG_ARCHIVE_OLD",
            toolTip = "#LOC_BLSG_ARCHIVE_OLD_TIP")]
        public bool archiveSaves = true;

        [GameParameters.CustomParameterUI("#LOC_BLSG_DELETE_OLD",
            toolTip = "#LOC_BLSG_DELETE_OLD_TIP")]
        public bool deleteSaves = false;


        bool oldHourUnit, oldDayUnit;

        [GameParameters.CustomParameterUI("#LOC_BLSG_UNIT_HOURS",
            toolTip = "#LOC_BLSG_UNIT_HOURS_TIP")]
        public bool hourUnit = false;
        [GameParameters.CustomParameterUI("#LOC_BLSG_UNIT_DAYS",
            toolTip = "#LOC_BLSG_UNIT_DAYS_TIP")]
        public bool dayUnit = true;

        [GameParameters.CustomIntParameterUI("#LOC_BLSG_AGE", minValue = 1, maxValue = 1440)]
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