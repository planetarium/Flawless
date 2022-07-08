using System;
using System.Diagnostics.Contracts;
using Libplanet.Store;
using Flawless.Models.Encounters;

namespace Flawless.States
{
    /// <summary>
    /// A <see cref="DataModel"/> representing character's scene and progress.
    /// </summary>
    public class SceneState : DataModel
    {
        public const bool InitialInMenu = true;
        public const bool InitialOnWorldMap = false;
        public const bool InitialOnRoad = false;
        public const bool InitialInEncounter = false;
        public const long InitialStageCleared = 0;
        public const long InitialEncounterCleared = 0;
        public const long StagesPerSession = 4;
        public const long EncountersPerStage = 6;
        public const bool InitialFreeHealUsed = false;
        public const bool InitialFreeResetPointsUsed = false;
        public const bool InitialFreeUpgradeWeaponUsed = false;

        public bool InMenu { get; private set; }
        public bool OnWorldMap { get; private set; }
        public bool OnRoad { get; private set; }
        public bool InEncounter { get; private set; }
        public long StageCleared { get; private set; }
        public long EncounterCleared { get; private set; }

        /// <summary>
        /// A random seed to deterministically generate the next <see cref="Encounter"/>.
        /// This value is updated only when <see cref="EncounterCleared"/> changes.
        /// </summary>
        public long Seed { get; private set; }

        public bool FreeHealUsed { get; private set; }
        public bool FreeResetPointsUsed { get; private set; }
        public bool FreeUpgradeWeaponUsed { get; private set; }

        /// <summary>
        /// Creates a new <see cref="SceneState"/> instance.
        /// </summary>
        public SceneState(long seed)
            : base()
        {
            InMenu = InitialInMenu;
            OnWorldMap = InitialOnWorldMap;
            OnRoad = InitialOnRoad;
            InEncounter = InitialInEncounter;
            StageCleared = InitialStageCleared;
            EncounterCleared = InitialEncounterCleared;
            Seed = seed;
            FreeHealUsed = InitialFreeHealUsed;
            FreeResetPointsUsed = InitialFreeResetPointsUsed;
            FreeUpgradeWeaponUsed = InitialFreeUpgradeWeaponUsed;
        }

        private SceneState(
            bool inMenu,
            bool onWorldMap,
            bool onRoad,
            bool inEncounter,
            long stageCleared,
            long encounterCleared,
            long seed,
            bool freeHealUsed,
            bool freeResetPointsUsed,
            bool freeUpgradeWeaponUsed)
        {
            if (Convert.ToInt32(inMenu) + Convert.ToInt32(onWorldMap) +
                Convert.ToInt32(onRoad) + Convert.ToInt32(inEncounter) != 1)
            {
                throw new ArgumentException(
                    $"Exactly one of {nameof(inMenu)}, {nameof(onWorldMap)}, " +
                    $"{nameof(onRoad)}, {nameof(inEncounter)} must be true");
            }

            InMenu = inMenu;
            OnWorldMap = onWorldMap;
            OnRoad = onRoad;
            InEncounter = inEncounter;
            StageCleared = stageCleared;
            EncounterCleared = encounterCleared;
            Seed = seed;
            FreeHealUsed = freeHealUsed;
            FreeResetPointsUsed = freeResetPointsUsed;
            FreeUpgradeWeaponUsed = freeUpgradeWeaponUsed;
        }

        /// <summary>
        /// Decodes a stored <see cref="SceneState"/>.
        /// </summary>
        /// <param name="encoded">A <see cref="SceneState"/> encoded as
        /// a <see cref="Bencodex.Types.Dictionary"/>.</param>
        public SceneState(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }

