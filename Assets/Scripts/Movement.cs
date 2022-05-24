using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float playerSpeed = 10.0f;
    public float gravityValue = -9.81f;
    public float jumpForce;
    public int jumpLeft = 2;
    public float dashSpeed;
    public float dashTime;
    public int dashLeft = 2;
    public float playerHeight;
    public float slopeForce;

    [Header("Ledge Grab")]
    public Transform leftHand;
    public Transform rightHand;
    public Transform feet;
    public float grabRange;

    [Header("References")]
    public Transform orientation;
    public LayerMask ground;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode slowKey = KeyCode.R;

    //bools
    private bool jumping = false;
    private bool dashing = false;
    private bool grabing = false;
    private bool groundedPlayer;
    private bool searchingLedge = false;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private RaycastHit slopeHit;
    float horizontalInput;
    float verticalInput;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    private void FixedUpdate()
    {
        if (!grabing)
        {
            MovePlayer();
            GravityController();
        }
    }
    private void Update()
    {
        groundedPlayer = controller.isGrounded;
        JumpController();
        MyInput();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(jumpKey) && jumpLeft>0 && !dashing)
        {
            Jump();
        }
        if (!groundedPlayer && !searchingLedge && !grabing && !dashing)
        {
            StartCoroutine(FindLedge());
        }
        if (Input.GetKeyDown(dashKey) && dashLeft > 0)
        {
            StartCoroutine(Dash());
        }
    }
    private void MovePlayer()
    {
        Vector3 move = orientation.forward * verticalInput + orientation.right * horizontalInput;
        controller.Move(move.normalized * Time.deltaTime * playerSpeed);
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
        if ((horizontalInput != 0 || verticalInput != 0) && OnSlope())
        {
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }
    }
    private void Jump()
    {
        jumpLeft--;
        jumping = true;
        playerVelocity.y = jumpForce;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    private void GravityController()
    {
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    private void JumpController()
    {
        if (groundedPlayer)
        {
            jumpLeft = 2;
            jumping = false;
            dashLeft = 2;
        }
        else
        {
            if (jumpLeft == 2)
                jumpLeft = 1;
            if ((controller.collisionFlags & CollisionFlags.Above) != 0)
            {
                playerVelocity.y = 0f;
            }
        }
    }
    private void GrabLedge(Vector3 grabPoint)
    {
        grabing = true;
        StartCoroutine(Grab(grabPoint));
    }
    IEnumerator Grab(Vector3 grabPoint)
    {
        playerVelocity.y = 0;
        while (feet.position.y<grabPoint.y)
        {
            controller.Move(new Vector3(0, 0.01f, 0));
            yield return null;
        }
        controller.Move(orientation.forward.normalized*0.1f);
        playerVelocity.y = 0;
        grabing = false;
    }
    IEnumerator Dash()
    {
        dashing = true;
        dashLeft--;
        float startTime = Time.time;
        Vector3 move = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (move == new Vector3(0, 0, 0))
        {
            move = orientation.forward;
        }
        while (Time.time < startTime + dashTime)
        {
            controller.Move(move.normalized * dashSpeed * Time.deltaTime);
            if (OnSlope())
                controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
            yield return null;
        }
        dashing = false;
    }
    IEnumerator FindLedge()
    {
        print("In corutine");
        searchingLedge = true;
        RaycastHit leftHandHit;
        RaycastHit rightHandHit;
        while (jumping && !grabing)
        {
            if (Physics.Raycast(leftHand.position, Vector3.down, out leftHandHit, grabRange) && Physics.Raycast(rightHand.position, Vector3.down, out rightHandHit, grabRange))
            {
                Debug.Log(GameObject.ReferenceEquals(leftHandHit.transform.gameObject, rightHandHit.transform.gameObject));
                if (leftHandHit.point!=leftHand.position && rightHandHit.point != rightHand.position && GameObject.ReferenceEquals(leftHandHit.transform.gameObject, rightHandHit.transform.gameObject))
                {
                    if (leftHandHit.point.y > rightHandHit.point.y)
                        GrabLedge(leftHandHit.point);
                    else
                        GrabLedge(rightHandHit.point);
                    searchingLedge = false;
                    break;
                }
            }
            yield return null;
        }
        searchingLedge = false;
    }
    private bool OnSlope()
    {
        Physics.Raycast(transform.position, Vector3.down, out slopeHit);
        if (slopeHit.normal != new Vector3(0, 1, 0) || slopeHit.transform.tag == "Stairs")
        {
            if (!jumping)
            {
                return true;
            }
            return false;
        }
        return false;
    }
    public bool IsDashing() 
    {
        return dashing;
    }
}
