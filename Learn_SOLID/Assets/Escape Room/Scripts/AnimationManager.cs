using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;
    public AnimationParentInfo[] animationInfor;

    private void Start()
    {
        instance = this;
        SetUpAnimation();
    }

    public void SetUpAnimation()
    {
        for (int i = 0; i < animationInfor.Length; i++)
        {
            animationInfor[i].animationController.animationParentInfo = animationInfor[i];
        }

        for (int i = 0; i < animationInfor.Length; i++)
        {
            int tempI = 0;
            tempI = i;
            for (int j = 0; j < animationInfor[i].animationChildInfos.Length; j++)
            {
                int tempJ = 0;
                tempJ = j;

                animationInfor[tempI].animationChildInfos[tempJ].clickButton?.onClick
                    .AddListener(() => PlayAnimation(animationInfor[tempI]));
            }
        }
    }

    public void PlayAnimation(AnimationParentInfo animationParentInfor)
    {
        AnimationChildInfo newAnimationChildInfo =
            animationParentInfor.animationChildInfos[animationParentInfor.currentAnimationIndex];

        if (newAnimationChildInfo.isAnimationDone) return;

        if (newAnimationChildInfo.canClick)
        {
            //PlayAnimation 
            StartCoroutine(PlayDraAndDropAnimation(animationParentInfor));
        }
    }

    public void PlayAnimation(AnimationParentInfo animationParentInfor, InventoryItem dropItem)
    {
        AnimationChildInfo newAnimationChildInfo =
            animationParentInfor.animationChildInfos[animationParentInfor.currentAnimationIndex];

        if (newAnimationChildInfo.canDragAndDrop &&
            newAnimationChildInfo.inventroyId == dropItem.inventorySlot.inventoryItem.id)
        {
            //PlayAnimation 
            StartCoroutine(PlayDraAndDropAnimation(animationParentInfor));
        }
    }

    private IEnumerator PlayDraAndDropAnimation(AnimationParentInfo animationParentInfor)
    {
        var newAnimationChildInfo =
            animationParentInfor.animationChildInfos[animationParentInfor.currentAnimationIndex];

        animationParentInfor.currentAnimationIndex++;
        //Fina animation Length
        var animationDuration = newAnimationChildInfo.animationClip.length;

        EscapeRoomManager.instance.blockPanel.SetActive(true);

        if (newAnimationChildInfo.hintId != -1)
        {
            HintManager.instance.CompleteGivenHint(newAnimationChildInfo.hintId);
            HintDetail currentHintDetai = HintManager.instance.GetHintDetail(newAnimationChildInfo.hintId);
            HintManager.instance.selectedHint = currentHintDetai;
            HintManager.instance.currentHintId = HintManager.instance.selectedHint.nextHintId;
        }

        //Play Animation 
        animationParentInfor.animator.Play(newAnimationChildInfo.animationClip.name);

        yield return new WaitForSeconds(animationDuration);

        //After Complete the animation need to do
        newAnimationChildInfo.interactionState.SetInteractionState();
        newAnimationChildInfo.onAnimationEnd?.Invoke();

        if (animationParentInfor.currentAnimationIndex == animationParentInfor.animationChildInfos.Length)
            animationParentInfor.isDone = true;

        EscapeRoomManager.instance.blockPanel.SetActive(false);

        if (animationParentInfor.currentAnimationIndex == animationParentInfor.animationChildInfos.Length)
            animationParentInfor.onAllAnimationEnd?.Invoke();

        //Check Is There Any Other Work have to Do!!
        if (newAnimationChildInfo.haveToDoNext == HaveToDoNext.Nothing)
            yield break;

        StartCoroutine(PlayDraAndDropAnimation(animationParentInfor));
    }

    public void PlayAutoAnimation(AnimationParentInfo animationParentInfor)
    {
        if (animationParentInfor.currentAnimationIndex >= animationParentInfor.animationChildInfos.Length ||
            !animationParentInfor.animationChildInfos[animationParentInfor.currentAnimationIndex].canPlay)
            return;

        //PlayAnimation 
        StartCoroutine(PlayDraAndDropAnimation(animationParentInfor));
    }


    //---Animamation Function

    public void SetCanPlayAnimation(int parentAniationId, int childAniationId, bool value)
    {
        animationInfor[parentAniationId].animationChildInfos[childAniationId].canPlay = value;
    }
}


[System.Serializable]
public class AnimationParentInfo
{
    public string name;
    public Animator animator;
    public bool isDone = false;
    public int currentAnimationIndex;
    public AnimationChildInfo[] animationChildInfos;
    public UnityEvent onAllAnimationEnd;
    public AnimationController animationController;
}


[System.Serializable]
public class AnimationChildInfo
{
    public string name;
    public AnimationType animationType;
    public AnimationClip animationClip;
    public bool isAnimationDone = false;
    public bool canPlay = false;
    public bool canDragAndDrop;
    public int inventroyId;
    public bool canShowText;
    public string interactionText;
    public bool canClick;
    public Button clickButton;
    public InteractionState interactionState;
    public UnityEvent onAnimationEnd;
    public HaveToDoNext haveToDoNext;
    public int hintId = -1;
}

public enum AnimationType
{
    Automatic,
    DragAndDrop,
    Click
}


public enum HaveToDoNext
{
    Nothing,
    CanDoNextAnimation
}