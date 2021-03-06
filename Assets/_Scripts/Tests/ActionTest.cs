using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NUnit.Framework;
using Bencodex.Types;
using Flawless.Actions;
using Flawless.States;
using Libplanet;
using Libplanet.Action;
using Libplanet.Crypto;

public class ActionTest
{
    private readonly EnvironmentState _environmentState = new EnvironmentState();

    [Test]
    public void CreateAccountAction()
    {
        var playerName = "ssg";
        var playerKey = new PrivateKey();
        var playerAddress = playerKey.ToAddress();
        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
            }.ToImmutableDictionary()
        );
        var action = new CreateAccountAction(playerAddress, playerName);
        var seed = 123;
        IAccountStateDelta nextState = action.Execute(new ActionContext
        {
            PreviousStates = previousStates,
            Signer = playerAddress,
            BlockIndex = 23,
            Random = new TestRandom(seed),
        });
        var playerState = new PlayerState((Dictionary)nextState.GetState(playerAddress));
        var weaponState = new WeaponState((Dictionary)nextState.GetState(playerState.WeaponAddress));

        Assert.AreEqual(playerAddress, playerState.Address);
        Assert.AreEqual(playerName, playerState.Name);
        Assert.AreEqual(playerState.WeaponAddress, weaponState.Address);
        Assert.AreEqual(40, playerState.GetMaxHealth(weaponState));
        Assert.AreEqual(seed, playerState.SceneState.Seed);
    }

    [Test]
    public void ResetSessionAction()
    {
        var playerName = "ssg";
        var playerKey = new PrivateKey();
        var playerAddress = playerKey.ToAddress();
        var seed = 123;
        var playerState = new PlayerState(playerAddress, playerName, seed);
        var weaponAddress = playerState.WeaponAddress;
        var weaponState = new WeaponState(
            address: weaponAddress,
            price: 10000L);
        var random = new System.Random();
        playerState = playerState.Proceed(random.Next());

        var action = new ResetSessionAction();
        var invalidPreviousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = playerState.Encode(),
                [weaponAddress] = weaponState.Encode(),
            }.ToImmutableDictionary()
        );
        Assert.Throws<ArgumentException>(() => action.Execute(new ActionContext
        {
            PreviousStates = invalidPreviousStates,
            Signer = playerAddress,
            BlockIndex = 23,
            Random = new TestRandom(seed),
        }));

        while (!playerState.SceneState.InMenu)
        {
            playerState = playerState.Proceed(random.Next());
        }
        Assert.AreEqual(SceneState.StagesPerSession, playerState.SceneState.StageCleared);
        Assert.AreEqual(0, playerState.SceneState.EncounterCleared);
        Assert.AreNotEqual(seed, playerState.SceneState.Seed);

        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = playerState.Encode(),
                [weaponAddress] = weaponState.Encode(),
            }.ToImmutableDictionary()
        );
        var newSeed = 12345;
        IAccountStateDelta nextState = action.Execute(new ActionContext
        {
            PreviousStates = previousStates,
            Signer = playerAddress,
            BlockIndex = 23,
            Random = new TestRandom(newSeed),
        });
        var nextPlayerState = new PlayerState((Dictionary)nextState.GetState(playerAddress));
        var nextWeaponState = new WeaponState((Dictionary)nextState.GetState(weaponAddress));

        Assert.AreEqual(playerAddress, nextPlayerState.Address);
        Assert.AreEqual(playerName, nextPlayerState.Name);
        Assert.AreEqual(weaponAddress, nextPlayerState.WeaponAddress);
        Assert.AreEqual(40, nextPlayerState.GetMaxHealth(nextWeaponState));
        Assert.AreEqual(newSeed, nextPlayerState.SceneState.Seed);
        Assert.AreEqual(0, nextPlayerState.SceneState.StageCleared);
        Assert.AreEqual(0, nextPlayerState.SceneState.EncounterCleared);
        Assert.AreEqual(0, nextWeaponState.Price);
    }

    [Test]
    public void StartSessionAction()
    {
        var playerName = "ssg";
        var playerKey = new PrivateKey();
        var playerAddress = playerKey.ToAddress();
        var seed = 123;
        var playerState = new PlayerState(playerAddress, playerName, seed);
        var random = new System.Random();
        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = playerState.Encode(),
            }.ToImmutableDictionary()
        );
        var action = new StartSessionAction();
        var newSeed = 456;
        IAccountStateDelta nextState = action.Execute(new ActionContext
        {
            PreviousStates = previousStates,
            Signer = playerAddress,
            BlockIndex = 23,
            Random = new TestRandom(newSeed),
        });
        var nextPlayerState = new PlayerState((Dictionary)nextState.GetState(playerAddress));

        // Seed should change.
        Assert.AreEqual(seed, nextPlayerState.SceneState.Seed);
        Assert.False(nextPlayerState.SceneState.InMenu);
        Assert.True(nextPlayerState.SceneState.OnWorldMap);

        previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = nextPlayerState.Encode(),
            }.ToImmutableDictionary()
        );
        Assert.Throws<ArgumentException>(() => action.Execute(new ActionContext
        {
            PreviousStates = previousStates,
            Signer = playerAddress,
            BlockIndex = 37,
            Random = new TestRandom(seed),
        }));
    }

    [Test]
    public void UpgradeWeaponAction_Execute_Basic()
    {
        var playerKey = new PrivateKey();
        Address playerAddress = playerKey.ToAddress();
        PlayerState playerState = new PlayerState(
            name: "ssg",
            address: playerAddress,
            seed: 4).AddGold(1000L);
        Dictionary playerStateDict = playerState
            .Encode()
            .SetItem("SceneState", playerState.SceneState.Encode()
                .SetItem("InEncounter", true)
                .SetItem("EncounterCleared", 5));
        playerState = new PlayerState(playerStateDict);
        Address weaponAddress = playerState.WeaponAddress;
        WeaponState weaponState = new WeaponState(
            grade: 1,
            address: playerState.WeaponAddress,
            price: 10000L);

        var action = new UpgradeWeaponAction();

        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = playerState.Encode(),
                [weaponAddress] = weaponState.Encode(),
            }.ToImmutableDictionary()
        );

        IAccountStateDelta nextState = action.Execute(
            new ActionContext
            {
                PreviousStates = previousStates,
                Signer = playerAddress,
                BlockIndex = 0,
            }
        );

        var playerStateAfterUpgrade = new PlayerState(
            (Dictionary)nextState.GetState(playerAddress)
        );
        var upgradedWeaponState = new WeaponState(
            (Dictionary)nextState.GetState(playerState.WeaponAddress));

        Assert.AreEqual(995, playerStateAfterUpgrade.Gold);
        Assert.True(
            weaponState.Health <= upgradedWeaponState.Health &&
            weaponState.Attack <= upgradedWeaponState.Attack &&
            weaponState.Defense <= upgradedWeaponState.Defense &&
            weaponState.Speed <= upgradedWeaponState.Speed);
        Assert.True(
            weaponState.Health < upgradedWeaponState.Health ||
            weaponState.Attack < upgradedWeaponState.Attack ||
            weaponState.Defense < upgradedWeaponState.Defense ||
            weaponState.Speed < upgradedWeaponState.Speed);
        Assert.AreEqual(
            weaponState.Lifesteal,
            upgradedWeaponState.Lifesteal);
        Assert.AreEqual(
            weaponState.Price,
            upgradedWeaponState.Price);
        Assert.AreEqual(
            weaponState.Grade + 1,
            upgradedWeaponState.Grade);
    }

    [Test]
    public void UpgradeWeaponAction_Execute_NotEnoughGold()
    {
        var playerKey = new PrivateKey();
        Address playerAddress = playerKey.ToAddress();
        PlayerState playerState = new PlayerState(
            name: "ssg",
            address: playerAddress,
            seed: 4).AddGold(1L);
        Dictionary playerStateDict = playerState
            .Encode()
            .SetItem("SceneState", playerState.SceneState.Encode()
                .SetItem("InEncounter", true)
                .SetItem("EncounterCleared", 5));
        playerState = new PlayerState(playerStateDict);
        Address weaponAddress = playerState.WeaponAddress;
        WeaponState weaponState = new WeaponState(
            grade: 1,
            address: playerState.WeaponAddress,
            price: 10000L);

        var action = new UpgradeWeaponAction();

        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = playerState.Encode(),
                [weaponAddress] = weaponState.Encode(),
            }.ToImmutableDictionary()
        );

        Assert.Throws<Exception>(() => {
            action.Execute(
                new ActionContext
                {
                    PreviousStates = previousStates,
                    Signer = playerAddress,
                    BlockIndex = 0,
                }
            );
        });
    }

    [Test]
    public void UpgradeWeaponAction_Execute_NotInSmith()
    {
        var playerKey = new PrivateKey();
        Address playerAddress = playerKey.ToAddress();
        PlayerState playerState = new PlayerState(
            name: "ssg",
            address: playerAddress,
            seed: 13).AddGold(1000L);
        Dictionary playerStateDict = playerState
            .Encode()
            .SetItem("SceneState", playerState.SceneState.Encode()
                .SetItem("InEncounter", true)
                .SetItem("EncounterCleared", 5));
        playerState = new PlayerState(playerStateDict);
        Address weaponAddress = playerState.WeaponAddress;
        WeaponState weaponState = new WeaponState(
            grade: 1,
            address: playerState.WeaponAddress,
            price: 10000L);

        var action = new UpgradeWeaponAction();

        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = playerState.Encode(),
                [weaponAddress] = weaponState.Encode(),
            }.ToImmutableDictionary()
        );

        Assert.Throws<Exception>(() => {
            action.Execute(
                new ActionContext
                {
                    PreviousStates = previousStates,
                    Signer = playerAddress,
                    BlockIndex = 0,
                }
            );
        });
    }

    [Test]
    public void InitializeStatesAction_Execute()
    {
        var skillPresetSheetCsv = "TBD";
        var action = new InitalizeStatesAction(
            skillPresetSheetCsv);
        var previousStates = new State();
        var nextStates = action.Execute(
            new ActionContext
            {
                PreviousStates = previousStates,
                Signer = default,
                BlockIndex = 0,
            }
        );

        var environmentState = new EnvironmentState(
            (Bencodex.Types.Dictionary) nextStates.GetState(
                EnvironmentState.EnvironmentAddress
            )
        );

        Assert.AreEqual(
            skillPresetSheetCsv,
            environmentState.SkillPresets
        );
    }

    [Test]
    public void BattleAction_Execute_Win()
    {
        var playerKey = new PrivateKey();
        Address playerAddress = playerKey.ToAddress();
        Bencodex.Types.Dictionary playerDict = (Dictionary) new PlayerState(
            name: "ssg",
            address: playerAddress,
            seed: 13
        ).Encode();

        Bencodex.Types.Dictionary sceneDict =
            (Dictionary) playerDict["SceneState"];
        Bencodex.Types.Dictionary statsDict =
            (Dictionary) playerDict["StatsState"];

        var playerSkills = new[]
        {
            "DownwardSlash",
            "DownwardSlash",
            "UpwardSlash",
            "DownwardThrust",
            "SideStep",
            "UpwardSlash",
            "DownwardSlash",
            "UpwardThrust",
            "SideStep",
            "UpwardSlash",
        }.ToImmutableList();

        var playerState = new PlayerState(
            playerDict
                .SetItem(
                    "SceneState",
                    sceneDict
                        .SetItem("InMenu", false)
                        .SetItem("InEncounter", true)
                        .SetItem("EncounterCleared", 1)
                        .SetItem("Seed", 10)
                )
                .SetItem(
                    "StatsState",
                    statsDict
                        .SetItem("Strength", 10000)
                )
        ).SetOwnedSkills(playerSkills);
        var weaponAddress = playerState.WeaponAddress;
        var weaponState = new WeaponState(weaponAddress, price: 100L);

        var action = new BattleAction(playerSkills);

        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = playerState.Encode(),
                [weaponAddress] = weaponState.Encode(),
            }.ToImmutableDictionary()
        );

        var nextState = action.Execute(
            new ActionContext
            {
                PreviousStates = previousStates,
                Signer = playerAddress,
                BlockIndex = 0,
                Random = new TestRandom(),
            }
        );

        var playerStateAfterBattle = new PlayerState(
            (Dictionary)nextState.GetState(playerAddress));
        var weaponStateAfterBattle = new WeaponState(
            (Dictionary)nextState.GetState(weaponAddress));

        Assert.AreEqual(2, playerStateAfterBattle.SceneState.EncounterCleared);
        Assert.AreEqual(100, weaponStateAfterBattle.Price);
    }

    [Test]
    public void BattleAction_Execute_Loss()
    {
        var playerKey = new PrivateKey();
        Address playerAddress = playerKey.ToAddress();
        Bencodex.Types.Dictionary playerDict = (Dictionary) new PlayerState(
            name: "ssg",
            address: playerAddress,
            seed: 13
        ).Encode();

        Bencodex.Types.Dictionary sceneDict =
            (Dictionary) playerDict["SceneState"];

        var playerState = new PlayerState(
            playerDict.SetItem(
                "SceneState",
                sceneDict
                    .SetItem("EncounterCleared", 1)
                    .SetItem("Seed", 10)
            )
        );
        var weaponAddress = playerState.WeaponAddress;
        var weaponState = new WeaponState(weaponAddress, price: 100L);

        var action = new BattleAction(
            new[]
            {
                "DownwardSlash",
                "DownwardSlash",
                "UpwardSlash",
                "DownwardThrust",
                "SideStep",
                "UpwardSlash",
                "DownwardSlash",
                "UpwardThrust",
                "SideStep",
                "UpwardSlash",
            }.ToImmutableList()
        );

        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = playerState.Encode(),
                [weaponAddress] = weaponState.Encode(),
            }.ToImmutableDictionary()
        );

        var nextState = action.Execute(
            new ActionContext
            {
                PreviousStates = previousStates,
                Signer = playerAddress,
                BlockIndex = 0,
                Random = new TestRandom(),
            }
        );

        var playerStateAfterBattle = new PlayerState(
            (Dictionary)nextState.GetState(playerAddress));
        var weaponStateAfterBattle = new WeaponState(
            (Dictionary)nextState.GetState(weaponAddress));

        Assert.AreEqual(0, playerStateAfterBattle.SceneState.StageCleared);
        Assert.AreEqual(0, playerStateAfterBattle.SceneState.EncounterCleared);
        Assert.AreEqual(0, weaponStateAfterBattle.Price);
    }
}
