using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class UniqaLevel : MonoBehaviour
{
    public GameObject[] glass;
    public Room[] rooms;
    public List<int> glassNumbers = new List<int>() { 0, 1, 2, 3, 4 };

    private void Awake()
    {
        glassNumbers = new List<int>() { 0, 1, 2, 3, 4 };
    }

    public void Randomize(int level, UniqaLevel previousLevel)
    {
        glassNumbers.Clear();
        int mustBeGlassNumber = previousLevel.glassNumbers[Random.Range(0, previousLevel.glassNumbers.Count)];
        bool mustBeAnotherGlassOnLeft = Random.Range(0, 2) == 1;
        if (mustBeGlassNumber == 0) mustBeAnotherGlassOnLeft = false;
        else if (mustBeGlassNumber == 4) mustBeAnotherGlassOnLeft = true;
        int mustBeAnotherGlass = mustBeGlassNumber + (mustBeAnotherGlassOnLeft ? -1 : 1);
        glassNumbers.Add(mustBeGlassNumber);
        glassNumbers.Add(mustBeAnotherGlass);
        glassNumbers.Add(mustBeAnotherGlass);
        for (int i = 0; i < 5; i++)
        {
            bool isGlass = Random.Range(0f, 1f) < .25f;
            if (mustBeGlassNumber == i || mustBeAnotherGlass == i) isGlass = true;
            glass[i].SetActive(isGlass);
            if (!isGlass) rooms[i].Randomize();
            rooms[i].gameObject.SetActive(!isGlass);
        }
    }

    public void SetFinished()
    {
        foreach (var o in glass)
            o.SetActive(true);
        foreach (var room in rooms)
            room.gameObject.SetActive(false);
    }
}
