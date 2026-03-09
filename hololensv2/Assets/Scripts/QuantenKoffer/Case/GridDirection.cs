namespace QuantenKoffer.Case
{
    /// <summary>
    /// Cardinal directions of the grid (North,East,South,West)
    /// </summary>
    public enum GridDirection
    {
        N,
        E,
        S,
        W
    }

    /// <summary>
    /// Helper functions for GridDirection enum
    /// </summary>
    public static class GridDirectionExtensions
    {
        public static GridDirection Opposite(this GridDirection direction)
        {
            return (int)direction < 2 ? (direction + 2) : (direction - 2);
        }

        public static GridDirection Previous(this GridDirection direction)
        {
            return direction == GridDirection.N ? GridDirection.W : (direction - 1);
        }

        public static GridDirection Next(this GridDirection direction)
        {
            return direction == GridDirection.W ? GridDirection.N : (direction + 1);
        }
    }
}