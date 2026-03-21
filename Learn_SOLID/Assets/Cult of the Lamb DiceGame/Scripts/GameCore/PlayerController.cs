using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool canAutoInitial = false;
    public int totalRow;
    public int totalColumn;

    [SerializeField] PlayerView playerView;
    [SerializeField] PlayerModel playerModel;

    private void Awake()
    {
        if (!canAutoInitial) return;

        playerModel = new PlayerModel(totalRow, totalColumn);
    }
}