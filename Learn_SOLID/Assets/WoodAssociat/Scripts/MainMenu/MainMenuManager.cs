using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WoodAssociat
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject settingPanel;
        [SerializeField] private GameObject shopPanel;

        [SerializeField] Button shopButton;
        [SerializeField] Button settingsButton;

        [SerializeField] Button shopCloseButton;
        [SerializeField] Button settingsCloseButton;

        [SerializeField] Button playButton;


        //Unity function
        private void OnEnable()
        {
            shopButton.onClick.AddListener(ShowShopPanel);
            shopCloseButton.onClick.AddListener(HideShopPanel);

            settingsButton.onClick.AddListener(ShowSettingsPanel);
            settingsCloseButton.onClick.AddListener(HideSettingsPanel);

            playButton.onClick.AddListener(PlayGame);
        }

        private void OnDisable()
        {
            shopButton.onClick.RemoveListener(ShowShopPanel);
            shopCloseButton.onClick.RemoveListener(HideShopPanel);

            settingsButton.onClick.RemoveListener(ShowSettingsPanel);
            settingsCloseButton.onClick.RemoveListener(HideSettingsPanel);

            playButton.onClick.RemoveListener(PlayGame);
        }


        //Own function
        private void ShowSettingsPanel()
        {
            settingPanel?.SetActive(true);
        }

        private void HideSettingsPanel()
        {
            settingPanel?.SetActive(false);
        }

        private void ShowShopPanel()
        {
            shopPanel?.SetActive(true);
        }

        private void HideShopPanel()
        {
            shopPanel?.SetActive(false);
        }

        private void PlayGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}