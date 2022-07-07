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
}
