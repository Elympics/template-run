using System.Collections.Generic;
using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    [System.Serializable]
    private struct Tile
    {
        public Transform tileTransform;
        public Transform tileEndTransform;
    }

    [SerializeField] private bool shouldMove = true;
    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;
    [SerializeField] private float backgroundSpeed;
    [SerializeField] private float leftScreenBorder;
    [SerializeField] private List<Tile> backgroundTiles;

    private void Start()
    {
        if (gameStateSynchronizer != null)
            gameStateSynchronizer.SubscribeToGameStateChange(AdjustToGameState);
    }

    private void AdjustToGameState(int oldState, int newState)
    {
        shouldMove = (GameState)newState == GameState.Gameplay;
    }

    private void Update()
    {
        if (!shouldMove) return;

        Vector3 moveVector = backgroundSpeed * Time.deltaTime * Vector3.left;
        foreach (var tile in backgroundTiles)
            tile.tileTransform.position += moveVector;

        for (int i = 0; i < backgroundTiles.Count; i++)
        {
            var tile = backgroundTiles[i];
            if (tile.tileEndTransform.position.x <= leftScreenBorder)
            {
                int nextIndex = i - 1;
                if (i == 0) nextIndex = backgroundTiles.Count - 1;
                tile.tileTransform.position = backgroundTiles[nextIndex].tileEndTransform.position;
            }
        }
    }
}
