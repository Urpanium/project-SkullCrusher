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

        public Dector3 direction;

        public int length;

        public int width;

        public int height;

        /*
         * only used by prototypes
         */

        /*
         * if prototypeIndex is greater than MustSpawnPrototypes.Count, then it's from CanSpawnPrototypes
         */
        public int prototypeIndex;

        public int rotation;

    }
}