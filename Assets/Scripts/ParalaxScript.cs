using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParalaxScript : MonoBehaviour
{
    private Camera Cam;

    [SerializeField] private float XParalaxMultiplier;
    [SerializeField] private float YParalaxMultiplier;

    private Vector3 StartPos;
    private Vector2 CameraStartPos;
    private float StartZ;

    //Transform Subject;
    void Start()
    {
        StartPos = transform.position;
    }

    private Vector2 X;

    bool a = true;
    int b = 0;

    void Update()
    {
        if (a)
        {
            if (b > 10)
            {
                a = false;
                Cam = Camera.main;
                CameraStartPos = Cam.transform.position;
            }
            b++;
            return;
        }
        if (Cam != null)
        {
            X = Cam.transform.position;
            transform.position = new Vector3(StartPos.x + ((X.x - CameraStartPos.x) * XParalaxMultiplier), StartPos.y + ((X.y - CameraStartPos.y) * YParalaxMultiplier), StartPos.z);
        }
    }
}
