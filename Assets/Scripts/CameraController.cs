using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  //GameObject to follow
  private GameObject player;

    void LateUpdate()
    {
      if(player)
      {
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
      }
    }

    public void ResetPlayer()
    {
      player = null;
    }

    public void SetPlayer(GameObject obj)
    {
      player = obj;
    }
}
