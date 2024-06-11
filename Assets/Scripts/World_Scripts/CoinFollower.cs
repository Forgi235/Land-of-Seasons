using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinFollower : MonoBehaviour
{
    public Transform Follower;
    public CoinFollower CoFo;
    public CoinScript CoSc;

    public int FollowNumber;
    public static int CollectNumber;

    public bool StartedCollecting;
    private bool helper;
    private bool helper2;

    public static bool PlayerDied;

    private void Awake()
    {
        StartedCollecting = false;
        ClearFollower();
        FollowNumber = 0;
        helper = false;
        helper2 = false;
    }
    private void Update()
    {
        if(StartedCollecting)
        {
            if(FollowNumber == 0)
            {
                return;
            }
            if (Follower == null && helper) 
            {
                CollectNumber = FollowNumber;
            }
            if(CollectNumber == FollowNumber && helper)
            {
                helper = false;
                Invoke(nameof(CollectYourself), 0.25f);
            }
        }
    }
    private void CollectYourself()
    {
        CollectNumber--;
        GetComponent<CoinScript>().CollectCoin();
    }

    public Transform GetWhoToFollow()
    {
        Transform x = transform;
        if(Follower != null)
        {
            x = CoFo.GetWhoToFollow();
        }
        return x;
    }

    public void SetFollower(Transform follower)
    {
        helper2 = false;
        Follower = follower;
        CoFo = Follower.gameObject.GetComponent<CoinFollower>();
        CoFo.SetFollowNumber(FollowNumber + 1);
        CoSc = Follower.gameObject.GetComponent<CoinScript>();
    }
    public void CollectCoins()
    {
        if (!helper2)
        {
            helper2 = true;
            if (Follower != null)
            {
                CoFo.CollectCoins();
            }
            StartedCollecting = true;
            helper = true;
        }
    }
    public void ClearFollower()
    {
        Follower = null;
        CoFo = null;
        CoSc = null;
        FollowNumber = 0;
        helper = false;
    }
    public void StopCollecting(bool y)
    {
        helper2 = false;
        if (y)
        {
            CancelInvoke(nameof(CollectYourself));
        }
        StartedCollecting = false;
        helper = false;
        CollectNumber = 0;
        if(CoFo != null)
        {
            CoFo.StopCollecting(true);
        }
    }
    public void SetFollowNumber(int num)
    {
        FollowNumber = num;
    }
}
