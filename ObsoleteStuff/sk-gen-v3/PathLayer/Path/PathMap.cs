using System;
using System.Collections.Generic;
using Level.Generation.Util;
using UnityEngine;

namespace Level.Generation.PathLayer.Path
{
    [Serializable]
    public class PathMap
    {
        public readonly Dector3 size;

        
        public Dector3 minBound;
        public Dector3 maxBound;
        private byte[,,] map;
        
        
        /*
         * TODO: remove
         */

        public int sets = 0;
        public int gets = 0;

        public PathMap(Dector3 mapSize)
        {
            size = mapSize;
            UnityEngine.Debug.Log($"MAP SIZE: {size}");
            //ValidatePosition(Dector3.Back);
            map = new byte[size.x, size.y, size.z];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        //map[x, y, z] = PathTile.MaxByteValue;
                        map[x, y, z] = 0;
                    }
                }
            }

            minBound = new Dector3(size);
            maxBound = Dector3.Zero;
        }

        public bool IsTileEmpty(int x, int y, int z)
        {
            //return GetTile(x, y, z) > PathTile.MaxByteValue;
            return GetTile(x, y, z) == 0;
        }

        public bool IsTileEmpty(Dector3 position)
        {
            //return GetTile(position) > PathTile.MaxByteValue;
            return GetTile(position) == 0;
        }

        public byte GetTile(int x, int y, int z)
        {
            Dector3 position = new Dector3(x, y, z);
            return GetTile(position);
        }

        public byte GetTile(Dector3 position)
        {
            ValidatePosition(position);
            gets++;
            return map[position.x, position.y, position.z];
        }

        public void SetTile(Dector3 position, PathTile tile)
        {
            SetTile(position, tile.ToByte());
        }

        public void SetTile(int x, int y, int z, PathTile tile)
        {
            Dector3 position = new Dector3(x, y, z);
            SetTile(position, tile.ToByte());
        }

        public void SetTile(Dector3 position, byte tile)
        {
            ValidatePosition(position);
            
            map[position.x, position.y, position.z] = tile;
            sets++;
            if (!IsTileEmpty(position))
            {
                BoundCheck(position);
            }
            else
            {
                RefreshBounds();
            }
        }

        
        
        private void BoundCheck(Dector3 position)
        {
            if (position.x < minBound.x)
                minBound.x = position.x;
            if (position.y < minBound.y)
                minBound.y = position.y;
            if (position.z < minBound.z)
                minBound.z = position.z;

            if (position.x > maxBound.x)
                maxBound.x = position.x;
            if (position.y > maxBound.y)
                maxBound.y = position.y;
            if (position.z > maxBound.z)
                maxBound.z = position.z;
        }

        private void RefreshBounds()
        {
            for (int x = minBound.x; x < maxBound.x; x++)
            {
                for (int y = minBound.y; y < maxBound.y; y++)
                {
                    for (int z = minBound.z; z < maxBound.z; z++)
                    {
                        BoundCheck(new Dector3(x, y, z));
                    }
                }
            }
        }

        public bool IsPositionValid(Dector3 position)
        {
            //UnityEngine.Debug.Log($"MAP SIZE: {size}");
            return position.x >= 0 && position.x < size.x
                                  && position.y >= 0 && position.y < size.y
                                  && position.z >= 0 && position.z < size.z;
        }

        public void ValidatePosition(Dector3 position)
        {
            if (!IsPositionValid(position))
            {
                throw new Exception($"Invalid position: {position}");
            }
        }
    }
}