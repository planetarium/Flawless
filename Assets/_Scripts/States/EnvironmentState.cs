using System.Collections.Immutable;
using Libplanet;
using Libplanet.Store;

namespace Flawless.States
{
    public class EnvironmentState : DataModel
    {
        public static readonly Address EnvironmentAddress =
            new Address("0000000000000000000000000000000000000502");

        // FIXME
        public string SkillPresets { get; private set; }

        public EnvironmentState() : base()
        {
            SkillPresets = string.Empty;
        }

        public EnvironmentState(
            string skillPresets
        ) : base()
        {
            SkillPresets = skillPresets;
        }

        public EnvironmentState(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }
    }
}
