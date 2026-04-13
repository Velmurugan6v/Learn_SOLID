using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public RectTransform gameBoard;
    public PlayerController playerPrefab;

    public Image player1DiceImage;
    public Image player2DiceImage;
    public Sprite[] diceSprites;

    public PlayerController player1;
    public PlayerController player2;

    public Image playerOneDiceImage;
    public Image playerTwoDiceImage;

    public Button player1DiceButton;
    public Button player2DiceButton;

    public Text player1ScoreText;
    public Text player2ScoreText;

    public int player1Score;
    public int player2Score;

    //public static Action<int> OnPlayerOneScoreChange;
    //public static Action<int> OnPlayerTwoScoreChange;


    private void Start()
    {
        SetUpPlayers();

        player1DiceButton.onClick.AddListener(() => player1.OnDiceRoll?.Invoke());
        player2DiceButton.onClick.AddListener(() => player2.OnDiceRoll?.Invoke());

        //OnPlayerTwoScoreChange += PlayerOneScoreUpdate;
        //OnPlayerTwoScoreChange += PlayerTwoScoreUpdate;

        if (player1 != null)
            player1.SetUpPlayer(player1DiceImage, diceSprites, player1ScoreText, player2);

        if (player2 != null)
            player2.SetUpPlayer(player2DiceImage, diceSprites, player2ScoreText, player1);
    }

    private void SetUpPlayers()
    {
        player1 = Instantiate(playerPrefab, gameBoard, false) as PlayerController;
        player2 = Instantiate(playerPrefab, gameBoard, false) as PlayerController;

        player1.transform.SetParent(gameBoard);
        player2.transform.SetParent(gameBoard);
    }

    private void OnDestroy()
    {
        player1DiceButton.onClick.RemoveAllListeners();

        //OnPlayerOneScoreChange -= PlayerOneScoreUpdate;
        //OnPlayerTwoScoreChange -= PlayerTwoScoreUpdate;
    }

    public void PlaceDice(int columnIndex, int value)
    {
    }

    private void PlayerOneScoreUpdate(int score)
    {
        player1Score += score;
        player1ScoreText.text = player1Score + "";
    }

    private void PlayerTwoScoreUpdate(int score)
    {
        player2Score += score;
        player2ScoreText.text = player2Score + "";
    }

    public void ResetGame()
    {
        player1.ResetPlayerData();
        player2.ResetPlayerData();
    }
}