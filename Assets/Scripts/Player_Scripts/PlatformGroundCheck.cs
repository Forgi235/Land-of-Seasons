using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGroundCheck : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isGround;
    private float platformRotationRaw;
    private int counter = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform") && !collision.isTrigger)
        {
            platformRotationRaw = collision.transform.rotation.eulerAngles.z;
            if (platformRotationRaw == 0)
            {
                counter++;
                isGround = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform") && !collision.isTrigger)
        {
            platformRotationRaw = collision.transform.rotation.eulerAngles.z;
            if (platformRotationRaw == 0)
            {
                counter--;
                if (counter == 0)
                {
                    isGround = false;
                }
                else if(counter < 0) 
                {
                    counter = 0; 
                }
            }

        }
    }
    public bool IsItGround()
    {
        return isGround;
    }
}
