using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDeadlyGrassLogic : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            animator.SetTrigger("Deadly");
        }
    }
    private void GetDeadly()
    {
        transform.tag = "Death";
    }
    private void GetSafe()
    {
        transform.tag = "Untagged";
    }
    private void ResetDeadly()
    {
        animator.ResetTrigger("Deadly");
    }
}
