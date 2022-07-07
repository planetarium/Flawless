using System.Collections.Generic;
using Flawless.Battle;

namespace Flawless.Models.Encounters
{
    public class SmithEncounter : Encounter
    {
        public List<Weapon> Weapons { get; private set; }

        public SmithEncounter(long stage, long encounter, long seed)
            : base(stage, encounter, seed)
        {
            Weapons = new List<Weapon>();
        }
    }
}
