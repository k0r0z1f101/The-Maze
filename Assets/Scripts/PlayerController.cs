using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// using Player.Inventory;

public class PlayerController : MonoBehaviour
{
  //character parent gameobject
  private CharacterController controller;

  //inputs for character movements
  private PlayerInput playerInput;
  private InputAction walkAction;

  //animations for character movements
  private Animator animator;

  //Walk
  [Header("Vitesse de marche")]
  [SerializeField]
  private int walkSpeed = 2;
  private Vector2 walkValue;
  private bool isWalking;
  private bool isMoving;

  //Turning
  [Header("Vitesse de rotation")]
  [SerializeField]
  private int rotSpeed = 45;
  private Vector2 turnValue;
  private InputAction turnAction;

  //Run
  private InputAction runAction;
  private float runValue;
  private bool isRunning;

  //Jump
  private InputAction jumpAction;
  private float jumpValue;
  private bool jumpTrigger;
  private bool isJumping;
  [Header("Force du saut")]
  [SerializeField]
  private float jumpStrength = 2.0f;

  //Gravity
  [Header("Force de Gravité")]
  [SerializeField]
  private float gravity = 9.8f;
  private float currentFallingSpeed = 0.0f;
  private float maxFallingSpeed = 55.0f;

  private Player.Inventory _inventory;

    // Start is called before the first frame update
    void Awake()
    {
      controller = GetComponent<CharacterController>();
      animator = transform.GetChild(0).GetComponent<Animator>();
      CameraController cam = GameObject.Find("CameraContainer").GetComponent<CameraController>();
      cam.SetPlayer(gameObject);

      playerInput = GetComponent<PlayerInput>();
      walkAction = playerInput.actions["Walk"];
      turnAction = playerInput.actions["Turn"];
      runAction = playerInput.actions["Run"];
      jumpAction = playerInput.actions["Jump"];
      playerInput.actions.Enable();
    }

    void Update()
    {
      GetInputValues();
      if(!isJumping)
        ApplyGravity(); //apply gravity before move
      Move(); //position transformation
      SetStates();
    }

    void Move()
    {
      float walkMod = (!controller.isGrounded ? (isRunning ? jumpStrength * 2.5f : jumpStrength) : 1.0f);

      float newY = (isJumping && transform.position.y < jumpStrength ? 3.0f : 0.0f) - currentFallingSpeed;

      controller.Move(new Vector3(0, newY, 0) * walkSpeed * (isRunning ? 2.5f : 1.0f) * Time.deltaTime);
      transform.Translate(walkValue.y * transform.forward * walkSpeed * (isRunning ? 2.5f : 1.0f) * Time.deltaTime, Space.World);
      transform.Rotate(0, turnValue.x * rotSpeed * (isRunning ? 2.5f : 1.0f) * Time.deltaTime, 0, Space.World);
    }

    void ApplyGravity()
    {
      currentFallingSpeed += gravity * Time.deltaTime * 0.5f;
      currentFallingSpeed = currentFallingSpeed > maxFallingSpeed ? maxFallingSpeed : currentFallingSpeed;
    }

    void SetStates()
    {
      if(isMoving)
        animator.SetBool("IsWalking", true);
      else
        animator.SetBool("IsWalking", false);

      if(isRunning)
        animator.SetBool("IsRunning", true);
      else
        animator.SetBool("IsRunning", false);

      if(isJumping && controller.isGrounded)
      {
        animator.SetBool("IsJumpingWalking", false);
        animator.SetBool("IsJumpingRunning", false);
      }

      if(controller.isGrounded)
        currentFallingSpeed = 0.0f;

      if(isJumping && transform.position.y > jumpStrength)
        isJumping = false;

      if(jumpTrigger && isMoving)
        Jump(); //trigger animation and jump
    }

    void GetInputValues()
    {
      walkValue = walkAction.ReadValue<Vector2>();
      isMoving = walkValue.magnitude > 0 ? true : false;

      runValue = runAction.ReadValue<float>();
      isRunning = runValue == 1 ? true : false;

      turnValue = turnAction.ReadValue<Vector2>();

      if(!isJumping)
      {
        jumpValue = jumpAction.ReadValue<float>();
        jumpTrigger = jumpValue == 1 ? true : false;
      }
    }

    void Jump()
    {
      isJumping = true;
      jumpTrigger = false;

      if(isRunning)
        animator.SetTrigger("jumpRunningTrigger");
      else
        animator.SetTrigger("jumpTrigger");
    }

    private void OnTriggerEnter(Collider other)
    {
      if(other.tag == "Coin")
      {
        Destroy(other.gameObject);
        ++_inventory.coins;
        Debug.Log(_inventory.coins);
      }
    }
}
