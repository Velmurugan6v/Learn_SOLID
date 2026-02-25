using System;
using System.Collections;
using UnityEngine;

public class GameContoller : MonoBehaviour
{
    public int totalRounds = 5;

    public GameView view;

    public GameModel _model;


    private void Start()
    {
        _model = new GameModel(totalRounds);

        _model.OnScoreChanged += UpdateUI;
        _model.OnRoundResult += view.ShowRoundResult;
        _model.OnGameFinished += HandleGameFinished;
    }

    private void OnDestroy()
    {
        _model.OnScoreChanged -= UpdateUI;
        _model.OnRoundResult -= view.ShowRoundResult;
        _model.OnGameFinished -= HandleGameFinished;
    }

    private void UpdateUI()
    {
        view.UpdateScore(_model.PlayerScore, _model.ComputerScore);
        view.UpdateRounds(_model.RemainingRounds);
    }

    private void HandleGameFinished()
    {
        view.ShowFinalResult(_model.PlayerScore, _model.ComputerScore);
    }

    public void SelectRock()
    {
        print("Select Rock");
        StartCoroutine(RoundSequence(Choice.Rock));
    }

    public void SelectPaper()
    {
        print("Select Paper");
        StartCoroutine(RoundSequence(Choice.Paper));
    }

    public void SelectScissors()
    {
        print("Select Scissors");
        StartCoroutine(RoundSequence(Choice.Scissor));
    }

    private IEnumerator RoundSequence(Choice playerChoice)
    {
        view.SetUIButtonsShow(0f, 0.5f);
        Choice computerChoice = _model.GetComputerChoice();
        view.SetChoseIconVisible(true);
        view.UpdatePlayerAndComputerChoseIcon(playerChoice, computerChoice);
        
        yield return new WaitForSeconds(2f);
        view.SetChoseIconVisible(false);
        view.BattleAnimation(playerChoice, computerChoice);

        yield return new WaitForSeconds(2f);
        view.SetRoundResult(true);
        
        yield return new WaitForSeconds(0.5f);
        _model.ResolveRound(playerChoice, computerChoice);
        view.SetRoundResult(false);
        view.SetUIButtonsShow(1f, 0.7f);
    }   
}