using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePlayer : MonoBehaviour
{
    [SerializeField] private float forceX;
    [SerializeField] private float forceY;
    public float getForceX() { return forceX; }
    public float getForceY() { return forceY; }
}
