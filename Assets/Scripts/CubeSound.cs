using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeSound : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();

    private void Awake()
    {
        audioClips.Add(Resources.Load("cubeOnGround1") as AudioClip);
        audioClips.Add(Resources.Load("cubeOnGround2") as AudioClip);
        audioClips.Add(Resources.Load("cubeOnGround3") as AudioClip);
        audioClips.Add(Resources.Load("cubeOnGround4") as AudioClip);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(audioClips[Random.Range(0, audioClips.Count - 1)]);
        }
    }
}
