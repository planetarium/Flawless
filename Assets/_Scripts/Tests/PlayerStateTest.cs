using System;
using System.Linq;
using NUnit.Framework;
using Flawless.States;
using Libplanet;
using Libplanet.Crypto;

public class PlayerStateTest
{
    [Test]
    public void UseFreeHeal()
    {
        Address address = new Address(new PrivateKey().PublicKey);
        string name = "foo";
        PlayerState playerState = new PlayerState(address, name, 0);
        Assert.False(playerState.SceneState.FreeHealUsed);
        Assert.False(playerState.SceneState.FreeResetStatsUsed);

        playerState = playerState.UseFreeHeal();
        Assert.True(playerState.SceneState.FreeHealUsed);
        Assert.False(playerState.SceneState.FreeResetStatsUsed);

        playerState = playerState.UseFreeResetStats();
        Assert.True(playerState.SceneState.FreeHealUsed);
        Assert.True(playerState.SceneState.FreeResetStatsUsed);

        Assert.Throws<ArgumentException>(() => playerState.UseFreeHeal());
        Assert.Throws<ArgumentException>(() => playerState.UseFreeResetStats());
    }

    [Test]
    public void EditHealth()
    {
        Address address = new Address(new PrivateKey().PublicKey);
        string name = "foo";
        long health = 1_000;
        PlayerState playerState = new PlayerState(address, name, 0);
        Assert.Zero(playerState.StatsState.Health);
        playerState = playerState.EditHealth(health);
        Assert.AreEqual(health, playerState.StatsState.Health);
    }

    [Test]
    public void AddExperience()
    {
        Address address = new Address(new PrivateKey().PublicKey);
        string name = "foo";
        long experience = 1_000;
        PlayerState playerState = new PlayerState(address, name, 0);

        playerState = playerState.AddExperience(experience);
        Assert.AreEqual(experience, playerState.StatsState.Experience);

        Assert.Throws<ArgumentException>(() => playerState.AddExperience(-1));
    }

    [Test]
    public void AddAndUseSkillPoints()
    {
        Address address = new Address(new PrivateKey().PublicKey);
        string name = "foo";
        PlayerState playerState = new PlayerState(address, name, 0);
        long points = 3;
        long initialStrength = playerState.StatsState.Strength;
        long initialDexterity = playerState.StatsState.Dexterity;
        long initialIntelligence = playerState.StatsState.Intelligence;

        Assert.Throws<ArgumentException>(() => playerState.DistributePoints(-1, -1, -1));
        Assert.Throws<ArgumentException>(() => playerState.DistributePoints(1, 1, 1));

        playerState = playerState.AddPoints(points);
        Assert.AreEqual(points, playerState.StatsState.Points);

        // Check strength
        playerState = playerState.DistributePoints(1, 0, 0);
        Assert.AreEqual(points - 1, playerState.StatsState.Points);
        Assert.AreEqual(initialStrength + 1, playerState.StatsState.Strength);

        playerState = playerState.ResetPoints();
        Assert.AreEqual(points, playerState.StatsState.Points);
        Assert.AreEqual(initialStrength, playerState.StatsState.Strength);

        // Check dexterity
        playerState = playerState.DistributePoints(1, 0, 0);
        Assert.AreEqual(points - 1, playerState.StatsState.Points);
        Assert.AreEqual(initialDexterity + 1, playerState.StatsState.Dexterity);

        playerState = playerState.ResetPoints();
        Assert.AreEqual(points, playerState.StatsState.Points);
        Assert.AreEqual(initialDexterity, playerState.StatsState.Dexterity);

        // Check intelligence
        playerState = playerState.DistributePoints(1, 0, 0);
        Assert.AreEqual(points - 1, playerState.StatsState.Points);
        Assert.AreEqual(initialIntelligence + 1, playerState.StatsState.Intelligence);

        playerState = playerState.ResetPoints();
        Assert.AreEqual(points, playerState.StatsState.Points);
        Assert.AreEqual(initialIntelligence, playerState.StatsState.Intelligence);
    }
}
