using Level.Generation.PathLayer.Path.Decisions;

namespace Level.Generation.PathLayer.Path.Snapshots
{
    public class PathSnapshot
    {
        // TODO: fill after generator is complete
        public PathMap map;
        public PathDecision decision;
        public int restores;

        public PathSnapshot(PathMap map, PathDecision decision)
        {
            this.map = map;
            this.decision = decision;
            restores = 0;
        }
    }
}