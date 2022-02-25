using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    public Collectible[] collectibles;


    private void Start()
    {
        foreach (Collectible col in collectibles)
        {
            col.gameObject.SetActive(false);
        }
    }
}
