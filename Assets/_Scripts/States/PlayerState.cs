using System;
using System.Diagnostics.Contracts;
using Libplanet;
using Libplanet.Store;

namespace Flawless.States
{
    /// <summary>
    /// A <see cref="DataModel"/> representing player state.
    /// </summary>
    public class PlayerState : DataModel
    {
        public const long InitialGold = 0;

        public string Name { get; private set; }
        public Address Address { get; private set; }
        public StatsState StatsState { get; private set; }
        public WeaponState WeaponState { get; private set; }
        public long Gold { get; private set; }
        public BestRecordState BestRecordState { get; private set; }

        /// <summary>
        /// Creates a new <see cref="PlayerState"/> instance.
        /// </summary>
        public PlayerState(string name, Address address)
            : base()
        {
            Name = name;
            Address = address;
            StatsState = new StatsState();
            WeaponState = new WeaponState();
            Gold = InitialGold;
            BestRecordState = new BestRecordState();
        }

        private PlayerState(
            string name,
            Address address,
            StatsState statsState,
            WeaponState weaponState,
            long gold,
            BestRecordState bestRecordState)
        {
            Name = name;
            Address = address;
            StatsState = statsState;
            WeaponState = weaponState;
            Gold = gold;
            BestRecordState = bestRecordState;
        }

        /// <summary>
        /// Decodes a stored <see cref="PlayerState"/>.
        /// </summary>
        /// <param name="encoded">A <see cref="PlayerState"/> encoded as
        /// a <see cref="Bencodex.Types.Dictionary"/>.</param>
        public PlayerState(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }

        [Pure]
        public PlayerState UpdateStats(StatsState statsState)
        {
            return new PlayerState(
                name: Name,
                address: Address,
                statsState: statsState,
                weaponState: WeaponState,
                gold: Gold,
                bestRecordState: BestRecordState);
        }

        [Pure]
        public PlayerState UpgradeWeapon(WeaponState weaponState)
        {
            return new PlayerState(
                name: Name,
                address: Address,
                statsState: StatsState,
                weaponState: weaponState,
                gold: Gold,
                bestRecordState: BestRecordState);
        }

        [Pure]
        public PlayerState SellWeapon()
        {
            return new PlayerState(
                name: Name,
                address: Address,
                statsState: StatsState,
                weaponState: new WeaponState(),
                gold: Gold + WeaponState.GetPrice(),
                bestRecordState: BestRecordState);
        }

        [Pure]
        public PlayerState AddGold(long gold)
        {
            if (gold < 0)
            {
                throw new ArgumentException(
                    $"Cannot add negative amount of gold.");
            }
            else
            {
                return new PlayerState(
                    name: Name,
                    address: Address,
                    statsState: StatsState,
                    weaponState: WeaponState,
                    gold: Gold + gold,
                    bestRecordState: BestRecordState);
            }
        }

        [Pure]
        public PlayerState SubtractGold(long gold)
        {
            if (gold < 0)
            {
                throw new ArgumentException(
                    $"Cannot subtract negative amount of gold.");
            }
            else
            {
                return new PlayerState(
                    name: Name,
                    address: Address,
                    statsState: StatsState,
                    weaponState: WeaponState,
                    gold: Gold + gold,
                    bestRecordState: BestRecordState);
            }
        }

        [Pure]
        public PlayerState UpdateBestRecord(BestRecordState bestRecordState)
        {
            return new PlayerState(
                name: Name,
                address: Address,
                statsState: StatsState,
                weaponState: WeaponState,
                gold: Gold,
                bestRecordState: bestRecordState);
        }

        [Pure]
        public PlayerState ResetPlayer()
        {
            return new PlayerState(
                name: Name,
                address: Address,
                statsState: new StatsState(),
                weaponState: new WeaponState(),
                gold: InitialGold,
                bestRecordState: BestRecordState);
        }
    }
}
