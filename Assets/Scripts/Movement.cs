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
    public float gripForce;
    public float gripRaycastRange;
    public Transform head;
    public Transform maxPoint;
    public Transform feet;
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode slowKey = KeyCode.R;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    private bool isDashing = false;
    Vector3 moveDirection;
    Vector3 gripDirection;
    public RaycastHit headHit;
    public RaycastHit maxPointHit;
    public RaycastHit feetHit;

    Rigidbody rb;

    [HideInInspector] public bool atPulling;
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
        if(!isDashing && !atPulling)
            MovePlayer();
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
        {
            rb.drag = groundDrag;
            if (jumpLeft == 0)
                jumpLeft = maxJumps;
            dashLeft = maxDashes;
            atPulling = false;
        }
        else
        {
            if (jumpLeft == 2)
                jumpLeft = 1;
            rb.drag = 0;
        }
        if (!atPulling)
        {
            MyInput();
            SpeedControl();
        }
    }
    private void Update()
    {
        /* IF YOU NEED HERE YOU HAVE SLOWING TIME ON SLOWKEY IT IS HARD TO BALLANCE WITH DASH
        if (Input.GetKeyDown(slowKey))
        {
            Time.timeScale = slowRatio;
        }
        */
        if (!grounded)
        {
            Pull();
        }
        if (Input.GetKeyDown(dashKey) && dashLeft > 0 &&!isDashing && !atPulling)
        {
            isDashing = true;
            dashLeft--;
            StartCoroutine(Dash());
        }
        if (Input.GetKeyDown(jumpKey) && jumpLeft > 0 && !isDashing && !atPulling)
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

    private void Pull()
    {
        gripDirection = orientation.rotation * rb.transform.forward;
        if (!Physics.Raycast(head.position, gripDirection, out headHit, gripRaycastRange, whatIsGround) && Physics.Raycast(maxPoint.position, gripDirection, out maxPointHit, gripRaycastRange, whatIsGround) && !atPulling)
        {
            atPulling = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            StartCoroutine(Pulling());
        }
    }
    IEnumerator Pulling()
    {
        gripDirection = orientation.rotation * rb.transform.forward;
        gripDirection = gripDirection.normalized;
        while (Physics.Raycast(feet.position, gripDirection, out feetHit, gripRaycastRange, whatIsGround))
        {
            gameObject.transform.position += new Vector3(0, gripForce, 0);
            yield return null;
        }
        gameObject.transform.position += gripDirection * 0.7f;
        rb.useGravity = true;
        atPulling = false;
    }
    IEnumerator Dash()
    {
        bool needNormalize = true;
        float startTime = Time.time;
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (moveDirection == new Vector3(0, 0, 0))
        {
            moveDirection = orientation.rotation * rb.transform.forward;
            needNormalize = false;
        }
        while (Time.time < startTime + dashTime && !Physics.Raycast(maxPoint.position, gripDirection, out maxPointHit, gripRaycastRange, whatIsGround) && !atPulling)
        {
            if (needNormalize)
                rb.AddForce(moveDirection.normalized * dashSpeed, ForceMode.Impulse);
            else
                rb.AddForce(moveDirection * dashSpeed, ForceMode.Impulse);
            yield return null;
        }
        isDashing = false;
    }
}
