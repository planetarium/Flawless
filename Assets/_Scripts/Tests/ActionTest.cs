using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
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

    [Test]
    public void SellWeaponAction_Execute()
    {
        var playerKey = new PrivateKey();
        Address playerAddress = playerKey.ToAddress();
        var weaponState = new WeaponState().UpgradeWeapon(
            health: 10,
            attack: 15,
            defense: 20,
            speed: 30,
            lifeSteal: 40
        );

        var playerState = new PlayerState(
            name: "ssg",
            address: playerAddress
        ).UpgradeWeapon(
            weaponState
        );
        long initialGold = playerState.Gold;
        
        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
                [playerAddress] = playerState.Encode(),
            }.ToImmutableDictionary()
        );
        var action = new SellWeaponAction();
        
        IAccountStateDelta nextState = action.Execute(new ActionContext
        {
            PreviousStates = previousStates,
            Signer = playerAddress,
            BlockIndex = 0,
        });

        var playerStateAfterSell = new PlayerState(
            (Dictionary)nextState.GetState(playerAddress)
        );

        Assert.AreEqual(0, playerStateAfterSell.WeaponState.GetPrice());
        Assert.AreEqual(
            initialGold + weaponState.GetPrice(),
            playerStateAfterSell.Gold
        );
    }
}
