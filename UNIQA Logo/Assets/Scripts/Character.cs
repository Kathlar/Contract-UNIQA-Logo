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

    public ParticleSystem landParticles, runParticles;
    private bool lost;

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
        if (lost) return;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) PressedMoveButton(false);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) PressedMoveButton(true);
        transform.position = Vector3.MoveTowards(transform.position, 
            runCharacterPositions[currentPosition].position, Time.deltaTime * speed);
        if (Vector3.Distance(transform.position, runCharacterPositions[currentPosition].position) < .1f)
            changingPosition = false;
    }

    public void PressedMoveButton(bool right)
    {
        if (beforeStart) return;
        if (!right && currentPosition <= 0) return;
        if (right && currentPosition >= 4) return;
        changingPosition = true;
        currentPosition += right ? 1 : -1;
    }

    public void StartGameplay()
    {
        beforeStart = false;
    }

    public Transform tt;
    public void OnLose(Transform targetPoint, bool won)
    {
        transform.SetParent(targetPoint);
        if (lost) return;
        lost = true;
        tt = targetPoint;
        StartCoroutine(OnLoseCoroutine(targetPoint, won));
    }

    private IEnumerator OnLoseCoroutine(Transform targetPoint, bool won)
    {
        animator.SetTrigger("Gameplay");
        do
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero,
                Time.deltaTime * 10);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetPoint.rotation,
                Time.deltaTime * 400);
            yield return null;
        } while (Vector3.Distance(transform.localPosition, Vector3.zero) > .05f);

        animator.applyRootMotion = true;
        transform.localPosition = Vector3.zero;
        transform.rotation = targetPoint.rotation;
        
        animator.SetTrigger("OnBuilding");
        yield return new WaitForSeconds(1);
        animator.SetTrigger("InBuilding");
        yield return new WaitForSeconds(1);
        animator.SetBool("Won", won);
        animator.SetTrigger("Finish");
        Quaternion targetRot = Quaternion.LookRotation(-transform.forward);
        do
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot,
                Time.deltaTime * 100);
            yield return null;
        } while (Quaternion.Angle(transform.rotation, targetRot) > 1);
    }
}
