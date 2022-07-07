using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Libplanet;

namespace Flawless.Models.Encounters
{
    public class SmithEncounter : Encounter
    {
        private const long Salt = 4;

        public List<Address> WeaponAddresses { get; private set; }

        public SmithEncounter(
            long stage,
            long encounter,
            long seed,
            ImmutableList<Address> availableWeaponAddresses
        )
            : base(stage, encounter, seed)
        {
            WeaponAddresses = Utils.Shuffle(seed, Salt, availableWeaponAddresses)
                .Take(5)
                .ToList();
        }
    }
}
