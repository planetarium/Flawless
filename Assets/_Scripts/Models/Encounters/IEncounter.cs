namespace Flawless.Models.Encounters
{
    public interface IEncounter
    {
        public long StageNumber { get; }
        public long EncounterNumber { get; }
        public long Seed { get; }
    }
}
