using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private GameObject virtualCam;
    [SerializeField] private GameObject AimationResetableObstaclesParnet;
    [SerializeField] private List<GameObject> LoadableObjects;

    DeathResetableScript DRS;
    CinemachineBrain MainCMBrain;
    private static RoomManager PreviousRoom;
    private Animator[] animators;
    private bool CheckIfIsIn;
    private Transform player;

    private static Transform newRoom;
    public static bool inTransition;

    private void Start()
    {
        DRS = GetComponentInChildren<DeathResetableScript>();
        UnLoadObjects();
        MainCMBrain = Camera.main.GetComponent<CinemachineBrain>();
        animators = AimationResetableObstaclesParnet.GetComponentsInChildren<Animator>();
    }
    private void Update()
    {
        if (CheckIfIsIn)
        {
            if(player.parent == null && player.parent != transform)
            {
                player.SetParent(transform);
                CheckIfIsIn = false;
                virtualCam.SetActive(true);
                if (PreviousRoom == null)
                {
                    PreviousRoom = this;
                }
                PlayerMovement ded_lol = player.GetComponent<PlayerMovement>();
                LoadObjects();
                ResetRoom();
                if (ded_lol.dead)
                {
                    ded_lol.dead = false;
                }
                else
                {
                    StartCoroutine(ChangeRoomLag());
                }

            }
        }    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            CheckIfIsIn = true;
            player = other.gameObject.transform;
            newRoom = transform;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            if (newRoom != null && newRoom != transform)
            {
                virtualCam.SetActive(false);
                if (other.gameObject.transform.parent == transform)
                {
                    other.gameObject.transform.SetParent(null);
                }
                newRoom = null;
            }
            CheckIfIsIn = false;
        }
    }
    public void ResetRoom()
    {
        foreach(Animator anim in animators)
        {
            anim.Rebind();
        }
        DRS.ResetResetable();
    }
    public void UnLoadObjects()
    {
        foreach (GameObject obj in LoadableObjects)
        {
            obj.SetActive(false);
        }
    }
    public void LoadObjects()
    {
        foreach (GameObject obj in LoadableObjects)
        {
            obj.SetActive(true);
        }
    }
    private IEnumerator ChangeRoomLag()
    {
        MainCMBrain.m_IgnoreTimeScale = true;
        yield return new WaitForSecondsRealtime(0.05f);
        inTransition = true;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.75f);
        Time.timeScale = 1f;
        inTransition = false;
        MainCMBrain.m_IgnoreTimeScale = false;
        PreviousRoom.UnLoadObjects();
        PreviousRoom = this;
    }
}
