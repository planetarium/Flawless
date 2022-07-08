using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Libplanet;

namespace Flawless.Models.Encounters
{
    public class SmithEncounter : Encounter
    {
        private const long Salt = 4;

        public const long NumOptions = 3;

        public SmithEncounter(
            long stage,
            long encounter,
            long seed)
            : base(stage, encounter, seed)
        {
        }
    }
}
