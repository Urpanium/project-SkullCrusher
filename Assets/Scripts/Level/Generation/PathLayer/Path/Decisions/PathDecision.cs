using System;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.Decisions
{
    public class PathDecision
    {
        public PathDecisionType type;
        
        public Dector3 entry;

        /*
         * only used by corridor
         */

        public Dector3 size;
        public Dector3 to;

        /*
         * only used by prototypes
         */

        /*
         * if prototypeIndex is greater than MustSpawnPrototypes.Count, then it's from CanSpawnPrototypes
         */
        public int prototypeId;

        public int rotation;

        public override string ToString()
        {
            switch (type)
            {
                case PathDecisionType.Corridor:
                {
                    return $"Corridor from {entry} to {to}, size: {size}";
                }
                case PathDecisionType.End:
                {
                    return "End signal";
                }
                case PathDecisionType.Prototype:
                {
                    return $"Prototype ({prototypeId}) rotation {rotation} at point {entry}";
                }
                    
            }

            return "";
        }
    }
}