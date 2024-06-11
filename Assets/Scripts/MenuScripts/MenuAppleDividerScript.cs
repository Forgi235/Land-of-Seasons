using Assets.Scripts;
using Assets.Scripts.MenuScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAppleDividerScript : KillMe, ICoinStuff
{
    [SerializeField] bool isInCoinmanager;
    public bool isThisACoin()
    {
        return false;
    }
    private void Start()
    {
        if(!isInCoinmanager)
        {
            Debug.Log(this.gameObject + " - Add me to CoinManager");
        }
    }
}
