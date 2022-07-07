namespace Flawless.Models.Encounters
{
    public class BarEncounter : Encounter
    {
        private const long Salt = 2;

        public BarEncounter(long stage, long encounter, long seed)
            : base(stage, encounter, seed)
        {
        }
    }
}
