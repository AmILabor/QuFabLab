/// <summary>
/// Enthält den Pumpen-Baustein zur Erzeugung von Laserstrahlen im Ghost-Imaging-Experiment.
/// </summary>
using Microsoft.MixedReality.Toolkit;
using QuantenKoffer.Bricks;
using QuantenKoffer.Dialog;
using QuantenKoffer.Laser;
using UnityEngine;

namespace GhostImaging.Bricks
{
    /// <summary>
    /// Erzeugt Laserstrahlen für das Ghost-Imaging-Experiment (die Pumpe).
    /// </summary>
    public class Pump : Brick
    {
        [SerializeField] private GameObject LaserBeam;
        [SerializeField] private bool disabled = false;

        /// <summary>
        /// Startet das Erzeugen eines Laserstrahls über das Kontextmenü.
        /// </summary>
        [ContextMenu("Start")]
        public void CallStartBeam()
        {
            StartBeam();
        }

        /// <summary>
        /// Zeigt eine Erklärung für die Pumpe an.
        /// </summary>
        public void ShowExplanation()
        {
            AMI.Util.Console.Log("SHOWEXPLANTitle", "TITLE!");
            AMI.Util.Console.Log("SHOWEXPLANDescr", "DESCRIPTION");
        }

        /// <summary>
        /// Erzeugt einen Laserstrahl und leitet ihn an die Ausgangsvektoren weiter.
        /// </summary>
        /// <param name="beamParent">Optionaler Eltern-Transform für den Strahl.</param>
        /// <param name="speedMultiplier">Geschwindigkeitsmultiplikator für den Strahl.</param>
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

        /// <summary>
        /// Zeigt den Dialog für diesen Baustein an.
        /// </summary>
        public override void ShowDialog()
        {
            DialogHandler handler = dialogReference.GetComponent<DialogHandler>();
            handler.SetCurrentBrick(this);
            handler.DisableNavigationButtonsButDelete();
        }

        /// <summary>
        /// Setzt den deaktivierten Status zurück, wenn ein ausgehender Strahl sein Ziel erreicht.
        /// </summary>
        /// <param name="outgoingBeam">Der ausgehende Laserstrahl.</param>
        protected override void NotifySourceOnNextHit(LaserBeam outgoingBeam)
        {
            disabled = false;
        }

        /// <summary>
        /// Behandelt eingehende Laserstrahlen (zerstört sie).
        /// </summary>
        /// <param name="beam">Der eingehende Laserstrahl.</param>
        /// <returns>Ein leeres Array von Laserstrahlen.</returns>
        public override LaserBeam[] HandleLaser(LaserBeam beam)
        {
            AMI.Util.Console.Log("Destroying incoming Beam");
            beam.DestroyWhenDone();
            return new LaserBeam[] { };
        }

        /// <summary>
        /// Gibt den Ausgangsvektor für den Laserstrahl basierend auf der Rotation der Pumpe zurück.
        /// </summary>
        /// <param name="inVector">Der eingehende Richtungsvektor.</param>
        /// <returns>Ein Array mit einem Ausgangsvektor.</returns>
        protected override Vector3[] getOutVectors(Vector3 inVector)
        {
            return new[] { Vector3.left.RotateAround(Vector3.zero, transform.rotation.eulerAngles) };
        }
    }
}