using Elympics;

public class RandomManager : ElympicsMonoBehaviour
{
    private readonly ElympicsInt randomSeed = new ElympicsInt();
    public System.Random InitializedRandom { get; private set; }

    public void ResetRandom(int tick)
    {
        //We use system random with set seed to make sure, that while random, the stages we will be spawning will be the same on both server and client
        InitializedRandom = new System.Random(randomSeed.Value + tick);
    }

    public void SetSeed(int seed)
    {
        randomSeed.Value = seed;
    }
}
