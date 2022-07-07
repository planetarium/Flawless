using System;

namespace Flawless.Models
{
    public struct Stage
    {
        private enum SpecialEvents
        {
            HardBattle = 0,
            Shop = 1,
            Workshop = 2,
        }

        private int _steps;
        private readonly int _specialEventsInterval;
        private readonly int _specialEventSkips;

        public Stage(
            int specialEventsInterval,
            int specialEventSkips
        )
        {
            _specialEventsInterval = specialEventsInterval;
            _specialEventSkips = specialEventSkips;
            _steps = 0;
        }

        public IEvent NextEvent(int randomSeed)
        {
            _steps += 1;
            return CreateEvent(_steps, randomSeed);
        }

        private IEvent CreateEvent(int i, int randomSeed)
        {
            var random = new Random(randomSeed);

            if (i % _specialEventsInterval == 0)
            {
                if (i / _specialEventsInterval < _specialEventSkips)
                {
                    return new WorkshopEvent();
                }

                var choosen = (SpecialEvents) Enum.GetValues(typeof(SpecialEvents)).GetValue(
                    random.Next(Enum.GetNames(typeof(SpecialEvents)).Length)
                );

                return choosen switch
                {
                    SpecialEvents.HardBattle => new BattleEvent(
                        hard: true,
                        randomSeed: random.Next()
                    ),
                    SpecialEvents.Shop => new ShopEvent(),
                    SpecialEvents.Workshop => new WorkshopEvent(),
                    _ => throw new Exception("TBD")
                };
            }
            else
            {
                return new BattleEvent(
                    hard: false,
                    randomSeed: random.Next()
                );
            }
        }
    }
}
