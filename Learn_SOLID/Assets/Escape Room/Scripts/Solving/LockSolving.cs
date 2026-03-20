using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LockSolving : MonoBehaviour,IPointerClickHandler
{
    public LockInfo[] lockInfos;
    public bool canInteract;
    public string clueText;
    public bool[] hasClues;
    public bool isClueViewed;
    public UnityEvent onSolvingComplete;

    public AudioManager audioManager;


    private void Start()
    {
        for (int i = 0; i < lockInfos.Length; i++)
        {
            int tempValue = i;
            lockInfos[tempValue].lockClickButton.onClick.AddListener(() => ClickButton(lockInfos[tempValue]));
        }
    }


    private void ClickButton(LockInfo lockInfo)
    {
        audioManager.PlaySound(0);

        if (!isClueViewed) return;

        lockInfo.lockObjects[lockInfo.currentLockCount].SetActive(false);

        lockInfo.currentLockCount++;

        if (lockInfo.currentLockCount == lockInfo.lockObjects.Length)
            lockInfo.currentLockCount = 0;

        lockInfo.lockObjects[lockInfo.currentLockCount].SetActive(true);
    }

    public void ClueViewed(int clueIndex)
    {
        hasClues[clueIndex] = true;

        foreach (var hasClue in hasClues)
        {
            if (!hasClue)
                return;
        }

        isClueViewed = true;
    }

    public void CheckWon()
    {
        foreach (var lockInfo in lockInfos)
            if (lockInfo.currentLockCount != lockInfo.finalLockCount)
                return;

        print("Won");
        onSolvingComplete?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isClueViewed)
        {
            EscapeRoomManager.instance.ShowAlphaText(clueText);
        }
    }
}

[System.Serializable]
public class LockInfo
{
    public GameObject[] lockObjects;
    public Button lockClickButton;
    public int currentLockCount;
    public int finalLockCount;
}