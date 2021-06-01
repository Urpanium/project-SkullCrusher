using System;
using Level.Generation.Util;

namespace Level.Gen.Util
{
    public class Map3<T> where T : class
    {
        public readonly Dector3 Size;
        private T[,,] _map;

        public Map3(Dector3 size)
        {
            _map = new T[size.x, size.y, size.z];
            Size = size;
        }

        public T GetTile(int x, int y, int z)
        {
            Dector3 position = new Dector3(x, y, z);
            return GetTile(position);
        }

        public T GetTile(Dector3 position)
        {
            IsPositionCorrect(position);
            return _map[position.x, position.y, position.z];
        }

        public void SetTile(Dector3 position, T tile)
        {
            IsPositionCorrect(position);
            _map[position.x, position.y, position.z] = tile;
        }

        public void SetTile(int x, int y, int z, T tile)
        {
            Dector3 position = new Dector3(x, y, z);
            IsPositionCorrect(position);
            _map[position.x, position.y, position.z] = tile;
        }

        public void Clear()
        {
            _map = new T[Size.x, Size.y, Size.z];
        }

        public bool IsValidPosition(Dector3 position)
        {
            return position.x < 0 || position.x > Size.x
                                  || position.y < 0 || position.y > Size.y
                                  || position.z < 0 || position.z > Size.z;
        }


        private void IsPositionCorrect(Dector3 position)
        {
            if(!IsValidPosition(position))
            {
                throw new Exception($"Invalid tile coordinates: {position}");
            }
        }
    }
}