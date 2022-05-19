using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public AudioSource playOnCollect;
    public GameObject collectPrefab;
    
    private void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        Gameplay.collectedCollectibles++;

        GameObject collect = Instantiate(collectPrefab, transform.position, transform.rotation);
        Destroy(collect, 1);
        gameObject.SetActive(false);
    }
}
