using System;
using WaveFunctionCollapse3D.Util;

namespace WaveFunctionCollapse3D.PathLayer.PathGeneration
{
    public class PathNode
    {
        public Dector3 Position { get; set; }

        public PathNodeType Type { get; set; }
        /*
         * checkpoints are some important map segments (weapon spawn, cutscene, scripted fragment, start and end points),
         * which shouldn't have alternative paths 
         */
        public bool IsCheckpoint { get; set; }

        public bool CanGoToTop { get; set; }
        public bool CanGoToBottom { get; set; }
        public bool CanGoToForward { get; set; }
        public bool CanGoToLeft { get; set; }
        public bool CanGoToBack { get; set; }
        public bool CanGoToRight { get; set; }

        public PathNode()
        {
        }

        public PathNode(Dector3 position)
        {
            Position = position;
        }

        public void SetDirectionAccess(int index, bool value)
        {
            if (index < 0 || index > 5)
                throw new Exception($"Invalid direction index: {index}");
            
            switch (index)
            {
                case 0:
                {
                    CanGoToTop = value;
                    break;
                }
                case 1:
                {
                    CanGoToBottom = value;
                    break;
                }
                case 2:
                {
                    CanGoToForward = value;
                    break;
                }
                case 3:
                {
                    CanGoToLeft = value;
                    break;
                }
                case 4:
                {
                    CanGoToBack = value;
                    break;
                }
                case 5:
                {
                    CanGoToRight = value;
                    break;
                }
                
            }
        }
        public bool GetDirectionAccess(int index)
        {
            if (index < 0 || index > 5)
                throw new Exception($"Invalid direction index: {index}");
            
            switch (index)
            {
                case 0:
                {
                    return CanGoToTop;
                }
                case 1:
                {
                    return CanGoToBottom;
                }
                case 2:
                {
                    return CanGoToForward;
                }
                case 3:
                {
                    return CanGoToLeft;
                }
                case 4:
                {
                    return CanGoToBack;
                }
                case 5:
                {
                    return CanGoToRight;
                }
                
            }

            /*
             * wha
             */

            throw new Exception("Some shit happened. GetDirectionAccess ended with this");
        }
        public void AllowGoToAnyDirection()
        {
            CanGoToTop = true;
            CanGoToBottom = true;
            CanGoToForward = true;
            CanGoToLeft = true;
            CanGoToBack = true;
            CanGoToRight = true;
        }

        public override string ToString()
        {
            return $"PathNode ({Position.X};{Position.Y},{Position.Z})";
        }
    }
}