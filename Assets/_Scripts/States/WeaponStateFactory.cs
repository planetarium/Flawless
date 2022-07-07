using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Flawless.Data;
using Libplanet;
using Libplanet.Crypto;

namespace Flawless.States
{
    public static class WeaponStateFactory
    {
        private static readonly byte[] AddressKey = 
            new byte[] { 0x00, 0x01, 0x02, 0x03 };
        public static IEnumerable<WeaponState> CreateWeaponStates(
            WeaponSheet sheet
        )
        {
            foreach (KeyValuePair<int, WeaponSheet.Row> kv in sheet)
            {
                yield return new WeaponState(
                    address: DeriveAddress(kv.Key),
                    attack: kv.Value.Attack,
                    defense: kv.Value.Defense,
                    grade: kv.Value.Grade,
                    health: kv.Value.HP,
                    lifeSteal: kv.Value.LifeStealPercentage,
                    price: kv.Value.Price,
                    speed: kv.Value.Speed
                );
            }
        }

        public static Address DeriveAddress(int id)
        {
            byte[] hashed;
            using (var hmac = new HMACSHA1(AddressKey))
            {
                hashed = hmac.ComputeHash(BitConverter.GetBytes(id));
            }

            return new Address(hashed);
        }
    }
}
