using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRandomizer : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    //to make it seem more random
    private static int LastIndex;
    private int newIndex;

    void Start()
    {
        newIndex = Random.Range(0, sprites.Length);
        if (newIndex == LastIndex)
        {
            newIndex++;
            if (newIndex >= sprites.Length)
            {
                newIndex = 0;
            }
        }
        LastIndex = newIndex;
        GetComponent<SpriteRenderer>().sprite = sprites[newIndex];
    }
}
