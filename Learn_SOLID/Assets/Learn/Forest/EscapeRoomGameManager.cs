using System;
using UnityEngine;
using UnityEngine.UI;

public class EscapeRoomGameManager : MonoBehaviour
{
    [Header("Rooms")] public GameObject[] rooms;
    public int currentRoom;

    [Header("ZoomShot")] public bool isZoomShotOpened = false;
    public ZoomShotInformation[] zoomShots;
    public ZoomShotInformation currentOpenedZoomShot;
    public Button zoomShotCloseButton;

    [Header("Animation")] public Animator animator;

    [Header("Inventory")] public InventoryObject[] inventoryObjects;
    public InventorySlot[] inventorySlots;

    private void Start()
    {
        IniIt();
        SetupZoomShotCloseButton();
    }

    private void SetupZoomShotCloseButton()
    {
        zoomShotCloseButton.onClick.AddListener(CloseZoomShot);
    }

    public void IniIt()
    {
        currentRoom = 0;

        foreach (GameObject room in rooms)
            room.SetActive(false);

        rooms[currentRoom].SetActive(true);
    }

    #region Rooms
    public void NextRoom()
    {
        rooms[currentRoom].SetActive(false);

        currentRoom++;

        if (currentRoom > rooms.Length - 1)
            currentRoom = 0;

        rooms[currentRoom].SetActive(true);
    }

    public void PreviousRoom()
    {
        rooms[currentRoom].SetActive(false);

        currentRoom--;

        if (currentRoom < 0)
            currentRoom = rooms.Length - 1;

        rooms[currentRoom].SetActive(true);
    }
    
    #endregion

    #region ZoomShot

    //ZoomShot Work

    public void OpenZoomShot(int zoomShotIndex)
    {
        zoomShotCloseButton.gameObject.SetActive(true);

        ZoomShotInformation zoomShot = zoomShots[zoomShotIndex];

        if (isZoomShotOpened == false)
        {
            isZoomShotOpened = true;
            currentOpenedZoomShot = zoomShot;
        }

        currentOpenedZoomShot.zoomShotGameObject.SetActive(true);
    }

    public void CloseZoomShot()
    {
        if (currentOpenedZoomShot == null) return;

        isZoomShotOpened = false;

        currentOpenedZoomShot.zoomShotGameObject.SetActive(false);

        currentOpenedZoomShot = null;

        zoomShotCloseButton.gameObject.SetActive(false);
    }

    #endregion

    #region Inventory
    
    
    #endregion
}

/// <summary>
/// ZoomShot Information Class
/// </summary>
[Serializable]
public class ZoomShotInformation
{
    public GameObject zoomShotGameObject;
    public Button clickButton;
    public bool canClickZoomShot;
}


[Serializable]
public class InventoryObject
{
    public string inventoryName;
    public int inventoryId;
    public Sprite inventoryIcon;
    public bool canClickInventory;
    public InventorySlot inventorySlot;
}
