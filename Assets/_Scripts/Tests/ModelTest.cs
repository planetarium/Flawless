using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Flawless.Models;

public class ModelTest
{
    [Test]
    public void Stage_GenerateStage()
    {
        int randomSeed = 52;
        Type[] expected = new[]
        {
            typeof(BattleEvent),
            typeof(BattleEvent),
            typeof(WorkshopEvent),
            typeof(BattleEvent),
            typeof(BattleEvent),
            typeof(WorkshopEvent),
            typeof(BattleEvent),
            typeof(BattleEvent),
            typeof(ShopEvent),
            typeof(BattleEvent),
        };
        
        List<IEvent> actual = Stage.GenerateStage(
            randomSeed: randomSeed,
            events: 10,
            specialEventsInterval: 3,
            specialEventsSkips: 1
        ).Events;

        Assert.AreEqual(
            expected,
            actual.Select(e => e.GetType()).ToList()
        );
    }
}
