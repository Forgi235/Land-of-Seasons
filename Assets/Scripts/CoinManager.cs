using Assets.Scripts;
using Assets.Scripts.MenuScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class CoinManagerHelper
{
    public List<List<bool>> CoinList = new List<List<bool>>();

    public List<bool> Level0 = new List<bool>();
    public List<bool> Level1 = new List<bool>();
    public List<bool> Level2 = new List<bool>();
    public List<bool> Level3 = new List<bool>();
    public List<bool> Level4 = new List<bool>();
    public List<bool> Level5 = new List<bool>();
    public List<bool> Level6 = new List<bool>();
    public List<bool> Level7 = new List<bool>();
    public List<bool> Level8 = new List<bool>();
    public List<bool> Level9 = new List<bool>();
    public List<bool> Level10 = new List<bool>();
    public List<bool> Level11 = new List<bool>();
    public List<bool> Level12 = new List<bool>();
    public List<bool> Level13 = new List<bool>();
    public List<bool> Level14 = new List<bool>();
    public List<bool> Level15 = new List<bool>();


    public CoinManagerHelper() 
    {
        LoadCMH();
    }

    public List<bool> GetLevelList(int LevelID)
    {
        return CoinList[LevelID];
    }

    public void LoadCMH()
    {
        
        //if you are reading this:
        //I relly tried making it dynamic, trust me, but shit just dont work
        CoinList.Clear();

        CoinList.Add(Level0);
        CoinList.Add(Level1);
        CoinList.Add(Level2);
        CoinList.Add(Level3);
        CoinList.Add(Level4);
        CoinList.Add(Level5);
        CoinList.Add(Level6);
        CoinList.Add(Level7);
        CoinList.Add(Level8);
        CoinList.Add(Level9);
        CoinList.Add(Level10);
        CoinList.Add(Level11);
        CoinList.Add(Level12);
        CoinList.Add(Level13);
        CoinList.Add(Level14);
        CoinList.Add(Level15);

    }
}
public class CoinManager : MonoBehaviour
{
    public string FileName = "CollectedCoins.json";
    public string inputFilePath;

    [SerializeField] private int LevelID;
    [SerializeField] private bool NotAllCoinsAdded_OrTesting;

    private CoinManagerHelper CMH;
    [SerializeField] private List<KillMe> _CoinsAndDividers;
    private List<ICoinStuff> CoinsAndDividers = new List<ICoinStuff>();

    [SerializeField] private MenuAppleHolderContainerScript MAHCS;

    private List<CoinScript> Coins = new List<CoinScript>();
    public List<bool> LocalCoinsCollectedStatus = new List<bool>();

    public List<ICoinStuff> GetCoinsAndDividers()
    {
        return CoinsAndDividers;
    }
    public List<CoinScript> GetCoins()
    {
        return Coins;
    }

    void Awake()
    {
        CoinsAndDividers.Clear();
        foreach(var CND in _CoinsAndDividers)
        {
            try
            {
                CoinsAndDividers.Add(CND as ICoinStuff);
            }
            catch { }
        }
        Coins.Clear();
        foreach(ICoinStuff coin in CoinsAndDividers)
        {
            if(coin.isThisACoin())
            {
                Coins.Add(coin as CoinScript);
            }
        }
        if(NotAllCoinsAdded_OrTesting)
        {
            Debug.Log("WARNING - Coin manager is in testing more and wont save coins (turn off when all coins are added into the game)");
        }
        inputFilePath = Path.Combine(Application.persistentDataPath, FileName);
        CMH = new CoinManagerHelper();
        // to prevent index out of range exception
        foreach (CoinScript coin in Coins) 
        {
            CMH.CoinList[LevelID].Add(false);
            LocalCoinsCollectedStatus.Add(false);
        }
        //
        Debug.Log("Check \"CoinManager\" Level ID");
        //

        if (!File.Exists(inputFilePath) || NotAllCoinsAdded_OrTesting)
        {
            SaveCoinListToFile();
        }
        LoadCoinList();
        LocalLoadCoins();
        CheckCollectedCoins();
    }

    private void LoadCoinList()
    {
        var outputJSON = File.ReadAllText(inputFilePath);
        CoinManagerHelper CList = JsonUtility.FromJson<CoinManagerHelper>(outputJSON);
        CMH = CList;
    }
    public void SetLocalCollectedStatus(CoinScript source)
    {
        LocalCoinsCollectedStatus[Coins.IndexOf(source)] = true;
        MAHCS.UpdateCoinStatus(LocalCoinsCollectedStatus);
    }
    private void SaveCoinListToFile()
    {
        var inputJSON = JsonUtility.ToJson(CMH);
        File.WriteAllText(inputFilePath, inputJSON);
    }
    public void LocalSaveCoins()
    {
        for(int i = 0; i < Coins.Count; i++) 
        {
            CMH.CoinList[LevelID][i] = Coins[i].Collected;
        }
        SaveCoinListToFile();
    }
    public void LocalLoadCoins()
    {
        for (int i = 0; i < Coins.Count; i++)
        {
            Coins[i].Collected = CMH.CoinList[LevelID][i];
        }
    }   
    public List<bool> GetCollectedStatus()
    {
        return CMH.CoinList[LevelID];
    }
    private void CheckCollectedCoins()
    {
        foreach(CoinScript coin in Coins)
        {
            coin.CheckIfCollected();
        }
    }
}