        /// <summary>
        /// Proceeds to next state and updated <see cref="Seed"/> with <paramref name="seed"/>.
        /// The vaue of <see cref="Seed"/> is only updated when an <see cref="Encounter"/>
        /// is cleared.
        /// </summary>
        /// <param name="seed">A random seed to newly assign to <see cref="Seed"/>
        /// only when <see cref="EncounterCleared"/> is changed.</param>
        /// <returns>The next state of current <see cref="SceneState"/>.</returns>
        [Pure]
        public SceneState Proceed(long seed)
        {
            bool inMenu = InMenu;
            bool onWorldMap = OnWorldMap;
            bool onRoad = OnRoad;
            bool inEncounter = InEncounter;
            long stageCleared = StageCleared;
            long encounterCleared = EncounterCleared;
            long nextSeed = Seed;

            if (inMenu)
            {
                if (stageCleared != 0 || encounterCleared !=0)
                {
                    throw new ArgumentException(
                        $"Can proceed from {nameof(InMenu)} true state only when " +
                        $"both {nameof(StageCleared)} and {nameof(EncounterCleared)} are 0; " +
                        $"create a new initial {nameof(SceneState)} instead.");
                }
                else
                {
                    inMenu = false;
                    onWorldMap = true;
                }
            }
            else if (onWorldMap)
            {
                onWorldMap = false;
                onRoad = true;
            }
            else if (onRoad)
            {
                onRoad = false;
                inEncounter = true;
            }
            else if (inEncounter)
            {
                inEncounter = false;
                encounterCleared += 1;
                nextSeed = seed;
                if (encounterCleared == EncountersPerStage)
                {
                    encounterCleared = 0;
                    stageCleared += 1;
                    if (stageCleared == StagesPerSession)
                    {
                        inMenu = true;
                    }
                    else
                    {
                        onWorldMap = true;
                    }
                }
                else
                {
                    onRoad = true;
                }
            }
            else
            {
                throw new ArgumentException(
                    $"Something went wrong.");
            }

            return new SceneState(
                inMenu: inMenu,
                onWorldMap: onWorldMap,
                onRoad: onRoad,
                inEncounter: inEncounter,
                stageCleared: stageCleared,
                encounterCleared: encounterCleared,
                seed: nextSeed,
                freeHealUsed: FreeHealUsed,
                freeResetPointsUsed: FreeResetPointsUsed,
                freeUpgradeWeaponUsed: FreeUpgradeWeaponUsed);
        }

        /// <summary>
        /// Changes <see cref="FreeHealUsed"/> flag to <see langword="true"/>.
        /// This does not actually heal the character.  Use <see cref="PlayerState.EditHealth"/>
        /// to actually adjust health.
        /// </summary>
        public SceneState UseFreeHeal()
        {
            if (FreeHealUsed)
            {
                throw new ArgumentException(
                    $"Free heal already used.");
            }

            return new SceneState(
                inMenu: InMenu,
                onWorldMap: OnWorldMap,
                onRoad: OnRoad,
                inEncounter: InEncounter,
                stageCleared: StageCleared,
                encounterCleared: EncounterCleared,
                seed: Seed,
                freeHealUsed: true,
                freeResetPointsUsed: FreeResetPointsUsed,
                freeUpgradeWeaponUsed: FreeUpgradeWeaponUsed);
        }

        /// <summary>
        /// Changes <see cref="FreeResetPointsUsed"/> flag to <see langword="true"/>.
        /// This does not actually reset stats.  Use <see cref="PlayerState.ResetStats"/>
        /// to actually reset stats.
        /// </summary>
        public SceneState UseFreeResetPoints()
        {
            if (FreeResetPointsUsed)
            {
                throw new ArgumentException(
                    $"Free reset stats already used.");
            }

            return new SceneState(
                inMenu: InMenu,
                onWorldMap: OnWorldMap,
                onRoad: OnRoad,
                inEncounter: InEncounter,
                stageCleared: StageCleared,
                encounterCleared: EncounterCleared,
                seed: Seed,
                freeHealUsed: FreeHealUsed,
                freeResetPointsUsed: true,
                freeUpgradeWeaponUsed: FreeUpgradeWeaponUsed
            );
        }

        [Pure]
        public SceneState UpgradeWeapon()
        {
            return new SceneState(
                inMenu: InMenu,
                onWorldMap: OnWorldMap,
                onRoad: OnRoad,
                inEncounter: InEncounter,
                stageCleared: StageCleared,
                encounterCleared: EncounterCleared,
                seed: Seed,
                freeHealUsed: FreeHealUsed,
                freeResetPointsUsed: FreeResetPointsUsed,
                freeUpgradeWeaponUsed: true
            );
        }

        [Pure]
        public Encounter GetEncounter(EnvironmentState environmentState)
        {
            return Encounter.GenerateEncounter(
                StageCleared + 1,
                EncounterCleared + 1,
                Seed,
                environmentState.AvailableWeapons
            );
        }
    }
}
