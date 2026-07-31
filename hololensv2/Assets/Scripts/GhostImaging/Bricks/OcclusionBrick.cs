/// <summary>
/// Enthält die verschiedenen Baustein-Typen für das Ghost-Imaging-Experiment.
/// </summary>
using QuantenKoffer.Bricks;
using QuantenKoffer.Laser;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GhostImaging.Bricks
{
    /// <summary>
    /// Ein Baustein, der eingehende Laserstrahlen absorbiert (okkludiert).
    /// </summary>
    public class OcclusionBrick : Brick
    {
        /// <summary>
        /// Zerstört den eingehenden Laserstrahl bei Kollision.
        /// </summary>
        /// <param name="beam">Der eingehende Laserstrahl.</param>
        public override void HandleColision(LaserBeam beam)
        {
            Destroy(beam.gameObject);
        }

        /// <summary>
        /// Gibt ein leeres Array zurück, da keine Strahlen weitergeleitet werden.
        /// </summary>
        /// <param name="beam">Der eingehende Laserstrahl.</param>
        /// <returns>Ein leeres Array von Laserstrahlen.</returns>
        public override LaserBeam[] HandleLaser(LaserBeam beam)
        {
            return new LaserBeam[] { };
        }


        /// <summary>
        /// Gibt ein leeres Array von Ausgangsvektoren zurück.
        /// </summary>
        /// <param name="inVector">Der eingehende Richtungsvektor.</param>
        /// <returns>Ein leeres Array von Vektoren.</returns>
        protected override Vector3[] getOutVectors(Vector3 inVector)
        {
            return new Vector3[] { };
        }
    }
}