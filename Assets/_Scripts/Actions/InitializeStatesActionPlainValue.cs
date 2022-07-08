using Libplanet;
using Libplanet.Store;

namespace Flawless.Actions
{
    public class InitializeStatesActionPlainValue : DataModel
    {
        public string SkillPresetSheetCsv { get; private set; }

        public InitializeStatesActionPlainValue(
            string skillPresetSheetCsv
        )
            : base()
        {
            SkillPresetSheetCsv = skillPresetSheetCsv;
        }

        // Used for deserializing stored action.
        public InitializeStatesActionPlainValue(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }
    }
}
