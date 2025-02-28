using Elympics;
using ElympicsPlayPad.Samples.AsyncGame;

public class RandomManager : SynchronizedRandomizerBase, IUpdatable
{
    private readonly ElympicsInt randomSeed = new ElympicsInt();

    public System.Random InitializedRandom { get; private set; }

    public override int InitialSeed => randomSeed.Value;

    public override void InitializeRandomization(int seed)
    {
        randomSeed.Value = seed;
    }

    public void ElympicsUpdate()
    {
        ResetRandom((int)Elympics.Tick); //Using the base seed and a tick we create a temporary predictable seed
    }

    public void ResetRandom(int tick)
    {
        //We use system random with set seed to make sure, that while random, the stages we will be spawning will be the same on both server and client
        InitializedRandom = new System.Random(randomSeed.Value + tick);
    }
}
