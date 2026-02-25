using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    //Variables

    [Header("UI")] public Text playerScoreText;
    public Text computerScoreText;
    public Text remainingRoundText;
    public Text resultText;

    public Image playerChoseImage;
    public Image computerChoseImage;

    public Sprite[] playerSprites;
    public Sprite[] computerSprites;
    
    public GameObject roundResultGameObject;

    public CanvasGroup uiClickButtonsCanvasGroup;

    [Header("Animator")] public Animator animator;
    public Animator resultAnimation;


    //Methods
    public void UpdateScore(int playerScore, int computerScore)
    {
        playerScoreText.text = playerScore.ToString();
        computerScoreText.text = computerScore.ToString();
    }

    public void UpdateRounds(int remainingRounds)
    {
        remainingRoundText.text = remainingRounds.ToString();
    }

    public void UpdatePlayerAndComputerChoseIcon(Choice playerChoice, Choice computerChoice)
    {
        int playerChoiceInt = (int)playerChoice;
        int computerChoiceInt = (int)computerChoice;

        playerChoseImage.sprite = playerSprites[playerChoiceInt];
        computerChoseImage.sprite = computerSprites[computerChoiceInt];
    }

    public void ShowRoundResult(string result)
    {
        resultText.text = result;
        animator.Play(result);
    }

    public void SetRoundResult(bool value)
    {
        roundResultGameObject.gameObject.SetActive(value);
    }

    public void BattleAnimation(Choice playerChoice, Choice computerChoice)
    {
        if (playerChoice == computerChoice)
        {
            animator.Play("Tie");
            return;
        }

        string animName = $"{playerChoice}_{computerChoice}";
        animator.Play(animName);
    }

    public void ShowFinalResult(int playerScore, int computerScore)
    {
        if (playerScore > computerScore)
            resultAnimation.SetTrigger("PlayerWon");
        else
            resultAnimation.SetTrigger("ComputerWon");
    }

    public void SetChoseIconVisible(bool value)
    {
        playerChoseImage.gameObject.SetActive(value);
        computerChoseImage.gameObject.SetActive(value);
    }

    public void SetUIButtonsShow(float value, float duration)
    {
        uiClickButtonsCanvasGroup.DOFade(value, duration);
    }
}