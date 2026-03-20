using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public AnimationClip puzzleFadeAnimationClip;
    public PuzzleManager puzzleManager;
    public GameObject puzzleAnimation;

    public void BG1_SolvingCompleted()
    {
        AnimationManager.instance.SetCanPlayAnimation(3, 0, true);
        HintManager.instance.CompleteGivenHint(18);
        InnerPanelManager.instance.OpenInnerPanel(1);
        Invoke(nameof(ChangeSolvingHint), 0.5f);
    }

    private void ChangeSolvingHint()
    {
        HintManager.instance.selectedHint = HintManager.instance.GetHintDetail(16);
        HintManager.instance.currentHintId = 17;
    }

    public void BG2_AfterGoldStickUsed()
    {
        AnimationManager.instance.SetCanPlayAnimation(10, 0, true);
        EscapeRoomManager.instance.OpenCloseShot(10);
    }

    public void BG2_PuzzleCompleted()
    {
        AnimationManager.instance.SetCanPlayAnimation(10, 1, true);
        HintManager.instance.CompleteGivenHint(51);
        EscapeRoomManager.instance.OpenCloseShot(10);
        Invoke(nameof(ChangePuzzleHint), 0.5f);
    }

    private void ChangePuzzleHint()
    {
        HintManager.instance.selectedHint = HintManager.instance.GetHintDetail(50);
        HintManager.instance.currentHintId = 51;
    }

    public void BG2_PuzzleOpen()
    {
        StartCoroutine(PuzzleFade());
    }

    public void BG2_PuzzleClosed()
    {
        StartCoroutine(PuzzlCloseFade());
    }

    private IEnumerator PuzzleFade()
    {
        puzzleAnimation.SetActive(true);
        puzzleManager.PuzzleOpenFade();
        yield return new WaitForSeconds(puzzleFadeAnimationClip.length / 2);
        puzzleAnimation.SetActive(false);
    }

    private IEnumerator PuzzlCloseFade()
    {
        puzzleAnimation.SetActive(true);
        yield return new WaitForSeconds(puzzleFadeAnimationClip.length / 2);
        puzzleManager.ClosePuzzle();
        yield return new WaitForSeconds(puzzleFadeAnimationClip.length / 2);
        puzzleAnimation.SetActive(false);
    }

    //Hint Clear 
    public void MakeHintNull()
    {
        Invoke(nameof(DelayHintClear), 0.5f);
    }

    private void DelayHintClear()
    {
        HintManager.instance.ClearHint();
    }
}