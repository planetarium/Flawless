using System;
using System.Diagnostics.Contracts;
using System.Collections.Immutable;
using System.Linq;
using Libplanet;
using Libplanet.Store;

namespace Flawless.States
{
    /// <summary>
    /// A <see cref="DataModel"/> representing player state.
    /// </summary>
    public class PlayerState : DataModel
    {
        public static readonly Address Unequipped = default;
        public const long InitialGold = 0;

        public string Name { get; private set; }
        public Address Address { get; private set; }
        public SceneState SceneState { get; private set; }
        public StatsState StatsState { get; private set; }
        public long Gold { get; private set; }
        public BestRecordState BestRecordState { get; private set; }
        public ImmutableList<Address> Inventory { get; private set;}
        public Address EquippedWeapon { get; private set; }

        /// <summary>
        /// Creates a new <see cref="PlayerState"/> instance.
        /// </summary>
        public PlayerState(string name, Address address, long seed)
            : base()
        {
            Name = name;
            Address = address;
            SceneState = new SceneState(seed);
            StatsState = new StatsState();
            Gold = InitialGold;
            BestRecordState = new BestRecordState();
            Inventory = ImmutableList<Address>.Empty;
            EquippedWeapon = Unequipped;
        }

        private PlayerState(
            string name,
            Address address,
            SceneState sceneState,
            StatsState statsState,
            long gold,
            BestRecordState bestRecordState,
            ImmutableList<Address> inventory,
            Address equippedWeapon)
        {
            Name = name;
            Address = address;
            SceneState = sceneState;
            StatsState = statsState;
            Gold = gold;
            BestRecordState = bestRecordState;
            Inventory = inventory;
            EquippedWeapon = equippedWeapon;
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
                sceneState: SceneState,
                statsState: statsState,
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon
            );
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
                    sceneState: SceneState,
                    statsState: StatsState,
                    gold: Gold + gold,
                    bestRecordState: BestRecordState,
                    inventory: Inventory,
                    equippedWeapon: EquippedWeapon
                );
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
                    sceneState: SceneState,
                    statsState: StatsState,
                    gold: Gold - gold,
                    bestRecordState: BestRecordState,
                    inventory: Inventory,
                    equippedWeapon: EquippedWeapon
                );
            }
        }

        [Pure]
        public PlayerState UpdateBestRecord(BestRecordState bestRecordState)
        {
            return new PlayerState(
                name: Name,
                address: Address,
                sceneState: SceneState,
                statsState: StatsState,
                gold: Gold,
                bestRecordState: bestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon
            );
        }

        [Pure]
        public PlayerState ResetPlayer(long seed)
        {
            return new PlayerState(
                name: Name,
                address: Address,
                sceneState: new SceneState(seed),
                statsState: new StatsState(),
                gold: InitialGold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon
            );
        }

        [Pure]
        public PlayerState AddWeapon(WeaponState weapon)
        {
            if (HasWeapon(weapon.Address))
            {
                throw new ArgumentException(
                    $"The given weapon({weapon.Address}) already is in the " +
                    $"player({Address})'s inventory."
                );
            }

            return new PlayerState(
                name: Name,
                address: Address,
                sceneState: SceneState,
                statsState: new StatsState(),
                gold: InitialGold,
                bestRecordState: BestRecordState,
                inventory: Inventory.Add(weapon.Address),
                equippedWeapon: EquippedWeapon
            );
        }

        [Pure]
        public PlayerState RemoveWeapon(WeaponState weapon)
        {
            if (!HasWeapon(weapon.Address))
            {
                throw new ArgumentException(
                    $"The player({Address}) doesn't have the given " +
                    $"weapon({weapon.Address})."
                );
            }

            ImmutableList<Address> nextInventory =
                Inventory.Where(a => a != weapon.Address).ToImmutableList();

            return new PlayerState(
                name: Name,
                address: Address,
                sceneState: SceneState,
                statsState: new StatsState(),
                gold: InitialGold,
                bestRecordState: BestRecordState,
                inventory: nextInventory,
                equippedWeapon: (EquippedWeapon == weapon.Address)
                    ? Unequipped
                    : EquippedWeapon
            );
        }

        [Pure]
        public PlayerState Equip(WeaponState weapon)
        {
            if (!HasWeapon(weapon.Address))
            {
                throw new ArgumentException(
                    $"The player({Address}) doesn't have the given " +
                    $"weapon({weapon.Address})."
                );
            }

            return new PlayerState(
                name: Name,
                address: Address,
                sceneState: SceneState,
                statsState: new StatsState(),
                gold: InitialGold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: weapon.Address
            );
        }

        [Pure]
        public PlayerState Proceed(long seed)
        {
            return new PlayerState(
                name: Name,
                address: Address,
                sceneState: SceneState.Proceed(seed),
                statsState: new StatsState(),
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon);
        }

        [Pure]
        private bool HasWeapon(Address weaponAddress) =>
            Inventory.FirstOrDefault(a => a == weaponAddress) != default;
    }
}
