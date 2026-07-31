/// <summary>
/// Steuert die Geschwindigkeit der Laserstrahlen über einen Schieberegler oder Tasten.
/// Bietet Funktionen zum Erhöhen, Verringern, Pausieren und Zurücksetzen der Geschwindigkeit.
/// </summary>
using System;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;

namespace QuantenKoffer.Laser
{
    public enum SpeedSettings
    {
        increase,
        decrease,
        pause,
        normal
    }

    /// <summary>
    /// Steuert die Geschwindigkeit der Laserstrahlen über Schieberegler oder Tasten.
    /// </summary>
    public class LaserSpeedController : MonoBehaviour
    {
        private float currentValue = 0.5f;
        private float valueMemory = 0.5f;
        [SerializeField] public float SpeedSteps = 0.25f;
        [SerializeField] private Transform laserContainer;
        [SerializeField] private UnityEvent<float> SliderValueScaled;

        /// <summary>
        /// Prüft, ob aktive Laserstrahlen im Laser-Container vorhanden sind.
        /// </summary>
        /// <returns>True, wenn Laser aktiv sind</returns>
        private bool LasersAreActive()
        {
            try
            {
                laserContainer.GetChild(0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Wendet die Geschwindigkeitsänderung auf alle aktiven Laser an.
        /// </summary>
        /// <param name="speed">Neue Geschwindigkeit</param>
        private void ApplySpeedToActiveLasers(float speed)
        {
            LaserBeam[] lasers = laserContainer.GetComponentsInChildren<LaserBeam>();
            foreach (var laser in lasers)
            {
                laser.SetSpeed(speed);
            }
        }

        /// <summary>
        /// Skaliert den Slider-Wert (0-1) auf einen Geschwindigkeitsmultiplikator (0-2).
        /// </summary>
        /// <param name="value">Slider-Wert</param>
        /// <returns>Skalierter Geschwindigkeitsmultiplikator</returns>
        private float ScaleSetting(float value)
        {
            value = Math.Clamp(value, 0, 1);
            float scaledSpeedMultiplier = 2 * value;
            if (scaledSpeedMultiplier <= 0) scaledSpeedMultiplier = 0.00001f;
            return scaledSpeedMultiplier;
        }

        /// <summary>
        /// Wendet den skalierten Geschwindigkeitsmultiplikator auf die Laser an und löst das Event aus.
        /// </summary>
        /// <param name="scaledSpeedMultiplier">Skalierter Multiplikator</param>
        private void ApplySpeedModifier(float scaledSpeedMultiplier)
        {
            bool lasersRunning = LasersAreActive();
            if (lasersRunning)
                ApplySpeedToActiveLasers(scaledSpeedMultiplier);
            SliderValueScaled.Invoke(scaledSpeedMultiplier);
        }

        /// <summary>
        /// Wird beim Aktualisieren des Sliders aufgerufen und wendet die neue Geschwindigkeit an.
        /// </summary>
        /// <param name="sliderEvent">Slider-Event-Daten</param>
        public void OnSliderUpdate(SliderEventData sliderEvent)
        {
            float scaledSpeedMultiplier = ScaleSetting(sliderEvent.NewValue);
            ApplySpeedModifier(scaledSpeedMultiplier);
        }

        /// <summary>
        /// Behandelt Geschwindigkeitsänderungen basierend auf der gewählten Richtung (erhöhen, verringern, pausieren, normal).
        /// </summary>
        /// <param name="direction">Geschwindigkeitsrichtung</param>
        private void HandleSpeedChange(SpeedSettings direction)
        {
            switch (direction)
            {
                case SpeedSettings.normal:
                    if (currentValue == 0.0f)
                        currentValue = valueMemory;
                    else
                        currentValue = 0.5f;
                    break;
                case SpeedSettings.pause:
                    valueMemory = currentValue;
                    currentValue = 0.0f;
                    break;
                case SpeedSettings.increase:
                    currentValue += SpeedSteps;
                    valueMemory = currentValue;
                    break;
                case SpeedSettings.decrease:
                    currentValue -= SpeedSteps;
                    valueMemory = currentValue;
                    break;
            }

            float scaledSpeedMultiplier = ScaleSetting(currentValue);
            ApplySpeedModifier(scaledSpeedMultiplier);
        }

        /// <summary>
        /// Erhöht die Lasergeschwindigkeit.
        /// </summary>
        public void IncreaseSpeed()
        {
            HandleSpeedChange(SpeedSettings.increase);
        }

        /// <summary>
        /// Verringert die Lasergeschwindigkeit.
        /// </summary>
        public void DecreaseSpeed()
        {
            HandleSpeedChange(SpeedSettings.decrease);
        }

        /// <summary>
        /// Pausiert die Laserbewegung.
        /// </summary>
        public void PauseSpeed()
        {
            HandleSpeedChange(SpeedSettings.pause);
        }

        /// <summary>
        /// Setzt die Lasergeschwindigkeit auf den Normalwert zurück.
        /// </summary>
        public void NormalSpeed()
        {
            HandleSpeedChange(SpeedSettings.normal);
        }
    }
}