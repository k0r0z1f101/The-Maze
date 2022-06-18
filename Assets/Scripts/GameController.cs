using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Reflection;
using UnityEngine.InputSystem;
using TMPro;

public class GameController : MonoBehaviour
{
    //list of converted walls
    private List<Vector2> convertedWalls;
    //list of converted coins
    private List<Vector2> convertedCoins;
    //list of converted doors position
    private List<Vector2> convertedDoorsPos;
    //list of converted doors direction
    private List<Vector2> convertedDoorsDir;
    //list of converted breakable walls
    private List<Vector2> convertedBreakWalls;

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
    private GameObject coins;
    private GameObject doorsContainer;
    private GameObject breakableWalls;

    //Color mode, will copy the pictures each pixel to wall colors
    [Header("colored walls if true")]
    [SerializeField]
    private bool colorMode = false;

    //hero Position
    private Vector2 pos;

    private CameraController cam;

    GameObject goldCoin;

    GameObject door;

    GameObject canvasGO;

    private Vector2 ballPos;

    GameObject ball;
    
    //song player
    private GameObject jukebox;
    
    void ConvPixToVec()
    {
      List<Vector2> ignoredBlue = new List<Vector2>();
        convertedWalls = new List<Vector2>();
        convertedCoins = new List<Vector2>();
        convertedDoorsPos = new List<Vector2>();
        convertedDoorsDir = new List<Vector2>();
        convertedBreakWalls = new List<Vector2>();
        levelImg = new Texture2D(0, 0);
        levelImg = lvlImages[levelToLoad];
        int nbrOfRows = levelImg.width;
        int nbrOfCols = levelImg.height;

        Color yellowColor = new Color(1.0f,1.0f,0.0f,1.0f);
        Color blackColor = new Color(0.0f,0.0f,0.0f,1.0f);
        Color pinkColor = new Color(1.0f,0.0f,1.0f,1.0f);
        Color redColor = new Color(1.0f,0.0f,0.0f,1.0f);
        Color blueColor = new Color(0.0f,0.0f,1.0f,1.0f);
        Color greenColor = new Color(0.0f,1.0f,0.0f,1.0f);
        for(int i = 0; i < nbrOfRows; ++i)
          for(int j = 0; j < nbrOfCols; ++j)
          {
              Color pixColor = levelImg.GetPixel(i, j);

              //walls array
              if(pixColor == blackColor)
                convertedWalls.Add(new Vector2(i, j));
              
              //breakable walls array
              if(pixColor == greenColor)
                convertedBreakWalls.Add(new Vector2(i, j));

              //hero position
              if(pixColor == pinkColor)
              {
                pos.x = i;
                pos.y = j;
              }

              //ball position
              if(pixColor == redColor)
              {
                ballPos.x = i;
                ballPos.y = j;
              }

              //gold coins array
              if(pixColor == yellowColor)
                convertedCoins.Add(new Vector2(i, j));

              //doors array
              if (pixColor == blueColor)
              {
                if(!ignoredBlue.Contains(new Vector2(i, j)))
                {
                  convertedDoorsPos.Add(new Vector2(i, j));
                  ignoredBlue.Add(new Vector2((float)i, (float)j));
                  for (int e = -1; e < 2; ++e)
                  {
                    for (int f = -1; f < 2; ++f)
                    {
                      if (!(e == 0 && f == 0))
                      {
                        Vector2 adjBlock = new Vector2((float)(i + e), (float)(j + f));
                        if (!ignoredBlue.Contains(adjBlock) && levelImg.GetPixel(i + e, j + f) == blueColor)
                        {
                          convertedDoorsDir.Add(new Vector2(e, f));
                          ignoredBlue.Add(adjBlock);
                        }
                      }
                    }
                  }
                }
              }
          }
    }

    private void Awake()
    {
      ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
      ground.GetComponent<Renderer>().material.color = Color.red;
      cam = GameObject.Find("CameraContainer").GetComponent<CameraController>();

      //Set Gold Coin object as template for future coins
      goldCoin = Resources.Load("GoldCoinPrefab") as GameObject;
      goldCoin.transform.localScale = new Vector3(50, 50, 50);
      goldCoin.tag = "Coin";

      //Set Door object as template for future doors
      door = Resources.Load("Door") as GameObject;
      door.tag = "Door";
    }

