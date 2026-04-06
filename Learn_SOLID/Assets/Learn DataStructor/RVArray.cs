using System;
using UnityEngine;

public class RVArray : MonoBehaviour
{
    public NewArray newArray;

    private void Start()
    {
        newArray = new NewArray(5);
        newArray.Insert(0, 10);
        newArray.Insert(1, 20);
        newArray.Insert(2, 30);
        newArray.Insert(3, 40);
        newArray.Insert(4, 50);
        newArray.Insert(5, 60); //it will get Error

        for (int i = 0; i < newArray.Size; i++)
        {
            print($"{i} + {newArray.Get(i)}");
        }

        print(newArray.Get(0));
        print(newArray.Get(3));
        print(newArray.Get(5));

        //After
        print($"Before Set {newArray.Get(3)}");
        newArray.Set(3, 1951);
        print($"After Set {newArray.Get(3)}");

        //Search value
        print($"Value at index:  {newArray.Searche(1951)}");

        //Delete element in array
        newArray.Remove(3);
        print($"After Remove {newArray.Get(3)}");
    }
}

public class NewArray
{
    private int[] arr;
    public int Length { get; set; }
    public int Size { get; set; }

    //Initialize
    public NewArray(int length)
    {
        this.Length = length;
        arr = new int[length];
        Size = 0;
    }

    public int Get(int index)
    {
        if (index > Size || index < 0)
            Debug.LogError("Invalid Index");

        if (index >= Length)
        {
            Debug.LogError("Index is out of range");
            return -1;
        }

        return arr[index];
    }

    public void Set(int index, int value)
    {
        if (index > Size)
            Debug.LogError("Invalid Index");
        if (index < 0)
            Debug.LogError("Need to initialize Array");

        arr[index] = value;
    }

    public int Searche(int value)
    {
        for (int i = 0; i < Size; i++)
            if (arr[i] == value)
                return i;

        return -1;
    }

    public void Insert(int index, int value)
    {
        if (index < 0)
        {
            Debug.LogError("Invalid Index");
            return;
        }

        if (index >= Length)
        {
            Debug.LogError("Index is out of range");
            return;
        }

        for (int i = Size; i > index; i--)
        {
            arr[i] = arr[i - 1];
        }

        arr[index] = value;
        Size++;
    }

    public void Remove(int index)
    {
        if (index < 0)
        {
            Debug.LogError("Invalid Index");
            return;
        }

        if (index > Length)
        {
            Debug.LogError("Index is out of range");
            return;
        }

        for (int i = index; i < Size - 1; i++)
        {
            arr[i] = arr[i + 1];
        }

        Size--;
    }
}


/*
------Array----
--Get/
--Set/
--Delete/
--Insert/
--Search/


*/