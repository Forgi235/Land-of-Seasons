using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AAAAAAAAA : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Camera>().orthographicSize = 7.625f;
    }
    int x = 0;
    bool y = true;
    private void Update()
    {
        if (y)
        {
            if (x > 10)
            {
                y = false;
                GetComponent<Camera>().orthographicSize = 7.625f;
                Camera.main.orthographicSize = 7.625f;
            }
            x++;
        }
        GetComponent<Camera>().orthographicSize = 7.625f;
        Camera.main.orthographicSize = 7.625f;
    }
}
