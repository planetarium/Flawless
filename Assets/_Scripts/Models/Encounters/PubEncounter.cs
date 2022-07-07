namespace Flawless.Models.Encounters
{
    public class PubEncounter : Encounter
    {
        private const long Salt = 2;

        public PubEncounter(long stage, long encounter, long seed)
            : base(stage, encounter, seed)
        {
        }
    }
}
