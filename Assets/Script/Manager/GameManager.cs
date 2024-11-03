using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    PlayerMovementContoller player;

    PlayerInputController inputController;

    ItemDataManager itemDataManager;

    public ItemDataManager ItemDataManager
    {
        get
        {
            if (itemDataManager == null)
            {
                itemDataManager = FindAnyObjectByType<ItemDataManager>();
            }

            return itemDataManager;
        }
    }

    public PlayerMovementContoller Player
    {
        get
        {
            if (player == null)
            {
                player = FindAnyObjectByType<PlayerMovementContoller>();
            }

            return player;
        }
    }
    public PlayerInputController InputController
    {
        get
        {
            if (inputController == null)
            {
                inputController = FindAnyObjectByType<PlayerInputController>();
            }

            return inputController;
        }
    }

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<PlayerMovementContoller>();

        itemDataManager = GetComponent<ItemDataManager>();

        inputController = FindAnyObjectByType<PlayerInputController>();
    }

    protected override void OnPreInitialize()
    {
        base.OnPreInitialize();
    }
}
