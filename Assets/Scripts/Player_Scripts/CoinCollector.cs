using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    private PlayerMovement PM;
    private CoinFollower CF;
    void Awake()
    {
        PM = GetComponent<PlayerMovement>();
        CF = GetComponent<CoinFollower>();
    }
    void Update()
    {
        if(PM.CanCollectCoin() && CF.Follower != null)
        {
            GetComponent<CoinFollower>().CollectCoins();
        }
    }
}
