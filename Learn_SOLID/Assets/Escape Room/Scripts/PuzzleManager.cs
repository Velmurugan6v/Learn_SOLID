using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public UnityEvent onPuzzleOpen;
    public UnityEvent onPuzzleClosed;
    public UnityEvent onPuzzleComplete;
    public UnityEvent onPuzzleReset;
    public UnityEvent onPuzzleSkip;
    public UnityEvent onPuzzleHowToPlay;
    public UnityEvent onHowToPlayPanelClose;

    public Button puzzleCloseButton;
    public Button skipButton;
    public Button restartButton;
    public Button howToPlayButton;
    public Button howToPlayPanelCloseButton;

    public GameObject mainCamera;
    public GameObject puzzleCamera;
    public GameObject puzzleCanvas;
    public GameObject blockCanvas;
    public GameObject howToPlayPanel;
    public GameObject puzzlePrefab;
    public GameObject fadeAnimationGameObject;
    public AnimationClip fadeAnimationClip;


    private void OnEnable()
    {
        //Buttons
        /*puzzleCloseButton.onClick.AddListener(ClosePuzzle);*/
        puzzleCloseButton.onClick.AddListener(PuzzleClose);
        puzzleCloseButton.onClick.AddListener(ResetPuzzle);
        restartButton.onClick.AddListener(ResetPuzzle);
        skipButton.onClick.AddListener(SkipPuzzle);
        howToPlayButton.onClick.AddListener(ShowHowToPlay);
        howToPlayPanelCloseButton.onClick.AddListener(CloseHowToPlayPanel);

        //Event
    }

    private void OnDisable()
    {
        /*puzzleCloseButton.onClick.RemoveListener(ClosePuzzle);*/
        puzzleCloseButton.onClick.RemoveListener(PuzzleClose);
        puzzleCloseButton.onClick.RemoveListener(ResetPuzzle);
        restartButton.onClick.RemoveListener(ResetPuzzle);
        skipButton.onClick.RemoveListener(SkipPuzzle);
        howToPlayButton.onClick.RemoveListener(ShowHowToPlay);
        howToPlayPanelCloseButton.onClick.RemoveListener(CloseHowToPlayPanel);
    }

    [ContextMenu("Puzzle Open")]
    public void OpenPuzzle()
    {
        mainCamera.SetActive(false);
        puzzleCamera.SetActive(true);
        puzzleCanvas.SetActive(true);
        blockCanvas.SetActive(true);
        puzzlePrefab.SetActive(true);
    }

    [ContextMenu("Puzzle Close")]
    public void ClosePuzzle()
    {
        mainCamera.SetActive(true);
        puzzleCamera.SetActive(false);
        puzzleCanvas.SetActive(false);
        blockCanvas.SetActive(false);
        puzzlePrefab.SetActive(false);
    }

    private void PuzzleClose()
    {
        onPuzzleClosed?.Invoke();
    }

    public void PuzzleCloseFade()
    {
        StartCoroutine(FadeAnimation(ClosePuzzle));
    }

    public void PuzzleOpenFade()
    {
        StartCoroutine(FadeAnimation(OpenPuzzle));
    }

    private IEnumerator FadeAnimation(Action onDoingAnimation = null)
    {
        fadeAnimationGameObject.SetActive(true);
        yield return new WaitForSeconds(fadeAnimationClip.length / 2);
        onDoingAnimation?.Invoke();
        yield return new WaitForSeconds(fadeAnimationClip.length / 2);
        fadeAnimationGameObject.SetActive(false);
    }

    public void ShowHowToPlay()
    {
        howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlayPanel()
    {
        howToPlayPanel.SetActive(false);
    }

    public void SkipPuzzle()
    {
        onPuzzleSkip?.Invoke();
        onPuzzleClosed?.Invoke();
    }

    public void ResetPuzzle()
    {
        onPuzzleReset?.Invoke();
    }
}