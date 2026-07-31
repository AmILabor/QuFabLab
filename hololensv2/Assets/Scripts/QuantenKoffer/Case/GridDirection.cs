/// <summary>
/// Enthält die Aufzählung der Himmelsrichtungen (N, E, S, W) für das Spielfeldraster
/// sowie Erweiterungsmethoden zur Richtungsnavigation (Gegenteil, Vorherige, Nächste).
/// </summary>
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
        /// <summary>
        /// Gibt die entgegengesetzte Himmelsrichtung zurück.
        /// </summary>
        /// <param name="direction">Aktuelle Richtung</param>
        /// <returns>Entgegengesetzte Richtung</returns>
        public static GridDirection Opposite(this GridDirection direction)
        {
            return (int)direction < 2 ? (direction + 2) : (direction - 2);
        }

        /// <summary>
        /// Gibt die vorherige Himmelsrichtung (im Uhrzeigersinn rückwärts) zurück.
        /// </summary>
        /// <param name="direction">Aktuelle Richtung</param>
        /// <returns>Vorherige Richtung</returns>
        public static GridDirection Previous(this GridDirection direction)
        {
            return direction == GridDirection.N ? GridDirection.W : (direction - 1);
        }

        /// <summary>
        /// Gibt die nächste Himmelsrichtung (im Uhrzeigersinn vorwärts) zurück.
        /// </summary>
        /// <param name="direction">Aktuelle Richtung</param>
        /// <returns>Nächste Richtung</returns>
        public static GridDirection Next(this GridDirection direction)
        {
            return direction == GridDirection.W ? GridDirection.N : (direction + 1);
        }
    }
}