/// <summary>
/// Periskop-Baustein. Sendet Laserstrahlen vom Rand des Spielfelds aus und stoppt bei Kollision mit einem anderen Baustein.
/// Implementiert die Laserstart-Logik und die Benachrichtigung bei erfolgreicher Weiterleitung.
/// </summary>
using Microsoft.MixedReality.Toolkit;
using QuantenKoffer.Dialog;
using QuantenKoffer.Laser;
using UnityEngine;

namespace QuantenKoffer.Bricks
{
    /// <summary>
    /// Periskop-Baustein, der Laserstrahlen vom Spielfeldrand aussendet und bei Kollision stoppt.
    /// </summary>
    public class Periscope : Brick
    {
        [SerializeField] private GameObject LaserBeam;
        [SerializeField] private bool disabled = false;

        /// <summary>
        /// Startet den Laserstrahl des Periskops (für ContextMenu).
        /// </summary>
        [ContextMenu("Start")]
        public void CallStartBeam()
        {
            StartBeam();
        }

        /// <summary>
        /// Setzt das Laserstrahl-Prefab für das Periskop.
        /// </summary>
        /// <param name="lb">Laserstrahl-Prefab</param>
        public void SetLaserBeam(GameObject lb)
        {
            LaserBeam = lb;
        }

        public void StartBeam(Transform beamParent = null, float speedMultiplier = 1.0f)
        {
            if (!disabled)
            {
                GameObject beamGo = GameObject.Instantiate(LaserBeam);
                LaserBeam beam = beamGo.GetComponent<LaserBeam>();
                LaserBeam[] beams = HandleLaserBase(beam, getOutVectors(beam.direction));
                foreach (var _beam in beams)
                {
                    if (beamParent != null) _beam.transform.parent = beamParent;
                    _beam.SetSpeed(speedMultiplier);
                    _beam.Draw();
                }

                disabled = beams.Length > 0;
                Destroy(beamGo);
            }
        }

        public override void ShowDialog()
        {
            DialogHandler handler = dialogReference.GetComponent<DialogHandler>();
            handler.SetCurrentBrick(this);
            handler.DisableNavigationButtonsButDelete();
        }

        /// <summary>
        /// Verhindert, dass ein neuer Beam gestartet wird, bevor dieser beim nächsten Spielstein angekommen ist.
        /// </summary>
        /// <param name="outgoingBeam"> </param>
        // outgoingBeam wird nicht verwendet (refactoring)
        protected override void NotifySourceOnNextHit(LaserBeam outgoingBeam)
        {
            disabled = false;
        }

        public override LaserBeam[] HandleLaser(LaserBeam beam)
        {
            AMI.Util.Console.Log("Destroying incoming Beam");
            beam.DestroyWhenDone();
            return new LaserBeam[] { };
        }

        protected override Vector3[] getOutVectors(Vector3 inVector)
        {
            return new[] { Vector3.right.RotateAround(Vector3.zero, transform.rotation.eulerAngles) };
        }
    }
}