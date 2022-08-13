using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
  private AudioSource audioSource;
  private AudioClip leftStep;
  private AudioClip rightStep;
  private AudioClip runLeftStep;
  private AudioClip runRightStep;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        leftStep = Resources.Load("leftfootstep") as AudioClip;
        rightStep = Resources.Load("rightfootstep") as AudioClip;
        runLeftStep = Resources.Load("runleftfootstep") as AudioClip;
        runRightStep = Resources.Load("runrightfootstep") as AudioClip;
    }

    void StepLeft()
    {
      audioSource.PlayOneShot(leftStep);
    }

    void StepRight()
    {
      audioSource.PlayOneShot(rightStep);
    }
    
    void RunStepLeft()
    {
      audioSource.PlayOneShot(runLeftStep);
    }

    void RunStepRight()
    {
      audioSource.PlayOneShot(runRightStep);
    }
}
