using System;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
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

        public Address Address { get; private set; }
        public string Name { get; private set; }
        public SceneState SceneState { get; private set; }
        public StatsState StatsState { get; private set; }
        public long Gold { get; private set; }
        public BestRecordState BestRecordState { get; private set; }
        public ImmutableList<Address> Inventory { get; private set;}
        public Address EquippedWeapon { get; private set; }
        public SkillsState SkillsState { get; private set; }

        /// <summary>
        /// Creates a new <see cref="PlayerState"/> instance.
        /// </summary>
        public PlayerState(Address address, string name, long seed)
            : base()
        {
            Address = address;
            Name = name;
            SceneState = new SceneState(seed);
            StatsState = new StatsState();
            Gold = InitialGold;
            BestRecordState = new BestRecordState();
            Inventory = ImmutableList<Address>.Empty;
            EquippedWeapon = Unequipped;
            SkillsState = new SkillsState();
        }

        private PlayerState(
            Address address,
            string name,
            SceneState sceneState,
            StatsState statsState,
            long gold,
            BestRecordState bestRecordState,
            ImmutableList<Address> inventory,
            Address equippedWeapon,
            SkillsState skillsState)
        {
            Address = address;
            Name = name;
            SceneState = sceneState;
            StatsState = statsState;
            Gold = gold;
            BestRecordState = bestRecordState;
            Inventory = inventory;
            EquippedWeapon = equippedWeapon;
            SkillsState = skillsState;
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
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: statsState,
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState
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
                    address: Address,
                    name: Name,
                    sceneState: SceneState,
                    statsState: StatsState,
                    gold: Gold + gold,
                    bestRecordState: BestRecordState,
                    inventory: Inventory,
                    equippedWeapon: EquippedWeapon,
                    skillsState: SkillsState
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
                    address: Address,
                    name: Name,
                    sceneState: SceneState,
                    statsState: StatsState,
                    gold: Gold - gold,
                    bestRecordState: BestRecordState,
                    inventory: Inventory,
                    equippedWeapon: EquippedWeapon,
                    skillsState: SkillsState
                );
            }
        }

        [Pure]
        public PlayerState UpdateBestRecord(BestRecordState bestRecordState)
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: StatsState,
                gold: Gold,
                bestRecordState: bestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState
            );
        }

        [Pure]
        public PlayerState ResetPlayer(long seed)
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: new SceneState(seed),
                statsState: new StatsState(),
                gold: InitialGold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState
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
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: new StatsState(),
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory.Add(weapon.Address),
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState
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
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: new StatsState(),
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: nextInventory,
                equippedWeapon: (EquippedWeapon == weapon.Address)
                    ? Unequipped
                    : EquippedWeapon,
                skillsState: SkillsState
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
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: new StatsState(),
                gold: InitialGold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: weapon.Address,
                skillsState: SkillsState
            );
        }

        [Pure]
        public PlayerState Proceed(long seed)
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState.Proceed(seed),
                statsState: new StatsState(),
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState);
        }

        [Pure]
        public PlayerState EditHealth(long health)
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: StatsState.EditHealth(health),
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState);
        }

        /// <summary>
        /// Adds expeirence to character.  This does not actually add skill points to
        /// <see cref="StatsState"/> automatically.  Manually add skill points using
        /// <see cref="AddPoints"/>.
        /// </summary>
        [Pure]
        public PlayerState AddExperience(long experience)
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: StatsState.AddExperience(experience),
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState);
        }

        /// <summary>
        /// Adds skill points to character.
        /// </summary>
        [Pure]
        public PlayerState AddPoints(long points)
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: StatsState.AddPoints(points),
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState);
        }

        /// <summary>
        /// Resets spent skill points.
        /// </summary>
        [Pure]
        public PlayerState ResetPoints()
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: StatsState.ResetPoints(),
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState);
        }

        [Pure]
        public PlayerState DistributePoints(long strength, long dexterity, long intelligence)
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: StatsState.DistributePoints(strength, dexterity, intelligence),
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState);
        }

        /// <summary>
        /// Changes <see cref="SceneState.FreeHealUsed"/> flag to <see langword="true"/>.
        /// This does not actually heal the character.  Use <see cref="EditHealth"/>
        /// to actually adjust health.
        /// </summary>
        [Pure]
        public PlayerState UseFreeHeal()
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState.UseFreeHeal(),
                statsState: StatsState,
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState);
        }

        /// <summary>
        /// Changes <see cref="SceneState.FreeResetStatsUsed"/> flag to <see langword="true"/>.
        /// This does not actually reset stats.  Use <see cref="ResetStats"/>
        /// to actually reset stats.
        /// </summary>
        [Pure]
        public PlayerState UseFreeResetStats()
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState.UseFreeResetStats(),
                statsState: StatsState,
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState);
        }

        [Pure]
        public PlayerState SetOwnedSkills(ImmutableList<string> skills)
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: StatsState,
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState.SetOwnedSkills(skills));
        }

        [Pure]
        public PlayerState SetEquippedSkills(ImmutableList<string> skills)
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: StatsState,
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState.SetEquippedSkills(skills));
        }

        [Pure]
        public PlayerState UseUpgradeWeapon()
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState.UpgradeWeapon(),
                statsState: StatsState,
                gold: Gold,
                bestRecordState: BestRecordState,
                inventory: Inventory,
                equippedWeapon: EquippedWeapon,
                skillsState: SkillsState);
        }

        [Pure]
        private bool HasWeapon(Address weaponAddress) =>
            Inventory.FirstOrDefault(a => a == weaponAddress) != default;
    }
}
