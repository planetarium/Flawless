namespace Flawless.Models.Encounters
{
    public class PubEncounter : Encounter
    {
        private const long Salt = 2;

        public long HealPrice => StageNumber * 10;
        public long HealPercentage => 30;
        public long ResetPointsPrice => StageNumber * 10;

        public PubEncounter(long stageNumber, long encounterNumber, long seed)
            : base(stageNumber, encounterNumber, seed)
        {
        }
    }
}
