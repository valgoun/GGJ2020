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
    public float baseDurability;

    [Header("Debug")]
    public bool DontDetoriateInput;

    [Header("Inputs")]
    public List<Key> BannedKeys;

    [NonSerialized] public List<Key> AvailableKeys = new List<Key>();
    [NonSerialized] public ITreatedInput RegisteredPlayer;
    [NonSerialized] public Dictionary<InputType, InputEntity> CurrentInputs = new Dictionary<InputType, InputEntity>();

    bool _inputMenu;
    
    void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        AvailableKeys = Master.Instance.CurrentInputs;
        if (AvailableKeys.Count == 0)
            foreach (Key key in Enum.GetValues(typeof(Key)))
                if (!BannedKeys.Contains(key))
                    AvailableKeys.Add(key);

        foreach (InputType key in StartInputs.Keys)
        {
            if (Master.Instance.EasyStart == 0 || (Master.Instance.EasyStart == 1
                && (key == InputType.MOVE_FORWARD || key == InputType.MOVE_BACKWARD || key == InputType.MOVE_LEFT || key == InputType.MOVE_RIGHT)))
                RegisterInput(key, new InputEntity(StartInputs[key], baseDurability));
            else
                RegisterInput(key, new InputEntity(GetRandomInput().Item1, baseDurability));
        }
    }

    void Update()
    {
        if (Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            _inputMenu = !_inputMenu;
            Time.timeScale = (_inputMenu) ? 0 : 1;

            if (_inputMenu)
                InputUI.Instance.Open();
            else
                InputUI.Instance.Close();
        }

        if (!_inputMenu)
        {
            foreach (InputEntity input in CurrentInputs.Values)
                input.PollInput();

            ComputeMelee();
            ComputeMovement();
            ComputeShoot();
        }
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
                    if (CurrentInputs[InputType.SHOOT_FORWARD].ReduceDurability())
                        CurrentInputs.Remove(InputType.SHOOT_FORWARD);
                }
                else if (vertical == -1)
                {
                    if (CurrentInputs[InputType.SHOOT_BACKWARD].ReduceDurability())
                        CurrentInputs.Remove(InputType.SHOOT_BACKWARD);
                }
                else if (horizontal == -1)
                {
                    if (CurrentInputs[InputType.SHOOT_LEFT].ReduceDurability())
                        CurrentInputs.Remove(InputType.SHOOT_LEFT);
                }
                else if (horizontal == 1)
                {
                    if (CurrentInputs[InputType.SHOOT_RIGHT].ReduceDurability())
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
        if (CurrentInputs.ContainsKey(type))
            CurrentInputs.Remove(type);
        if (!input.IsEmpty)
            CurrentInputs.Add(type, input);

        input.MyLocation = InputLocationType.ASSIGNED;
        input.MyAssignedInput = type;
    }
}

public interface ITreatedInput
{
    bool OnMelee();
    void OnMove(Vector2 direction);
    bool OnShoot(Vector2 direction);
}
