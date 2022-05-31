using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour, Input.IPlayerActions, IDamageable
{
    private readonly int IsWalkingHash = Animator.StringToHash("varIsWalking");    

    // Input Events
    public event Action FireEvent;
    public event Action JumpEvent;
    public event Action MenuEvent;

    [Header("Stats")]
    public float maxHealth = 100;
    public float currHealth;   

    [Header("Movement")]
    public float playerSpeed = 2.0f;
    public float gravityMultiplier = 3.0f;
    public float jumpHeight = 3.0f;

    [Header("Look")]
    public float lookSensitivity = 100f;
    public float minXLook = -90f;
    public float maxXLook = 90f;

    [Header("Input Values")]
    public bool isAiming;
    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool hasJumped;
    public Vector3 moveDirection = Vector3.zero;
    public float xRotation = 0f;

    [Header("UI")]
    public Canvas menuCanvas;
    public GameObject inGameMenu;
    public GameObject deathMenu;
    public TextMeshProUGUI healthText;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip playerDeath;

    private Input input;
    public CharacterController controller;
    public Camera mainCamera;
    public Bow bow;

    private void Start()
    {
        input = new Input();
        input.Player.SetCallbacks(this);
        input.Player.Enable();

        JumpEvent += Jump;
        FireEvent += Fire;
        MenuEvent += EnableDisableMenu;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currHealth = maxHealth;
    }

    private void Update()
    {
        if (currHealth <= 0) return;
        
        Move();
        Look();

        UpdateHealthText();
    }

    private void Look()
    {
        if (lookInput != Vector2.zero)
        {
            // Rotate camera on X-axis
            xRotation += -lookInput.y * lookSensitivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, minXLook, maxXLook);
            mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            // Rotate player
            transform.Rotate(Vector3.up * lookInput.x * lookSensitivity * Time.deltaTime);
        }
    }

    private void Move()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        //if (moveInput != Vector2.zero)
        //    bowAnimator.SetBool(IsWalkingHash, true);
        //else
        //    bowAnimator.SetBool(IsWalkingHash, false);

        // Determine speeds and direction
        float curSpeedX = moveInput.y * playerSpeed;
        float curSpeedY = moveInput.x * playerSpeed;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Handle jumping
        if (hasJumped)
        {
            moveDirection.y = jumpHeight;
            hasJumped = false;
        }
        else
            moveDirection.y = movementDirectionY;

        // Apply gravity
        if (!controller.isGrounded)
            moveDirection.y -= gravityMultiplier * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);
    }

    private void UpdateHealthText()
    {
        healthText.text = $"Health: {currHealth} / {maxHealth}";
    }

    private void OnDestroy()
    {
        JumpEvent -= Jump;
        FireEvent -= Fire;
        MenuEvent -= EnableDisableMenu;

        input.Player.Disable();
    }

    private void Jump()
    {
        if (controller.isGrounded)
        {
            hasJumped = true;
        }
    }

    private void Fire()
    {   
        if (bow is object && bow.enabled)
            bow.Fire();
    }

    public void TakeDamage(float damage)
    {
        currHealth = Mathf.Clamp(currHealth - damage, 0, maxHealth);
        if (currHealth <= 0)
            Die();
    }

    public void Die()
    {
        menuCanvas.gameObject.SetActive(true);
        inGameMenu.SetActive(false);
        deathMenu.SetActive(true);
    }

    private void EnableDisableMenu()
    {
        menuCanvas.gameObject.SetActive(!menuCanvas.gameObject.activeSelf);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnAimFire(InputAction.CallbackContext context)
    {
        if (context.performed)
            bow.isDrawing = true;
        else if (context.canceled)
        {
            FireEvent?.Invoke();
        }        
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        JumpEvent?.Invoke();
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        MenuEvent?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.name);
    }
}
