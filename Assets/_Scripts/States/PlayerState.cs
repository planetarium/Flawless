using System;
using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Diagnostics.Contracts;
using System.Linq;
using Flawless.Battle;
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

        public Address Address { get; private set; }
        public string Name { get; private set; }
        public SceneState SceneState { get; private set; }
        public StatsState StatsState { get; private set; }
        public long Gold { get; private set; }
        public BestRecordState BestRecordState { get; private set; }

        /// <summary>
        /// Player's fixed <see cref="Address"/> for character's <see cref="WeaponState"/> slot.
        /// </summary>
        public Address WeaponAddress { get; private set; }

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
            WeaponAddress = new Address(HashDigest<SHA1>.DeriveFrom(address.ByteArray).ByteArray);
            SkillsState = new SkillsState();
        }

        private PlayerState(
            Address address,
            string name,
            SceneState sceneState,
            StatsState statsState,
            long gold,
            BestRecordState bestRecordState,
            Address weaponAddress,
            SkillsState skillsState)
        {
            Address = address;
            Name = name;
            SceneState = sceneState;
            StatsState = statsState;
            Gold = gold;
            BestRecordState = bestRecordState;
            WeaponAddress = weaponAddress;
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
                weaponAddress: WeaponAddress,
                skillsState: SkillsState);
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
                    weaponAddress: WeaponAddress,
                    skillsState: SkillsState);
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
                    weaponAddress: WeaponAddress,
                    skillsState: SkillsState);
            }
        }

        [Pure]
        public PlayerState PutDamage(long amount)
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: StatsState.PutDamage(amount),
                gold: Gold,
                bestRecordState: BestRecordState,
                weaponAddress: WeaponAddress,
                skillsState: SkillsState);
        }

        [Pure]
        public PlayerState Heal(long amount)
        {
            return new PlayerState(
                address: Address,
                name: Name,
                sceneState: SceneState,
                statsState: StatsState.Heal(amount),
                gold: Gold,
                bestRecordState: BestRecordState,
                weaponAddress: WeaponAddress,
                skillsState: SkillsState);
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
                weaponAddress: WeaponAddress,
                skillsState: SkillsState);
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
                weaponAddress: WeaponAddress,
                skillsState: SkillsState);
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
                weaponAddress: WeaponAddress,
                skillsState: SkillsState);
        }

        [Pure]
        public long GetMaxHealth(WeaponState weaponState)
        {
            if (weaponState.Address != default && !HasWeapon(weaponState.Address))
            {
                throw new ArgumentException(
                    $"The player({Address}) doesn't have the given " +
                    $"weapon({weaponState.Address}).");
            }

            return (StatsState.Strength * 8) + weaponState.Health;
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
                weaponAddress: WeaponAddress,
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
                weaponAddress: WeaponAddress,
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
                weaponAddress: WeaponAddress,
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
                weaponAddress: WeaponAddress,
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
                weaponAddress: WeaponAddress,
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
                weaponAddress: WeaponAddress,
                skillsState: SkillsState.SetEquippedSkills(skills));
        }

        [Pure]
        public Character GetCharacter()
        {
            var character = new Character(
                (int)StatsState.Strength,
                (int)StatsState.Dexterity,
                (int)StatsState.Intelligence,
                SkillsState.OwnedSkills.ToList()
            );

            character.Stat.HP -= (int)StatsState.Damages;
            return character;
        }

        [Pure]
        private bool HasWeapon(Address weaponAddress) => (weaponAddress == WeaponAddress);
    }
}
