using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PrefabRandomizer : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private bool HasAnimator;
    [SerializeField] private bool HasPolygonCollider;
    [SerializeField] private bool HasBoxCollider;

    //to make it seem more random
    private static int LastIndex;
    private int newIndex;

    void Start()
    {
        newIndex = Random.Range(0, prefabs.Length);
        if(newIndex == LastIndex )
        {
            newIndex++;
            if(newIndex >= prefabs.Length)
            {
                newIndex = 0;
            }
        }
        LastIndex = newIndex;

        GameObject newPrefab = prefabs[newIndex];
        transform.GetComponent<SpriteRenderer>().sprite = newPrefab.GetComponent<SpriteRenderer>().sprite;
        transform.GetComponent<SpriteRenderer>().sortingOrder = newPrefab.GetComponent<SpriteRenderer>().sortingOrder;
        if (HasAnimator )
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = newPrefab.GetComponent<Animator>().runtimeAnimatorController;
        }
        if (HasBoxCollider)
        {
            transform.GetComponent<BoxCollider2D>().size = newPrefab.GetComponent<BoxCollider2D>().size;
            transform.GetComponent<BoxCollider2D>().offset = newPrefab.GetComponent<BoxCollider2D>().offset;
        }
        if (HasPolygonCollider) 
        {
            transform.GetComponent<PolygonCollider2D>().points = newPrefab.GetComponent<PolygonCollider2D>().points;
        }
    }
}
