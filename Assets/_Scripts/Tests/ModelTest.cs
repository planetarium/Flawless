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
    public void Stage_NextEvent()
    {
        var stage = new Stage(
            specialEventsInterval: 3,
            specialEventSkips: 1
        );

        Type[] expected = new[]
        {
            typeof(BattleEvent),
            typeof(BattleEvent),
            typeof(ShopEvent),
            typeof(BattleEvent),
            typeof(BattleEvent),
            typeof(WorkshopEvent),
            typeof(BattleEvent),
            typeof(BattleEvent),
            typeof(BattleEvent),
            typeof(BattleEvent),
        };
        
        var randomSeed = 52;
        var rng = new System.Random(randomSeed);
        var actual = Enumerable.Range(0, 10)
            .Select(_ => stage.NextEvent(rng.Next(0, 100)))
            .ToList();

        Assert.AreEqual(
            expected,
            actual.Select(e => e.GetType()).ToList()
        );
    }
}
