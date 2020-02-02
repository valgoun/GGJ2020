using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputLocationType
{
    ASSIGNED,
    INVENTORY
}

public class InputObject : MonoBehaviour, IDestroyable
{
    public float Durability = 10;

    [Header("Graphics")]
    public float RotationSpeed;
    public float BaseHeight;
    public float FloatSpeed;
    public float FloatHeight;
    public AnimationCurve FloatCurve;

    [Header("References")]
    public TextMeshProUGUI KeyText;

    [NonSerialized] public InputEntity MyInput;

    float _lifeStart;

    public event Action OnDestroyEvent;

    void Start()
    {
        MyInput = new InputEntity(Durability);
        KeyText.text = Keyboard.current[MyInput.MyKey].displayName;

        _lifeStart = Time.time;
    }

    private void Update()
    {
        transform.eulerAngles += Vector3.up * RotationSpeed * Time.deltaTime;
        Vector3 position = transform.position;
        position.y = BaseHeight + FloatHeight * FloatCurve.Evaluate(((Time.time - _lifeStart) % FloatSpeed) / FloatSpeed);
        transform.position = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
            if (InputInventory.Instance.AddToInventory(MyInput))
            {
                OnDestroyEvent?.Invoke();
                Master.Instance.InputCollected++;
                Destroy(gameObject);
            }
    }
}

public class InputEntity
{
    public float MyDurability;
    public float MyStartDurability;
    public Key MyKey;
    public ControllerType MyType;
    public bool IsEmpty;

    public InputLocationType MyLocation;
    public int MyInventoryLocation;
    public InputType MyAssignedInput;

    public Action<bool> OnUpdate;

    bool _lastState;
    bool _currentState;

    public InputEntity()
    {
        IsEmpty = true;
    }

    public InputEntity(float durability)
    {
        MyDurability = durability;
        MyStartDurability = durability;

        var value = InputManager.Instance.GetRandomInput();
        MyKey = value.Item1;
        MyType = value.Item2;
    }

    public InputEntity(Key key, float durability)
    {
        MyDurability = durability;
        MyStartDurability = durability;

        MyKey = key;
        MyType = ControllerType.KEYBOARD;
    }

    public bool ReduceDurability(float value = 1)
    {
        if (InputManager.Instance.DontDetoriateInput)
            return false;

        MyDurability -= value;
        bool dead = (MyDurability <= 0);
        OnUpdate?.Invoke(dead);
        return dead;
    }

    public void PollInput(bool forceDead = false)
    {
        _lastState = _currentState;

        _currentState = false;
        if (!forceDead)
        {
            switch (MyType)
            {
                case ControllerType.KEYBOARD:
                    _currentState = Keyboard.current[MyKey].isPressed;
                    break;

                case ControllerType.MOUSE:
                    break;

                case ControllerType.CONTROLLER:
                    break;

                default:
                    _currentState = false;
                    break;
            }
        }
    }

    public bool GetInputDown()
    {
        return (!_lastState && _currentState);
    }

    public bool GetInput()
    {
        return _currentState;
    }

    public bool GetInputUp()
    {
        return (_lastState && !_currentState);
    }

    //void Used(Action logic)
    //{
    //    logic();
    //} 
}
