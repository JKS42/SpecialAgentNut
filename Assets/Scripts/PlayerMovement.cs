using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private bool isMoving;
    private bool isSprinting;
    private bool isJumping;
    private float footstepTimer;
    private float groundedTimer;
    private float nextShootTime;

    public float currentSpeed;

    [Header("Audio")]
    [SerializeField] private float footstepInterval = 0.5f;
    [SerializeField] private float sprintFootstepInterval = 0.3f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 30f;
    [SerializeField] private int projectileDamage = 20;
    [SerializeField] private float shootCooldown = 0.25f;
    [SerializeField] private float projectileSpawnOffset = 1f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float crouchHeight = 0.6f;

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
    private readonly float gravity = -9.81f;
    private float normalHeight = 2f;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (inputActions == null)
            inputActions = Resources.Load<InputActionAsset>("InputSystem_Actions");

        if (inputActions == null)
        {
            Debug.LogError("PlayerMovement needs an InputActionAsset assigned.");
            enabled = false;
            return;
        }

        playerActionMap = inputActions.FindActionMap("Player", true);
        moveAction = playerActionMap.FindAction("Move", true);
        sprintAction = playerActionMap.FindAction("Sprint", true);
        jumpAction = playerActionMap.FindAction("Jump", true);
        crouchAction = playerActionMap.FindAction("Crouch", true);
        attackAction = playerActionMap.FindAction("Attack", true);

        if (characterController != null)
            normalHeight = characterController.height;
    }

    private void OnEnable()
    {
        if (playerActionMap == null)
            return;

        playerActionMap.Enable();
        jumpAction.performed += OnJump;
        crouchAction.performed += OnCrouch;
        crouchAction.canceled += OnStopCrouch;
        attackAction.performed += OnAttack;
    }

    private void OnDisable()
    {
        if (playerActionMap == null)
            return;

        jumpAction.performed -= OnJump;
        crouchAction.performed -= OnCrouch;
        crouchAction.canceled -= OnStopCrouch;
        attackAction.performed -= OnAttack;
        playerActionMap.Disable();
    }

    private void Update()
    {
        if (characterController == null)
            return;

        if (animator != null)
            animator.SetLayerWeight(0, 1f);

        HandleMovement();

        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
            animator.SetFloat("currentSpeed", currentSpeed);
        }

        HandleFootsteps();
        ApplyGravity();
        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded && isJumping && velocity.y <= 0)
            isJumping = false;

        groundedTimer = characterController.isGrounded
            ? 0.15f
            : groundedTimer - Time.deltaTime;
    }

    private void HandleMovement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        isMoving = moveInput.magnitude > 0.1f;
        isSprinting = sprintAction.IsPressed();

        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        currentSpeed = isMoving ? moveSpeed * (isSprinting ? sprintMultiplier : 1f) : 0f;

        velocity.x = moveDirection.x * currentSpeed;
        velocity.z = moveDirection.z * currentSpeed;
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
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

        if (animator != null)
            animator.SetTrigger("Jumping");

        if (SFXManager.Instance != null)
            SFXManager.Instance.PlaySound("Jump");

        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        groundedTimer = 0f;
    }

    private void HandleFootsteps()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        bool isWalking = groundedTimer > 0f && moveInput.magnitude > 0.1f;

        if (!isWalking)
        {
            footstepTimer = 0f;
            return;
        }

        float currentInterval = isSprinting ? sprintFootstepInterval : footstepInterval;
        footstepTimer += Time.deltaTime;

        if (footstepTimer < currentInterval)
            return;

        if (SFXManager.Instance != null)
            SFXManager.Instance.PlaySound("Footstep");

        footstepTimer = 0f;
    }

    private void OnCrouch(InputAction.CallbackContext context)
    {
        if (characterController != null)
            characterController.height = crouchHeight;
    }

    private void OnStopCrouch(InputAction.CallbackContext context)
    {
        if (characterController != null)
            characterController.height = normalHeight;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        ShootProjectile();
    }

    private void ShootProjectile()
    {
        if (Time.time < nextShootTime)
            return;

        if (projectilePrefab == null)
        {
            Debug.LogWarning("PlayerMovement needs a projectile prefab assigned before the player can shoot.");
            return;
        }

        Transform spawnTransform = firePoint != null
            ? firePoint
            : playerCamera != null
                ? playerCamera.transform
                : transform;

        Vector3 shootDirection = spawnTransform.forward.normalized;
        Vector3 spawnPosition = spawnTransform.position + shootDirection * projectileSpawnOffset;
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(shootDirection));

        bullet projectileDamageScript = projectile.GetComponent<bullet>();
        if (projectileDamageScript != null)
            projectileDamageScript.ConfigureForEnemies(projectileDamage);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            projectileRb.useGravity = false;
            projectileRb.linearVelocity = shootDirection * projectileSpeed;
        }

        nextShootTime = Time.time + shootCooldown;
    }
}
