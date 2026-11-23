using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    public InputAction _moveAction;
    public InputAction _lookAction;
    public InputAction _jumpAction;
    public InputAction _dashAction;
    public InputAction _attackAction;
    public InputAction _specialAction;
    public InputAction _nextAction;
    public InputAction _prevAction;


    public Vector2 Move { private set; get; }
    public Vector2 Look { private set; get; }
    public bool Jump { private set; get; }
    public bool JumpEnd { private set; get; }
    public bool JumpStart { private set; get; }
    public bool Dash { private set; get; }
    public bool Attack { private set; get; }
    public bool Special { private set; get; }
    public bool Interact { private set; get; }
    public bool Next { private set; get; }
    public bool Prev { private set; get; }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = InputSystem.actions["Move"];
        _lookAction = InputSystem.actions["Look"];
        _dashAction = InputSystem.actions["Dash"];
        _jumpAction = InputSystem.actions["Jump"];
        _attackAction = InputSystem.actions["Attack"];
        _specialAction = InputSystem.actions["Special"];
        _nextAction = InputSystem.actions["Next"];
        _prevAction = InputSystem.actions["Previous"];
    }

    // Update is called once per frame
    void Update()
    {
        Move = _moveAction.ReadValue<Vector2>();
        Look = _lookAction.ReadValue<Vector2>();
        JumpStart = _jumpAction.WasPressedThisFrame();
        JumpEnd = _jumpAction.WasReleasedThisFrame();
        Jump = _jumpAction.WasPerformedThisFrame();
        Dash = _dashAction.WasPressedThisFrame();
        Attack = _attackAction.WasPressedThisFrame();
        Special = _specialAction.WasPressedThisFrame();
        Interact = _nextAction.WasPressedThisFrame();
        Next = _nextAction.WasPressedThisFrame();
        Prev = _prevAction.WasPressedThisFrame();
    }
}