    private void OnValidate()
    {
        if(ground)
        {
            wallsHeight = Mathf.Clamp(wallsHeight,0,10);

            if(!colorMode)
              ConvPixToVec();

            Destroy(ground);
            ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.GetComponent<Renderer>().material.color = Color.red;
            
            breakableWalls = GameObject.Find("BreakableWallsContainer");
            if(breakableWalls)
              Destroy(breakableWalls);
            breakableWalls = new GameObject("BreakableWallsContainer");

            canvasGO = GameObject.Find("Canvas");
            if(canvasGO)
              Destroy(canvasGO);
            canvasGO = new GameObject("Canvas");
            CreateUI();

            coins = GameObject.Find("CoinsContainer");
            if(coins)
              Destroy(coins);
            coins = new GameObject("CoinsContainer");

            doorsContainer = GameObject.Find("DoorsContainer");
            if(doorsContainer)
              Destroy(doorsContainer);
            doorsContainer   = new GameObject("DoorsContainer");

            ball = GameObject.Find("Ball");
            if(ball)
              Destroy(ball);
            ball = new GameObject("Ball");

            DrawLevel();
            DisplayCoins();
            PositionHero();
            PlaceBall();
            AddJukebox();
        }
    }

    private void DrawLevel()
    {
        ground.transform.localScale = new Vector3(levelImg.width * 0.1f, 0.01f, levelImg.height * 0.1f);
        ground.transform.position = new Vector3(levelImg.width * 0.5f, 0.0f, levelImg.height * 0.5f);
        if(!colorMode)
        {
          for(int i = 0; i < convertedWalls.Count; ++i)
          {
              GameObject newBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
              newBlock.transform.localScale = new Vector3(1.0f, wallsHeight, 1.0f);
              newBlock.transform.position = new Vector3(convertedWalls[i].x, (wallsHeight * 0.5f) + 0.01f, convertedWalls[i].y);
              newBlock.transform.SetParent(ground.transform);
          }
          
          for (int i = 0; i < convertedBreakWalls.Count; ++i)
          {
            for(int j = 0; j < (int)wallsHeight; ++j)
            {
              GameObject newBreakWalls = Resources.Load("BreakableWalls") as GameObject;
              newBreakWalls.transform.position = new Vector3(convertedBreakWalls[i].x, j + 1, convertedBreakWalls[i].y);
              Instantiate(newBreakWalls, breakableWalls.transform);
              //newBreakWalls.transform.SetParent(breakableWalls.transform);
              print(newBreakWalls.name);
            }
          }

          for(int i = 0; i < convertedDoorsPos.Count; ++i)
            PositionDoor(i);

          for(int i = 0; i < convertedCoins.Count; ++i)
            PlaceCoin(new Vector2(convertedCoins[i].x, convertedCoins[i].y));
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

    private void PositionHero()
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
      GameObject kyle = Resources.Load("Robot Kyle Prefab") as GameObject;
      kyle.AddComponent<AudioSource>();
      AudioSource kyleAudioSource = kyle.GetComponent<AudioSource>();
      kyleAudioSource.spatialBlend = 1.0f;
      kyle.AddComponent<Footsteps>();

      //set animator to "PlayerAnimController"
      RuntimeAnimatorController animatorCont = Resources.Load("PlayerAnimController") as RuntimeAnimatorController;
      Animator anim = kyle.GetComponent<Animator>();
      anim.runtimeAnimatorController = animatorCont;

      //instantiate kyle with characterController as parent
      Instantiate(kyle, new Vector3(pos.x, 0, pos.y), Quaternion.identity).transform.SetParent(characterController.transform);

      //add AudioSource AddComponent
      characterController.AddComponent<AudioSource>();
      AudioSource audioSource = characterController.GetComponent<AudioSource>();
      audioSource.playOnAwake = false;
      audioSource.loop = true;
      audioSource.spatialBlend = 1.0f;
      AudioClip audioClip = Resources.Load("footsteps") as AudioClip;
      audioSource.clip = audioClip;

      //add Player Controller Script
      PlayerController controlScript = characterController.AddComponent<PlayerController>();
      controlScript.enabled = true;
    }

    void PositionDoor(int index)
    {
      GameObject newDoor = door;
      Vector2 pos = convertedDoorsPos[index];
      Quaternion dir = new Quaternion(0,0,0,1);
      if(convertedDoorsDir[index].x == -1 || convertedDoorsDir[index].x != 1)
      {
        pos.x -= 1.0f;
        dir = new Quaternion(0,1,0,1);
      }
      Instantiate(newDoor, new Vector3(pos.x + 0.5f, 0, pos.y + 0.5f), dir).transform.SetParent(doorsContainer.transform);
    }

    void PlaceCoin(Vector2 pos)
    {
      GameObject newCoin = goldCoin;

      ConstantForce force = newCoin.GetComponent<ConstantForce>();
      force.torque = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f));

