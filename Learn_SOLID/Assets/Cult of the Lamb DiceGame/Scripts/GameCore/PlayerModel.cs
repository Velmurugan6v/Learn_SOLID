using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerModel
{
    private int _rowCount;
    private int _columnCount;
    public ColumnSlot[] slotColumnArray;

    public PlayerModel(int rowCount, int columnCount)
    {
        _rowCount = rowCount;
        _columnCount = columnCount;
        SetupColumnArrays();
    }

    private void SetupColumnArrays()
    {
        slotColumnArray = new ColumnSlot[_columnCount];

        for (int i = 0; i < slotColumnArray.Length; i++)
        {
            slotColumnArray[i] = new ColumnSlot(_rowCount);
        }
    }
}

[System.Serializable]
public class ColumnSlot
{
    public int rowCount;
    public int currentSlotIndex;
    public int totalSlotsValueCount;
    public Slot[] slotArray;

    public ColumnSlot(int rowCount)
    {
        this.rowCount = rowCount;
        slotArray = new Slot[this.rowCount];
    }
}
