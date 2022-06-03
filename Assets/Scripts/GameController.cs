using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Reflection;

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

    void ConvPixToVec()
    {
        convertedPix = new List<Vector2>();
        levelImg = new Texture2D(0, 0);
        levelImg = lvlImages[levelToLoad];
        int nbrOfRows = levelImg.width;
        int nbrOfCols = levelImg.height;

        Color whiteColor = new Color(1.0f,1.0f,1.0f,1.0f);
        for(int i = 0; i < nbrOfRows; ++i)
          for(int j = 0; j < nbrOfCols; ++j)
          {
              Color pixColor = levelImg.GetPixel(i, j);
              if(pixColor != whiteColor)
                convertedPix.Add(new Vector2(i, j));
          }

    }

    void Awake()
    {
        ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.GetComponent<Renderer>().material.color = Color.red;
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
}
