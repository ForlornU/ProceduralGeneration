using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Movement : MonoBehaviour
{
    #region variables

    [SerializeField]
    Transform _groundCollider;
    public LayerMask groundLayer;
    private bool _grounded;

    [SerializeField]
    protected MoveSettings movesettings;

    protected CharacterController controller;

    bool _crouching = false;

    protected Vector3 moveDirection;
    protected Vector3 dash = Vector3.zero;

    protected int _airJumpCount = 0;

    #endregion

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    protected void UpdateMovement(Vector2 input)
    {
        moveDirection.x = input.x * movesettings.movespeed;
        moveDirection.z = input.y * movesettings.movespeed;

        moveDirection = transform.TransformDirection(moveDirection);
    }

    protected void Jump()
    {
        if (!_grounded)
        {
            if (_airJumpCount >= movesettings.airJumps)
                return;

            _airJumpCount++;
        }

        moveDirection.y = 0f;
        moveDirection += Vector3.up * movesettings.jumpforce;
    }

    protected void Crouch()
    {
        _crouching = !_crouching;

        if (_crouching)
            controller.height = 0.5f;
        else
            controller.height = 2f;
    }

    protected IEnumerator Dash()
    {

        float starttime = 0f;
        float endtime = starttime + movesettings.dashtime;

        moveDirection.y = 0f;
        dash = moveDirection * movesettings.dashforce;

        while (starttime < endtime)
        {
            moveDirection.y = 0f;
            starttime += Time.deltaTime;
            yield return null;
        }
    }

    protected virtual void LateUpdate()
    {
        isGrounded();
        Gravity();
        ReduceMomentum();
    }

    private void Gravity()
    {
        moveDirection.y -= (Physics.gravity.magnitude + 6f) * Time.deltaTime;

        if (_grounded)
        {
            _airJumpCount = 0;
            moveDirection.y = Mathf.Clamp(moveDirection.y, -10f, 999f);
        }
    }

    private void ReduceMomentum()
    {
        if (Mathf.Abs(dash.magnitude) < 0.3f)
        {
            dash = Vector3.zero;
            return;
        }

        dash = Vector3.MoveTowards(dash, Vector3.zero, dash.magnitude / 10f);
    }

    protected void FixedUpdate()
    {
        controller.Move((moveDirection + dash) * Time.deltaTime);
    }

    void isGrounded()
    {
        if (Physics.CheckSphere(_groundCollider.position, movesettings.groundColliderSize, groundLayer))
        {
            _grounded = true;
            return;
        }

        _grounded = false;
    }
}
