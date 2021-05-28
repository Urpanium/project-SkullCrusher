using System;

namespace WaveFunctionCollapse3D.Util
{
    public class Map3<T> where T : class
    {
        public readonly Dector3 Size;
        private T[,,] _map;

        public Map3(Dector3 size)
        {
            _map = new T[size.X, size.Y, size.Z];
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
            return _map[position.X, position.Y, position.Z];
        }

        public void SetTile(Dector3 position, T tile)
        {
            IsPositionCorrect(position);
            _map[position.X, position.Y, position.Z] = tile;
        }

        public void SetTile(int x, int y, int z, T tile)
        {
            Dector3 position = new Dector3(x, y, z);
            IsPositionCorrect(position);
            _map[position.X, position.Y, position.Z] = tile;
        }

        public void Clear()
        {
            _map = new T[Size.X, Size.Y, Size.Z];
        }

        public bool IsValidPosition(Dector3 position)
        {
            return position.X < 0 || position.X > Size.X
                                  || position.Y < 0 || position.Y > Size.Y
                                  || position.Z < 0 || position.Z > Size.Z;
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