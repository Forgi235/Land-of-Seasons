using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleBlockScript : MonoBehaviour
{
    [SerializeField] private GameObject Particles;
    [SerializeField] private GameObject ChildWithCollider;

    private GameObject LocalParticles;
    private CrumbleBlockContainerScript ParentCBCS;
    private BoxCollider2D TriggerBC2D;
    private SpriteRenderer SR;

    private void Start()
    {
        ParentCBCS = transform.parent.GetComponent<CrumbleBlockContainerScript>();
        TriggerBC2D = GetComponent<BoxCollider2D>();
        SR = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            ParentCBCS.CrumbleChilderenTiles(collision);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            ParentCBCS.MinusCheck();
        }
    }
    private void DestroyParticles()
    {
        Destroy(LocalParticles);
    }
    private void OnDisable()
    {
        DestroyParticles();
    }
    public void GetDestroyed()
    {
        ChildWithCollider.gameObject.SetActive(false);
        LocalParticles = Instantiate(Particles, transform);
        SR.enabled = false;
        TriggerBC2D.enabled = false;
        // a little higher than particle lifetime 
        Invoke(nameof(DisableObject), 3f);
    }
    private void DisableObject()
    {
        gameObject.SetActive(false);
    }
    public void Reset()
    {
        if (SR == null || ParentCBCS == null || TriggerBC2D == null)
        {
            Start();
        }
        DestroyParticles();
        ChildWithCollider.gameObject.SetActive(true);
        SR.enabled = true;
        CancelInvoke(nameof(DisableObject));
        TriggerBC2D.enabled = true;
        gameObject.SetActive(true);
    }
}
