using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileManager : MonoBehaviour
{
    public List<Tile> tiles;
    public List<Transform> tilesTransform;
    public CoinDrag[] coins;

    public PuzzleManager puzzleManager;

    private void Start()
    {
        foreach (CoinDrag coin in coins)
            coin.OnTileChanged += CheckWin;
    }


    [ContextMenu("ResetCoin")]
    public void ResetCoin()
    {
        foreach (CoinDrag coin in coins)
            coin.ResetCoin();
    }

    public void CheckWin()
    {
        foreach (CoinDrag coin in coins)
            if (!coin.IsCoinReachedDestiny())
                return;

        print("You win!");
        puzzleManager.onPuzzleClosed?.Invoke();
        Invoke(nameof(Win), 1f);
    }

    private void Win()
    {
        puzzleManager.onPuzzleComplete.Invoke();
    }

    public void SkipPuzzle()
    {
        Invoke(nameof(Win), 1.2f);
    }
}

[System.Serializable]
public class Tile
{
    public int id;
    public bool isOccupied = false;
    public int topLeftId = -1;
    public int topRightId = -1;
    public int bottomLeftId = -1;
    public int bottomRightId = -1;
    public int alternateId = -1;
}