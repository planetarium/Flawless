using Flawless.Battle;

namespace Flawless.Models.Encounters
{
    public class BattleEncounter : Encounter
    {
        private const long Salt = 3;

        public bool Hard { get; private set; }
        public Character Enemy { get; private set; }

        public BattleEncounter(long stage, long encounter, long seed, bool hard = false)
            : base(stage, encounter, seed)
        {
            Hard = hard;
            Enemy = null;
        }
    }
}
