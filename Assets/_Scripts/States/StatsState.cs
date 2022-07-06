using System;
using System.Diagnostics.Contracts;
using Libplanet.Store;

namespace Flawless.States
{
    /// <summary>
    /// A <see cref="DataModel"/> representing character stats.
    /// </summary>
    public class StatsState : DataModel
    {
        public const long DefaultInitialStrength = 5;

        public const long DefaultInitialDexterity = 4;

        public const long DefaultInitialIntelligence = 0;

        public const long DefaultInitialPoints = 0;

        public long Strength { get; private set; }

        public long Dexterity { get; private set; }

        public long Intelligence { get; private set; }

        public long Points { get; private set; }

        /// <summary>
        /// Creates a new <see cref="StatsState"/> instance.
        /// </summary>
        public StatsState()
            : base()
        {
            Strength = DefaultInitialStrength;
            Dexterity = DefaultInitialDexterity;
            Intelligence = DefaultInitialIntelligence;
            Points = DefaultInitialPoints;
        }

        private StatsState(long strength, long dexterity, long intelligence, long points)
            : base()
        {
            Strength = strength;
            Dexterity = dexterity;
            Intelligence = intelligence;
            Points = points;
        }

        /// <summary>
        /// Decodes a stored <see cref="StatsState"/>.
        /// </summary>
        /// <param name="encoded">A <see cref="StatsState"/> encoded as
        /// a <see cref="Bencodex.Types.Dictionary"/>.</param>
        public StatsState(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }

        [Pure]
        public StatsState DistributePoints(
            long strength,
            long dexterity,
            long intelligence)
        {
            long points = strength + dexterity + intelligence;

            if (points > Points)
            {
                throw new ArgumentException(
                    $"Cannot spend {points} points from {Points} points.");
            }
            else
            {
                return new StatsState(
                    strength: Strength + strength,
                    dexterity: Dexterity + dexterity,
                    intelligence: Intelligence + intelligence,
                    points: Points - points);
            }
        }
    }
}
