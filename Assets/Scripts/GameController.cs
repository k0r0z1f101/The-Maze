using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Reflection;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    //list of converted pictures
    private List<Vector2> convertedPix;

    [Header("height of the walls")]
    [SerializeField]
    private float wallsHeight;

    //the image to be converted
    private Texture2D levelImg;

    [Header("images to convert to levels")]
    [SerializeField]
    private List<Texture2D> lvlImages;

    [Header("level to load (if too high load last level)")]
    [SerializeField]
    private int levelToLoad = 0;

    private GameObject ground;

    //Color mode, will copy the pictures each pixel to wall colors
    [Header("colored walls if true")]
    [SerializeField]
    private bool colorMode = false;

    //hero Position
    private Vector2 pos;

    private CameraController cam;

    void ConvPixToVec()
    {
        convertedPix = new List<Vector2>();
        levelImg = new Texture2D(0, 0);
        levelImg = lvlImages[levelToLoad];
        int nbrOfRows = levelImg.width;
        int nbrOfCols = levelImg.height;

        Color whiteColor = new Color(1.0f,1.0f,1.0f,1.0f);
        Color blackColor = new Color(0.0f,0.0f,0.0f,1.0f);
        Color pinkColor = new Color(1.0f,0.000f,1.0f,1.0f);
        for(int i = 0; i < nbrOfRows; ++i)
          for(int j = 0; j < nbrOfCols; ++j)
          {
              Color pixColor = levelImg.GetPixel(i, j);

              //walls array
              if(pixColor == blackColor)
                convertedPix.Add(new Vector2(i, j));

              //hero position
              if(pixColor == pinkColor)
              {
                pos.x = i;
                pos.y = j;
              }

          }

    }

    void Awake()
    {
        ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.GetComponent<Renderer>().material.color = Color.red;
        cam = GameObject.Find("CameraContainer").GetComponent<CameraController>();
    }

    void OnValidate()
    {
        if(ground){
            wallsHeight = Mathf.Clamp(wallsHeight,0,10);

            if(!colorMode)
              ConvPixToVec();

            Destroy(ground);
            ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.GetComponent<Renderer>().material.color = Color.red;

            DrawLevel();
            PositionHero();
        }
    }

    void  DrawLevel()
    {
        ground.transform.localScale = new Vector3(levelImg.width * 0.1f, 0.01f, levelImg.height * 0.1f);
        ground.transform.position = new Vector3(levelImg.width * 0.5f, 0.0f, levelImg.height * 0.5f);
        if(!colorMode)
          for(int i = 0; i < convertedPix.Count; ++i)
          {
              GameObject newBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
              newBlock.transform.localScale = new Vector3(1.0f, wallsHeight, 1.0f);
              newBlock.transform.position = new Vector3(convertedPix[i].x, (wallsHeight * 0.5f) + 0.01f, convertedPix[i].y);
              newBlock.transform.SetParent(ground.transform);
          }
        else
        {
            levelImg = new Texture2D(0, 0);
            levelImg = lvlImages[levelToLoad];
            int nbrOfRows = levelImg.width;
            int nbrOfCols = levelImg.height;

            for(int i = 0; i < nbrOfRows; ++i)
              for(int j = 0; j < nbrOfCols; ++j)
              {
                  Color pixColor = levelImg.GetPixel(i, j);
                  GameObject newBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                  newBlock.transform.localScale = new Vector3(1.0f, wallsHeight, 1.0f);
                  newBlock.transform.position = new Vector3(i, (wallsHeight * 0.5f) + 0.01f, j);
                  newBlock.GetComponent<Renderer>().material.color = pixColor;
                  newBlock.transform.SetParent(ground.transform);
              }
        }
    }

    void PositionHero()
    {
      //DESTROY ANY OTHER HERO
      cam.ResetPlayer();
      Destroy(GameObject.Find("CharacterController"));

      //CREATE GAMEOBJECT TO ADD ROBOT KYLE TO
      GameObject characterController = new GameObject("CharacterController");

      //set hero position
      characterController.transform.position = new Vector3(pos.x, 0.0f, pos.y);

      //skin width : 0.0001, center: 0.87. radius: 0.29, height: 1.81
      CharacterController controllerComp = characterController.AddComponent<CharacterController>();
      controllerComp.skinWidth = 0.0001f;
      controllerComp.center = new Vector3(0, 0.87f, 0);
      controllerComp.radius = 0.29f;
      controllerComp.height = 1.81f;

      //set actions asset to "PlayerInputs" InputAction
      PlayerInput inputs = characterController.AddComponent<PlayerInput>();
      InputActionAsset actions = Resources.Load("Player Inputs") as InputActionAsset;
      inputs.actions = actions;

      //ADD ROBOT KYLE TO NEW GAMEOBJECT
      GameObject kyle = Resources.Load("Robot Kyle") as GameObject;

      //set animator to "PlayerAnimController"
      RuntimeAnimatorController animatorCont = Resources.Load("PlayerAnimController") as RuntimeAnimatorController;
      Animator anim = kyle.GetComponent<Animator>();
      anim.runtimeAnimatorController = animatorCont;

      //instantiate kyle with characterController as parent
      Instantiate(kyle, new Vector3(pos.x, 0, pos.y), Quaternion.identity).transform.SetParent(characterController.transform);

      //add Player Controller Script
      PlayerController controlScript = characterController.AddComponent<PlayerController>();
      controlScript.enabled = true;
    }

    void PlaceCoin()
    {

    }

}
