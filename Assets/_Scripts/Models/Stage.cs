using System;
using System.Collections.Generic;
using System.Linq;

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

        public static Stage GenerateStage(
            int randomSeed,
            int events = 30,
            int specialEventsInterval = 3,
            int specialEventsSkips = 1
        )
        {
            var random = new Random(randomSeed);
            int nonBattleEvents = events / specialEventsInterval;

            IEvent CreateEvent(int i)
            {
                if ((i + 1) % specialEventsInterval == 0)
                {
                    if (i / specialEventsInterval < specialEventsSkips)
                    {
                        return new WorkshopEvent();
                    }

                    var choosen = (SpecialEvents) Enum.GetValues(typeof(SpecialEvents)).GetValue(
                        random.Next(Enum.GetNames(typeof(SpecialEvents)).Length)
                    );
                    
                    return choosen switch 
                    {
                        SpecialEvents.HardBattle => new BattleEvent(hard: true, randomSeed: random.Next()),
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

            return new Stage()
            {
                Events = Enumerable.Range(0, events).Select(CreateEvent).ToList(),
            };
        }

        public List<IEvent> Events { get; private set; }
    }
}
