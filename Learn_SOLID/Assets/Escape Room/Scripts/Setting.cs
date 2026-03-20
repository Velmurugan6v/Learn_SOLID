using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public Image[] allCloseShotButtons;
    public Color visibleColor, hideColor;
    public GameObject settingPanel;
    public Toggle toggle;

    public void OpenSetting()
    {
        settingPanel.SetActive(true);
    }
    
    public void CloseSetting()
    {
        settingPanel.SetActive(false);
    }

    public void ChangeColor()
    {
        if (toggle.isOn)
            ShowAllCloseShotButtons();
        else
            HideAllCloseShotButtons();
    }

    public void ShowAllCloseShotButtons()
    {
        foreach (Image closeShotButton in allCloseShotButtons)
            closeShotButton.color = visibleColor;
    }

    public void HideAllCloseShotButtons()
    {
        foreach (Image closeShotButton in allCloseShotButtons)
            closeShotButton.color = hideColor;
    }
}