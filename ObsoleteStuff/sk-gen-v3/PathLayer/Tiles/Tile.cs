namespace Level.Generation.PathLayer.Tiles
{
    /* Tile is not same class as Prototype
     * Tile represents an area in level space and defines some
     * initial generation condition, such as
     * where level starts and end,
     * where player gets weapons etc
     */
    
    // TODO: consider removing this shit
    public class Tile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TileType Type { get; set; }
    }
}