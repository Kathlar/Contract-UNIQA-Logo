using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waving : MonoBehaviour
{
    private void Start()
    {
        string number = Random.Range(0, 2) == 0 ? "1" : "2";
        GetComponent<Animator>().Play("Waving" + number, -1, Random.Range(0.0f, 1.0f));
    }
}
