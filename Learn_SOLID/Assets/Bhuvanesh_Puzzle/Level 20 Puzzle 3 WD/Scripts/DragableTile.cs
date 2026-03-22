using System;
using DG.Tweening;
using UnityEngine;

public class DragableTile : MonoBehaviour
{
    public Camera mainCamera;
    public float minDistance = 0.5f;
    public int initialTileId;
    public int destinyTileId;
    public ProtractorTile currentTile;
    public Action OnTileChanged;
    public ProtractorManager_L20 tileManager;

    private void Awake()
    {
        currentTile = tileManager.grid[initialTileId];
        currentTile.isOccupied = true;
        transform.position = GetTileTransform(currentTile.id).position;
    }

    private void OnMouseUp()
    {
        Vector2 dir = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        if (dir.sqrMagnitude < minDistance * minDistance)
            return;

        dir.Normalize();

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (angle < 0) angle += 360;

        int sector = Mathf.RoundToInt(angle / 22.5f) % 16;

        print(sector);

        if (sector == 0)
        {
            //Right  
            Move(currentTile.rightId);
        }
        else if (sector == 1)
        {
            //Right Up-->
            Move(currentTile.rightUpId);
        }
        else if (sector == 2)
        {
            //Top Right Diagonal
            Move(currentTile.topRightDiagonal);
        }
        else if (sector == 3)
        {
            //Top right-->
            Move(currentTile.topRightId);
        }
        else if (sector == 4)
        {
            //Top
            Move(currentTile.topId);
        }
        else if (sector == 5)
        {
            //Top Left--->
            Move(currentTile.topLeftId);
        }
        else if (sector == 6)
        {
            //Top Left Diagonal
            Move(currentTile.topLeftDiagonal);
        }
        else if (sector == 7)
        {
            //Left Up--->
            Move(currentTile.leftUpId);
        }
        else if (sector == 8)
        {
            //Left
            Move(currentTile.leftId);
        }

        else if (sector == 9)
        {
            //Left Down--->
            Move(currentTile.leftDownId);
        }
        else if (sector == 10)
        {
            //Bottom Left Diagonal
            Move(currentTile.bottomLeftDiagonal);
        }
        else if (sector == 11)
        {
            //Down Left--->
            Move(currentTile.downLeftId);
        }
        else if (sector == 12)
        {
            //Bottom
            Move(currentTile.bottomId);
        }
        else if (sector == 13)
        {
            //Down Right-->
            Move(currentTile.downRightId);
        }
        else if (sector == 14)
        {
            //Bottom Right Diagonal
            Move(currentTile.bottomRightDiagonalId);
        }
        else if (sector == 15)
        {
            //Right Down-->
            Move(currentTile.rightDownId);
        }
        /*int x = dir.x > 0 ? 1 : -1;
        int y = dir.y > 0 ? 1 : -1;

        if (x == 1 && y == 1)
        {
            Debug.Log("Top Right");
            Move(currentTile.topRightDiagonal);
        }
        else if (x == -1 && y == 1)
        {
            Debug.Log("Top Left");
            Move(currentTile.topLeftDiagonal);
        }
        else if (x == -1 && y == -1)
        {
            Debug.Log("Bottom Left");
            Move(currentTile.bottomLeftDiagonal);
        }
        else
        {
            Debug.Log("Bottom Right");
            Move(currentTile.bottomRightDiagonalId);
        }*/
    }

    void Move(int tileId)
    {
        if (tileId == -1)
            return;

        ProtractorTile nextTile = GetTile(tileId);

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

    public ProtractorTile GetTile(int currentTileId)
    {
        ProtractorTile nextTile = tileManager.grid[currentTileId];

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

    public void MoveFinalPosition()
    {
        transform.DOMove(tileManager.tilesTransform[destinyTileId].position,0.5f);
    }
}