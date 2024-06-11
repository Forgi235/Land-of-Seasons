using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform OriginalParent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb = collision.GetComponent<Rigidbody2D>();
            if (rb.velocity.y <= 0.01f)
            {
                OriginalParent = collision.transform.parent;
                collision.gameObject.transform.SetParent(transform);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            collision.gameObject.transform.SetParent(OriginalParent);
        }
    }
}
