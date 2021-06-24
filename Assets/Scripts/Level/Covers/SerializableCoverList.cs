using System;
using System.Collections.Generic;

namespace Level.Covers
{
    [Serializable]
    public class SerializableCoverList
    {
        // only exists because serializing just list does not work
        public List<Cover> covers;

        public SerializableCoverList(List<Cover> covers)
        {
            this.covers = covers;
        }
    }
}