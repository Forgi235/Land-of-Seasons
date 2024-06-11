using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class VineBridgeLogic : MonoBehaviour
{
    [SerializeField] private bool Is_A_Wall;
    private bool playerStateBool;

    private List<Animator> animators;
    private GameObject parent;
    private PlayerMovement PM = null;

    private void Start()
    {
        PM = null;
        animators = new List<Animator>();
        parent = transform.parent.gameObject;
        parent.GetComponentsInChildren<Animator>(animators);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(this.gameObject);
        if (collision.gameObject.tag == "Player" && !collision.isTrigger)
        {
            if (PM == null)
            {
                PM = collision.GetComponent<PlayerMovement>();
            }
            if(Is_A_Wall)
            {
                playerStateBool = PM.isWalled();
            }
            else
            {
                playerStateBool = PM.isGrounded();
            }
            if (playerStateBool)
            {
                foreach (Animator anim in animators)
                {
                    anim.SetTrigger("Decay");
                }
            }
        }
    }
    private void ResetDecay()
    {
        foreach (Animator anim in animators)
        {
            anim.ResetTrigger("Decay");
        }
    }
}
