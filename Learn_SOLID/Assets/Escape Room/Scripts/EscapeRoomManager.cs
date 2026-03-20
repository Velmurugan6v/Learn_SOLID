using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscapeRoomManager : MonoBehaviour
{
    public static EscapeRoomManager instance;
    [Header("CloseShot")] public CloseShotInformation[] closeShotGameObject;
    public bool isCloseShotOpened = false;
    public CloseShotInformation currentOpenedCloseShot;
    public Button closeShotCloseButton;
    public GameObject closeShotPanel;
    public Ease closeShotEase;
    public float animationDuration;
    public InnerPanelManager innerPanelInformation;
    public GameObject blockPanel;
    public GameObject backGroundBlockPanel;

    [Header("Alpha Text")] public GameObject alphaTextGameObject;
    public Text alphaText;

    [Header("BG")] public bool isCustomeBg = false;
    public GameObject[] bGs;
    public GameObject[] closeShots;
    public int currentBgIndex = 0;
    public GameObject bgChangeAnimationPanel;
    public AnimationClip bgChangeAnimationClip;

    [Header("Audio")] public AudioManager audioManager;


    private void Start()
    {
        instance = this;
        Application.targetFrameRate = 60;
        SetUpCloseShot();
        BgSetUp();
        SetBgHint();
    }

    private void BgSetUp()
    {
        if (!isCustomeBg)
        {
            currentBgIndex = 0;
        }

        foreach (var bG in bGs)
            bG.SetActive(false);

        foreach (var closeShot in closeShots)
            closeShot.SetActive(false);

        bGs[currentBgIndex].SetActive(true);
        closeShots[currentBgIndex].SetActive(true);
    }

    private void SetUpCloseShot()
    {
        for (int i = 0; i < closeShotGameObject.Length; i++)
        {
            int tempIndex = 0;
            tempIndex = i;
            closeShotGameObject[tempIndex].closeShotTriggerButton.onClick.AddListener(() => OpenCloseShot(tempIndex));
        }
    }

    //Closet
    public void OpenCloseShot(int closeShotIndex)
    {
        if (innerPanelInformation.isInnerPanelOpened)
        {
            innerPanelInformation.CloseAllInnerPanels();
        }
        else if (isCloseShotOpened)
        {
            CloseShotNoAnimation();
        }

        audioManager.PlaySound(0);

        CloseShotInformation newCloseShotInformation = closeShotGameObject[closeShotIndex];

        isCloseShotOpened = true;

        closeShotPanel.SetActive(true);

        SetFullScreenBlockCanvas(true);
        SetBackGroundBlockCanvas(true);

        currentOpenedCloseShot = newCloseShotInformation;

        newCloseShotInformation.onCloseShotOpened?.Invoke();

        //CloseShot
        newCloseShotInformation.closeShotObject.transform.localScale = Vector3.one * 0.15f;
        newCloseShotInformation.closeShotObject.transform.position =
            newCloseShotInformation.closeShotOriginPosition.transform.position;

        newCloseShotInformation.closeShotObject.SetActive(true);

        newCloseShotInformation.closeShotObject.transform.DOScale(Vector3.one, animationDuration)
            .SetEase(closeShotEase);
        newCloseShotInformation.closeShotObject.transform.DOLocalMove(new Vector3(0f, 0f, 0f), animationDuration)
            .SetEase(closeShotEase).OnComplete(() => SetFullScreenBlockCanvas(false));

        //panel
        closeShotPanel.transform.localScale = Vector3.one * 0.15f;
        closeShotPanel.transform.position =
            newCloseShotInformation.closeShotOriginPosition.transform.position;
        closeShotPanel.SetActive(true);
        closeShotPanel.transform.DOScale(Vector3.one, animationDuration).SetEase(closeShotEase);
        closeShotPanel.transform.DOLocalMove(new Vector3(0f, 0f, 0f), animationDuration).SetEase(closeShotEase);


        //Have to from orgin to center
    }

    public void CloseShotNoAnimation()
    {
        if (!isCloseShotOpened)
            return;

        currentOpenedCloseShot.closeShotObject.transform.localScale = Vector3.one * 0.15f;
        currentOpenedCloseShot.closeShotObject.transform.position =
            currentOpenedCloseShot.closeShotOriginPosition.position;

        currentOpenedCloseShot.closeShotObject.SetActive(false);
        currentOpenedCloseShot = null;
        isCloseShotOpened = false;
        SetFullScreenBlockCanvas(false);
        SetBackGroundBlockCanvas(false);
        closeShotPanel.SetActive(false);
    }

    public void CloseShot()
    {
        SetFullScreenBlockCanvas(true);

        audioManager.PlaySound(0);

        HintManager.instance.OnHintClose?.Invoke();

        currentOpenedCloseShot.closeShotObject.transform.DOScale(Vector3.one * 0.15f, animationDuration);
        currentOpenedCloseShot.closeShotObject.transform
            .DOMove(currentOpenedCloseShot.closeShotOriginPosition.position, animationDuration)
            .OnComplete(() =>
            {
                currentOpenedCloseShot.closeShotObject.SetActive(false);
                currentOpenedCloseShot = null;
                isCloseShotOpened = false;
                SetFullScreenBlockCanvas(false);
                SetBackGroundBlockCanvas(false);
            });
        closeShotPanel.SetActive(false);
    }


    //----Alpha Text---
    public void ShowAlphaText(string alphaTextValue)
    {
        StartCoroutine(AlphatTextCoroutine(alphaTextValue));
    }

    private IEnumerator AlphatTextCoroutine(string alphaTextValue)
    {
        alphaText.text = alphaTextValue;
        alphaTextGameObject.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        alphaTextGameObject.SetActive(false);
    }

    //--Block Panel
    public void SetFullScreenBlockCanvas(bool value)
    {
        blockPanel.SetActive(value);
    }

    public void SetBackGroundBlockCanvas(bool value)
    {
        backGroundBlockPanel.SetActive(value);
    }

    //---BG---
    [ContextMenu("NextBG")]
    public void NextBg()
    {
        bGs[currentBgIndex].SetActive(false);
        closeShots[currentBgIndex].SetActive(false);

        currentBgIndex++;

        if (currentBgIndex >= bGs.Length)
            print("There is no bg here");

        bGs[currentBgIndex].SetActive(true);
        closeShots[currentBgIndex].SetActive(true);
    }

    public void PreviousBg()
    {
        bGs[currentBgIndex].SetActive(false);
        closeShots[currentBgIndex].SetActive(false);

        currentBgIndex--;

        if (currentBgIndex <= 0)
            print("There is no bg here");

        bGs[currentBgIndex].SetActive(true);
        closeShots[currentBgIndex].SetActive(true);
    }


    public void SetBgHint() //Very Bad Code
    {
        HintManager.instance.currentBgHints.Clear();

        if (currentBgIndex < 0)
            return;

        print(currentBgIndex);

        for (int i = HintManager.instance.bgHintStar[currentBgIndex];
             i < HintManager.instance.bgHintStar[currentBgIndex + 1];
             i++)
        {
            HintManager.instance.currentBgHints.Add(HintManager.instance.hints[i]);
        }
    }

    public void GoToNextBg()
    {
        StartCoroutine(NextBgChangeAnimation());
    }

    public void GoToPreviousBg()
    {
        StartCoroutine(PreviousBgChangeAnimation());
    }

    public IEnumerator NextBgChangeAnimation()
    {
        bgChangeAnimationPanel.SetActive(true);

        yield return new WaitForSeconds(bgChangeAnimationClip.length / 2f);
        NextBg();
        SetBgHint();

        yield return new WaitForSeconds(bgChangeAnimationClip.length / 2f);
        bgChangeAnimationPanel.SetActive(false);
    }

    public IEnumerator PreviousBgChangeAnimation()
    {
        bgChangeAnimationPanel.SetActive(true);

        yield return new WaitForSeconds(bgChangeAnimationClip.length / 2f);
        PreviousBg();
        SetBgHint();

        yield return new WaitForSeconds(bgChangeAnimationClip.length / 2f);
        bgChangeAnimationPanel.SetActive(false);
    }

    private IEnumerator BgChangeAnimationCoroutine()
    {
        bgChangeAnimationPanel.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        bgChangeAnimationPanel.SetActive(false);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

[Serializable]
public class CloseShotInformation
{
    public string name;
    public GameObject closeShotObject;
    public Transform closeShotOriginPosition;
    public Button closeShotTriggerButton;
    public UnityEvent onCloseShotOpened;
}