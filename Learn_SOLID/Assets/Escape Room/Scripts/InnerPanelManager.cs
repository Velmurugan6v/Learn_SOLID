using System;
using UnityEngine;
using UnityEngine.UI;

public class InnerPanelManager : MonoBehaviour
{
    public static InnerPanelManager instance;
    public InnerPanelObjectInformation[] InnerPanels;
    public GameObject innerPanelPanel;
    public Button innerPanelCloseButton;

    public bool isInnerPanelOpened = false;
    public InnerPanelObjectInformation selectedInnerPanel;
    public EscapeRoomManager escapeRoomManager;

    public void Start()
    {
        instance = this;
    }

    public void OpenInnerPanel(int innerPanelIndex)
    {
        if (escapeRoomManager.isCloseShotOpened)
        {
            escapeRoomManager.CloseShot();
        }
        else if (isInnerPanelOpened)
        {
            selectedInnerPanel.InnerPanelObject.SetActive(false);
            innerPanelPanel.SetActive(false);
            selectedInnerPanel = null;
            isInnerPanelOpened = false;
        }

        InnerPanelObjectInformation innerPanel = InnerPanels[innerPanelIndex];

        isInnerPanelOpened = true;

        selectedInnerPanel = innerPanel;

        innerPanelPanel.SetActive(true);

        innerPanel.InnerPanelObject.SetActive(true);
    }


    public void CloseInnerPanel(bool canCallCloseCallback = true)
    {
        selectedInnerPanel.InnerPanelObject.SetActive(false);
        innerPanelPanel.SetActive(false);
        selectedInnerPanel = null;
        isInnerPanelOpened = false;

        if (canCallCloseCallback)
            HintManager.instance.OnHintClose?.Invoke();
        else
            HintManager.instance.OnHintClear?.Invoke();
    }

    public void CloseAllInnerPanels()
    {
        selectedInnerPanel.InnerPanelObject.SetActive(false);
        innerPanelPanel.SetActive(false);
        selectedInnerPanel = null;
        isInnerPanelOpened = false;
    }
}

[System.Serializable]
public class InnerPanelObjectInformation
{
    public string name;
    public GameObject InnerPanelObject;
    public int hintId;
}