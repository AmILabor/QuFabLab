using AMI.Util;
using Microsoft.MixedReality.Toolkit;
using QuantenKoffer.Laser;
using UnityEngine;

namespace QuantenKoffer.Bricks
{
    /// <summary>
    /// Diese Klasse erweitert Brick um Reflektionshandling durch getOutVectors(Vector3 inVector).
    /// </summary>
    public class Brick45 : Brick
    {
        /// <summary>
        /// Wird aufgerufen, wenn der Strahl mit dem Spiegel kollidiert.
        /// Findet raus, wo der Strahl als Nächstes hin soll.
        /// </summary>
        /// <param name="beam"> Eingehender Strahl </param>
        /// <returns> Array ausgehender Strahlen </returns>
        public override LaserBeam[] HandleLaser(LaserBeam beam)
        {
            AMI.Util.Console.Log("Handling laser from beam", beam);
            LaserBeam[] beams = HandleLaserBase(beam, getOutVectors(beam.direction));
            foreach (var next_beam in beams)
            {
                next_beam.Draw();
            }
            return beams;
        }

        /// <summary>
        /// Berechnet den Winkel, in welchen der Strahl zu reflektieren ist. 
        /// </summary>
        /// <param name="inVector"></param>
        /// <returns></returns>
        protected Vector3[] getOutVectors(Vector3 inVector)
        {
            Vector3 normalizedVector = NormalizeInDirection(inVector);
            if (normalizedVector == Vector3.right)
                return new[] { DenormalizeInVector(Vector3.forward) };
            if (normalizedVector == Vector3.forward)
                return new[] { DenormalizeInVector(Vector3.right) };

            return new Vector3[] { };
        }
    }
}