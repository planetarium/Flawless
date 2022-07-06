using System;
using System.Diagnostics.Contracts;
using Libplanet.Store;

namespace Flawless.States
{
    /// <summary>
    /// A <see cref="DataModel"/> representing player's best record.
    /// </summary>
    public class BestRecordState : DataModel
    {
        public const long InitialStage = 0;

        public const long InitialTurns = 0;

        public long Stage { get; private set; }

        public long Turns { get; private set; }

        /// <summary>
        /// Creates a new <see cref="BestRecordState"/> instance.
        /// </summary>
        public BestRecordState()
            : base()
        {
            Stage = InitialStage;
            Turns = InitialTurns;
        }

        private BestRecordState(long stage, long turns)
            : base()
        {
            Stage = stage;
            Turns = turns;
        }

        /// <summary>
        /// Decodes a stored <see cref="BestRecordState"/>.
        /// </summary>
        /// <param name="encoded">A <see cref="BestRecordState"/> encoded as
        /// a <see cref="Bencodex.Types.Dictionary"/>.</param>
        public BestRecordState(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }

        [Pure]
        public BestRecordState UpdateBestRecord(
            long stage,
            long turns)
        {
            if ((stage > Stage) || (stage == Stage && turns < Turns))
            {
                return new BestRecordState(stage, turns);
            }
            else
            {
                return this;
            }
        }
    }
}
