using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectBlockerScript : MonoBehaviour
{
    private PlayerMovement PM;
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PM = collision.GetComponent<PlayerMovement>();
            PM.CoinBlock();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PM.CoinUnBlock();
        }
    }
}
