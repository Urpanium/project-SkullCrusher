using System;
using Level.Generation.Util;
using UnityEngine;

namespace Level.Generation.PathLayer.Path
{
    public class PathMap
    {
        public Dector3 size;
        private byte[,,] map;

        public PathMap(Dector3 mapSize)
        {
            size = mapSize;
            map = new byte[size.x, size.y, size.z];
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    for (int z = 0; z < map.GetLength(0); z++)
                    {
                        map[x, y, z] = PathTile.MaxByteValue;
                    }
                }
            }
        }

        public bool IsTileEmpty(int x, int y, int z)
        {
            return GetTile(x, y, z) > PathTile.MaxByteValue;
        }
        public bool IsTileEmpty(Dector3 position)
        {
            return GetTile(position) > PathTile.MaxByteValue;
        }

        public byte GetTile(int x, int y, int z)
        {
            Dector3 position = new Dector3(x, y, z);
            return GetTile(position);
        }

        public byte GetTile(Dector3 position)
        {
            return map[position.x, position.y, position.z];
        }

        public void SetTile(Dector3 position, PathTile tile)
        {
            SetTile(position, tile.ToByte());
        }

        public void SetTile(int x, int y, int z, PathTile tile)
        {
            SetTile(x, y, z, tile.ToByte());
        }

        public void SetTile(Dector3 position, byte tile)
        {
            ValidatePosition(position);
            map[position.x, position.y, position.z] = tile;
        }

        public void SetTile(int x, int y, int z, byte tile)
        {
            Dector3 position = new Dector3(x, y, z);
            ValidatePosition(position);
            map[position.x, position.y, position.z] = tile;
        }

        public void ValidatePosition(Dector3 position)
        {
            if (position.x < 0 || position.x > size.x
                               || position.y < 0 || position.y > size.y
                               || position.z < 0 || position.z > size.z)
            {
                throw new Exception($"Invalid position: {position}");
            }
        }
    }
}