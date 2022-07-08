using System;
using System.Diagnostics.Contracts;
using Flawless.Battle;
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
        public const long InitialId = 0;
        public const long InitialHealth = 0;
        public const long InitialAttack = 0;
        public const long InitialDefense = 0;
        public const long InitialGrade = 0;
        public const long InitialPrice = 0;
        public const long InitialSpeed = 0;
        public const long InitialLifesteal = 0;

        public Address Address { get; private set; }
        public string Name { get; private set; }
        public long Id { get; private set; }
        public long Health { get; private set; }
        public long Attack { get; private set; }
        public long Defense { get; private set; }
        public long Speed { get; private set; }
        public long Lifesteal { get; private set; }
        public long Grade { get; private set; }
        public long Price { get; private set; }

        /// <summary>
        /// Creates a new <see cref="WeaponState"/> instance.
        /// </summary>
        public WeaponState(Address address)
            : base()
        {
            Address = address;
            Name = InitialName;
            Id = InitialId;
            Health = InitialHealth;
            Attack = InitialAttack;
            Defense = InitialDefense;
            Speed = InitialSpeed;
            Lifesteal = InitialLifesteal;
            Grade = InitialGrade;
            Price = InitialPrice;
        }

        public WeaponState(
            Address address,
            string name = InitialName,
            long id = InitialId,
            long health = InitialHealth,
            long attack = InitialAttack,
            long defense = InitialDefense,
            long speed = InitialSpeed,
            long lifesteal = InitialLifesteal,
            long grade = InitialGrade,
            long price = InitialPrice
        ) : base()
        {
            Address = address;
            Id = id;
            Name = name;
            Health = health;
            Attack = attack;
            Defense = defense;
            Speed = speed;
            Lifesteal = lifesteal;
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
            long speed)
        {
            if (health <= 0 && attack <= 0 && defense <= 0 && speed <= 0)
            {
                throw new ArgumentException(
                    $"Can't set 0 points for upgrade."
                );
            }
            else if (health + attack + defense + speed > 3)
            {
                throw new ArgumentException(
                    "Can't applied upgrade points greater than 3; " +
                    $"health: {health}, " +
                    $"attack: {attack}, " +
                    $"defense: {defense}, " +
                    $"speed: {speed}."
                );
            }
            else
            {
                return new WeaponState(
                    address: Address,
                    id: Id,
                    name: Name,
                    health: Health + (health * 10 * Grade),
                    attack: Attack + (attack * 2 * Grade),
                    defense: Defense + (defense * 1 * Grade),
                    speed: Speed + (speed * 1 * Grade),
                    lifesteal: Lifesteal,
                    grade: Grade + 1,
                    price: Price);
            }
        }

        [Pure]
        public Weapon GetWeapon()
        {
            return new Weapon(
                id: (int)Id,
                grade: (int)Grade,
                hp: (int)Health,
                atk: (int)Attack,
                def: (int)Defense,
                spd: (int)Speed,
                lifestealPercentage: (int)Lifesteal);
        }
    }
}
