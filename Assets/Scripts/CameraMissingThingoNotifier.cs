using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraMissingThingoNotifier : MonoBehaviour
{
    private void Awake()
    {
        if(transform.GetComponent<CinemachineVirtualCamera>().Follow.gameObject == null)
        {
            Debug.Log(transform + " - Missing Camera Pivot");
        }
    }
}
