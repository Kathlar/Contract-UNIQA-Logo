using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject[] sets;

    public void Randomize()
    {
        int randomSet = Random.Range(0, sets.Length);
        for (int i = 0; i < sets.Length; i++)
            sets[i].SetActive(i == randomSet);
    }
}
