using System;
using System.Diagnostics.Contracts;
using Libplanet.Store;

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
        public const long StagesPerSession = 10;
        public const long EncountersPerStage = 10;

        public bool InMenu { get; private set; }
        public bool OnWorldMap { get; private set; }
        public bool OnRoad { get; private set; }
        public bool InEncounter { get; private set; }
        public long StageCleared { get; private set; }
        public long EncounterCleared { get; private set; }
        public long Seed { get; private set; }

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
        }

        private SceneState(
            bool inMenu,
            bool onWorldMap,
            bool onRoad,
            bool inEncounter,
            long stageCleared,
            long encounterCleared,
            long seed)
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
        /// Proceeds to next state.
        /// </summary>
        /// <param name="seed">A random seed to newly assign to <see cref="SceneState.Seed"/>.</param>
        /// <returns>The next state of current <see cref="SceneState"/>.</returns>
        public SceneState Proceed(long seed)
        {
            bool inMenu = InMenu;
            bool onWorldMap = OnWorldMap;
            bool onRoad = OnRoad;
            bool inEncounter = InEncounter;
            long stageCleared = StageCleared;
            long encounterCleared = EncounterCleared;

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
                seed: seed);
        }
    }
}
