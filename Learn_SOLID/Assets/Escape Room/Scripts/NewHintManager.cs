using System;
using UnityEngine;

public class NewHintManager : HintManager
{
    private void Start()
    {
        instance = this;
    }

    public override void ShowHint()
    {
        //Wall DownSide
        if (selectedHint.hintId == 2 && selectedHint != null)
        {
            if (!GetHintDetail(1).isHintCompleted) //Wall Piece
            {
                ShowHintGlow(GetHintDetail(0), true);
            }
            else
            {
                base.ShowHint();
            }
        }
        else if (selectedHint.hintId == 9 && selectedHint != null) //Rope Inner
        {
            if (!GetHintDetail(1).isHintCompleted) //Wall Piece
            {
                ShowHintGlow(GetHintDetail(0), true);
            }
            else if (!GetHintDetail(2).isHintCompleted) //Red Germ, Hook Dc
            {
                ShowHintGlow(GetHintDetail(2), true);
            }
            else
            {
                base.ShowHint();
            }
        }
        else if (selectedHint.hintId == 12 && selectedHint != null) //River
        {
            if (!GetHintDetail(1).isHintCompleted) //Wall Piece
            {
                ShowHintGlow(GetHintDetail(0), true);
            }
            else if (!GetHintDetail(2).isHintCompleted) //Red Germ, Hook Dc
            {
                ShowHintGlow(GetHintDetail(2), true);
            }
            else if (!GetHintDetail(7).isHintCompleted) //Rope DC
            {
                ShowHintGlow(GetHintDetail(6), true);
            }
            else if (!GetHintDetail(11).isHintCompleted) //Rope Inner
            {
                ShowHintGlow(GetHintDetail(9), true);
            }
            else
            {
                base.ShowHint();
            }
        }
        else if (selectedHint.hintId == 15 && selectedHint != null) //Clue in BG
        {
            if (!GetHintDetail(1).isHintCompleted) //Wall Piece
            {
                ShowHintGlow(GetHintDetail(0), true);
            }
            else if (!GetHintDetail(2).isHintCompleted) //Red Germ, Hook Dc
            {
                ShowHintGlow(GetHintDetail(2), true);
            }
            else if (!GetHintDetail(7).isHintCompleted) //Rope DC
            {
                ShowHintGlow(GetHintDetail(6), true);
            }
            else if (!GetHintDetail(11).isHintCompleted) //Rope Inner
            {
                ShowHintGlow(GetHintDetail(9), true);
            }
            else if (!GetHintDetail(14).isHintCompleted) //Small Box
            {
                ShowHintGlow(GetHintDetail(10), true);
            }
            else if (!GetHintDetail(15).isHintCompleted) //Clue in BG
            {
                ShowHintGlow(GetHintDetail(15), true);
            }
            else
            {
                base.ShowHint();
            }
        }
        else if (selectedHint.hintId == 16 && selectedHint != null) //Small Box Inner
        {
            if (!GetHintDetail(15).isHintCompleted) //Clue in BG
            {
                ShowHintGlow(GetHintDetail(15), true);
            }
            else
            {
                base.ShowHint();
            }
        }
        else if (selectedHint.hintId == 23 && selectedHint != null)
        {
            if (!GetHintDetail(1).isHintCompleted) //Wall Piece
            {
                ShowHintGlow(GetHintDetail(0), true);
            }
            else if (!GetHintDetail(2).isHintCompleted) //Red Germ, Hook Dc
            {
                ShowHintGlow(GetHintDetail(2), true);
            }
            else if (!GetHintDetail(7).isHintCompleted) //Rope DC
            {
                ShowHintGlow(GetHintDetail(6), true);
            }
            else if (!GetHintDetail(11).isHintCompleted) //Rope Inner
            {
                ShowHintGlow(GetHintDetail(9), true);
            }
            else if (!GetHintDetail(14).isHintCompleted) //Small Box
            {
                ShowHintGlow(GetHintDetail(12), true);
            }
            else if (!GetHintDetail(15).isHintCompleted) //Clue in BG
            {
                ShowHintGlow(GetHintDetail(15), true);
            }
            else if (!GetHintDetail(16).isHintCompleted) //Small Box Inner
            {
                ShowHintGlow(GetHintDetail(16), true);
            }
            else if (!GetHintDetail(22).isHintCompleted) //Wood DC
            {
                ShowHintGlow(GetHintDetail(22), true);
            }
            else
            {
                base.ShowHint();
            }
        }
        //-----BG 2 Gameplay Hint-----
        else if (selectedHint.hintId == 33 && selectedHint != null) //---Lamb Process
        {
            if (!GetHintDetail(34).isHintCompleted) //Click
            {
                ShowHintGlow(GetHintDetail(34));
            }
            else if (!GetHintDetail(28).isHintCompleted) //Parrel Click
            {
                ShowHintGlow(GetHintDetail(28), true);
            }
            else if (!GetHintDetail(33).isHintCompleted) //Lamb Process
            {
                ShowHintGlow(GetHintDetail(33), true);
            }
            else
            {
                base.ShowHint();
            }
        }
        else if (selectedHint.hintId == 48 && selectedHint != null) //---Lamb Process
        {
            if (!GetHintDetail(47).isHintCompleted) //Click
            {
                ShowHintGlow(GetHintDetail(47), true);
            }
            else
            {
                base.ShowHint();
            }
        }
        else if (selectedHint.hintId == 50 && selectedHint != null) //---Rock Door 
        {
            if (!GetHintDetail(47).isHintCompleted) //Gold Hand
            {
                ShowHintGlow(GetHintDetail(47), true);
            }
            else if (!GetHintDetail(48).isHintCompleted) //Gold Wheel Process
            {
                ShowHintGlow(GetHintDetail(48), true);
            }
            else
            {
                base.ShowHint();
            }
        }
        else if (selectedHint.hintId == 53 && selectedHint != null) //---Boat Repair CS
        {
            if (!GetHintDetail(28).isHintCompleted) //Parrel Click
            {
                ShowHintGlow(GetHintDetail(28), true);
            }
            else if (!GetHintDetail(33).isHintCompleted) //Lamb Process
            {
                ShowHintGlow(GetHintDetail(33), true);
            }
            else if (!GetHintDetail(43).isHintCompleted) //Threat NAil Inner
            {
                ShowHintGlow(GetHintDetail(41), true);
            }
            else if (!GetHintDetail(46).isHintCompleted) //Cloth Inner
            {
                ShowHintGlow(GetHintDetail(44), true);
            }
            else if (!GetHintDetail(47).isHintCompleted) //Gold Hand
            {
                ShowHintGlow(GetHintDetail(47), true);
            }
            else if (!GetHintDetail(48).isHintCompleted) //Gold Wheel Process
            {
                ShowHintGlow(GetHintDetail(48), true);
            }
            else if (!GetHintDetail(52).isHintCompleted) //Thutpu Get Peocess
            {
                ShowHintGlow(GetHintDetail(52), true);
            }
            else
            {
                base.ShowHint();
            }
        }
        else
        {
            base.ShowHint();
        }
    }
}