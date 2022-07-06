using Libplanet.Store;

namespace Flawless.States
{
    /// <summary>
    /// A <see cref="DataModel"/> representing player state.
    /// </summary>
    public class PlayerState : DataModel
    {
        public StatsState StatsState { get; private set; }

        public BestRecordState BestRecordState { get; private set; }

        /// <summary>
        /// Creates a new <see cref="PlayerState"/> instance.
        /// </summary>
        public PlayerState()
            : base()
        {
            StatsState = new StatsState();
            BestRecordState = new BestRecordState();
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
    }
}
