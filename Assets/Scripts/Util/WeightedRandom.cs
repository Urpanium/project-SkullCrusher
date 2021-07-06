using System;
using System.Collections;
using System.Collections.Generic;

namespace Util
{
    public class WeightedRandom
    {
        private Random random;

        public WeightedRandom(int seed)
        {
            random = new Random(seed);
        }

        public int Index(List<float> weights)
        {
            float max = 0.0f;
            foreach (float weight in weights)
            {
                max += weight;
            }

            float randomValue = (float) random.NextDouble() * max;

            for (int i = 0; i < weights.Count; i++)
            {
                randomValue -= weights[i];
                if (randomValue < 0)
                    return i;
            }

            return weights.Count - 1;
        }
        
    }
}