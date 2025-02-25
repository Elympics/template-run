using System;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class SynchronizedRandomizer : SynchronizedRandomizerBase
    {
        private const uint CombineHash = 0x9E3779B9;

        private int initialSeed;

        private Random tickDependentRandom = null;
        private long tickOffset = -1;

        public override void InitializeRandomization(int seed)
        {
            initialSeed = seed;

            GlobalRandom = new Random(seed);
            tickDependentRandom = new Random(seed);
        }

        public override int InitialSeed => initialSeed;

        public Random GlobalRandom { get; private set; }

        public Random TickDependentRandomizer
        {
            get
            {
                // synchronizing the randomization with current Tick
                if (tickOffset != Elympics.Tick)
                {
                    tickOffset = Elympics.Tick;
                    tickDependentRandom = new Random(CombineSeed(InitialSeed, unchecked((uint)tickOffset)));
                }

                return tickDependentRandom;
            }
        }

        protected virtual int CombineSeed(int initialSeed, uint offset)
            => unchecked((int)(initialSeed + offset * CombineHash));
    }
}
