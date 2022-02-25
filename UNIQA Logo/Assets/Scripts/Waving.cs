using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Waving : MonoBehaviour
{
    private static List<Waving> _wavings = new List<Waving>();
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _wavings.Add(this);
    }

    private void OnDisable()
    {
        _wavings.Remove(this);
    }

    public static void Wave()
    {
        foreach (Waving waving in _wavings)
        {
            string number = Random.Range(0, 2) == 0 ? "1" : "2";
            waving.animator.SetTrigger("Wave");
            waving.animator.Play("Waving1" + number, -1, Random.Range(0.0f, 1.0f));
        }
    }
}
