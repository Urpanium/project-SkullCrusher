using System;
using System.Collections.Generic;

namespace Level.Covers.Classes.Util.BinarySerialization
{
    [Serializable]
    public class BinarySerializableCoverList
    {
        public List<int> clusterIds;
        public List<CoverData> covers;
        

        public BinarySerializableCoverList(SerializableCoverList serializableCoverList)
        {
            covers = new List<CoverData>();
            clusterIds = serializableCoverList.clusterIds;
            for (int i = 0; i < serializableCoverList.covers.Count; i++)
            {
                covers.Add(new CoverData(serializableCoverList.covers[i]));
            }
            
        }

        public SerializableCoverList ToSerializableCoverList()
        {
            List<Cover> normalCovers = new List<Cover>();
            for (int i = 0; i < covers.Count; i++)
            {
                normalCovers.Add(covers[i].ToCover());
            }
            SerializableCoverList result = new SerializableCoverList(normalCovers, clusterIds);
            return result;
        }
        
    }
}