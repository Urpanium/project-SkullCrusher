using Level.Gen.Util;
using Level.Gen.VisualLayer;
using WaveFunctionCollapse3D.VisualLayer;

namespace WaveFunctionCollapse3D.PathLayer.Snapshots
{
    public class PathGenerationDecision
    {
        public GenerationStructure GenerationStructure { get; set; }
        /*
         * minPoint of prototype
         * or
         * center of corridor
         */
        public Dector3 Point { get; set; }

        /*
         * only used by corridor
         */
        
        public Dector3 Direction { get; set; }
        public int Length { get; set; }
        
        public int Width { get; set; }
        
        public int Height { get; set; }
        
        /*
         * only used by prototypes
         */

        public Prototype Prototype { get; set; }
        public int Rotation { get; set; }
    }
}