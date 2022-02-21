using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimationTimeRandomizer : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Animator>().Play("Talking", -1, Random.Range(0.0f, 1.0f));
    }
}
