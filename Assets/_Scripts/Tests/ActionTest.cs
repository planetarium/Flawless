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
        Address weaponAddress = new PrivateKey().ToAddress();
        Address playerAddress = playerKey.ToAddress();
        WeaponState weaponState = new WeaponState(
            address: weaponAddress,
            price: 10000L
        );

        var playerState = new PlayerState(
            name: "ssg",
            address: playerAddress
        ).AddWeapon(weaponState);

        long initialGold = playerState.Gold;
        var previousStates = new State(
            new Dictionary<Address, IValue>
            {
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
}
