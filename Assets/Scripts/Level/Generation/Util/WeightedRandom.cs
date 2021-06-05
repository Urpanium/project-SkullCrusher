using System;
using System.Collections.Generic;

namespace Level.Generation.Util
{
    public class WeightedRandom
    {
        private Random random;

        public WeightedRandom(int seed)
        {
            random = new Random(seed);
        }

        public List<T> RandomWeightedList<T>(List<T> objects, List<float> weights)
        {
            List<T> result = new List<T>();
            for (int i = 0; i < weights.Count; i++)
            {
                int index = RandomWeightedIndex(weights);
                T item = objects[index];
                result.Add(item);

                objects.RemoveAt(index);
                weights.RemoveAt(index);
            }

            return result;
        }


        public int RandomWeightedIndex(List<float> weights)
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

            return 0;
        }
    }
}