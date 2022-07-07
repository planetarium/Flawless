using System;
using System.Linq;
using NUnit.Framework;
using Flawless.States;

public class SceneStateTest
{
    [Test]
    public void Proceed()
    {
        long seed = 0;
        SceneState sceneState = new SceneState(0);
        Assert.IsTrue(sceneState.InMenu);

        seed++;
        sceneState = sceneState.Proceed(seed);
        Assert.IsTrue(sceneState.OnWorldMap);
        Assert.AreEqual(seed, sceneState.Seed);

        seed++;
        sceneState = sceneState.Proceed(seed);
        Assert.IsTrue(sceneState.OnRoad);
        Assert.AreEqual(seed, sceneState.Seed);

        seed++;
        sceneState = sceneState.Proceed(seed);
        Assert.IsTrue(sceneState.InEncounter);
        Assert.AreEqual(seed, sceneState.Seed);
        Assert.AreEqual(0, sceneState.EncounterCleared);

        // If stage is not cleared, go back to road.
        // EncounterCleared is bumped when clearing encounter.
        seed++;
        sceneState = sceneState.Proceed(seed);
        Assert.IsTrue(sceneState.OnRoad);
        Assert.AreEqual(seed, sceneState.Seed);
        Assert.AreEqual(1, sceneState.EncounterCleared);

        // Proceed until last encounter of a stage.
        while (!(sceneState.InEncounter && sceneState.EncounterCleared == SceneState.EncountersPerStage - 1))
        {
            seed++;
            sceneState = sceneState.Proceed(seed);
        }

        // If stage is cleared, go back to world map.
        // StageCleared is bumped when clearing the last encounter.
        // EncounterCleared should be reset.
        seed++;
        sceneState = sceneState.Proceed(seed);
        Assert.IsTrue(sceneState.OnWorldMap);
        Assert.AreEqual(seed, sceneState.Seed);
        Assert.AreEqual(1, sceneState.StageCleared);
        Assert.AreEqual(0, sceneState.EncounterCleared);

        // Proceed until back to menu.
        while (!sceneState.InMenu)
        {
            seed++;
            sceneState = sceneState.Proceed(seed);
        }

        Assert.AreEqual(SceneState.StagesPerSession, sceneState.StageCleared);
        Assert.AreEqual(0, sceneState.EncounterCleared);
    }
}
