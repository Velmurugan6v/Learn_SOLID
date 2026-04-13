using System;
using System.Collections.Generic;
using CultOfTheLamb_DiceGame;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    public bool canUseCustomvalue = false;
    public bool canAutoInitial = false;
    public int totalRow;
    public int totalColumn;


    public PlayerView playerView;
    public PlayerModel playerModel;

    [SerializeField] IDice _playerDice;

    [SerializeField] private int diceGivenNumber;

    [SerializeField] Sprite[] diceSprites;
    [SerializeField] Image diceImage;

    public List<Slot> _sameValues;
    public List<Slot> _differentValues;

    public List<Slot> _opponentDifferentValues;
    public List<Slot> _opponentSameValues;

    public PlayerController opponent;

    //Event
    public Action<int> OnColumnSelected;
    public Action OnDiceRoll;

    private void Awake()
    {
        _playerDice = new PlayerDice();

        if (!canAutoInitial) return;

        playerModel = new PlayerModel(totalRow, totalColumn);

        //This below code have to put in single Entry point methos
    }

    private void OnEnable()
    {
        OnColumnSelected += AddDiceValueToPlayer;
        OnDiceRoll += RollDice;
    }

    private void OnDisable()
    {
        OnColumnSelected -= AddDiceValueToPlayer;
        OnDiceRoll -= RollDice;
    }

    public void SetUpPlayer(Image diceImage, Sprite[] diceSprites, Text playerTotalScoreText, PlayerController opponent)
    {
        this.diceImage = diceImage;
        this.diceSprites = diceSprites;
        this.opponent = opponent;
        playerView.SetUpPlayerScoreText(playerTotalScoreText);
    }

    [ContextMenu("Roll Dice")]
    public void RollDice()
    {
        if (!canUseCustomvalue)
            diceGivenNumber = _playerDice.RollDice();

        diceImage.sprite = diceSprites[diceGivenNumber - 1];
    }

    public void AddDiceValueToPlayer(int diceColumn)
    {
        if (diceGivenNumber == -1)
            return;

        ColumnSlot currentSelectedColumn = playerModel.slotColumnArray[diceColumn];

        print(diceColumn);

        if (currentSelectedColumn == null)
            return;

        if (currentSelectedColumn.currentSlotIndex >= currentSelectedColumn.slotArray.Length)
            return;

        currentSelectedColumn.slotArray[currentSelectedColumn.currentSlotIndex].SlotCurrentValue += diceGivenNumber;

        currentSelectedColumn.totalSlotsValueCount += diceGivenNumber;

        DiceMoveAnimation(currentSelectedColumn.slotArray[currentSelectedColumn.currentSlotIndex].SlotImage);

        FindSameDiceValueInColumn(currentSelectedColumn);

        MultipleDiceValues(currentSelectedColumn);

        playerView.ShowColumnTotal(diceColumn, currentSelectedColumn.totalSlotsValueCount);

        currentSelectedColumn.currentSlotIndex++;

        playerView.ShowTotalColumnTotal(GetColumnTotalValue());


        //opponet Check

        /*ColumnSlot opponetColumn = opponent.playerModel.slotColumnArray[diceColumn];

        ReduceOpponentValue(opponetColumn);
        opponent.playerView.ShowColumnTotal(diceColumn, opponetColumn.totalSlotsValueCount);
        opponent.playerView.ShowTotalColumnTotal(GetOpponentColumnTotalValue());*/
        SetDiceValueDefault();
    }

    private void DiceMoveAnimation(Image columDiceImage)
    {
        
        
        columDiceImage.sprite = diceSprites[diceGivenNumber - 1];
    }

    public void FindSameDiceValueInColumn(ColumnSlot currentSelectedColumn)
    {
        for (int i = 0; i < currentSelectedColumn.slotArray.Length; i++)
        {
            if (currentSelectedColumn.slotArray[i].SlotCurrentValue == diceGivenNumber)
                _sameValues.Add(currentSelectedColumn.slotArray[i]);
            else
                _differentValues.Add(currentSelectedColumn.slotArray[i]);
        }

        if (_sameValues.Count == 2)
        {
            foreach (Slot slot in _sameValues)
            {
                slot.SlotImage.color = Color.gray;
            }
        }
        else if (_sameValues.Count == 3)
        {
            foreach (Slot slot in _sameValues)
            {
                slot.SlotImage.color = Color.yellow;
            }
        }
    }

    public void ReduceOpponentValue(ColumnSlot currentSelectedColumn)
    {
        for (int i = 0; i < currentSelectedColumn.slotArray.Length; i++)
        {
            if (currentSelectedColumn.slotArray[i].SlotCurrentValue == diceGivenNumber)
                _opponentSameValues.Add(currentSelectedColumn.slotArray[i]);
            else
                _opponentDifferentValues.Add(currentSelectedColumn.slotArray[i]);
        }


        int totalMultipleCount = 0;
        int totalDiceValue = 0;
        int remainValue = 0;
        int remainCount = 0;


        for (int i = 0; i < _opponentSameValues.Count; i++)
        {
            print(i);
            totalMultipleCount++;
            totalDiceValue += _opponentSameValues[i].SlotCurrentValue;
            _opponentSameValues[i].SlotImage.sprite = null;
            _opponentSameValues[i].SlotImage.color = Color.white;
        }

        foreach (Slot slot in _opponentDifferentValues)
        {
            remainCount++;
            remainValue += slot.SlotCurrentValue;
        }


        totalDiceValue *= totalMultipleCount;

        if (totalMultipleCount == 3)
        {
            currentSelectedColumn.currentSlotIndex = 0;
            currentSelectedColumn.totalSlotsValueCount = 0;
        }
        else if (totalMultipleCount == 2 && totalDiceValue == 1)
        {
            currentSelectedColumn.currentSlotIndex = 1;
            currentSelectedColumn.totalSlotsValueCount = remainValue;
        }
        else if (totalMultipleCount == 1 && totalDiceValue == 2)
        {
            currentSelectedColumn.currentSlotIndex = 2;
            currentSelectedColumn.totalSlotsValueCount = remainValue;
        }
        else
        {
            currentSelectedColumn.currentSlotIndex = 0;
            currentSelectedColumn.totalSlotsValueCount = remainValue;
        }

        /*_opponentSameValues.Clear();
        _opponentDifferentValues.Clear();*/
    }

    private void MultipleDiceValues(ColumnSlot currentSelectedColumn)
    {
        if (_sameValues.Count <= 1)
        {
            _sameValues.Clear();
            _differentValues.Clear();
            return;
        }

        int totalMultipleCount = 0;
        int totalDiceValue = 0;
        int remainValue = 0;


        for (int i = 0; i < _sameValues.Count; i++)
        {
            totalMultipleCount++;
            totalDiceValue += _sameValues[i].SlotCurrentValue;
        }

        foreach (Slot slot in _differentValues)
        {
            remainValue += slot.SlotCurrentValue;
        }


        totalDiceValue *= totalMultipleCount;

        if (totalMultipleCount == 2)
        {
            currentSelectedColumn.totalSlotsValueCount = totalDiceValue + remainValue;
        }
        else
        {
            currentSelectedColumn.totalSlotsValueCount = totalDiceValue;
        }

        _sameValues.Clear();
        _differentValues.Clear();
    }


    private int GetColumnTotalValue()
    {
        int totalValue = 0;

        for (int i = 0; i < playerModel.slotColumnArray.Length; i++)
        {
            totalValue += playerModel.slotColumnArray[i].totalSlotsValueCount;
        }

        return totalValue;
    }

    private int GetOpponentColumnTotalValue()
    {
        int totalValue = 0;

        for (int i = 0; i < opponent.playerModel.slotColumnArray.Length; i++)
        {
            totalValue += opponent.playerModel.slotColumnArray[i].totalSlotsValueCount;
        }

        return totalValue;
    }

    private void SetDiceValueDefault()
    {
        diceImage.sprite = null;
        diceGivenNumber = -1;
    }

    public void ResetPlayerData()
    {
        playerModel.ResetColumnSlot();
        playerView.ResetPlayerScoreText();
    }
}