using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools
{

    //Returns the index of random from weights
    static public int WeightedRandom(List<float> weights)
    {
        float total = 0;

        foreach (float weight in weights)
        {
            total += weight;
        }

        float random = Random.Range(1, total);

        float cursor = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            cursor += weights[i];
            if (cursor >= random)
            {
                return i;
            }
        }
        return -1;

    }


}
