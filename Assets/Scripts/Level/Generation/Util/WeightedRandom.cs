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

        public List<object> RandomizeList(List<object> objects, List<double> weights)
        {
            List<object> result = new List<object>();
            for (int i = 0; i < weights.Count; i++)
            {
                int index = RandomIndex(weights);
                object item = objects[index];
                result.Add(item);
                
                objects.RemoveAt(index);
                weights.RemoveAt(index);
            }

            return result;
        }


        public int RandomIndex(List<double> weights)
        {
            double max = 0.0;
            foreach (double weight in weights)
            {
                max += weight;
            }

            double randomValue = random.NextDouble() * max;

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