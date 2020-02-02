using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputUI : SerializedMonoBehaviour
{
    public static InputUI Instance;

    [Header("Menu")]
    public Dictionary<InputType, Transform> InputPositions;
    public Transform InventoryPosition;
    public GameObject UI;

    [Header("InGame")]
    public Dictionary<InputType, Transform> IndicatorPositions;
    public GameObject InGame;

    [Header("References")]
    public GameObject PlaceholderPrefab;
    public GameObject InputPrefab;
    public GameObject EmptyPrefab;

    Transform[] _inventoryPositions;
    List<InputButton> _buttons = new List<InputButton>();
    List<InputButton> _ui = new List<InputButton>();

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

        ReloadUI();
    }

    public void Open()
    {
        UI.SetActive(true);
        InGame.SetActive(false);

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
        InGame.SetActive(true);

        foreach (InputButton button in _buttons)
            if (button)
                Destroy(button.gameObject);

        _buttons.Clear();
    }

    public void DestroyButton(InputButton button)
    {
        SoundManager.Instance.PlaySound(SoundTypes.MENU_SELECTION);

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

        InputUI.Instance.ReloadUI();
    }

    public void ReloadUI()
    {
        foreach (InputButton button in _ui)
            if (button)
                Destroy(button.gameObject);
        _ui.Clear();

        foreach(InputType key in IndicatorPositions.Keys)
        {
            InputButton button;
            InputEntity entity = null;
            if (InputManager.Instance.CurrentInputs.ContainsKey(key))
                entity = InputManager.Instance.CurrentInputs[key];
            if (entity != null && entity.MyDurability > 0)
                button = Instantiate(InputPrefab, IndicatorPositions[key]).GetComponent<InputButton>();
            else
                button = Instantiate(EmptyPrefab, IndicatorPositions[key]).GetComponent<InputButton>();

            button.MyButton.interactable = false;
            button.Initialize(entity);
            button.MyRect.localPosition = Vector3.zero;
            _ui.Add(button);
        }
    }
}
