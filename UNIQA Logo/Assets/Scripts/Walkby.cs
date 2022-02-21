using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Walkby : MonoBehaviour
{
    private NavMeshAgent agent;
    
    public Transform endPoint;

    private Vector3 startPosition, endPosition, currentTargetPosition;
    private bool going, goingBack;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        startPosition = transform.position;
        endPosition = currentTargetPosition = endPoint.position;
        if (Random.Range(0f, 1f) > .2f) Invoke(nameof(GoToAnotherPoint), Random.Range(3, 16f));
        else
        {
            Vector3 vectorToEnd = endPosition - startPosition;
            transform.position += vectorToEnd.normalized * Random.Range(0, vectorToEnd.magnitude - 10);
            GoToAnotherPoint();
        }
    }

    private void Update()
    {
        if (!going) return;
        if (Vector3.Distance(transform.position, currentTargetPosition) < .5f) OnReachedPoint();
    }

    void GoToAnotherPoint()
    {
        going = true;
        agent.SetDestination(currentTargetPosition);
    }

    void OnReachedPoint()
    {
        going = false;
        goingBack = !goingBack;
        currentTargetPosition = goingBack ? startPosition : endPosition;
        
        Invoke(nameof(GoToAnotherPoint), Random.Range(1f, 6f));
    }

    private void OnDrawGizmos()
    {
        if (!endPoint) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + Vector3.up, endPoint.position + Vector3.up);
    }
}
