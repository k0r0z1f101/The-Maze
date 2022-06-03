using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
  private int walkSpeed;
  private Vector2 walkValue;
  private bool isWalking;
  private bool isMoving;

  //Turning
  [SerializeField]
  private int rotSpeed;

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
  private float jumpStrength = 1.0f;

  //Gravity
  [Header("Force de Gravité")]
  [SerializeField]
  private float gravity;
  private float currentFallingSpeed = 0.0f;
  private float maxFallingSpeed = 55.0f;

    // Start is called before the first frame update
    void Awake()
    {
      controller = GetComponent<CharacterController>();
      playerInput = GetComponent<PlayerInput>();
      animator = transform.GetChild(0).GetComponent<Animator>();
      walkAction = playerInput.actions["Walk"];
      runAction = playerInput.actions["Run"];
      jumpAction = playerInput.actions["Jump"];
    }

    void Update()
    {
      GetInputValues();
      SetRotation();
      if(!isJumping)
        ApplyGravity(); //apply gravity before move
      Move(); //position transformation
      SetStates();
    }

    void Move()
    {
      float walkMod = (!controller.isGrounded ? (isRunning ? jumpStrength * 2.5f : jumpStrength) : 1.0f);
      float newX = walkValue.x * walkMod;
      float newY = (isJumping && transform.position.y < jumpStrength ? 3.0f : 0.0f) - currentFallingSpeed;
      float newZ = walkValue.y * walkMod;

      controller.Move(new Vector3(newX, newY, newZ) * walkSpeed * (isRunning ? 2.5f : 1.0f) * Time.deltaTime);
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

      if(!isJumping)
      {
        jumpValue = jumpAction.ReadValue<float>();
        jumpTrigger = jumpValue == 1 ? true : false;
      }
    }

    void SetRotation()
    {
      if(isMoving)
      {
        Quaternion rot = Quaternion.LookRotation(new Vector3(walkValue.x, 0, walkValue.y));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotSpeed * Time.deltaTime * (isRunning ? 2.5f : 1.0f));
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
}
