using Flawless.Battle;

namespace Flawless.Models.Encounters
{
    public class BattleEncounter : Encounter
    {
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
