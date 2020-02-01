using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputType
{
    MOVE_FORWARD,
    MOVE_BACKWARD,
    MOVE_LEFT,
    MOVE_RIGHT,
    SHOOT_FORWARD,
    SHOOT_BACKWARD,
    SHOOT_LEFT,
    SHOOT_RIGHT,
    MELEE
}

public enum ControllerType
{
    KEYBOARD,
    MOUSE,
    CONTROLLER
}

public class InputManager : SerializedMonoBehaviour
{
    public static InputManager Instance;

    public Dictionary<InputType, Key> StartInputs;
    public float inputDecreasePerSecond = 1;

    [Header("Debug")]
    public bool DontDetoriateInput;

    [Header("Inputs")]
    public List<Key> BannedKeys;

    [NonSerialized] public List<Key> AvailableKeys = new List<Key>();
    [NonSerialized] public ITreatedInput RegisteredPlayer;
    [NonSerialized] public Dictionary<InputType, InputEntity> CurrentInputs = new Dictionary<InputType, InputEntity>();

    Mapping _mapping;
    
    void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        foreach(InputType key in StartInputs.Keys)
            RegisterInput(key, new InputEntity(StartInputs[key], 10));

        foreach (Key key in Enum.GetValues(typeof(Key)))
            if (BannedKeys.Contains(key))
                AvailableKeys.Add(key);
    }

    void Update()
    {
        foreach (InputEntity input in CurrentInputs.Values)
            input.PollInput();

        ComputeMelee();
        ComputeMovement();
        ComputeShoot();
    }

    void ComputeMelee()
    {
        if (CurrentInputs.ContainsKey(InputType.MELEE))
        {
            InputEntity input = CurrentInputs[InputType.MELEE];
            if (input.GetInputDown())
                if (RegisteredPlayer.OnMelee())
                    if (input.ReduceDurability())
                        CurrentInputs.Remove(InputType.MELEE);
        }
    }

    void ComputeMovement()
    {
        int vertical = 0;
        int horizontal = 0;

        if (CurrentInputs.ContainsKey(InputType.MOVE_FORWARD))
            if (CurrentInputs[InputType.MOVE_FORWARD].GetInput())
                vertical++;
        if (CurrentInputs.ContainsKey(InputType.MOVE_BACKWARD))
            if (CurrentInputs[InputType.MOVE_BACKWARD].GetInput())
                vertical--;

        if (vertical == 1)
        {
            if (CurrentInputs[InputType.MOVE_FORWARD].ReduceDurability(Time.deltaTime * inputDecreasePerSecond))
                CurrentInputs.Remove(InputType.MOVE_FORWARD);
        }
        else if (vertical == -1)
        {
            if (CurrentInputs[InputType.MOVE_BACKWARD].ReduceDurability(Time.deltaTime * inputDecreasePerSecond))
                CurrentInputs.Remove(InputType.MOVE_BACKWARD);
        }

        if (CurrentInputs.ContainsKey(InputType.MOVE_LEFT))
            if (CurrentInputs[InputType.MOVE_LEFT].GetInput())
                horizontal--;
        if (CurrentInputs.ContainsKey(InputType.MOVE_RIGHT))
            if (CurrentInputs[InputType.MOVE_RIGHT].GetInput())
                horizontal++;

        if (horizontal == -1)
        {
            if (CurrentInputs[InputType.MOVE_LEFT].ReduceDurability(Time.deltaTime * inputDecreasePerSecond))
                CurrentInputs.Remove(InputType.MOVE_LEFT);
        }
        else if (horizontal == 1)
        {
            if (CurrentInputs[InputType.MOVE_RIGHT].ReduceDurability(Time.deltaTime * inputDecreasePerSecond))
                CurrentInputs.Remove(InputType.MOVE_RIGHT);
        }

        RegisteredPlayer.OnMove(new Vector2(horizontal, vertical));
    }

    void ComputeShoot()
    {
        int vertical = 0;
        int horizontal = 0;

        if (CurrentInputs.ContainsKey(InputType.SHOOT_FORWARD))
            if (CurrentInputs[InputType.SHOOT_FORWARD].GetInput())
                vertical++;
        if (CurrentInputs.ContainsKey(InputType.SHOOT_BACKWARD))
            if (CurrentInputs[InputType.SHOOT_BACKWARD].GetInput())
                vertical--;

        if (vertical == 0)
        {
            if (CurrentInputs.ContainsKey(InputType.SHOOT_LEFT))
                if (CurrentInputs[InputType.SHOOT_LEFT].GetInput())
                    horizontal--;
            if (CurrentInputs.ContainsKey(InputType.SHOOT_RIGHT))
                if (CurrentInputs[InputType.SHOOT_RIGHT].GetInput())
                    horizontal++;
        }

        Vector2 direction = new Vector2(horizontal, vertical);
        if (direction != Vector2.zero)
        {
            if (RegisteredPlayer.OnShoot(direction))
            {
                if (vertical == 1)
                {
                    if (CurrentInputs[InputType.SHOOT_FORWARD].ReduceDurability(Time.deltaTime * inputDecreasePerSecond))
                        CurrentInputs.Remove(InputType.SHOOT_FORWARD);
                }
                else if (vertical == -1)
                {
                    if (CurrentInputs[InputType.SHOOT_BACKWARD].ReduceDurability(Time.deltaTime * inputDecreasePerSecond))
                        CurrentInputs.Remove(InputType.SHOOT_BACKWARD);
                }
                else if (horizontal == -1)
                {
                    if (CurrentInputs[InputType.SHOOT_LEFT].ReduceDurability(Time.deltaTime * inputDecreasePerSecond))
                        CurrentInputs.Remove(InputType.SHOOT_LEFT);
                }
                else if (horizontal == 1)
                {
                    if (CurrentInputs[InputType.SHOOT_RIGHT].ReduceDurability(Time.deltaTime * inputDecreasePerSecond))
                        CurrentInputs.Remove(InputType.SHOOT_RIGHT);
                }
            }
        }
    }

    public Tuple<Key, ControllerType> GetRandomInput()
    {
        return new Tuple<Key, ControllerType>(AvailableKeys[UnityEngine.Random.Range(0, AvailableKeys.Count)], ControllerType.KEYBOARD);
    }

    public void RegisterInput(InputType type, InputEntity input)
    {
        if (!CurrentInputs.ContainsKey(type))
            CurrentInputs.Add(type, input);
        else
            Debug.LogError("wtf");
    }
}

public interface ITreatedInput
{
    bool OnMelee();
    void OnMove(Vector2 direction);
    bool OnShoot(Vector2 direction);
}
