using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    public static HintManager instance;
    public Button hintButton;
    public GameObject hintGlowGameObject;
    public List<HintDetail> hints = new List<HintDetail>();
    public HintDetail selectedHint;
    public bool isSelectedHave = false;
    public Action OnHintClose;
    public Action OnHintClear;
    private Transform _targetInventory;
    public int currentHintId = -1;

    [Header("BG Hint Count")] public int[] bgHintStar;
    public List<HintDetail> currentBgHints = new List<HintDetail>();

    private void Awake()
    {
        InItHint();
    }

    private void OnEnable()
    {
        hintButton.onClick.AddListener(ShowHint);
        OnHintClose += HideHint;
        OnHintClear += ClearHint;
    }

    private void OnDisable()
    {
        hintButton.onClick.RemoveListener(ShowHint);
        OnHintClose -= HideHint;
        OnHintClear -= ClearHint;
    }


    private void InItHint()
    {
        for (int i = 0; i < hints.Count; i++)
        {
            int hintIndex = i;
            hints[i].hintShowButton?.onClick.AddListener(() => CompleteHint(hints[hintIndex]));
        }
    }

    public virtual void ShowHint()
    {
        if (currentHintId == -1)
        {
            selectedHint = currentBgHints.Find(h => !h.isHintCompleted);
        }
        else
        {
            selectedHint = FindNextIncompleteHint(currentHintId);
        }

        if (selectedHint == null)
        {
            Debug.Log("No hint available");
            return;
        }

        ShowHintByType(selectedHint);
    }

    void ShowHintByType(HintDetail hint)
    {
        switch (hint.hintType)
        {
            case HintType.Bg:

                if (EscapeRoomManager.instance.isCloseShotOpened)
                    EscapeRoomManager.instance.CloseShotNoAnimation();

                StartCoroutine(ShowHintGlowObject(hint.hintShowButton.transform));
                break;

            case HintType.CloseShot:

                StartCoroutine(ShowHintGlowObject(hint.hintShowButton.transform));
                break;

            case HintType.Inventory:

                for (int i = 0; i < InventoryObjectManager.instance.inventorySlots.Count; i++)
                {
                    var slot = InventoryObjectManager.instance.inventorySlots[i];

                    if (slot.inventoryItem != null &&
                        hint.inventoryId == slot.inventoryItem.id)
                    {
                        _targetInventory = slot.transform;
                        break;
                    }
                }

                if (_targetInventory != null)
                    StartCoroutine(ShowHintGlowObject(_targetInventory));

                break;

            case HintType.InnerPanel:

                for (int i = 0; i < InventoryObjectManager.instance.inventorySlots.Count; i++)
                {
                    var slot = InventoryObjectManager.instance.inventorySlots[i];

                    if (slot.inventoryItem != null &&
                        hint.inventoryId == slot.inventoryItem.id)
                    {
                        _targetInventory = slot.transform;
                        int innerPanelId = slot.inventoryItem.innerPanelId;
                        currentHintId = InnerPanelManager.instance.InnerPanels[innerPanelId].hintId;
                        break;
                    }
                }

                if (_targetInventory != null)
                    StartCoroutine(ShowHintGlowObject(_targetInventory));

                break;
        }

        /*HintGraphRunner.instance.RequestHint();*/
    }

    public virtual void HintTypeCloseShot()
    {
    }

    public void DisplayHint(Transform hintPosition)
    {
        StartCoroutine(ShowHintGlowObject(hintPosition));
    }

    private IEnumerator ShowHintGlowObject(Transform hintTransform)
    {
        hintGlowGameObject.transform.position = hintTransform.position;
        hintGlowGameObject.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        hintGlowGameObject.SetActive(false);
    }

    public void CompleteHint(HintDetail hint)
    {
        if (hint.isLastProcess)
        {
            hint.isHintCompleted = true;

            foreach (int needToCompleted in hint.needToCompleteHint)
                GetHintDetail(needToCompleted).isHintCompleted = true;
        }

        if (hint.nextHintId != -1)
        {
            selectedHint = hint;
            currentHintId = hint.nextHintId;
        }
        else
            currentHintId = -1;
    }

    public void SetInnerPanelHint(int hintId)
    {
        if (selectedHint != null)
            currentHintId = selectedHint.nextHintId;
    }

    public void CompleteGivenHint(int hintId)
    {
        selectedHint = hints.Find(a => a.hintId == hintId);
        selectedHint.isHintCompleted = true;
        isSelectedHave = true;

        if (selectedHint.isLastProcess)
        {
            foreach (int hintIndex in selectedHint.needToCompleteHint)
            {
                HintDetail hint = GetHintDetail(hintIndex);
                hint.isHintCompleted = true;
            }
        }
    }

    HintDetail FindNextIncompleteHint(int hintId)
    {
        HintDetail hint = currentBgHints.Find(h => h.hintId == hintId);

        if (hint == null)
            return null;

        // if hint not completed → return it
        if (!hint.isHintCompleted)
            return hint;

        // if completed and there is next hint → check next
        if (hint.nextHintId != -1)
            return FindNextIncompleteHint(hint.nextHintId);

        return null;
    }

    public HintDetail GetHintDetail(int hintIndex)
    {
        return hints.Find(h => h.hintId == hintIndex);
    }

    public void ShowHintGlow(HintDetail hint, bool canClose = false)
    {
        selectedHint = hint;

        if (hint.hintType == HintType.InnerPanel)
        {
            for (int i = 0; i < InventoryObjectManager.instance.inventorySlots.Count; i++)
            {
                var slot = InventoryObjectManager.instance.inventorySlots[i];

                if (slot.inventoryItem != null &&
                    hint.inventoryId == slot.inventoryItem.id)
                {
                    _targetInventory = slot.transform;
                    int innerPanelId = slot.inventoryItem.innerPanelId;
                    currentHintId = InnerPanelManager.instance.InnerPanels[innerPanelId].hintId;
                    break;
                }
            }

            DisplayHint(_targetInventory.transform);
        }
        else
        {
            DisplayHint(hint.hintShowButton.transform);
        }

        if (canClose)
        {
            if (EscapeRoomManager.instance.isCloseShotOpened)
            {
                EscapeRoomManager.instance.CloseShotNoAnimation();
            }
            else if (InnerPanelManager.instance.isInnerPanelOpened)
            {
                InnerPanelManager.instance.CloseInnerPanel(false);
            }
        }
    }

    private void HideHint()
    {
        hintGlowGameObject.SetActive(false);
        selectedHint = null;
        isSelectedHave = false;
        currentHintId = -1;
    }

    [ContextMenu("Clear HInt")]
    public void ClearHint()
    {
        print("sadadsf");
        selectedHint = null;
        isSelectedHave = false;
        currentHintId = -1;
    }
}

[System.Serializable]
public class HintDetail
{
    public string name;
    public int hintId;
    public HintType hintType;
    public Button hintShowButton;
    public int inventoryId;
    public int innerPanelId;
    public bool isHintViewed = false;
    public bool isHintCompleted = false;
    public bool isLastProcess = false;
    public int[] needToCompleteHint;
    public int nextHintId;
}

public enum HintType
{
    Bg,
    CloseShot,
    InnerPanel,
    Inventory,
    Custome
}