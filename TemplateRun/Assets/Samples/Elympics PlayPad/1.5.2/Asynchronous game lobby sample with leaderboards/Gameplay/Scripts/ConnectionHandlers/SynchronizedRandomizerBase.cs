using Elympics;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public abstract class SynchronizedRandomizerBase : ElympicsMonoBehaviour
    {
        public abstract int InitialSeed { get; }

        public abstract void InitializeRandomization(int seed);
    }
}
