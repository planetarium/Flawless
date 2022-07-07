using Flawless;

namespace Flawless.Models.Encounters
{
    public abstract class Encounter : IEncounter
    {
        // These should add up to less than 100
        public const long PubChance = 33;
        public const long SmithChance = 33;
        private const long Salt = 1;

        public long StageNumber { get; private set; }
        public long EncounterNumber { get; private set; }
        public long Seed { get; private set; }

        public Encounter(
            long stageNumber,
            long encounterNumber,
            long seed)
        {
            StageNumber = stageNumber;
            EncounterNumber = encounterNumber;
            Seed = seed;
        }

        public static Encounter GenerateEncounter(long stageNumber, long encounterNumber, long seed)
        {
            long randomValue = Utils.Random(100, seed, Salt);

            if (encounterNumber % 3 != 0)
            {
                return new BattleEncounter(stageNumber, encounterNumber, seed);
            }
            else
            {
                if (stageNumber == 1 && encounterNumber == 3)
                {
                    return new BarEncounter(stageNumber, encounterNumber, seed);
                }
                else
                {
                    if (randomValue < PubChance)
                    {
                        return new BarEncounter(stageNumber, encounterNumber, seed);
                    }
                    else if (randomValue < PubChance + SmithChance)
                    {
                        return new SmithEncounter(stageNumber, encounterNumber, seed);
                    }
                    else
                    {
                        return new BattleEncounter(stageNumber, encounterNumber, seed, true);
                    }
                }
            }
        }
    }
}
