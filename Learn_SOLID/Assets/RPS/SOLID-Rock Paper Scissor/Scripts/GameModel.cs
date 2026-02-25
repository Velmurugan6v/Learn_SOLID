using UnityEngine;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class GameModel
{
    public int PlayerScore;
    public int ComputerScore;
    public int RemainingRounds;


    public event Action OnScoreChanged;
    public event Action<string> OnRoundResult;
    public event Action OnGameFinished;

    public GameModel(int totalRounds)
    {
        RemainingRounds = totalRounds;
    }

    public Choice GetComputerChoice()
    {
        return (Choice)Random.Range(0, 3);
    }

    public void ResolveRound(Choice playerChoice, Choice computerChoice)
    {
        if (playerChoice == computerChoice)
        {
            //Result Tie
            OnRoundResult?.Invoke("Tie");
        }
        else if (IsPlayerWin(playerChoice, computerChoice))
        {
            PlayerScore++;
            RemainingRounds--;
            OnRoundResult?.Invoke("Player win");
        }
        else
        {
            ComputerScore++;
            RemainingRounds--;
            OnRoundResult?.Invoke("Computer won");
        }

        OnScoreChanged?.Invoke();

        if (RemainingRounds <= 0)
            OnGameFinished?.Invoke();
    }

    private bool IsPlayerWin(Choice playerChoiceType, Choice computerChoiceType)
    {
        return (playerChoiceType == Choice.Rock &&
                computerChoiceType == Choice.Scissor) ||
               (playerChoiceType == Choice.Paper &&
                computerChoiceType == Choice.Rock) ||
               (playerChoiceType == Choice.Scissor &&
                computerChoiceType == Choice.Paper);
    }
}