using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    Player player;

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

    public Player Player
    {
        get
        {
            if (player == null)
            {
                player = FindAnyObjectByType<Player>();
            }

            return player;
        }
    }

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();

        itemDataManager = GetComponent<ItemDataManager>();
    }

    protected override void OnPreInitialize()
    {
        base.OnPreInitialize();
    }
}
