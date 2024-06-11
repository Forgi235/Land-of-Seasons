using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyGrassLogic : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Invoke(nameof(SetDeadly), 0.5f);
        }
    }
    private void SetDeadly()
    {
        animator.SetTrigger("Deadly");
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
