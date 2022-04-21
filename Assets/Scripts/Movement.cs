using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float dashSpeed;

    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float maxJumps;
    private float jumpLeft;    
    public float maxDashes;
    private float dashLeft;
    public float dashTime;
    public float slowRatio;
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode slowKey = KeyCode.R;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    private bool isDashing = false;
    Vector3 moveDirection;

    Rigidbody rb;
    public bool DashingState()
    {
        return isDashing;
    }
    private void Start()
    {
        dashLeft = maxDashes;
        jumpLeft = maxJumps;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    private void FixedUpdate()
    {
        if(!isDashing)
            MovePlayer();
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
        {
            rb.drag = groundDrag;
            if (jumpLeft == 0)
                jumpLeft = maxJumps;
            dashLeft = maxDashes;
        }
        else
        {
            if (jumpLeft == 2)
                jumpLeft = 1;
            rb.drag = 0;
        }
        MyInput();
        SpeedControl();
    }
    private void Update()
    {
        /* IF YOU NEED HERE YOU HAVE SLOWING TIME ON SLOWKEY IT IS HARD TO BALLANCE WITH DASH
        if (Input.GetKeyDown(slowKey))
        {
            Time.timeScale = slowRatio;
        }
        */
        if (Input.GetKeyDown(dashKey) && dashLeft > 0 &&!isDashing)
        {
            isDashing = true;
            dashLeft--;
            StartCoroutine(Dash());
        }
        if (Input.GetKeyDown(jumpKey) && jumpLeft > 0 && !isDashing)
        {
            Jump();
        }
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    private void MovePlayer()
    { 
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }
    private void Jump()
    {
        jumpLeft--;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }
    IEnumerator Dash()
    {
        bool needNormalize = true;
        float startTime = Time.time;
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if(moveDirection == new Vector3(0, 0, 0))
        {
            moveDirection = orientation.rotation * rb.transform.forward;
            needNormalize = false;
        }
        while (Time.time < startTime + dashTime)
        {
            if(needNormalize)
                rb.AddForce(moveDirection.normalized * dashSpeed, ForceMode.Impulse);
            else
                rb.AddForce(moveDirection * dashSpeed, ForceMode.Impulse);
            yield return null;
        }
        isDashing = false;
    }
}
