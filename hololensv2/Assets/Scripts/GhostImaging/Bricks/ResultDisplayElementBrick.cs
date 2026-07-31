/// <summary>
/// Enthält den Baustein zur Anzeige von Ghost-Imaging-Ergebnissen.
/// </summary>
using QuantenKoffer.Bricks;
using QuantenKoffer.Laser;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace GhostImaging.Bricks
{
    /// <summary>
    /// Ein Anzeigeelement, das den Trefferstatus im Ghost-Imaging-Ergebnis visualisiert.
    /// </summary>
    public class ResultDisplayElementBrick : Brick
    {
        private GameObject TopRenderer;
        private GameObject HitRenderer;

        /// <summary>
        /// Initialisiert die Renderer für den Normal- und Trefferzustand.
        /// </summary>
        private void Start()
        {
            TopRenderer = gameObject.GetNamedChild("oben");
            HitRenderer = gameObject.GetNamedChild("oben_hit");
            HitRenderer.SetActive(false);
        }

        /// <summary>
        /// Behandelt eingehende Laserstrahlen (derzeit keine Weiterleitung).
        /// </summary>
        /// <param name="beam">Der eingehende Laserstrahl.</param>
        /// <returns>Ein leeres Array von Laserstrahlen.</returns>
        public override LaserBeam[] HandleLaser(LaserBeam beam)
        {
            return new LaserBeam[] { };
        }

        /// <summary>
        /// Schaltet die Hervorhebung über das Kontextmenü ein.
        /// </summary>
        [ContextMenu("HighlightOn")]
        public void HLOn()
        {
            Highlight(true);
        }

        /// <summary>
        /// Schaltet die Hervorhebung über das Kontextmenü aus.
        /// </summary>
        [ContextMenu("HighlightOff")]
        public void HLOff()
        {
            Highlight(false);
        }

        /// <summary>
        /// Schaltet die Hervorhebung des Elements ein oder aus.
        /// </summary>
        /// <param name="active">Gibt an, ob die Hervorhebung aktiviert werden soll.</param>
        public void Highlight(bool active)
        {
            HitRenderer.SetActive(active);
            TopRenderer.SetActive(!active);
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