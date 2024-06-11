using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformWallCheck : MonoBehaviour
{
    private int platformRotation = 0;
    private float platformRotationRaw;
    private int counter = 0;
    private bool TouchingCrumbleBlock = false;
    private int CrumbleBlockCounter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("CrumbleBlock") && !collision.isTrigger)
        {
            CrumbleBlockCounter++;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform") && !collision.isTrigger)
        {
            platformRotationRaw = collision.transform.rotation.eulerAngles.z;
            if (platformRotationRaw != 0)
            {
                counter++;
                //player turns by changing scale.x to 1 or -1
                //platformRotation gets a rotation and gets if its negative (-1 (270 = -90)) or positive (1 (90 = 90))
                //if player scale == platformRotation then the player is facing the solid side of the platform
                if(platformRotationRaw == 90)
                {
                    platformRotation = 1;
                }
                else if(platformRotationRaw == 270)
                {
                    platformRotation = -1;
                }
            }
        }
        if(CrumbleBlockCounter > 0)
        {
            TouchingCrumbleBlock = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("CrumbleBlock") && !collision.isTrigger)
        {
            CrumbleBlockCounter--;
        }
        if(CrumbleBlockCounter < 0)
        {
            CrumbleBlockCounter = 0;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform") && !collision.isTrigger)
        {
            platformRotationRaw = collision.transform.rotation.eulerAngles.z;
            if (platformRotationRaw != 0)
            {
                counter--;
                if (counter == 0)
                {
                    platformRotation = 0;
                }
                else if (counter < 0) 
                {
                    counter = 0;
                }
            }
            
        }
        if (CrumbleBlockCounter == 0)
        {
            TouchingCrumbleBlock = false;
        }
    }
    public int GetPlatformRotation()
    {
        return platformRotation;
    }
    public bool isTouchingCrumbleBlock()
    {
        return TouchingCrumbleBlock;
    }
}
