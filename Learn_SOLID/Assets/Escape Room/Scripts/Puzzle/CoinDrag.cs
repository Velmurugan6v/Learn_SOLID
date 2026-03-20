using System;
using DG.Tweening;
using UnityEngine;

public class CoinDrag : MonoBehaviour
{
    public Camera mainCamera;
    public float minDistance = 0.5f;
    public int initialTileId;
    public int destinyTileId;
    public Tile currentTile;
    public Action OnTileChanged;
    public TileManager tileManager;

    private void Awake()
    {
        currentTile = tileManager.tiles[initialTileId];
        currentTile.isOccupied = true;
        transform.position = GetTileTransform(currentTile.id).position;
    }

    private void OnMouseUp()
    {
        Vector3 mouse = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        Vector2 dir = mouse - transform.position;

        if (dir.magnitude < minDistance)
            return;

        dir.Normalize();

        int x = dir.x > 0 ? 1 : -1;
        int y = dir.y > 0 ? 1 : -1;

        if (x == 1 && y == 1)
        {
            Debug.Log("Top Right");
            Move(currentTile.topRightId);
        }
        else if (x == -1 && y == 1)
        {
            Debug.Log("Top Left");
            Move(currentTile.topLeftId);
        }
        else if (x == -1 && y == -1)
        {
            Debug.Log("Bottom Left");
            Move(currentTile.bottomLeftId);
        }
        else
        {
            Debug.Log("Bottom Right");
            Move(currentTile.bottomRightId);
        }
    }

    void Move(int tileId)
    {
        if (tileId == -1)
            return;

        Tile nextTile = GetTile(tileId);

        if (nextTile.isOccupied)
            return;

        Transform target = GetTileTransform(tileId);

        currentTile.isOccupied = false;
        nextTile.isOccupied = true;

        transform.DOMove(target.position, .2f)
            .OnComplete(() =>
            {
                currentTile = nextTile;
                OnTileChanged?.Invoke();
            });
    }

    public Tile GetTile(int currentTileId)
    {
        Tile nextTile = tileManager.tiles[currentTileId];

        return nextTile;
    }

    public Transform GetTileTransform(int currentTileId)
    {
        return tileManager.tilesTransform[currentTileId];
    }

    public void ResetCoin()
    {
        currentTile.isOccupied = false;
        currentTile = GetTile(initialTileId);
        currentTile.isOccupied = true;
        transform.position = GetTileTransform(currentTile.id).position;
    }

    public bool IsCoinReachedDestiny()
    {
        return currentTile.alternateId == destinyTileId;
    }
}