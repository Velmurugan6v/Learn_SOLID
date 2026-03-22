using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ProtractorManager_L20 : MonoBehaviour
{
    public ProtractorTile[] grid;
    public List<Transform> tilesTransform;

    public DragableTile[] dragTiles;


    private void Start()
    {
        foreach (DragableTile tile in dragTiles)
            tile.OnTileChanged += CheckWin;
    }


    [ContextMenu("Reset Tiles")]
    public void ResetCoin()
    {
        foreach (DragableTile tile in dragTiles)
            tile.ResetCoin();
    }
    [ContextMenu("SkipAll")]
    public void Skip()
    {
        foreach (DragableTile tile in dragTiles)
            tile.MoveFinalPosition();
    }
    public void CheckWin()
    {
        foreach (DragableTile tile in dragTiles)
            if (!tile.IsCoinReachedDestiny())
                return;

        print("You win!");
    }
}

[System.Serializable]
public class ProtractorTile
{
    public int id;
    public bool isOccupied = false;
    public int topId = -1;
    public int rightId = -1;
    public int leftId = -1;
    public int bottomId = -1;
    public int topLeftDiagonal = -1;
    public int topRightDiagonal = -1;
    public int bottomLeftDiagonal = -1;
    public int bottomRightDiagonalId = -1;
    public int rightUpId = -1;
    public int rightDownId = -1;
    public int downLeftId = -1;
    public int downRightId = -1;
    public int leftUpId = -1;
    public int leftDownId = -1;
    public int topRightId = -1;
    public int topLeftId = -1;
    public int alternateId = -1;
}