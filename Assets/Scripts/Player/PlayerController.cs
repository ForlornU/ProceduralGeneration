using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Movement
{
    public PlayerInputActions playerControls;

    private void OnEnable()
    {
        playerControls = new PlayerInputActions();
        playerControls.Enable();

        playerControls.pActionMap.Jump.performed += InputJump;
        //_playerControls.PlayerInputActions.Dash.performed += InputDash;
        playerControls.pActionMap.Crouch.performed += InputCrouch;
    }

    Vector2 InputMovement()
    {
        return playerControls.pActionMap.Move.ReadValue<Vector2>();
    }

    private void InputJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    private void InputDash(InputAction.CallbackContext context)
    {
        StartCoroutine(Dash());
    }

    private void InputCrouch(InputAction.CallbackContext context)
    {
        Crouch();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        UpdateMovement(InputMovement());
    }
    
}
