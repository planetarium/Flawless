namespace Flawless.Models
{
    public class BattleEvent : IEvent
    {
        private readonly bool _hard;
        private readonly int _randomSeed;

        public BattleEvent(bool hard, int randomSeed)
        {
            _hard = hard;
            _randomSeed = randomSeed;
        }
    }
}
