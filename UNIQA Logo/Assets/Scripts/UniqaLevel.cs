using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class UniqaLevel : MonoBehaviour
{
    public Glass[] glass;
    public Room[] rooms;
    public List<int> glassNumbers = new List<int>() { 0, 1, 2, 3, 4 };

    private void Awake()
    {
        glassNumbers = new List<int>() { 0, 1, 2, 3, 4 };
    }

    public void Randomize(int level, UniqaLevel previousLevel)
    {
        glassNumbers.Clear();
        foreach (Glass glass1 in glass)
        {
            foreach (Collectible collectible in glass1.collectibles)
            {
                collectible.gameObject.SetActive(false);
            }
        }
        
        int mustBeGlassNumber = previousLevel.glassNumbers[Random.Range(0, previousLevel.glassNumbers.Count)];
        bool mustBeAnotherGlassOnLeft = Random.Range(0, 2) == 1;
        if (mustBeGlassNumber == 0) mustBeAnotherGlassOnLeft = false;
        else if (mustBeGlassNumber == 4) mustBeAnotherGlassOnLeft = true;
        int mustBeAnotherGlass = mustBeGlassNumber + (mustBeAnotherGlassOnLeft ? -1 : 1);
        glassNumbers.Add(mustBeGlassNumber);
        glassNumbers.Add(mustBeAnotherGlass);
        glassNumbers.Add(mustBeAnotherGlass);
        glassNumbers.Add(mustBeAnotherGlass);
        glassNumbers.Add(mustBeAnotherGlass);
        for (int i = 0; i < 5; i++)
        {
            bool isGlass = Random.Range(0f, 1f) < .15f;
            if (mustBeGlassNumber == i || mustBeAnotherGlass == i) isGlass = true;
            glass[i].gameObject.SetActive(isGlass);
            if (!isGlass) rooms[i].Randomize();
            rooms[i].gameObject.SetActive(!isGlass);
        }

        if (level % 20 == 19)
        {
            glass[mustBeAnotherGlass].collectibles[Gameplay.collectedCollectibles].gameObject.SetActive(true);
        }
    }

    public void SetFinished()
    {
        foreach (var o in glass)
            o.gameObject.SetActive(true);
        foreach (var room in rooms)
            room.gameObject.SetActive(false);
    }
}
