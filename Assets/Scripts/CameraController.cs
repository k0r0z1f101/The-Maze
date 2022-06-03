using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  //GameObject to follow
  [SerializeField]
  private GameObject player;

    void LateUpdate()
    {
      transform.position = player.transform.position;
    }
}
