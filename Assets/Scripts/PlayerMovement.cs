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
        float newLayerWeight = Mathf.Lerp(currentLayerweight, targetLayerWeight, Time.deltaTime * 5f);
        animator.SetLayerWeight(0, newLayerWeight);

        HandleMovement();

        ApplyGravity();

        characterController.Move(velocity * Time.deltaTime);
    }
    private void HandleMovement()
    {

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        if(isMoving == false && moveInput!= Vector2.zero)
        {
            isMoving = true;
            animator.CrossFadeInFixedTime("Walking", 0.1f,0);
        }
        else if(isMoving == true && moveInput.magnitude <= 0.1f)
        {
            isMoving = false;
            animator.CrossFadeInFixedTime("Idle", 0.1f,0);
        }

        bool isSprinting = sprintAction.IsPressed();

        // Convert 2D input to 3D movement

        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        // Apply sprint multiplier

        float currentSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);

        // Update horizontal velocity

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
        if (jumpAction.IsPressed() && characterController.isGrounded)
        {
            isJumping = true;
            animator.CrossFadeInFixedTime("Jumping", 0.1f,0);
        }
        else
        {
          isJumping = false;
           animator.CrossFadeInFixedTime("Idle", 0.1f,0);
        }


        if (characterController.isGrounded)

        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

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
