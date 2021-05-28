namespace WaveFunctionCollapse3D.PathLayer.PathGeneration
{
    public class PathGeneratorConfig
    {
        /*
         * seed (wow, really helpful)
         */
        public int Seed { get; set; }

        /*
         * path length parameters
         */
        public int MinimumPathLength { get; set; }

        public int MaximumPathLength { get; set; }
        
        
        /*
         * exist because we don't want previous
         * and current level to overlap
         */
        public int MaximumEndPointCorridorLength { get; set; }

        /*
         *possibility for each node to spawn after minimum reached
         */
        // TODO: remove if not used
        public float MinimumPathContinueChance { get; set; }

        public int MinimumOffsetBetweenMustSpawnPrototypes { get; set; }
        
        public int MinimumOffsetBetweenCanSpawnPrototypes { get; set; }

        /*
         * possibilities of generation to go in direction for example,
         * in some kind of underground laboratories we can make generator
         * build more vertical path
         */
        public float PathTopDirectionWeight { get; set; }
        public float PathBottomDirectionWeight { get; set; }
        public float PathForwardDirectionWeight { get; set; }
        public float PathLeftDirectionWeight { get; set; }
        public float PathBackDirectionWeight { get; set; }
        public float PathRightDirectionWeight { get; set; }
        
        /*
         * just weights
         * but in array
         */
        public float[] PathDirectionsWeights {
         get
         {
          float[] array =
          {
           PathTopDirectionWeight,
           PathBottomDirectionWeight,
           PathForwardDirectionWeight,
           PathLeftDirectionWeight,
           PathBackDirectionWeight,
           PathRightDirectionWeight
          };
          return array;
         }
        }
        
        
        /*
         * ranges of length for each direction
         */
        public int MinimumTopCorridorLength { get; set; }
        public int MaximumTopCorridorLength { get; set; }
        public int MaximumTopCorridorWidth { get; set; }
        public int MaximumTopCorridorHeight { get; set; }
        
        public int MinimumBottomCorridorLength { get; set; }
        public int MaximumBottomCorridorLength { get; set; }
        public int MaximumBottomCorridorWidth { get; set; }
        public int MaximumBottomCorridorHeight { get; set; }
        
        public int MinimumForwardCorridorLength { get; set; }
        public int MaximumForwardCorridorLength { get; set; }
        public int MaximumForwardCorridorWidth { get; set; }
        public int MaximumForwardCorridorHeight { get; set; }
        
        public int MinimumLeftCorridorLength { get; set; }
        public int MaximumLeftCorridorLength { get; set; }
        public int MaximumLeftCorridorWidth { get; set; }
        public int MaximumLeftCorridorHeight { get; set; }
        
        public int MinimumBackCorridorLength { get; set; }
        public int MaximumBackCorridorLength { get; set; }
        public int MaximumBackCorridorWidth { get; set; }
        public int MaximumBackCorridorHeight { get; set; }
        
        public int MinimumRightCorridorLength { get; set; }
        public int MaximumRightCorridorLength { get; set; }
        public int MaximumRightCorridorWidth { get; set; }
        public int MaximumRightCorridorHeight { get; set; }

        public int[] MinimumCorridorsLengths
        {
         get
         {
          int[] array =
          {
           MinimumTopCorridorLength,
           MinimumBottomCorridorLength,
           MinimumForwardCorridorLength,
           MinimumLeftCorridorLength,
           MinimumBackCorridorLength,
           MinimumRightCorridorLength
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
           MaximumTopCorridorLength,
           MaximumBottomCorridorLength,
           MaximumForwardCorridorLength,
           MaximumLeftCorridorLength,
           MaximumBackCorridorLength,
           MaximumRightCorridorLength
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
           MaximumTopCorridorHeight,
           MaximumBottomCorridorHeight,
           MaximumForwardCorridorHeight,
           MaximumLeftCorridorHeight,
           MaximumBackCorridorHeight,
           MaximumRightCorridorHeight
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
           MaximumTopCorridorWidth,
           MaximumBottomCorridorWidth,
           MaximumForwardCorridorWidth,
           MaximumLeftCorridorWidth,
           MaximumBackCorridorWidth,
           MaximumRightCorridorWidth
          };
          return array;
         }
        }
        
        public int MinimumRoomXSize { get; set; }
        public int MinimumRoomYSize { get; set; }
        public int MinimumRoomZSize { get; set; }
        
        public int MaximumRoomXSize { get; set; }
        public int MaximumRoomYSize { get; set; }
        public int MaximumRoomZSize { get; set; }
        
    }
}