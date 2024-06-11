using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindScript : MonoBehaviour
{

    [SerializeField] float X_Wind_Speed;
    [SerializeField] float Y_Wind_Speed;

    private bool isInZone = false;
    private bool WindUpdated = false;
    private bool XHelper;
    private bool YHelper;

    private PlayerMovement PM;

    private void Start()
    {
        if(X_Wind_Speed != 0)
        {
            XHelper = true;
        }
        if(Y_Wind_Speed != 0)
        {
            YHelper = true;
            if (XHelper)
            {
                YHelper = false;
                XHelper = false;

                X_Wind_Speed = 0;
                Y_Wind_Speed = 0;

                Debug.Log("Error: " + transform + " wind zone has both directions");
            }
        }
    }
    private void Update()
    {
        if(isInZone && !WindUpdated)
        {
            if ((XHelper && !PM.isXWinded()) || (YHelper && !PM.isYWinded()))
            {
                SetWind();
            }
        } 
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if (PM == null)
            {
                PM = collision.GetComponent<PlayerMovement>();
            }
            isInZone = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            isInZone = false;
            WindUpdated = false;
            ResetWind();
        }
    }
    private void SetWind()
    {
        WindUpdated = true;
        PM.SetWindSpeed(X_Wind_Speed, Y_Wind_Speed);
    }
    private void ResetWind()
    {
        PM.SetWindSpeed(0, 0);
    }
}
