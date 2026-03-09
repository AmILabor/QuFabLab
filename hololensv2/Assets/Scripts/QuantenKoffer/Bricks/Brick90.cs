using Microsoft.MixedReality.Toolkit;
using QuantenKoffer.Laser;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace QuantenKoffer.Bricks
{
    /// <summary>
    /// 90° Spiegel
    /// </summary>
    public class Brick90 : Brick
    {
        [SerializeField] public float additionalOffset = 0;
        [SerializeField] public float maxOffset = 0.0125f;
        [SerializeField] public float minOffset = 0.0f;
        [SerializeField] public float OffsetTestValue = 0.25f;
        private Vector3 OriginalCenter = Vector3.one; //!< Position von Child-GO center
        private Transform CenterTransform; //!< \see Brick::centerTransform///

        /// <summary>
        /// Greift sich die Position von Child-GO center
        /// </summary>
        public void Start()
        {
            CenterTransform = gameObject.GetNamedChild("Center").transform;
            OriginalCenter = CenterTransform.transform.position;
        }

        /// <summary>
        /// \see Brick::HandleLaser(LaserBeam beam) für eine Zusammenfassung der Methode
        /// und \see BeamSplitter::HandleLaser(LaserBeam beam) für eine detailliertere Erklärung
        /// </summary>
        /// <param name="beam">Eingehender Strahl</param>
        /// <returns></returns>
        public override LaserBeam[] HandleLaser(LaserBeam beam)
        {
            LaserBeam[] beams = HandleLaserBase(beam, getOutVectors(beam.direction));
            foreach (var next_beam in beams)
            {
                next_beam.Draw();
            }

            return beams;
        }

        /// <summary>
        /// Bewege das Center-Child-Objekt, um den Spiegelabstand zu variieren.
        /// </summary>
        /// <param name="offset">Rotationsfaktor</param>
        private void MoveCenter(float offset)
        {
            Vector3 move = offset *
                           Vector3.forward.RotateAround(Vector3.zero, transform.rotation.eulerAngles);
            Vector3 newPosition = OriginalCenter + move;
            CenterTransform.transform.position = newPosition;
            additionalOffset = offset;
        }

        /// <summary>
        /// Berechnet für MoveCenter(float offset) den offset-Wert.
        /// </summary>
        /// <param name="setting">Offset-Wert</param>
        public override void ApplySetting(float setting)
        {
            AMI.Util.Console.Log("Setting Value to: " + setting);
            SettingValue = setting;
            float offset = setting * (maxOffset * 2) - maxOffset;
            MoveCenter(offset);
        }

        /// <summary>
        /// Returnt den float Setting, welcher als Offset verwendet wird.
        /// </summary>
        /// <returns>Offset</returns>
        public override float GetSetting()
        {
            return SettingValue;
        }

        [ContextMenu("TestMoveByAdditionOffset")]
        private void MoveCenterByAdditionalOffset()
        {
            MoveCenter(additionalOffset);
        }

        [ContextMenu("TestOffset")]
        private void TestOffset()
        {
            ApplySetting(OffsetTestValue);
        }

        [ContextMenu("ResetOffset")]
        private void ResetOffset()
        {
            ApplySetting(0.5f);
        }

        /// <summary>
        /// \see Brick::getOutVectors(Vector3 inVector)
        /// </summary>
        /// <param name="inVector"> Vektor des eingehenden Strahles</param>
        /// <returns> Vektoren der auszugehenden Strahlen </returns>
        protected override Vector3[] getOutVectors(Vector3 inVector)
        {
            Vector3 normalizedVector = NormalizeInDirection(inVector);
            if (normalizedVector == Vector3.forward)
                return new[] { DenormalizeInVector(Vector3.forward) };

            return new Vector3[] { };
        }
    }
}