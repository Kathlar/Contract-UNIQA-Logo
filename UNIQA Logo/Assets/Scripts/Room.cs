using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    public GameObject[] sets;

    public Transform landPoint;
    public GameObject cam;
    
    public void Randomize()
    {
        int randomSet = Random.Range(0, sets.Length);
        for (int i = 0; i < sets.Length; i++)
            sets[i].SetActive(i == randomSet);
        sets[randomSet].transform.localScale = new Vector3(Random.Range(0, 2) == 0 ? 1 : -1, 1 ,1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") FindObjectOfType<Gameplay>().Lose(this);
    }

    public void ShowCamera(bool on)
    {
        cam.SetActive(on);
    }
}
