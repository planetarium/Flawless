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
        public const long InitialStrength = 5;
        public const long InitialDexterity = 4;
        public const long InitialIntelligence = 0;
        public const long InitialPoints = 0;
        public const long InitialDamages = 0;
        public const long InitialExperience = 0;

        public long Strength { get; private set; }
        public long Dexterity { get; private set; }
        public long Intelligence { get; private set; }
        public long Points { get; private set; }
        public long Damages { get; private set; }
        public long Experience { get; private set; }

        /// <summary>
        /// Creates a new <see cref="StatsState"/> instance.
        /// This creates a character with zero <see cref="Damages"/>.
        /// </summary>
        public StatsState()
            : base()
        {
            Strength = InitialStrength;
            Dexterity = InitialDexterity;
            Intelligence = InitialIntelligence;
            Points = InitialPoints;
            Damages = InitialDamages;
            Experience = InitialExperience;
        }

        private StatsState(
            long strength,
            long dexterity,
            long intelligence,
            long points,
            long damages,
            long experience)
            : base()
        {
            Strength = strength;
            Dexterity = dexterity;
            Intelligence = intelligence;
            Points = points;
            Damages = damages;
            Experience = experience;
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

        /// <summary>
        /// Adds expeirence to character.  This does not actually add skill points automatically.
        /// Manually add skill points using <see cref="AddPoints"/>.
        /// </summary>
        [Pure]
        public StatsState AddExperience(long experience)
        {
            return experience < 0
                ? throw new ArgumentException("Cannot lose experience with a negative value.")
                : new StatsState(
                    strength: Strength,
                    dexterity: Dexterity,
                    intelligence: Intelligence,
                    points: Points,
                    damages: Damages,
                    experience: Experience + experience);
        }

        /// <summary>
        /// Adds skill points to character.
        /// </summary>
        [Pure]
        public StatsState AddPoints(long points)
        {
            return points < 0
                ? throw new ArgumentException("Cannot lose points with a negative value.")
                : new StatsState(
                    strength: Strength,
                    dexterity: Dexterity,
                    intelligence: Intelligence,
                    points: Points + points,
                    damages: Damages,
                    experience: Experience);
        }

        /// <summary>
        /// Resets spent skill points.
        /// </summary>
        [Pure]
        public StatsState ResetPoints()
        {
            long points = (Strength - InitialStrength) + (Dexterity - InitialDexterity) + (Intelligence - InitialIntelligence);
            return new StatsState(
                strength: InitialStrength,
                dexterity: InitialDexterity,
                intelligence: InitialIntelligence,
                points: Points + points,
                damages: Damages,
                experience: Experience);
        }

        [Pure]
        public StatsState DistributePoints(long strength, long dexterity, long intelligence)
        {
            long points = strength + dexterity + intelligence;

            if (points > Points)
            {
                throw new ArgumentException(
                    $"Cannot spend {points} points from {Points} points.");
            }
            else if (strength < 0 || dexterity < 0 || intelligence < 0)
            {
                throw new ArgumentException(
                    $"Cannot lower stats with a negative value.");
            }
            else
            {
                return new StatsState(
                    strength: Strength + strength,
                    dexterity: Dexterity + dexterity,
                    intelligence: Intelligence + intelligence,
                    points: Points - points,
                    damages: Damages,
                    experience: Experience);
            }
        }

        [Pure]
        public StatsState Heal(long amount)
        {
            return new StatsState(
                strength: Strength,
                dexterity: Dexterity,
                intelligence: Intelligence,
                points: Points,
                damages: Math.Max(0, Damages - amount),
                experience: Experience
            );
        }

        [Pure]
        public StatsState PutDamage(long amount)
        {
            return new StatsState(
                strength: Strength,
                dexterity: Dexterity,
                intelligence: Intelligence,
                points: Points,
                damages: Damages + amount,
                experience: Experience
            );
        }
    }
}
