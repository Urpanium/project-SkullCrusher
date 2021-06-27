using System;
using System.Collections.Generic;

namespace Level.Covers.Util
{
    [Serializable]
    public class SerializableCoverList
    {
        public List<int> clusterIds;
        public List<Cover> covers;

        public SerializableCoverList(List<Cover> covers, List<int> clusterIds)
        {
            this.covers = covers;
            this.clusterIds = clusterIds;
        }
    }
}