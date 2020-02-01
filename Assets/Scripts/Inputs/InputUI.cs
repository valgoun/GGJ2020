using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUI : SerializedMonoBehaviour
{
    public static InputUI Instance;

    public GameObject PlaceholderPrefab;
    public GameObject InputPrefab;
    public GameObject EmptyPrefab;
    public Dictionary<InputType, Transform> InputPositions;
    public Transform InventoryPosition;
    public GameObject UI;

    Transform[] _inventoryPositions;
    List<InputButton> _buttons = new List<InputButton>();
    

    private void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        _inventoryPositions = new Transform[InputInventory.Instance.InventorySize];
        for (int i = 0; i < InputInventory.Instance.InventorySize; i++)
        {
            Transform position = Instantiate(PlaceholderPrefab, InventoryPosition).transform;
            _inventoryPositions[i] = position;
        }
    }

    public void Open()
    {
        UI.SetActive(true);

        foreach(InputType input in InputPositions.Keys)
        {
            InputButton button;
            InputEntity entity = null;
            if (InputManager.Instance.CurrentInputs.ContainsKey(input))
                entity = InputManager.Instance.CurrentInputs[input];
            if (entity == null)
            {
                entity = new InputEntity();
                entity.MyLocation = InputLocationType.ASSIGNED;
                entity.MyAssignedInput = input;

                button = Instantiate(EmptyPrefab, InputPositions[input]).GetComponent<InputButton>();
            }
            else
                button = Instantiate(InputPrefab, InputPositions[input]).GetComponent<InputButton>();

            button.Initialize(entity);
            button.MyRect.localPosition = Vector3.zero; 
            _buttons.Add(button);
        }

        for(int i = 0; i < InputInventory.Instance.InventorySize; i++)
        {
            InputButton button;
            InputEntity entity = InputInventory.Instance.Inventory[i];
            if (entity == null)
            {
                entity = new InputEntity();
                entity.MyLocation = InputLocationType.ASSIGNED;
                entity.MyInventoryLocation = i;

                button = Instantiate(EmptyPrefab, _inventoryPositions[i]).GetComponent<InputButton>();
            }
            else
                button = Instantiate(InputPrefab, _inventoryPositions[i]).GetComponent<InputButton>();

            button.Initialize(entity);
            button.MyRect.localPosition = Vector3.zero;
            _buttons.Add(button);
        }
    }

    public void Close()
    {
        UI.SetActive(false);

        foreach (InputButton button in _buttons)
            if (button)
                Destroy(button.gameObject);

        _buttons.Clear();
    }

    public void DestroyButton(InputButton button)
    {
        InputEntity entity = button.Entity;
        entity.IsEmpty = true;

        if (entity.MyLocation == InputLocationType.ASSIGNED)
            InputManager.Instance.CurrentInputs.Remove(entity.MyAssignedInput);
        else
            InputInventory.Instance.Inventory[entity.MyInventoryLocation] = null;

        Destroy(button.gameObject);

        button = Instantiate(EmptyPrefab, button.transform.parent).GetComponent<InputButton>();
        button.Initialize(entity);
        _buttons.Add(button);
    }
}
