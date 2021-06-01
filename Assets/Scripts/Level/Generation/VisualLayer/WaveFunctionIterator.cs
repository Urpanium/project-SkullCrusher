using System;
using System.Collections.Generic;
using Level.Gen.VisualLayer;
using Level.Generation.PathLayer.Tiles;

namespace WaveFunctionCollapse3D.VisualLayer
{
    public class WaveFunctionIterator
    {
        public readonly Prototype[] Prototypes;

        public Tile[,,] InitialMap;

        public Tile[,,] Map;

        // 3D array of lists
        // yeap
        private List<Tile>[,,] _possibleVariants;

        private Random _random;

        public WaveFunctionIterator(List<Prototype> prototypes, int width, int height, int length)
        {
            Prototypes = prototypes.ToArray();
            InitialMap = new Tile[width, height, length];
            Map = new Tile[width, height, length];
            _possibleVariants = new List<Tile>[width, height, length];
        }

        public WaveFunctionIterator(List<Prototype> prototypes, Tile[,,] initialMap)
        {
            Prototypes = prototypes.ToArray();
            InitialMap = initialMap;
            Map = initialMap;
            
            int width = initialMap.GetLength(0);
            int height = initialMap.GetLength(1);
            int length = initialMap.GetLength(2);
            
            _possibleVariants = new List<Tile>[width, height, length];
        }

        public void GenerateMap()
        {
            GenerateMap(DateTime.Now.Millisecond);
        }

        public void GenerateMap(int seed)
        {
            _random = new Random(seed);

            throw new NotImplementedException();
        }

        private void RecursiveEntropyUpdateIteration()
        {
        }

        private int[] GetMinimalEntropyPosition()
        {
            // x = -1 means that we couldn't find position
            // this means that we should stop iterating
            int[] minimalEntropyPosition = {-1, 0, 0};
            int minimalEntropyPossibilities = _possibleVariants[0, 0, 0].Count;
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    for (int z = 0; z < Map.GetLength(2); z++)
                    {
                        int currentPositionPossibilities = _possibleVariants[x, y, z].Count;
                        if (currentPositionPossibilities > 0 &&
                            currentPositionPossibilities < minimalEntropyPossibilities)
                        {
                            minimalEntropyPosition = new[]
                            {
                                x, y, z
                            };
                            minimalEntropyPossibilities = currentPositionPossibilities;
                        }
                    }
                }
            }

            return minimalEntropyPosition;
        }

        private int GetPrototype(List<int> ids)
        {
            float totalWeight = 0.0f;
            foreach (var id in ids)
            {
                totalWeight += Prototypes[id].Weight;
            }

            float randomNumber = (float) _random.NextDouble() * totalWeight;
            foreach (var id in ids)
            {
                float weight = Prototypes[id].Weight;
                if (randomNumber < weight)
                    return id;
                randomNumber -= weight;
            }

            // what the fuck
            return -1;
        }
    }
}