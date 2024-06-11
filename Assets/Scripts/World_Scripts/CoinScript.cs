using Assets.Scripts;
using Assets.Scripts.MenuScripts;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class CoinScript : KillMe, ICoinStuff
{
    public Transform Following; //usually the player 
    private bool PickedUp;
    private Vector2 StartingPosition;
    private float Distance_;
    private CoinFollower FollowingScript;

    [SerializeField] private float MoveSpeed;
    [SerializeField] private float MinDistance;
    [SerializeField] private CoinManager CoinManager;

    public bool Collected;
    
    public bool isThisACoin()
    {
        return true;
    }

    void Awake()
    {
        StartingPosition = transform.position;
        PickedUp = false;
    }
    private void Update()
    {
        if(PickedUp && CoinFollower.PlayerDied)
        {
            Invoke(nameof(ResetPosition), 0.75f);
            PickedUp = false;
        }
        if (PickedUp && Following != null)
        {
            Distance_ = Vector2.Distance(transform.position, Following.position);
            if (Distance_ < MinDistance)
            {
                return;
            }
            if (Distance_ > MinDistance * 5f)
            {
                transform.position = Vector2.MoveTowards(transform.position, Following.position, MoveSpeed * 2f * Time.deltaTime);
                return;
            }
            if (Distance_ > MinDistance * 3.5f)
            {
                transform.position = Vector2.MoveTowards(transform.position, Following.position, MoveSpeed * 1.66f * Time.deltaTime);
                return;
            }
            if (Distance_ > MinDistance * 2.5f)
            {
                transform.position = Vector2.MoveTowards(transform.position, Following.position, MoveSpeed * 1.33f * Time.deltaTime);
                return;
            }
            if (Distance_ > MinDistance * 2f)
            {
                transform.position = Vector2.MoveTowards(transform.position, Following.position, MoveSpeed * 1f * Time.deltaTime);
                return;
            }
            if (Distance_ > MinDistance * 1.5f)
            {
                transform.position = Vector2.MoveTowards(transform.position, Following.position, MoveSpeed * 0.8f * Time.deltaTime);
                return;
            }
            if (Distance_ > MinDistance * 1.25f)
            {
                transform.position = Vector2.MoveTowards(transform.position, Following.position, MoveSpeed * 0.6f * Time.deltaTime);
                return;
            }
            if (Distance_ > MinDistance * 1.125f)
            {
                transform.position = Vector2.MoveTowards(transform.position, Following.position, MoveSpeed * 0.4f * Time.deltaTime);
                return;
            }
            if (Distance_ > MinDistance * 1.0625)
            {
                transform.position = Vector2.MoveTowards(transform.position, Following.position, MoveSpeed * 0.2f * Time.deltaTime);
                return;
            }
            // Distance is > MinDistance
            transform.position = Vector2.MoveTowards(transform.position, Following.position, MoveSpeed * 0.1f * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!PickedUp && collision.CompareTag("Player"))
        {
            PickedUp = true;
            FollowingScript = collision.GetComponent<CoinFollower>();
            CancelCollecting();
            SetWhoToFollow(FollowingScript);
        }
    }
    private void CancelCollecting()
    {
        //false is here so the "CoinFollower" logic only happenes on the coins and not the player
        FollowingScript.StopCollecting(false);
    }
    public void SetWhoToFollow(CoinFollower followingScript)
    {
        Following = followingScript.GetWhoToFollow();
        FollowingScript = Following.GetComponent<CoinFollower>();
        FollowingScript.SetFollower(transform);
    }
    void ResetPosition()
    {
        transform.position = StartingPosition;
        GetComponent<CoinFollower>().ClearFollower();
    }
    public void CollectCoin()
    {
        Invoke(nameof(DelayedCollectFunction), 1f);
    }
    void DelayedCollectFunction()
    {
        Collected = true;
        CoinManager.LocalSaveCoins();
        CoinManager.SetLocalCollectedStatus(this);
        gameObject.SetActive(false);
    }
    public void CheckIfCollected()
    {
        if (Collected)
        {
            // Check Unity Bullshit 5

            // placeholder code
            GetComponent<SpriteRenderer>().color = Color.red;
            //
        }
    }
}
