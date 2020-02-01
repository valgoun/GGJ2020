using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputObject : MonoBehaviour
{
    public float Durability = 10;

    [NonSerialized] public InputEntity MyInput;

    void Start()
    {
        MyInput = new InputEntity(Durability);
    }
}

public class InputEntity
{
    public float MyDurability;
    public Key MyKey;
    public ControllerType MyType;

    bool _lastState;
    bool _currentState;

    public InputEntity(float durability)
    {
        MyDurability = durability;

        var value = InputManager.Instance.GetRandomInput();
        MyKey = value.Item1;
        MyType = value.Item2;
    }

    public InputEntity(Key key, float durability)
    {
        MyDurability = durability;

        MyKey = key;
        MyType = ControllerType.KEYBOARD;
    }

    public bool ReduceDurability(float value = 1)
    {
        if (InputManager.Instance.DontDetoriateInput)
            return false;

        MyDurability -= value;
        return (MyDurability <= 0);
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
