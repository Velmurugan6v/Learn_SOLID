using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //mach var
    [Header("Mech Var")]
    public string player_input;
    public string cpu_input;
    private string[] selection = { "Rock", "Paper", "Scissor" };

    //Points
    [Header("Points")]
    public int player_point;
    public int cpu_point;

    //Change
    public int no_of_chance;
    public int chance;

    public UIcontroler ui_controler;

    public GameObject start_panel;

    public Animator myAnim;

    public GameObject startPanel;

    public void GameFunction()
    {
        //CPU Input 
        cpu_input = selection[Random.Range(0, selection.Length)];

        if (cpu_input=="Rock")
        {
            ui_controler.CPUGivenNumberConvertToIcon(0);
        }
        else if (cpu_input == "Paper")
        {
            ui_controler.CPUGivenNumberConvertToIcon(1);
        }
        else if (cpu_input=="Scissor")
        {
            ui_controler.CPUGivenNumberConvertToIcon(2);
        }

        //Player and Computer Select Same
        if (player_input == cpu_input)
        {
            //print("Match die");
            ui_controler.AddPoints(0, 0);
            ui_controler.ReduceRoundCount(0);
            StartCoroutine(ui_controler.PointResoult("Round"));
            StartCoroutine(ui_controler.PlayAnimation("Tie"));
            StartCoroutine(ui_controler.TiePanel());
        }
        //player Input Rock
        else if (player_input == "Rock" && cpu_input == "Paper")
        {
            //print("Computer Get Point");
            ui_controler.AddPoints(0, 1);
            ui_controler.ReduceRoundCount(1);
            StartCoroutine(ui_controler.PointResoult("Computer Get 1 Point"));
            cpu_point++;
            no_of_chance++;
            StartCoroutine(ui_controler.PlayAnimation("RP"));
        }
        else if (player_input == "Rock" && cpu_input == "Scissor")
        {
            //print("Player Get Point");
            ui_controler.AddPoints(1,0);
            ui_controler.ReduceRoundCount(1);
            StartCoroutine(ui_controler.PointResoult("Player Get 1 Point"));
            player_point++;
            no_of_chance++;
            StartCoroutine(ui_controler.PlayAnimation("RS"));
        }
        //Player Input Paper
        else if (player_input == "Paper" && cpu_input == "Rock")
        {
            //print("Player Get Point");
            ui_controler.AddPoints(1,0);
            ui_controler.ReduceRoundCount(1);
            StartCoroutine(ui_controler.PointResoult("Player Get 1 Point"));
            player_point++;
            no_of_chance++;
            StartCoroutine(ui_controler.PlayAnimation("PR"));
        }
        else if (player_input == "Paper" && cpu_input == "Scissor")
        {
            //print("Computer Get Point");
            ui_controler.AddPoints(0,1);
            ui_controler.ReduceRoundCount(1);
            StartCoroutine(ui_controler.PointResoult("Computer Get 1 Point"));
            cpu_point++;
            no_of_chance++;
            StartCoroutine(ui_controler.PlayAnimation("PS"));
        }
        //Player Input Scissor
        else if (player_input == "Scissor" && cpu_input == "Paper")
        {
            //print("Player Get Point");
            ui_controler.AddPoints(1,0);
            ui_controler.ReduceRoundCount(1);
            StartCoroutine(ui_controler.PointResoult("Player Get 1 Point"));
            player_point++;
            no_of_chance++;
            StartCoroutine(ui_controler.PlayAnimation("SP"));
        }
        else if (player_input == "Scissor" && cpu_input == "Rock")
        {
            //print("Computer Get Point");
            ui_controler.AddPoints(0,1);
            ui_controler.ReduceRoundCount(1);
            StartCoroutine(ui_controler.PointResoult("Computer Get 1 Point"));
            cpu_point++;
            no_of_chance++;
            StartCoroutine(ui_controler.PlayAnimation("SR"));
        }


        //Match Resoult
        if (no_of_chance == chance)
        {
            //print("Game Over");
            //Resoult check
            Invoke("ResoultOfGame", 3.5f);
        }
    }

    public void SelecdRock()
    {
        player_input = "Rock";
        GameFunction();
        ui_controler.PlayerGivenNumberConvertToIcon(0);
        Invoke("DisableOfAllIcon",0.2f);
        //Clck sound Play
        ui_controler.PlaySfx();
        
    }

    public void SelectPaper()
    {
        player_input = "Paper";
        GameFunction();
        ui_controler.PlayerGivenNumberConvertToIcon(1);
        Invoke("DisableOfAllIcon", 0.2f);
        //Clck sound Play
        ui_controler.PlaySfx();
    }

    public void SelectScissor()
    {
        player_input = "Scissor";
        GameFunction();
        ui_controler.PlayerGivenNumberConvertToIcon(2);
        Invoke("DisableOfAllIcon", 0.2f);
        //Clck sound Play
        ui_controler.PlaySfx();
    }

    public void DisableOfAllIcon()
    {
        foreach (GameObject icon in ui_controler.all_icon)
        {
            icon.SetActive(false);
        }
    }

    public void ResoultOfGame()
    {
        if (player_point < cpu_point)
        {
            myAnim.SetTrigger("CpuWon");
        }
        else if (player_point > cpu_point)
        {
            myAnim.SetTrigger("PlayerWon");
        }
        else
        {
            print("Match Die");
        }
    }

    public void StartGame()
    {
        ui_controler.PlaySfx();
        myAnim.Play("Start");
        Invoke("StartPanelDisable",3f);
    }

    public void StartPanelDisable()
    {
        startPanel.SetActive(false);
    }

    public void QuitGame()
    {
        ui_controler.PlaySfx();
        print("Game Quit");
        Application.Quit();
    }
}
