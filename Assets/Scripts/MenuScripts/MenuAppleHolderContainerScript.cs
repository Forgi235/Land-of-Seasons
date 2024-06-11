using Assets.Scripts.MenuScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuAppleHolderContainerScript : MonoBehaviour
{
    [SerializeField] private GameObject MenuApplePrefab;
    [SerializeField] private GameObject DividerPrefab;
    [SerializeField] private GameObject HolderPrefab;

    [SerializeField] private CoinManager _CoinManager;

    [SerializeField] private Sprite CollectedSprite;
    [SerializeField] private Sprite LocalCollectedSprite;

    private List<GameObject> MenuApples = new List<GameObject>();
    private GameObject MenuApple;
    private GameObject Holder;

    private List<ICoinStuff> CoinsAndDividers = new List<ICoinStuff>();
    private List<CoinScript> Coins = new List<CoinScript>();
    private List<bool> localCoinStatus = new List<bool>();
    private List<bool> lastLocalCoinStatus = new List<bool>();

    private int objectCount;
    private int objectMaxCountInHolder;

    void Start()
    {
        CoinsAndDividers = _CoinManager.GetCoinsAndDividers();
        Coins = _CoinManager.GetCoins();
        localCoinStatus.Clear();

        foreach(CoinScript coin in Coins)
        {
            localCoinStatus.Add(false);
        }

        Holder = Instantiate(HolderPrefab, transform);
        objectMaxCountInHolder = Convert.ToInt32(HolderPrefab.GetComponent<RectTransform>().rect.width / (MenuApplePrefab.GetComponent<RectTransform>().rect.width + 10));

        CreateMenuCoins();
        setCollectedStatus();
    }
    private void CreateMenuCoins()
    {
        MenuApples.Clear();
        objectCount = 0;
        foreach(ICoinStuff coinStuff in CoinsAndDividers) 
        {
            objectCount++;
            if(objectCount > objectMaxCountInHolder)
            {
                objectCount = 0;
                //Create new holder
                Holder = Instantiate(HolderPrefab, transform);
            }
            if(coinStuff.isThisACoin())
            {
                MenuApple = Instantiate(MenuApplePrefab, Holder.transform);
                MenuApples.Add(MenuApple);
            }
            else
            {
                MenuApple = Instantiate(DividerPrefab, Holder.transform);
            }
        }
    }
    private void setCollectedStatus()
    {
        int j = 0;
        foreach(bool _bool in _CoinManager.GetCollectedStatus())
        {
            if(_bool)
            {
                MenuApples[j].GetComponent<Image>().sprite = CollectedSprite;
            }
            j++;
        }
    }

    public void UpdateCoinStatus(List<bool> _localCoinStatus)
    {
        lastLocalCoinStatus = localCoinStatus;
        localCoinStatus = _localCoinStatus;

        if (localCoinStatus.Count == MenuApples.Count)
        {
            for (int i = 0; i < MenuApples.Count; i++)
            {
                if (lastLocalCoinStatus[i] != localCoinStatus[i])
                {
                    // local Collect menu coin
                    MenuApples[i].GetComponent<Image>().sprite = LocalCollectedSprite;
                }
            }
        }
        else
        {
            Debug.Log("ERROR: -UpdateCoinStatus: localCoinStatus and MenuApples are not the same size");
        }
    }
}
