using UnityEngine.InputSystem;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private bool isMoving;
    private bool isSprinting;
    private bool isJumping;
    private bool isJogging;
    private float footstepTimer;
    private float groundedTimer;

    [Header("Audio")]
    [SerializeField] private float footstepInterval = 0.5f;
    [SerializeField] private float sprintFootstepInterval = 0.3f;

    [Header("Movement")]

    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float sprintMultiplier = 1.5f;
    [Header("References")]

    [SerializeField] private CharacterController characterController;

    [SerializeField] private InputActionAsset inputActions;

    [SerializeField] private Camera playerCamera;

    private InputActionMap playerActionMap;

    private InputAction moveAction;

    private InputAction sprintAction;

    private InputAction jumpAction;

    private InputAction crouchAction;

    private InputAction attackAction;

    private Vector3 velocity;

    private float gravity = -9.81f;

    [SerializeField] private float jumpForce = 5f;

    [SerializeField] private float crouchHeight = 0.6f;

    private float normalHeight = 2f;

    private void Awake()

    {

        animator = GetComponent<Animator>();

        // Get character controller if not assigned

        if (characterController == null)

            characterController = GetComponent<CharacterController>();

        // Get camera if not assigned

        if (playerCamera == null)

            playerCamera = GetComponentInChildren<Camera>();

        // Setup input system

        if (inputActions == null)

            inputActions = Resources.Load<InputActionAsset>("InputSystem_Actions");

        playerActionMap = inputActions.FindActionMap("Player");

        moveAction = playerActionMap.FindAction("Move");

        sprintAction = playerActionMap.FindAction("Sprint");

        jumpAction = playerActionMap.FindAction("Jump");

        crouchAction = playerActionMap.FindAction("Crouch");

        attackAction = playerActionMap.FindAction("Attack");

        // Store normal height

        normalHeight = characterController.height;

    }

    private void OnEnable()

    {

        playerActionMap.Enable();

        // Subscribe to input actions

        jumpAction.performed += OnJump;

        crouchAction.performed += OnCrouch;

        crouchAction.canceled += OnStopCrouch;

        attackAction.performed += OnAttack;

    }

    private void OnDisable()

    {

        playerActionMap.Disable();

        jumpAction.performed -= OnJump;

        crouchAction.performed -= OnCrouch;

        crouchAction.canceled -= OnStopCrouch;

        attackAction.performed -= OnAttack;

    }

    private void Update()
    {
        float currentLayerweight = animator.GetLayerWeight(0);
        float targetLayerWeight;

        if (isJumping)
        {
            targetLayerWeight = 1f;
        }
        else if (isSprinting)
        {
            targetLayerWeight = 0.75f;
        }
        else if (isJogging)
        {
            targetLayerWeight = 0.6f;
        }
        else if (isMoving)
        {
            targetLayerWeight = 0.5f;
        }
        else
        {
            targetLayerWeight = 0f;
        }

        float newLayerWeight = Mathf.Lerp(
            currentLayerweight,
            targetLayerWeight,
            Time.deltaTime * 5f);

        animator.SetLayerWeight(0, newLayerWeight);

        HandleMovement();
        HandleFootsteps();
        ApplyGravity();

        characterController.Move(velocity * Time.deltaTime);

        // Reset jump state when grounded again
        if (characterController.isGrounded && isJumping && velocity.y <= 0)
        {
            isJumping = false;
        }
        if (characterController.isGrounded)
        {
            groundedTimer = 0.15f;
        }
        else
        {
            groundedTimer -= Time.deltaTime;
        }
    }
    private void HandleMovement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        isMoving = moveInput.magnitude > 0.1f;

        isSprinting = sprintAction.IsPressed();

        if (isMoving)
        {
            if (isSprinting)
            {
                animator.CrossFadeInFixedTime("Walking", 0.1f, 0);
            }
            else
            {
                animator.CrossFadeInFixedTime("Walking", 0.1f, 0);
            }
        }
        else
        {
            animator.CrossFadeInFixedTime("Idle", 0.1f, 0);
        }

        Vector3 moveDirection =
            transform.forward * moveInput.y +
            transform.right * moveInput.x;

        float currentSpeed =
            moveSpeed * (isSprinting ? sprintMultiplier : 1f);

        velocity.x = moveDirection.x * currentSpeed;
        velocity.z = moveDirection.z * currentSpeed;
    }

    private void ApplyGravity()

    {

        if (characterController.isGrounded && velocity.y < 0)

        {

            velocity.y = -2f; // Small negative value to keep grounded

        }

        else

        {

            velocity.y += gravity * Time.deltaTime;

        }

    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (groundedTimer <= 0f)
            return;

        isJumping = true;

        animator.CrossFadeInFixedTime("Jumping", 0.1f, 0);

        SFXManager.Instance.PlaySound("Jump");

        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

        groundedTimer = 0f;
    }

    private void HandleFootsteps()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        bool isWalking =
            groundedTimer > 0f &&
            moveInput.magnitude > 0.1f;

        if (isWalking)
        {
            float currentInterval =
                isSprinting ? sprintFootstepInterval : footstepInterval;

            footstepTimer += Time.deltaTime;

            if (footstepTimer >= currentInterval)
            {
                SFXManager.Instance.PlaySound("Footstep");
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void OnCrouch(InputAction.CallbackContext context)

    {

        characterController.height = crouchHeight;

    }

    private void OnStopCrouch(InputAction.CallbackContext context)

    {

        characterController.height = normalHeight;

    }
private void OnAttack(InputAction.CallbackContext context)

    {

        

    }
}
