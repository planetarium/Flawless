using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
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
        var weaponState = new WeaponState();

        Assert.AreEqual(playerAddress, playerState.Address);
        Assert.AreEqual(playerName, playerState.Name);
        Assert.AreEqual(default(Address), playerState.EquippedWeaponAddress);
        Assert.AreEqual(40, playerState.GetMaxHealth(weaponState));
        Assert.AreEqual(seed, playerState.SceneState.Seed);
    }

    [Test]
    public void ResetSessionAction()
    {
        var playerName = "ssg";
        var playerKey = new PrivateKey();
        var playerAddress = playerKey.ToAddress();
        var weaponAddress = new PrivateKey().ToAddress();
        var weaponState = new WeaponState(
            address: weaponAddress,
            price: 10000L);
        var seed = 123;
        var playerState = new PlayerState(playerAddress, playerName, seed);
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

        Assert.AreEqual(playerAddress, nextPlayerState.Address);
        Assert.AreEqual(playerName, nextPlayerState.Name);
        Assert.AreEqual(default(Address), nextPlayerState.EquippedWeaponAddress);
        Assert.AreEqual(40, nextPlayerState.GetMaxHealth(new WeaponState()));
        Assert.AreEqual(newSeed, nextPlayerState.SceneState.Seed);
        Assert.AreEqual(0, nextPlayerState.SceneState.StageCleared);
        Assert.AreEqual(0, nextPlayerState.SceneState.EncounterCleared);
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
    public void SellWeaponAction_Execute()
    {
        var playerKey = new PrivateKey();
        Address weaponAddress = new PrivateKey().ToAddress();
        Address playerAddress = playerKey.ToAddress();
        WeaponState weaponState = new WeaponState(
            address: weaponAddress,
            price: 10000L);

        var playerState = new PlayerState(
            name: "ssg",
            address: playerAddress,
            seed: 13
        ).AddWeapon(weaponState);

        long initialGold = playerState.Gold;
        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = playerState.Encode(),
                [weaponAddress] = weaponState.Encode(),
            }.ToImmutableDictionary()
        );
        var action = new SellWeaponAction(
            weaponAddress: weaponState.Address
        );

        IAccountStateDelta nextState = action.Execute(new ActionContext
        {
            PreviousStates = previousStates,
            Signer = playerAddress,
            BlockIndex = 0,
        });

        var playerStateAfterSell = new PlayerState(
            (Dictionary)nextState.GetState(playerAddress)
        );
        var weaponStateAfterSell = new WeaponState(
            (Dictionary)nextState.GetState(weaponState.Address)
        );

        Assert.AreEqual(
            initialGold + weaponState.Price,
            playerStateAfterSell.Gold
        );
        CollectionAssert.DoesNotContain(
            playerStateAfterSell.Inventory,
            weaponState.Address
        );
    }

    [Test]
    public void UpgradeWeaponAction_Execute_Basic()
    {
        var playerKey = new PrivateKey();
        Address weaponAddress = new PrivateKey().ToAddress();
        Address playerAddress = playerKey.ToAddress();
        WeaponState weaponState = new WeaponState(
            grade: 1,
            address: weaponAddress,
            price: 10000L
        );
        Bencodex.Types.Dictionary playerDict = (Dictionary) new PlayerState(
            name: "ssg",
            address: playerAddress,
            seed: 13
        ).AddWeapon(weaponState).AddGold(100).Encode();

        Bencodex.Types.Dictionary sceneDict =
            (Dictionary) playerDict["SceneState"];

        var playerState = new PlayerState(
            playerDict.SetItem(
                "SceneState",
                sceneDict
                    .SetItem("FreeUpgradeWeaponUsed", true)
                    .SetItem("EncounterCleared", 5)
                    .SetItem("Seed", 10)
            )
        );

        var action = new UpgradeWeaponAction(
            health: 1,
            attack: 1,
            defense: 1,
            speed: 0,
            weaponAddress: weaponState.Address
        );

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
        byte[] hashed;
        using (var hmac = new HMACSHA1(weaponAddress.ToByteArray()))
        {
            hashed = hmac.ComputeHash(weaponAddress.ToByteArray());
        }
        var upgradedWeaponAddress = new Address(hashed);
        var upgradedWeaponState = new WeaponState(
            (Dictionary)nextState.GetState(upgradedWeaponAddress)
        );

        CollectionAssert.DoesNotContain(
            playerStateAfterUpgrade.Inventory,
            weaponAddress
        );
        CollectionAssert.Contains(
            playerStateAfterUpgrade.Inventory,
            upgradedWeaponAddress
        );
        Assert.AreEqual(95, playerStateAfterUpgrade.Gold);
        Assert.AreEqual(
            weaponState.Health + 10,
            upgradedWeaponState.Health
        );
        Assert.AreEqual(
            weaponState.Attack + 2,
            upgradedWeaponState.Attack
        );
        Assert.AreEqual(
            weaponState.Defense + 1,
            upgradedWeaponState.Defense
        );
        Assert.AreEqual(
            weaponState.Speed,
            upgradedWeaponState.Speed
        );
        Assert.AreEqual(
            weaponState.Lifesteal,
            upgradedWeaponState.Lifesteal
        );
        Assert.AreEqual(
            weaponState.Price,
            upgradedWeaponState.Price
        );
    }

    [Test]
    public void UpgradeWeaponAction_Execute_Free()
    {
        var playerKey = new PrivateKey();
        Address weaponAddress = new PrivateKey().ToAddress();
        Address playerAddress = playerKey.ToAddress();
        WeaponState weaponState = new WeaponState(
            grade: 1,
            address: weaponAddress,
            price: 10000L
        );
        Bencodex.Types.Dictionary playerDict = (Dictionary) new PlayerState(
            name: "ssg",
            address: playerAddress,
            seed: 13
        ).AddWeapon(weaponState).Encode();

        Bencodex.Types.Dictionary sceneDict =
            (Dictionary) playerDict["SceneState"];

        var playerState = new PlayerState(
            playerDict.SetItem(
                "SceneState",
                sceneDict
                    .SetItem("FreeUpgradeWeaponUsed", false)
                    .SetItem("EncounterCleared", 5)
                    .SetItem("Seed", 10)
            )
        );

        var action = new UpgradeWeaponAction(
            health: 1,
            attack: 1,
            defense: 1,
            speed: 0,
            weaponAddress: weaponState.Address
        );

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
        byte[] hashed;
        using (var hmac = new HMACSHA1(weaponAddress.ToByteArray()))
        {
            hashed = hmac.ComputeHash(weaponAddress.ToByteArray());
        }
        var upgradedWeaponAddress = new Address(hashed);
        var upgradedWeaponState = new WeaponState(
            (Dictionary)nextState.GetState(upgradedWeaponAddress)
        );

        CollectionAssert.DoesNotContain(
            playerStateAfterUpgrade.Inventory,
            weaponAddress
        );
    }

    [Test]
    public void UpgradeWeaponAction_Execute_NotEnoughGold()
    {
        var playerKey = new PrivateKey();
        Address weaponAddress = new PrivateKey().ToAddress();
        Address playerAddress = playerKey.ToAddress();
        WeaponState weaponState = new WeaponState(
            grade: 1,
            address: weaponAddress,
            price: 10000L
        );
        Bencodex.Types.Dictionary playerDict = (Dictionary) new PlayerState(
            name: "ssg",
            address: playerAddress,
            seed: 13
        ).AddWeapon(weaponState).Encode();

        Bencodex.Types.Dictionary sceneDict =
            (Dictionary) playerDict["SceneState"];

        var playerState = new PlayerState(
            playerDict.SetItem(
                "SceneState",
                sceneDict
                    .SetItem("FreeUpgradeWeaponUsed", true)
                    .SetItem("EncounterCleared", 5)
                    .SetItem("Seed", 10)
            )
        );

        var action = new UpgradeWeaponAction(
            health: 1,
            attack: 1,
            defense: 1,
            speed: 0,
            weaponAddress: weaponState.Address
        );

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
        Address weaponAddress = new PrivateKey().ToAddress();
        Address playerAddress = playerKey.ToAddress();
        WeaponState weaponState = new WeaponState(
            grade: 1,
            address: weaponAddress,
            price: 10000L
        );
        Bencodex.Types.Dictionary playerDict = (Dictionary) new PlayerState(
            name: "ssg",
            address: playerAddress,
            seed: 13
        ).AddWeapon(weaponState).Encode();

        Bencodex.Types.Dictionary sceneDict =
            (Dictionary) playerDict["SceneState"];

        var playerState = new PlayerState(
            playerDict.SetItem(
                "SceneState",
                sceneDict
                    .SetItem("FreeUpgradeWeaponUsed", false)
                    .SetItem("EncounterCleared", 1)
                    .SetItem("Seed", 10)
            )
        );

        var action = new UpgradeWeaponAction(
            health: 1,
            attack: 1,
            defense: 1,
            speed: 0,
            weaponAddress: weaponState.Address
        );

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
    public void UpgradeWeaponAction_Execute_MoreThan3Points()
    {
        var playerKey = new PrivateKey();
        Address weaponAddress = new PrivateKey().ToAddress();
        Address playerAddress = playerKey.ToAddress();
        WeaponState weaponState = new WeaponState(
            grade: 1,
            address: weaponAddress,
            price: 10000L
        );
        Bencodex.Types.Dictionary playerDict = (Dictionary) new PlayerState(
            name: "ssg",
            address: playerAddress,
            seed: 13
        ).AddWeapon(weaponState).Encode();

        Bencodex.Types.Dictionary sceneDict =
            (Dictionary) playerDict["SceneState"];

        var playerState = new PlayerState(
            playerDict.SetItem(
                "SceneState",
                sceneDict
                    .SetItem("FreeUpgradeWeaponUsed", false)
                    .SetItem("EncounterCleared", 5)
                    .SetItem("Seed", 10)
            )
        );

        var action = new UpgradeWeaponAction(
            health: 1,
            attack: 1,
            defense: 1,
            speed: 1,
            weaponAddress: weaponState.Address
        );

        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = playerState.Encode(),
                [weaponAddress] = weaponState.Encode(),
            }.ToImmutableDictionary()
        );

        Assert.Throws<ArgumentException>(() => {
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
    public void UpgradeWeaponAction_Execute_LessThan1Points()
    {
        var playerKey = new PrivateKey();
        Address weaponAddress = new PrivateKey().ToAddress();
        Address playerAddress = playerKey.ToAddress();
        WeaponState weaponState = new WeaponState(
            grade: 1,
            address: weaponAddress,
            price: 10000L
        );
        Bencodex.Types.Dictionary playerDict = (Dictionary) new PlayerState(
            name: "ssg",
            address: playerAddress,
            seed: 13
        ).AddWeapon(weaponState).Encode();

        Bencodex.Types.Dictionary sceneDict =
            (Dictionary) playerDict["SceneState"];

        var playerState = new PlayerState(
            playerDict.SetItem(
                "SceneState",
                sceneDict
                    .SetItem("FreeUpgradeWeaponUsed", false)
                    .SetItem("EncounterCleared", 5)
                    .SetItem("Seed", 10)
            )
        );

        var action = new UpgradeWeaponAction(
            health: 0,
            attack: 0,
            defense: 0,
            speed: 0,
            weaponAddress: weaponState.Address
        );

        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = playerState.Encode(),
                [weaponAddress] = weaponState.Encode(),
            }.ToImmutableDictionary()
        );

        Assert.Throws<ArgumentException>(() => {
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
        var weaponSheetCsv = "id,grade,price,hp,atk,def,spd,lifesteal\r\n1,1,5,1,4,7,10,0\r\n2,2,8,2,6,8,11,10\r\n3,3,10,3,5,9,11,15\r\n";
        var skillPresetSheetCsv = "TBD";
        var action = new InitalizeStatesAction(
            weaponSheetCsv,
            skillPresetSheetCsv
        );
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

        foreach (var wsAddress in environmentState.AvailableWeapons)
        {
            Assert.True(
                nextStates.GetState(wsAddress) is Bencodex.Types.Dictionary
            );
        }
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

        var action = new BattleAction(playerSkills);

        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [EnvironmentState.EnvironmentAddress] = _environmentState.Encode(),
                [playerAddress] = playerState.Encode(),
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
            (Dictionary)nextState.GetState(playerAddress)
        );

        Assert.AreEqual(2, playerStateAfterBattle.SceneState.EncounterCleared);
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
            (Dictionary)nextState.GetState(playerAddress)
        );

        Assert.AreEqual(0, playerStateAfterBattle.SceneState.StageCleared);
        Assert.AreEqual(0, playerStateAfterBattle.SceneState.EncounterCleared);
    }
}
