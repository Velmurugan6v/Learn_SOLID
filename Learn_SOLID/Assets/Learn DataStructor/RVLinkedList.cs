using UnityEngine;

public class RVLinkedList : MonoBehaviour
{
    public LinedList LinkedList;

    private void Start()
    {
        LinkedList = new LinedList();

        LinkedList.InsertBeginning(3);
        LinkedList.InsertBeginning(2);
        LinkedList.InsertBeginning(4);
        LinkedList.InsertBeginning(1);
        LinkedList.Display();
        LinkedList.Reverse();
        LinkedList.Display();
    }
}

public class Node
{
    public int Data;
    public Node NextNode;

    public Node(int data)
    {
        this.Data = data;
        NextNode = null;
    }
}

public class LinedList
{
    public Node HeadNote;

    public void InsertBeginning(int data)
    {
        Node newNode = new Node(data);

        newNode.NextNode = HeadNote;
        HeadNote = newNode;
    }

    public void InsertLast(int data)
    {
        Node newNode = new Node(data);
        Node tempNote = HeadNote;

        while (tempNote.NextNode != null)
        {
            tempNote = tempNote.NextNode;
        }

        tempNote.NextNode = newNode;
    }

    public void InsertIndex(int index, int data)
    {
        if (index < 0)
        {
            Debug.LogError("Invalid index");
            return;
        }

        Node newNode = new Node(data);
        Node tempNote = HeadNote;

        for (int i = 0; i < index - 1; i++) //3 4 5 2     //--> index - 2, data - 8
        {
            tempNote = tempNote.NextNode;
        }

        newNode.NextNode = tempNote.NextNode;
        tempNote.NextNode = newNode;
    }

    public void DeleteValue(int data)
    {
        Node tempNote = HeadNote;

        while (tempNote.NextNode != null && tempNote.NextNode.Data != data) // 5 7 9 3 1
        {
            tempNote = tempNote.NextNode;
        }

        if (tempNote.NextNode == null)
        {
            Debug.Log("Invalid Index");
            return;
        }

        tempNote.NextNode = tempNote.NextNode.NextNode;
    }

    public void DeleteIndex(int index)
    {
        if (index < 0)
        {
            Debug.LogError("Invalid index");
            return;
        }

        if (index == 0)
        {
            HeadNote = HeadNote.NextNode;
            return;
        }

        Node tempNote = HeadNote;

        for (int i = 0; i < index - 1; i++) // 5 7 9 3 1
        {
            tempNote = tempNote.NextNode;
        }

        tempNote.NextNode = tempNote.NextNode.NextNode;
    }

    public void Reverse()
    {
        Node previousNode = null;
        Node currentNode = HeadNote;
        Node nextNode = null;

        while (currentNode != null)
        {
            nextNode = currentNode.NextNode;
            currentNode.NextNode = previousNode;
            previousNode = currentNode;
            currentNode = nextNode;
        }

        HeadNote = previousNode;
    }

    public void Display()
    {
        Node tempNode = HeadNote;

        while (tempNode != null)
        {
            Debug.Log(tempNode.Data + "-->");
            tempNode = tempNode.NextNode;
        }

        Debug.Log("null");
    }
}