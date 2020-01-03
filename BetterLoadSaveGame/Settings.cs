
using System.Collections;
using System.Reflection;

namespace BetterLoadSaveGame
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    // HighLogic.CurrentGame.Parameters.CustomParams<BLSG>().useAlternateSkin
    public class BLSG : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "BetterLoadSaveGame"; } }
        public override string DisplaySection { get { return "Better Load Save Game"; } }
        public override int SectionOrder { get { return 3; } }
        public override bool HasPresets { get { return true; } }


        [GameParameters.CustomParameterUI("Replace stock Load Game dialog on F9",
            toolTip = "If enabled, the F7 key won't be used to open BLSG")]
        public bool replaceStock = true;


        [GameParameters.CustomParameterUI("Use alternate skin",
            toolTip = "Use a more minimiliast skin")]
        public bool useAlternateSkin = false;



        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {

        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {

            return true;
        }


        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {

            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }

}