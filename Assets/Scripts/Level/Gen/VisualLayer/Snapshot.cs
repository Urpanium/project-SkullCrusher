using System.Collections.Generic;
using WaveFunctionCollapse3D.PathLayer.Tiles;

namespace WaveFunctionCollapse3D.WaveFunction
{
    
    /* stores condition of map
     * maybe it will be used, if bad prototypes variants
     * were chosen, maybe not
     */
    public class Snapshot
    {
        public readonly Tile[,,] Map;
        public readonly List<Tile>[,,] PossibleVariants;

        public Snapshot(Tile[,,] map, List<Tile>[,,] possibleVariants)
        {
            Map = map;
            PossibleVariants = possibleVariants;
        }
    }
}