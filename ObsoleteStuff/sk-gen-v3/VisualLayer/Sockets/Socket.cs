using System;
using System.Text.RegularExpressions;

namespace WaveFunctionCollapse3D.VisualLayer.Sockets
{
    public class Socket
    {
        public int Number { get; }

        public SocketType Type { get; }

        public int Rotation { get; set; }

        public bool Symmetrical { get; }

        public bool Flipped { get; }


        private const string invalidSocketRegex = "-.*";

        private const string sideSocketRegex = "([0-9]+)((f)|(s))";

        private const string socketNumberRegex = "[0-9]+";

        private const string nonSideSocketRegex = "([0-9]+)_([0-3])";

        public Socket(int number = -1, SocketType socketType = SocketType.Side, int rotation = 0,
            bool symmetrical = true, bool flipped = true)
        {
            Number = number;
            Type = socketType;
            Rotation = rotation;
            Symmetrical = symmetrical;
            Flipped = flipped;
        }

        public static Socket Parse(string s)
        {
            // first, check if it is invalid
            if (Regex.IsMatch(s, invalidSocketRegex))
                return new Socket();
            Match sideMatch = Regex.Match(s, sideSocketRegex);
            if (IsFullMatch(s, sideMatch.Value))
            {
                return ParseSideSocket(s);
            }

            Match nonSideMatch = Regex.Match(s, nonSideSocketRegex);
            if (IsFullMatch(s, nonSideMatch.Value))
            {
                return ParseNonSideSocket(s);
            }

            Socket socket = new Socket();
            throw new NotImplementedException();
            return socket;
        }

        private static Socket ParseSideSocket(string s)
        {
            Match sideMatch = Regex.Match(s, sideSocketRegex);
            Match numberMatch = Regex.Match(s, socketNumberRegex);
            int number;
            if (int.TryParse(sideMatch.Value, out number))
            {
                int letterIndex = numberMatch.Index + numberMatch.Length + 1;
                if (s.Length == letterIndex + 1)
                {
                    if (s[letterIndex] == 's')
                    {
                        return new Socket(number, SocketType.Side, 0, true, false);
                    }

                    if (s[letterIndex] == 'f')
                    {
                        return new Socket(number, SocketType.Side, 0, false, false);
                    }
                }
                else
                {
                    return new Socket(number, SocketType.Side, 0, false, false);
                }
            }

            throw new Exception($"Error while parsing socket string (recognized as side socket): {s}");
        }

        private static Socket ParseNonSideSocket(string s)
        {
            Match nonSideMatch = Regex.Match(s, nonSideSocketRegex);
            Match numberMatch = Regex.Match(s, socketNumberRegex);
            int number;
            if (int.TryParse(nonSideMatch.Value, out number))
            {
                int letterIndex = numberMatch.Index + numberMatch.Length + 1;
                if (s.Length == letterIndex + 1)
                {
                    if (s[letterIndex] == 's')
                    {
                        return new Socket(number, SocketType.Side, 0, true, false);
                    }

                    if (s[letterIndex] == 'f')
                    {
                        return new Socket(number, SocketType.Side, 0, false, false);
                    }
                }
                else
                {
                    return new Socket(number, SocketType.Side, 0, false, false);
                }
            }

            throw new Exception($"Error while parsing socket string (recognized as non-side socket): {s}");
        }


        private static bool IsFullMatch(string fullString, string match)
        {
            return fullString.Equals(match);
        }

        public bool IsInvalid()
        {
            // слышь сам инвалид у меня просто прикус неправильный
            return Number == -1;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case SocketType.Side:
                {
                    if (Symmetrical)
                    {
                        return Number + "s";
                    }

                    return Number + (Flipped ? "f" : "");
                }

                case SocketType.NonSide:
                {
                    return Number + "_" + Rotation;
                }
            }

            return "-1";
        }
    }
}