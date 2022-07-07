using System.Collections.Immutable;
using Libplanet;
using Libplanet.Store;

namespace Flawless.States
{
    public class EnvironmentState : DataModel
    {
        public static readonly Address EnvironmentAddress =
            new Address("0000000000000000000000000000000000000502");
        public ImmutableList<Address> AvailableWeapons { get; private set; }

        // FIXME
        public string SkillPresets { get; private set; }

        public EnvironmentState() : base()
        {
            AvailableWeapons = ImmutableList<Address>.Empty;
            SkillPresets = string.Empty;
        }

        public EnvironmentState(
            ImmutableList<Address> availableWeapons,
            string skillPresets
        ) : base()
        {
            AvailableWeapons = availableWeapons;
            SkillPresets = skillPresets;
        }

        public EnvironmentState(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }

    }
}
