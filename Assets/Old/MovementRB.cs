using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementRB : MonoBehaviour
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

    [Header("Grip Edge")]
    public float gripForce;
    public float gripRaycastRange;
    public Transform head;
    public Transform maxPoint;
    public Transform minPoint;
    public Transform feet;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode slowKey = KeyCode.R;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;
    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    private bool isDashing = false;
    Vector3 moveDirection;
    Vector3 gripDirection;
    public RaycastHit headHit;
    public RaycastHit maxPointHit;
    public RaycastHit minPointHit;
    public RaycastHit feetHit;
    public RaycastHit groundHit;
    Rigidbody rb;
    public MovementState state;
    public enum MovementState
    {
        runninig,
        jumping,
        slope,
        pulling,
        dashing
    }
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
        if (!isDashing && !atPulling)
            MovePlayer();
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
        {
            exitingSlope = false;
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
        /*
        if(!exitingSlope)
            handleSlope();
        */
        /* IF YOU NEED HERE YOU HAVE SLOWING TIME ON SLOWKEY IT IS HARD TO BALLANCE WITH DASH
        if (Input.GetKeyDown(slowKey))
        {
            Time.timeScale = slowRatio;
        }
        Debug.DrawRay(head.position, gripDirection * gripRaycastRange, Color.yellow);
        Debug.DrawRay(maxPoint.position, gripDirection * gripRaycastRange, Color.red);
        Debug.DrawRay(minPoint.position, gripDirection * gripRaycastRange, Color.black);
        Debug.DrawRay(feet.position, gripDirection * gripRaycastRange, Color.green);
        Debug.DrawRay(maxPoint.position + gripDirection * gripRaycastRange, minPoint.position - maxPoint.position, Color.blue);
        */

        if (!grounded && Input.GetKey(KeyCode.W) && !isDashing)
        {
            //Pull();
        }
        if (Input.GetKeyDown(dashKey) && dashLeft > 0 && !isDashing && !atPulling)
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
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;

        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    private void MovePlayer()
    {
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (grounded)
            rb.AddForce(GetCurrentMoveDirection().normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(GetCurrentMoveDirection().normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        rb.useGravity = !OnSlope();
    }
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            if(angle < maxSlopeAngle)
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }    
    private bool WillBeOnSlope()
    {
        if (Physics.Raycast(transform.position + GetCurrentMoveDirection(), Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(GetCurrentMoveDirection(), slopeHit.normal).normalized;
    }

    private Vector3 GetCurrentMoveDirection()
    {
        return orientation.forward * verticalInput + orientation.right * horizontalInput;
    }
    private void Jump()
    {
        exitingSlope = true;
        jumpLeft--;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    private void Pull()
    {
        gripDirection = orientation.rotation * rb.transform.forward;
        if (!Physics.Raycast(head.position, gripDirection, out headHit, gripRaycastRange, whatIsGround) && Physics.Raycast(maxPoint.position + gripDirection * gripRaycastRange, (minPoint.position - maxPoint.position).normalized, out feetHit, maxPoint.position.y - minPoint.position.y, whatIsGround) && !atPulling)
        {
            atPulling = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
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
        rb.isKinematic = false;
    }

    IEnumerator Dash()
    {
        bool needNormalize = true;
        float startTime = Time.time;
        moveDirection = GetCurrentMoveDirection();
        if (moveDirection == new Vector3(0, 0, 0))
        {
            moveDirection = orientation.rotation * rb.transform.forward;
            needNormalize = false;
        }
        while (Time.time < startTime + dashTime && !Physics.Raycast(maxPoint.position, gripDirection, out maxPointHit, gripRaycastRange, whatIsGround) && !atPulling)
        {
            if (needNormalize && !rb.isKinematic)
                rb.AddForce(moveDirection.normalized * dashSpeed, ForceMode.Impulse);
            else if(!rb.isKinematic)
                rb.AddForce(moveDirection * dashSpeed, ForceMode.Impulse);
            yield return null;
        }
        isDashing = false;
    }

    void handleSlope()
    {
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        Physics.Raycast(transform.position, Vector3.down, out slopeHit);
        if(slopeHit.normal!= new Vector3(0,1,0) || slopeHit.transform.tag == "Slope")
        {
            Debug.Log("HEEJ");
            Vector3 tmp = rb.velocity;
            tmp.y = 0;
            rb.velocity = tmp;
        }
    }
}
