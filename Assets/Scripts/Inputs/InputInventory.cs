using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputInventory : MonoBehaviour
{
    public static InputInventory Instance;

    public int InventorySize = 6;

    [NonSerialized] public InputEntity[] Inventory;

    private void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;

        Inventory = new InputEntity[InventorySize];
    }

    public bool AddToInventory(InputEntity input)
    {
        bool failed = true;
        for (int i = 0; i < InventorySize; i++)
        {
            if (Inventory[i] == null)
            {
                failed = false;
                AddToInventory(input, i);
                break;
            }
        }

        return !failed;
    }

    public void AddToInventory(InputEntity input, int index)
    {
        Inventory[index] = (!input.IsEmpty) ? input : null;
        input.MyLocation = InputLocationType.INVENTORY;
        input.MyInventoryLocation = index;
    }
}
