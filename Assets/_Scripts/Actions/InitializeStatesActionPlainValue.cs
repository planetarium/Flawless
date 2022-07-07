using Libplanet;
using Libplanet.Store;

namespace Flawless.Actions
{
    public class InitializeStatesActionPlainValue : DataModel
    {
        public string WeaponSheetCsv { get; private set; }
        public string SkillPresetSheetCsv { get; private set; }

        public InitializeStatesActionPlainValue(
            string weaponSheetCsv, 
            string skillPresetSheetCsv
        )
            : base()
        {
            WeaponSheetCsv = weaponSheetCsv;
            SkillPresetSheetCsv = skillPresetSheetCsv;
        }

        // Used for deserializing stored action.
        public InitializeStatesActionPlainValue(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }
    }
}
