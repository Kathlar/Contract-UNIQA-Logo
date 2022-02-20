using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarsController : MonoBehaviour
{
    [SerializeField] private Transform[] cars;

    private void Start()
    {
        Invoke(nameof(PlayCars), 1);
    }

    void PlayCars()
    {
        StartCoroutine(PlayCarsCoroutine());
    }

    IEnumerator PlayCarsCoroutine()
    {
        int amountOfCars = Random.Range(1, 4);

        Transform[] shuffledCars = cars.OrderBy(x => Guid.NewGuid()).ToArray();
        Transform[] randomCars = new Transform[amountOfCars];
        Array.Copy(shuffledCars, randomCars, amountOfCars);

        foreach (Transform car in randomCars)
        {
            StartCoroutine(MoveCarCoroutine(car));
            yield return new WaitForSeconds(Random.Range(.8f, 2f));
        }
        
        Invoke(nameof(PlayCars), Random.Range(4f, 6f));
    }

    IEnumerator MoveCarCoroutine(Transform car)
    {
        Vector3 startPosition = car.localPosition;
        do
        {
            car.localPosition += car.forward * Time.deltaTime * 10;
            yield return null;
        } while (Mathf.Abs(car.localPosition.x) < 48);
        car.localPosition = startPosition;
    }
}