      Instantiate(newCoin, new Vector3(pos.x, 0.5f, pos.y), Quaternion.identity).transform.SetParent(coins.transform);
    }

    void PlaceBall()
    {
      ball.transform.position = new Vector3(ballPos.x, 0.5f , ballPos.y);
      GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
      sphere.transform.SetParent(ball.transform, false);

      ball.AddComponent<Rigidbody>();
      Material ballTexture = Resources.Load<Material>("Ball");
      MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
      meshRenderer.material = ballTexture;
    }

    void CreateUI()
    {
      canvasGO.AddComponent<Canvas>();
      canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
      canvasGO.AddComponent<CanvasScaler>();
      canvasGO.AddComponent<GraphicRaycaster>();


      GameObject coinsHUD = new GameObject("CoinsHUD");
      coinsHUD.transform.SetParent(canvasGO.transform);
      coinsHUD.AddComponent<RectTransform>();
      RectTransform coinsHUDRect = coinsHUD.GetComponent<RectTransform>();
      coinsHUDRect.anchoredPosition = new Vector2(0, -30);
      coinsHUDRect.anchorMin = new Vector2(0.5f, 1);
      coinsHUDRect.anchorMax = new Vector2(0.5f, 1);
      coinsHUDRect.sizeDelta = new Vector2(0, 0);

      GameObject textBG = new GameObject("TextBG");
      textBG.transform.SetParent(coinsHUD.transform);
      textBG.AddComponent<RawImage>();
      RawImage textBGImg = textBG.GetComponent<RawImage>();
      Texture2D textTexture = Resources.Load<Texture2D>("button");
      textBGImg.texture = textTexture;
      RectTransform textBGRect = textBG.GetComponent<RectTransform>();
      textBGRect.anchoredPosition = new Vector2(0, 0);
      textBGRect.sizeDelta = new Vector2(0, 0);
      textBG.AddComponent<VerticalLayoutGroup>();
      VerticalLayoutGroup textBGvertical = textBG.GetComponent<VerticalLayoutGroup>();
      RectOffset tempPadding = new RectOffset(30, 30, 8, 8);
      textBGvertical.padding = tempPadding;
      textBGvertical.childAlignment = TextAnchor.MiddleCenter;
      textBG.AddComponent<ContentSizeFitter>();
      ContentSizeFitter textBGFitter = textBG.GetComponent<ContentSizeFitter>();
      textBGFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
      textBGFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

      //add textmeshpro object
      GameObject tmp = new GameObject("Text (TMP)");
      tmp.transform.SetParent(textBG.transform);
      tmp.AddComponent<RectTransform>();
      RectTransform tmpRect = tmp.GetComponent<RectTransform>();
      tmp.AddComponent<TextMeshProUGUI>();
      tmp.GetComponent<TextMeshProUGUI>().color = Color.black;
      tmp.GetComponent<TextMeshProUGUI>().text = "25";
    }

    public void DisplayCoins(int off = 0)
    {
      string str = coins.transform.childCount - off == 1 ? "One last coin!" : (coins.transform.childCount - off == 0 ? "You won!!!" : (coins.transform.childCount - off).ToString() + " Coins left");
      GameObject.Find("Canvas/CoinsHUD/TextBG/Text (TMP)").GetComponent<TextMeshProUGUI>().text = str;
    }

    public void AddJukebox()
    {
      jukebox = GameObject.Find("Jukebox");
      if(jukebox) Destroy(jukebox);
      
      jukebox = new GameObject("Jukebox");
      jukebox.AddComponent<AudioSource>();
      AudioSource audioSource = jukebox.GetComponent<AudioSource>();
      audioSource.volume = 0.5f;
      audioSource.loop = true;
      audioSource.playOnAwake = false;

      jukebox.AddComponent<Jukebox>();
      Jukebox jukeboxScript = jukebox.GetComponent<Jukebox>();
    }
}
