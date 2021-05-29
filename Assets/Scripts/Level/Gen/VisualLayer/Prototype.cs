using System;
using System.Collections.Generic;
using Level.Gen.Util;
using WaveFunctionCollapse3D.VisualLayer.Sockets;

namespace Level.Gen.VisualLayer
{
    [Serializable]
    public class Prototype
    {
        public string Name { get; }
        public float Weight { get; }

        public Dector3 Size { get; }


        public int Rotation
        {
            get => Rotation;

            set
            {
                if (value <= 0 || value > 3)
                    throw new Exception($"Invalid rotation: {value}");
                for (int i = 0; i < 2; i++)
                {
                    Socket socket = Socket.Parse(Sockets[i]);
                    if (socket.Type == SocketType.NonSide)
                    {
                        socket.Rotation = value;
                    }

                    Sockets[i] = socket.ToString();
                }
            }
        }

        public List<Dector3> SocketsPositions { get; }
        private List<string> Sockets { get; }

        /* with rotation = 0
         * 0 Top
         * 1 Bottom
         * 2 Forward
         * 3 Left
         * 4 Back
         * 5 Right
         * with rotation = 1 we turn 90 deg to right, so we can conveniently just offset values
         * 0 Top
         * 1 Bottom
         * 2 Forward + 1 = 3
         * 3 Left + 1 = 4
         * 4 Back + 1 = 5
         * 5 Right + 1 = 2
         * etc.
         */


        /*
         * Adjacency rules:
         * side socket: X(f)(s), where X - index of socket, f - flipped, s - symmetrical
         * for example, "5f" - asymmetrical socket with index 5, flipped, means it could be connected to socket "5"
         * "6s" - symmetrical
         * -1 - invalid (no connections are available)
         * top and bottom sockets: 7_3 - where 3 is rotation (0, 1, 2, 3 => 0, 90, 180, 270 degrees)
         */

        public Prototype(string name, List<string> sockets, List<Dector3> socketsPositions, Dector3 size)
        {
            Name = name;
            Sockets = sockets;
            Size = size;
            SocketsPositions = socketsPositions;

            if (sockets.Count > GetSurfaceSquare())
            {
                throw new Exception($"Incorrect number of sockets: {sockets.Count}");
            }
        }

        // TODO: ой блять, эту хуйню из массива таскать, ещё и с учетом поворота, сукаааааа
        public string GetSocket(SocketPosition position)
        {
            /*
             * top is always top
             * bottom is always bottom
             * (c) Confucius
             */
            if (position == SocketPosition.Bottom || position == SocketPosition.Top)
                return Sockets[(int) position];

            position -= 2;
            int index = 2 + ((int) position + Rotation) % 4;

            return Sockets[index];
        }

        
    }
}