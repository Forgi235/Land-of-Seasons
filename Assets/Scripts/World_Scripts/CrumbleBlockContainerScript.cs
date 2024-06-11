using Assets.Scripts.World_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleBlockContainerScript : MonoBehaviour, Resetable
{
    private CrumbleBlockScript[] CBSList = new CrumbleBlockScript[0];

    [SerializeField] private GameObject OnPlayerPSFF;

    private List<Transform> childeren = new List<Transform>();
    private PlayerMovement PM;
    private GameObject LocalPSFF;
    private Vector3 PSFFLocation;
    private bool isCrumbled;
    private Collider2D collision;
    private bool startchecking;
    private int checkingCounter;

    void Start()
    {
        childeren.Clear();
        foreach(Transform t in transform)
        {
            childeren.Add(t);
        }
        CBSList = GetComponentsInChildren<CrumbleBlockScript>();
        isCrumbled = false;
        checkingCounter = 0;
    }
    private void Update()
    {
        if(checkingCounter > 0)
        {
            CheckToExecuteCrumble();
        }
    }
    public void CrumbleChilderenTiles(Collider2D coll) 
    {
        checkingCounter++;
        collision = coll;
        
    }
    public void MinusCheck()
    {
        checkingCounter--;
        if(checkingCounter < 0 )
        {
            checkingCounter = 0;
        }
    }
    private void CheckToExecuteCrumble()
    {
        if (!isCrumbled)
        {
            if (PM == null)
            {
                PM = collision.GetComponent<PlayerMovement>();
            }
            if (PM.GetIsDashing() && PM.isTouchingCrumbleBlock())
            {
                isCrumbled = true;
                PM.CrumbleBlockBounceBack();
                PSFFLocation = new Vector3(collision.transform.position.x, collision.transform.position.y, transform.position.z);
                LocalPSFF = Instantiate(OnPlayerPSFF, PSFFLocation, Quaternion.identity);
                LocalPSFF.transform.SetParent(transform);
                ExecuteCrumble();
                // a little higher than particle lifetime 
                Invoke(nameof(RemoveLocalPSFF), 0.075f);
            }
        }
    }
    private void RemoveLocalPSFF()
    {
        Destroy(LocalPSFF);
    }
    private void ExecuteCrumble()
    {
        foreach (CrumbleBlockScript CBS in CBSList)
        {
            CBS.GetDestroyed();
        }
    }
    public void Reset()
    {
        isCrumbled = false;
        foreach(Transform child in childeren)
        {
            child.gameObject.SetActive(true);
        }
        foreach (CrumbleBlockScript CBS in CBSList)
        {
            CBS.Reset();
        }
    }
}
