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

    public class LaserSpeedController : MonoBehaviour
    {
        private float currentValue = 0.5f;
        private float valueMemory = 0.5f;
        [SerializeField] public float SpeedSteps = 0.25f;
        [SerializeField] private Transform laserContainer;
        [SerializeField] private UnityEvent<float> SliderValueScaled;

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

        private void ApplySpeedToActiveLasers(float speed)
        {
            LaserBeam[] lasers = laserContainer.GetComponentsInChildren<LaserBeam>();
            foreach (var laser in lasers)
            {
                laser.SetSpeed(speed);
            }
        }

        private float ScaleSetting(float value)
        {
            value = Math.Clamp(value, 0, 1);
            float scaledSpeedMultiplier = 2 * value;
            if (scaledSpeedMultiplier <= 0) scaledSpeedMultiplier = 0.00001f;
            return scaledSpeedMultiplier;
        }

        private void ApplySpeedModifier(float scaledSpeedMultiplier)
        {
            bool lasersRunning = LasersAreActive();
            if (lasersRunning)
                ApplySpeedToActiveLasers(scaledSpeedMultiplier);
            SliderValueScaled.Invoke(scaledSpeedMultiplier);
        }

        public void OnSliderUpdate(SliderEventData sliderEvent)
        {
            float scaledSpeedMultiplier = ScaleSetting(sliderEvent.NewValue);
            ApplySpeedModifier(scaledSpeedMultiplier);
        }

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

        public void IncreaseSpeed()
        {
            HandleSpeedChange(SpeedSettings.increase);
        }

        public void DecreaseSpeed()
        {
            HandleSpeedChange(SpeedSettings.decrease);
        }

        public void PauseSpeed()
        {
            HandleSpeedChange(SpeedSettings.pause);
        }

        public void NormalSpeed()
        {
            HandleSpeedChange(SpeedSettings.normal);
        }
    }
}