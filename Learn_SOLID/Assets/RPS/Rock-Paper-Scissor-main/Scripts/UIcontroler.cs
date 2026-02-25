using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIcontroler : MonoBehaviour
{
    public GameObject player_selected_icon;
    public GameObject cpu_selected_icon;
    public Sprite[] player_selection_s, cpu_selection_s;
    public Image player_seletion_img;
    public Image cpu_seletion_img;
    public int player_point, cpu_point;
    public int remaining_round;
    public Text player_point_text, cpu_point_text;
    public Text remaining_round_text;
    public GameObject point_resolut;
    public Text point_resolut_text;
    public bool player_won, math_die;
    public GameObject player_finelResoult, cpu_finelResoult, math_die_resoult;
    public GameObject[] all_icon;
    public Animator anim;

    public GameObject tie_panel;

    public AudioSource sound_for_sfx;
    public AudioClip click_sfx;

    public GameObject exlore_fx;

    // Start is called before the first frame update
    void Start()
    {
        HidePlayerAndCPUChoseIcon();
        point_resolut.SetActive(false);
        remaining_round_text.text = FindObjectOfType<GameController>().chance.ToString();
        tie_panel.SetActive(false);
    }

    public void HidePlayerAndCPUChoseIcon()
    {
        player_selected_icon.SetActive(false);
        cpu_selected_icon.SetActive(false);
    }

    //Player Selection Function
    public void PlayerGivenNumberConvertToIcon(int spriteNumber)
    {
        ShowPlayerSelectIcon();
        player_seletion_img.sprite = player_selection_s[spriteNumber];
    }

    
    public void ShowPlayerSelectIcon()
    {
        player_selected_icon.SetActive(true);
    }

    //CPU Selection Fuction
    public void CPUGivenNumberConvertToIcon(int spriteNumber)
    {
        ShowCPUSelectIcon();
        cpu_seletion_img.sprite = cpu_selection_s[spriteNumber];
    }

    public void ShowCPUSelectIcon()
    {
        cpu_selected_icon.SetActive(true);
    }

    //Add Points 
    public void AddPoints(int player, int cpu)
    {
        player_point += player;
        player_point_text.text = player_point.ToString();
        cpu_point += cpu;
        cpu_point_text.text = cpu_point.ToString();
    }

    public void ReduceRoundCount(int value)
    {
        remaining_round -= value;
        remaining_round_text.text = remaining_round.ToString();
    }


    public IEnumerator PointResoult(string whoGetPoint)
    {
        point_resolut_text.text = whoGetPoint;
        yield return new WaitForSeconds(1);
        point_resolut.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        HidePlayerAndCPUChoseIcon();
        point_resolut.SetActive(false);
        EnableOfAllIcon();
    }

    public void EnableOfAllIcon()
    {
        foreach (GameObject icon in all_icon)
        {
            icon.SetActive(true);
        }
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Animation
    public IEnumerator PlayAnimation(string animName)
    {
        yield return new WaitForSeconds(1f);

        if (animName == "PS")
        {
            anim.Play("PS");
        }
        else if (animName == "PR")
        {
            anim.Play("PR");
        }
        else if (animName == "RS")
        {
            anim.Play("RS");
        }
        else if (animName == "RP")
        {
            anim.Play("RP");
        }
        else if (animName == "SR")
        {
            anim.Play("SR");
        }
        else if (animName == "SP")
        {
            anim.Play("SP");
        }
        else if (animName == "Tie")
        {
            anim.Play("Tie");
        }
    }

    public void PlayVFX()
    {
        if (exlore_fx == null) return;
        Instantiate(exlore_fx);
    }

    public IEnumerator TiePanel()
    {
        yield return new WaitForSeconds(2f);
        tie_panel.SetActive(true);
        yield return new WaitForSeconds(2f);
        tie_panel.SetActive(false);
    }

    public void PlaySfx()
    {
        sound_for_sfx.clip = click_sfx;
        sound_for_sfx.Play();
    }
}