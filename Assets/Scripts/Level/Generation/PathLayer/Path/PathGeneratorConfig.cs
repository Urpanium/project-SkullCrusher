using System;
using UnityEngine;

namespace Level.Generation.PathLayer.Path
{
    [Serializable]
    [CreateAssetMenu(fileName = "PathGenerationConfig", menuName = "Path Generation Config", order = 51)]
    public class PathGeneratorConfig : ScriptableObject
    {
        /*
         * seed (wow, really helpful)
         */
        public int seed;

        /*
         * limit the amount of decisions to speed up generation
         */

        public int perEntryDecisionsLimit;

        /*
         * path length parameters
         */
        public int minimumPathLength;

        public int maximumPathLength;


        /*
         * exist because we don't want previous
         * and current level to overlap
         */
        public int maximumEndPointCorridorLength;

        /*
         *possibility for each node to spawn after minimum reached
         */
        // TODO: remove if not used
        public float minimumPathContinueChance;

        public int minimumOffsetBetweenMustSpawnPrototypes;

        public int minimumOffsetBetweenCanSpawnPrototypes;

        /*
         * possibilities of generation to go in direction for example,
         * in some kind of underground laboratories we can make generator
         * build more vertical path
         */
        [Header("Directions Weights")] public float pathTopDirectionWeight;
        public float pathBottomDirectionWeight;
        public float pathForwardDirectionWeight;
        public float pathLeftDirectionWeight;
        public float pathBackDirectionWeight;
        public float pathRightDirectionWeight;

        /*
         * just weights
         * but in array
         */
        public float[] PathDirectionsWeights
        {
            get
            {
                float[] array =
                {
                    pathTopDirectionWeight,
                    pathBottomDirectionWeight,
                    pathForwardDirectionWeight,
                    pathLeftDirectionWeight,
                    pathBackDirectionWeight,
                    pathRightDirectionWeight
                };
                return array;
            }
        }


        /*
         * ranges of length for each direction
         */
        [Header("Top")] public int minimumTopCorridorLength;
        public int maximumTopCorridorLength;
        public int maximumTopCorridorWidth;
        public int maximumTopCorridorHeight;

        [Header("Bottom")] public int minimumBottomCorridorLength;
        public int maximumBottomCorridorLength;
        public int maximumBottomCorridorWidth;
        public int maximumBottomCorridorHeight;

        [Header("Forward")] public int minimumForwardCorridorLength;
        public int maximumForwardCorridorLength;
        public int maximumForwardCorridorWidth;
        public int maximumForwardCorridorHeight;

        [Header("Left")] public int minimumLeftCorridorLength;
        public int maximumLeftCorridorLength;
        public int maximumLeftCorridorWidth;
        public int maximumLeftCorridorHeight;

        [Header("Back")] public int minimumBackCorridorLength;
        public int maximumBackCorridorLength;
        public int maximumBackCorridorWidth;
        public int maximumBackCorridorHeight;

        [Header("Right")] public int minimumRightCorridorLength;
        public int maximumRightCorridorLength;
        public int maximumRightCorridorWidth;
        public int maximumRightCorridorHeight;

        public int[] MinimumCorridorsLengths
        {
            get
            {
                int[] array =
                {
                    minimumTopCorridorLength,
                    minimumBottomCorridorLength,
                    minimumForwardCorridorLength,
                    minimumLeftCorridorLength,
                    minimumBackCorridorLength,
                    minimumRightCorridorLength
                };
                return array;
            }
        }

        public int[] MaximumCorridorsLengths
        {
            get
            {
                int[] array =
                {
                    maximumTopCorridorLength,
                    maximumBottomCorridorLength,
                    maximumForwardCorridorLength,
                    maximumLeftCorridorLength,
                    maximumBackCorridorLength,
                    maximumRightCorridorLength
                };
                return array;
            }
        }

        public int[] MaximumCorridorsHeights
        {
            get
            {
                int[] array =
                {
                    maximumTopCorridorHeight,
                    maximumBottomCorridorHeight,
                    maximumForwardCorridorHeight,
                    maximumLeftCorridorHeight,
                    maximumBackCorridorHeight,
                    maximumRightCorridorHeight
                };
                return array;
            }
        }

        public int[] MaximumCorridorsWidths
        {
            get
            {
                int[] array =
                {
                    maximumTopCorridorWidth,
                    maximumBottomCorridorWidth,
                    maximumForwardCorridorWidth,
                    maximumLeftCorridorWidth,
                    maximumBackCorridorWidth,
                    maximumRightCorridorWidth
                };
                return array;
            }
        }

        [Header("Rooms")] public int minimumRoomXSize;
        public int minimumRoomYSize;
        public int minimumRoomZSize;

        public int maximumRoomXSize;
        public int maximumRoomYSize;
        public int maximumRoomZSize;
    }
}