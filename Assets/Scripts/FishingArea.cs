using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FishSpawnData
{
    public Fish fish;
    public float spawnWeight; // Higher number = more common in this spot
}
public class FishingArea : MonoBehaviour
{
    public string areaName; // For display/reference
    public List<FishSpawnData> fishPool = new List<FishSpawnData>();

    public Fish GetRandomFish()
    {
        if (fishPool.Count == 0)
            return null;

        float totalWeight = fishPool.Sum(data => data.spawnWeight);
        float randomPoint = Random.Range(0f, totalWeight);

        float currentWeight = 0f;
        foreach (var spawnData in fishPool)
        {
            currentWeight += spawnData.spawnWeight;
            if (randomPoint <= currentWeight)
                return spawnData.fish;
        }

        return fishPool[0].fish;
    }
}