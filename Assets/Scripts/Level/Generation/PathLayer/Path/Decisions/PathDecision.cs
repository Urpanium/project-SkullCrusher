using System;
using System.Collections.Generic;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.Decisions
{
    public class PathDecision
    {
        public PathDecisionType type;
        
        public Dector3 entry;

        public List<Dector3> newEntries;

        /*
         * used by corridor and room
         */

        public Dector3 size;
        
        /*
         * corridor only
         */
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
                    return "Ending decision";
                }
                case PathDecisionType.Prototype:
                {
                    return $"Prototype ({prototypeId}) rotation {rotation} at point {entry}";
                }
                case PathDecisionType.Room:
                {
                    return $"Room form {entry} with size {size}";
                }
                    
            }

            return "";
        }
    }
}