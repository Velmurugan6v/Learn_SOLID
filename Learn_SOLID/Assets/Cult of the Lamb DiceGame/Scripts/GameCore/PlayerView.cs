using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public Text[] playerScoreText;

    //Player Infor UI
    public Text playerTotalValueText;

    public Button[] columnClickButtons;

    public PlayerController playerController;


    public void OnEnable()
    {
        SetupColumnClickButtons();
    }

    private void OnDisable()
    {
        foreach (var clickButton in columnClickButtons)
        {
            clickButton.onClick.RemoveAllListeners();
        }
    }

    public void SetUpPlayerScoreText(Text playerTotalScoreText)
    {
        this.playerTotalValueText = playerTotalScoreText;
    }

    private void SetupColumnClickButtons()
    {
        for (int i = 0; i < columnClickButtons.Length; i++)
        {
            int index = i;
            columnClickButtons[i].onClick.AddListener(() =>
                {
                    Debug.Log("Button clicked: " + index);
                    ColumnSelected(index);
                }
            );
        }
    }


    private void ColumnSelected(int column)
    {
        if (playerController.OnColumnSelected != null)
            print("have sub");
        else
            print("not have sub");

        playerController.OnColumnSelected?.Invoke(column);
    }

    public void ShowColumnTotal(int columnTextCount, int totalValue)
    {
        playerScoreText[columnTextCount].text = totalValue + "";
    }

    public void ShowTotalColumnTotal(int columnTextCount)
    {
        playerTotalValueText.text = columnTextCount + "";
        //GameManager.OnPlayerOneScoreChange?.Invoke(columnTextCount);
    }

    public void ResetPlayerScoreText()
    {
        playerTotalValueText.text = "0";

        foreach (var text in playerScoreText)
        {
            text.text = "0";
        }
    }
}