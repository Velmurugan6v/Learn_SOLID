using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public Text[] playerScoreText;

    //Player Infor UI
    public Text playerTotalValueText;
    public Text enemyTotalValueText;

    public void ShowColumnTotal(int columnTextCount, int totalValue)
    {
        playerScoreText[columnTextCount].text = totalValue + "";
    }

    public void ShowTotalColumnTotal(int columnTextCount)
    {
        playerTotalValueText.text = columnTextCount + "";
    }
}