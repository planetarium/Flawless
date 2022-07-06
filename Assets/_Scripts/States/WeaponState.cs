using System;
using System.Diagnostics.Contracts;
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
        public const long InitialSpeed = 0;
        public const long InitialLifeSteal = 0;

        public string Name { get; private set; }
        public long Health { get; private set; }
        public long Attack { get; private set; }
        public long Defense { get; private set; }
        public long Speed { get; private set; }
        public long LifeSteal { get; private set; }

        /// <summary>
        /// Creates a new <see cref="WeaponState"/> instance.
        /// </summary>
        public WeaponState()
            : base()
        {
            Name = InitialName;
            Health = InitialHealth;
            Attack = InitialAttack;
            Defense = InitialDefense;
            Speed = InitialSpeed;
            LifeSteal = InitialLifeSteal;
        }

        private WeaponState(
            string name,
            long health,
            long attack,
            long defense,
            long speed,
            long lifeSteal)
            : base()
        {
            Name = name;
            Health = health;
            Attack = attack;
            Defense = defense;
            Speed = speed;
            LifeSteal = lifeSteal;
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
                    name: Name,
                    health: Health + health,
                    attack: Attack + attack,
                    defense: Defense + defense,
                    speed: Speed + speed,
                    lifeSteal: LifeSteal + lifeSteal);
            }
        }

        [Pure]
        public long GetPrice()
        {
            return Health + Attack + Defense + Speed + LifeSteal;
        }
    }
}
