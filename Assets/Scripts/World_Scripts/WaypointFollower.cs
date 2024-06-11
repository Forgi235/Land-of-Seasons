using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool ForwardAndBack = false;
    [SerializeField] private bool Loop = true;

    private int currentWaypointIndex = 0;
    private int addative = 1;

    private void Update()
    {
        if (Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < 0.1f)
        {
            currentWaypointIndex = addative;
            if((ForwardAndBack && currentWaypointIndex >= waypoints.Length) || (ForwardAndBack && currentWaypointIndex < 0))
            {
                addative = -addative;
            }
            else if(Loop && currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }
        if (currentWaypointIndex < waypoints.Length)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, Time.deltaTime * speed);
        }
    }
}
