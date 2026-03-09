using QuantenKoffer.Bricks;
using QuantenKoffer.Laser;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GhostImaging.Bricks
{
    public class OcclusionBrick : Brick
    {
        public override void HandleColision(LaserBeam beam)
        {
            Destroy(beam.gameObject);
        }

        public override LaserBeam[] HandleLaser(LaserBeam beam)
        {
            return new LaserBeam[] { };
        }


        protected override Vector3[] getOutVectors(Vector3 inVector)
        {
            return new Vector3[] { };
        }
    }
}