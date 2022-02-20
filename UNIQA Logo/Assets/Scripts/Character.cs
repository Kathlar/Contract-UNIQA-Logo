using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    private Animator animator;
    
    private enum State { beforeStart }
    private State state = State.beforeStart;

    private float timeOfNextStretch = -1;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        timeOfNextStretch = Time.realtimeSinceStartup + Random.Range(1.5f, 4f);
    }

    private void Update()
    {
        switch (state)
        {
            case State.beforeStart:
                Update_BeforeStart();
                break;
        }
    }

    private void Update_BeforeStart()
    {
        if (timeOfNextStretch < Time.realtimeSinceStartup)
        {
            animator.SetTrigger("RandomTrigger");
            int randomStretch = Random.Range(0, 2);
            animator.SetInteger("RandomInt", randomStretch);
            timeOfNextStretch = Time.realtimeSinceStartup + Random.Range(1.5f, 4f);
        }
    }
}
