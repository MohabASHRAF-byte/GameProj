using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    public float maxSpeed;

    private int desiredLane = 1;
    public float laneDistance = 2.5f;

    public bool isGrounded;
    public LayerMask groundLayer;
    public Transform groundCheck;

    public float jumpForce;
    public float Gravity = -20;

    public Animator animator;
    private bool isSliding = false;

    private TileManager tileManager;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        tileManager = FindObjectOfType<TileManager>();
    }

    void Update()
    {
        if (!PlayerManager.isGameStarted)
            return;

        if (forwardSpeed < maxSpeed)
            forwardSpeed += 0.1f * Time.deltaTime;

        animator.SetBool("isGameStarted", true);
        direction.z = forwardSpeed;
        
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.25f, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        if (tileManager != null && tileManager.autoAssistEnabled)
        {
            AutoAssistMove();
        }
        else
        {
            if (isGrounded)
            {
                if (goUp())
                {
                    Jump();
                }

                if (goDown() && !isSliding)
                    StartCoroutine(Slide());
            }
            else
            {
                direction.y += Gravity * Time.deltaTime;
                if (goDown() && !isSliding)
                {
                    StartCoroutine(Slide());
                    direction.y = -8;
                }
            }

            if (goRight())
            {
                desiredLane++;
                if (desiredLane == 3)
                    desiredLane = 2;
            }
            if (goLeft())
            {
                desiredLane--;
                if (desiredLane == -1)
                    desiredLane = 0;
            }
        }

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (desiredLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (desiredLane == 2)
            targetPosition += Vector3.right * laneDistance;

        if (transform.position == targetPosition)
            return;

        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
        if (moveDir.sqrMagnitude < diff.magnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);
    }

    private void FixedUpdate()
    {
        if (!PlayerManager.isGameStarted)
            return;
        controller.Move(direction * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        direction.y = jumpForce;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacle")
        {
            PlayerManager.gameOver = true;
            FindObjectOfType<AudioManager>().PlaySound("GameOver");
        }
    }

    private void AutoAssistMove()
    {
        GameObject currentTile = tileManager.GetCurrentTile();
        if (currentTile == null)
            return;

        float tileZ = currentTile.transform.position.z;
        float playerZ = transform.position.z;

        if (playerZ + 5f >= tileZ && playerZ <= tileZ + tileManager.tileLength)
        {
            string tileTag = currentTile.tag;

            switch (tileTag)
            {
                case "MoveUpTile":
                    desiredLane = 1;
                    if (isGrounded)
                        Jump();
                    break;
                
                case "MoveDownTile":
                    desiredLane = 1;
                    if (isGrounded && !isSliding)
                        StartCoroutine(Slide());
                    break;
                
                case "MoveLeftTile":
                    desiredLane = 0;
                    break;
                
                case "MoveCenterTile":
                    desiredLane = 1;
                    break;

                case "MoveRightTile":
                    desiredLane = 2;
                    break;
            }
        }

        if (!isGrounded)
        {
            direction.y += Gravity * Time.deltaTime;
        }
    }

    private bool goUp()
    {
        return SwipeManager.swipeUp || Input.GetKeyDown(KeyCode.UpArrow);
    }
    private bool goDown()
    {
        return SwipeManager.swipeDown || Input.GetKeyDown(KeyCode.DownArrow);
    }
    private bool goRight()
    {
        return SwipeManager.swipeRight || Input.GetKeyDown(KeyCode.RightArrow);
    }
    private bool goLeft()
    {
        return SwipeManager.swipeLeft || Input.GetKeyDown(KeyCode.LeftArrow);
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetBool("isSliding", true);
        controller.center = new Vector3(0, -0.5f, 0);
        controller.height = 1;

        yield return new WaitForSeconds(1.3f);

        controller.center = Vector3.zero;
        controller.height = 2;
        animator.SetBool("isSliding", false);
        isSliding = false;
    }
}
