using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    public Animator animator;

    private bool beforeStart = true;
    private float timeOfNextStretch = -1;

    public Transform[] runCharacterPositions;
    private int currentPosition = 2;
    private bool changingPosition;
    public float speed = 5;

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
        if (beforeStart) Update_BeforeStart();
        else Update_Gameplay();
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

    private void Update_Gameplay()
    {
        if (!changingPosition)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) PressedMoveButton(false);
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) PressedMoveButton(true);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                runCharacterPositions[currentPosition].position, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, runCharacterPositions[currentPosition].position) < .1f)
                changingPosition = false;
        }
    }

    public void PressedMoveButton(bool right)
    {
        if (beforeStart || changingPosition) return;
        if (!right && currentPosition <= 0) return;
        if (right && currentPosition >= 4) return;
        changingPosition = true;
        currentPosition += right ? 1 : -1;
    }

    public void StartGameplay()
    {
        beforeStart = false;
    }
}
