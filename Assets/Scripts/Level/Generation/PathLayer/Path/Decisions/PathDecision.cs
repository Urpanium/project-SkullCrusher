using System;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.Decisions
{
    public class PathDecision
    {
        public long id;
        /*
         * which must be included in id:
         * type - 4 bytes
         *
         * direction - 12 bytes
         * length - 4 bytes
         * width - 4 bytes
         * height - 4 bytes
         *
         * prototypeId - 4 bytes
         * rotation - 4 bytes
         *
         * Total size: 36 bytes
         * But max size is: 4 + 12 + 12
         */

        public PathDecisionType type;
        
        public Dector3 point;

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
         * using prototype id to make size of this class static
         */

        public int prototypeId;

        public int rotation;

    }
}