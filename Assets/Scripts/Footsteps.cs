using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
  private AudioSource audioSource;
  private AudioClip leftStep;
  private AudioClip rightStep;
    // Start is called before the first frame update
    void Awake()
    {
      // Debug.Log(GameObject.Find("CharacterController"));
        audioSource = GetComponent<AudioSource>();
          Debug.Log(audioSource);
        leftStep = Resources.Load("leftfootstep") as AudioClip;
        rightStep = Resources.Load("rightfootstep") as AudioClip;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StepLeft()
    {
      audioSource.PlayOneShot(leftStep);
    }

    void StepRight()
    {
      audioSource.PlayOneShot(rightStep);
    }
}
