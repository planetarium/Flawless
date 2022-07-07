using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Flawless.States;
using Flawless.Data;

public class StatesTest
{
    [Test]
    public void WeaponStateFactory_CreateWeaponStates()
    {
        string csv = "id,grade,price,hp,atk,def,spd,lifesteal\r\n1,1,5,1,4,7,10,0\r\n2,2,8,2,6,8,11,10\r\n3,3,10,3,5,9,11,15\r\n";
        var sheet = new WeaponSheet();
        sheet.Set(csv);

        WeaponState[] expected = new[]
        {
            new WeaponState(
                address: WeaponStateFactory.DeriveAddress(1),
                id: 1,
                grade: 1,
                price: 5,
                health: 1,
                attack: 4,
                defense: 7,
                speed: 10,
                lifesteal: 0
            ),
            new WeaponState(
                address: WeaponStateFactory.DeriveAddress(2),
                id: 2,
                grade: 2,
                price: 8,
                health: 2,
                attack: 6,
                defense: 8,
                speed: 11,
                lifesteal: 10
            ),
            new WeaponState(
                address: WeaponStateFactory.DeriveAddress(3),
                id: 3,
                grade: 3,
                price: 10,
                health: 3,
                attack: 5,
                defense: 9,
                speed: 11,
                lifesteal: 15
            ),
        };

        WeaponState[] actual =
            WeaponStateFactory.CreateWeaponStates(sheet).ToArray();

        Assert.AreEqual(
            expected.Select(e => e.Encode()),
            actual.Select(a => a.Encode())
        );
    }
}
