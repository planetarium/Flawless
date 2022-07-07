using System;
using System.Diagnostics.Contracts;
using Libplanet;
using Libplanet.Store;

namespace Flawless.States
{
    /// <summary>
    /// A <see cref="DataModel"/> representing character's weapon.
    /// </summary>
    public class WeaponState : DataModel
    {
        public const string InitialName = "Wooden Stick";
        public const long InitialHealth = 0;
        public const long InitialAttack = 0;
        public const long InitialDefense = 0;
        public const int InitialGrade = 0;
        public const long InitialPrice = 0;
        public const long InitialSpeed = 0;
        public const long InitialLifeSteal = 0;
        public static readonly Address InitialAddress = default;

        public Address Address { get; private set; }
        public string Name { get; private set; }
        public long Health { get; private set; }
        public long Attack { get; private set; }
        public long Defense { get; private set; }
        public long Speed { get; private set; }
        public long LifeSteal { get; private set; }
        public int Grade { get; private set; }
        public long Price { get; private set; }

        /// <summary>
        /// Creates a new <see cref="WeaponState"/> instance.
        /// </summary>
        public WeaponState()
            : base()
        {
            Address = InitialAddress;
            Name = InitialName;
            Health = InitialHealth;
            Attack = InitialAttack;
            Defense = InitialDefense;
            Speed = InitialSpeed;
            LifeSteal = InitialLifeSteal;
            Grade = InitialGrade;
            Price = InitialPrice;
        }

        public WeaponState(
            Address address = default,
            string name = InitialName,
            long health = InitialHealth,
            long attack = InitialAttack,
            long defense = InitialDefense,
            long speed = InitialSpeed,
            long lifeSteal = InitialLifeSteal,
            int grade = InitialGrade,
            long price = InitialPrice
        ) : base()
        {
            Address = address;
            Name = name;
            Health = health;
            Attack = attack;
            Defense = defense;
            Speed = speed;
            LifeSteal = lifeSteal;
            Grade = grade;
            Price = price;
        }

        /// <summary>
        /// Decodes a stored <see cref="WeaponState"/>.
        /// </summary>
        /// <param name="encoded">A <see cref="WeaponState"/> encoded as
        /// a <see cref="Bencodex.Types.Dictionary"/>.</param>
        public WeaponState(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }

        [Pure]
        public WeaponState UpgradeWeapon(
            long health,
            long attack,
            long defense,
            long speed,
            long lifeSteal)
        {
            if (health < 0 || attack < 0 || defense < 0 || speed < 0 || lifeSteal < 0)
            {
                throw new ArgumentException(
                    $"Cannot lower weapon stats.");
            }
            else
            {
                return new WeaponState(
                    address: Address,
                    name: Name,
                    health: Health + health,
                    attack: Attack + attack,
                    defense: Defense + defense,
                    speed: Speed + speed,
                    lifeSteal: LifeSteal + lifeSteal
                );
            }
        }
    }
}
