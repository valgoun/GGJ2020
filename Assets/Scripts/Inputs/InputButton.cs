﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputButton : MonoBehaviour
{
    [Header("Tweaking")]
    public bool canBeSelectedFirst = true;

    [Header("References")]
    public Image SelectionGlow;
    public TextMeshProUGUI InputText;
    public RectTransform DurabilityBar;

    [NonSerialized] public RectTransform MyRect;
    [NonSerialized] public InputEntity Entity;

    public static InputButton SelectedButton;

    public void Initialize(InputEntity entity)
    {
        Entity = entity;

        if (!entity.IsEmpty)
        {
            InputText.SetText(Keyboard.current[entity.MyKey].displayName);
            entity.OnUpdate += UpdateGraphics;
        }

        MyRect = GetComponent<RectTransform>();
        MyRect.anchorMax = Vector2.one * 0.5f;
        MyRect.anchorMin = Vector2.one * 0.5f;
    }

    public void Interacted()
    {
        if (!canBeSelectedFirst && SelectedButton == null)
            return;

        if(SelectedButton)
        {
            if (SelectionGlow)
                SelectionGlow.enabled = false;

            if (SelectedButton.SelectionGlow)
                SelectedButton.SelectionGlow.enabled = false;

            if (SelectedButton != this)
            {
                InputLocationType loc = Entity.MyLocation;
                int index = Entity.MyInventoryLocation;
                InputType key = Entity.MyAssignedInput;
                Transform transf = MyRect.parent;

                InputEntity ent = SelectedButton.Entity;
                if (ent.MyLocation == InputLocationType.ASSIGNED)
                    InputManager.Instance.RegisterInput(ent.MyAssignedInput, Entity);
                else
                    InputInventory.Instance.AddToInventory(Entity, ent.MyInventoryLocation);
                MyRect.SetParent(SelectedButton.MyRect.parent);
                MyRect.localPosition = Vector3.zero;

                if (loc == InputLocationType.ASSIGNED)
                    InputManager.Instance.RegisterInput(key, ent);
                else
                    InputInventory.Instance.AddToInventory(ent, index);
                SelectedButton.MyRect.SetParent(transf);
                SelectedButton.MyRect.localPosition = Vector3.zero;
            }
            SelectedButton = null;
        }
        else
        {
            if (SelectionGlow)
                SelectionGlow.enabled = true;
            SelectedButton = this;
        }
    }

    void UpdateGraphics(bool destroyed)
    {
        if (destroyed)
        {
            Destroy(this);
            return;
        }

        DurabilityBar.localScale = new Vector3(Entity.MyDurability / Entity.MyStartDurability , 1, 1);
    }

    private void OnDestroy()
    {
        if(Entity != null)
            Entity.OnUpdate -= UpdateGraphics;
    }
}