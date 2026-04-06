using System;
using System.Collections.Generic;
using CultOfTheLamb_DiceGame;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    public bool canAutoInitial = false;
    public int totalRow;
    public int totalColumn;

    [SerializeField] PlayerView playerView;
    [SerializeField] PlayerModel playerModel;

    [SerializeField] IDice _playerDice;

    [SerializeField] private int diceGivenNumber;

    [SerializeField] Sprite[] diceSprites;
    [SerializeField] Image diceImage;

    public List<Slot> _sameValues;

    private void Awake()
    {
        _playerDice = new PlayerDice();

        if (!canAutoInitial) return;

        playerModel = new PlayerModel(totalRow, totalColumn);
    }

    [ContextMenu("Roll Dice")]
    public void RollDice()
    {
        diceGivenNumber = _playerDice.RollDice();
        diceImage.sprite = diceSprites[diceGivenNumber - 1];
    }

    public void AddDiceValueToPlayer(int diceColumn)
    {
        ColumnSlot currentSelectedColumn = playerModel.slotColumnArray[diceColumn];

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

    private void MultipleDiceValues(ColumnSlot currentSelectedColumn)
    {
        if (_sameValues.Count <= 1)
        {
            _sameValues.Clear();
            return;
        }

        int totalMultipleCount = 0;
        int totalDiceValue = 0;


        for (int i = 0; i < _sameValues.Count; i++)
        {
            totalMultipleCount++;
            totalDiceValue += _sameValues[i].SlotCurrentValue;
        }

        print("old total value" + currentSelectedColumn.totalSlotsValueCount);
        currentSelectedColumn.totalSlotsValueCount -= diceGivenNumber * totalMultipleCount;

        totalDiceValue *= totalMultipleCount;

        print("same dice total Value" + totalDiceValue);
        print("multipler" + totalMultipleCount);
        print("old total value 1" + currentSelectedColumn.totalSlotsValueCount);

        currentSelectedColumn.totalSlotsValueCount += totalDiceValue;

        print("new total value 2" + currentSelectedColumn.totalSlotsValueCount);

        _sameValues.Clear();
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

    private void SetDiceValueDefault()
    {
        diceImage.sprite = null;
        diceGivenNumber = 0;
    }
}