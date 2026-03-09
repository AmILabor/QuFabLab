using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Rendering;

namespace AMI.DoppelExperiment
{ 
    public class InterferencePatternMenu : MonoBehaviour
    {

        [SerializeField] InterferencePattern intPat;
        [SerializeField] ParticleBeam particleBeam;
        private bool showIntPattern = true;
        [SerializeField] Transform slitLeft,slitMiddle,slitRight,slitTop,slitBot;
        float slitLeftStart,slitMiddleStart,slitRightStart,slitTopStart,slitBotStart;

        private void Start() {
            slitLeftStart = slitLeft.localPosition.x;
            slitMiddleStart = slitMiddle.localScale.x;
            slitRightStart = slitRight.localPosition.x;
            slitTopStart = slitTop.localScale.y;
            slitBotStart = slitBot.localScale.y;
            ScaleSlit();
        }
        public void ScaleSlit(){
            //Left Slid
            var pos = slitLeft.localPosition;
            pos.x = slitLeftStart + (intPat.SlitDistance / 10);
            slitLeft.localPosition = pos;
            //Right Slid
            pos = slitRight.localPosition;
            pos.x = slitRightStart - (intPat.SlitDistance / 10);
            slitRight.localPosition = pos;
            //Middle Slit
            pos = slitMiddle.localScale;
            pos.x = slitMiddleStart + (2f*intPat.SlitDistance / 10) - (2f*intPat.SlitWidth);
            slitMiddle.localScale = pos;
            //Top Slit
            pos = slitTop.localScale;
            pos.y = slitTopStart + (2f*intPat.SlitDistance / 10);
            slitTop.localScale = pos;
            //Bot Slit
            pos = slitBot.localScale;
            pos.y = slitBotStart + (2f*intPat.SlitDistance / 10);
            slitBot.localScale = pos;

            particleBeam.ShootParticles();
            UpdateInterferencePattern();
        }
        public void IncreaseSlitDistance()
        {
            intPat.SlitDistance += 500;        
            if (intPat.SlitDistance > 5000)
            {
                intPat.SlitDistance = 5000;

            }else
            {
                ScaleSlit();
            }
            Debug.Log("SlitDistance: " + intPat.SlitDistance);
        }
        public void DecreaseSlitDistance()
        {
            if(slitMiddleStart + (2f*intPat.SlitDistance / 10) - (2f*intPat.SlitWidth) > slitMiddleStart)
            {
                intPat.SlitDistance -= 500;
                
                if (intPat.SlitDistance <= 0)
                {
                    intPat.SlitDistance = 500;
                    

                }else
                {
                    ScaleSlit();
                }
                Debug.Log("SlitDistance: " + intPat.SlitDistance);

            }else{
                Util.Console.Log("InterferencePatternMenu","SlitWidth too big for SlitDistance");
            }
        }

        public void IncreaseSlitWidth()
        {
            if(slitMiddleStart + (2f*intPat.SlitDistance / 10) - (2f*intPat.SlitWidth) > slitMiddleStart)
            {
                intPat.SlitWidth += 50;
                
                if (intPat.SlitWidth > 500)
                {
                    intPat.SlitWidth = 500;

                }else
                {
                    ScaleSlit();
                }

                Debug.Log("SlitWidth: " + intPat.SlitWidth);

            }else
            {
                Util.Console.Log("InterferencePatternMenu","SlitWidth too small for SlitDistance");
            }
        }
        public void DecreaseSlitWidth()
        {

            intPat.SlitWidth -= 50;
            if (intPat.SlitWidth <= 0)
            {
                intPat.SlitWidth = 50;

            }else
            {
                ScaleSlit();
            }

            Debug.Log("SlitWidth: " + intPat.SlitWidth);

        }


        public void IncreaseWavelength()
        {

            intPat.Wavelength += 50;

            // 750 nm = Max. wavelength of visible light (the reddest light seeable)
            // 380 because increment is 50 nm and minimum is 380
            if (intPat.Wavelength > 780)
            {
                intPat.Wavelength = 780;

            }else
            {
                particleBeam.ShootParticles();
                UpdateInterferencePattern();
            }

            Debug.Log("Wavelength: " + intPat.Wavelength);
        }
        public void DecreaseWavelength()
        {

            intPat.Wavelength -= 50;
            

            // 380 nm = Min. wavelength of visible light (the purpelest light seeable)
            if (intPat.Wavelength <= 380)
            {
                intPat.Wavelength = 380;

            }else
            {
                particleBeam.ShootParticles();
                UpdateInterferencePattern();
            }

            Debug.Log("Wavelength: " + intPat.Wavelength);

        }

        public void IncreaseResolution()
        {
            switch (intPat.ScreenWidthInPixels)
            {
                case 3840:
                    intPat.ScreenWidthInPixels = 640;
                    intPat.ScreenHeightInPixels = 480;
                    break;
                case 640:
                    intPat.ScreenWidthInPixels = 1280;
                    intPat.ScreenHeightInPixels = 720;
                    break;
                case 1280:
                    intPat.ScreenWidthInPixels = 1920;
                    intPat.ScreenHeightInPixels = 1080;
                    break;
                case 1920:
                    intPat.ScreenWidthInPixels = 3840;
                    intPat.ScreenHeightInPixels = 2160;
                    break;
            }

        }

        public void UpdateInterferencePattern()
        {

            Vector2[] arr = intPat.updateInteferencePattern();
            if (showIntPattern == true)
            {
                intPat.drawInterferencePattern(arr);
            }
            else
            {
                intPat.drawInterferencePattern(arr, false);
            }
            intPat.drawDistributionCurve(arr);
        }

        public void ShowInterferencePattern()
        {
            if (showIntPattern == true)
            {
                showIntPattern = false;
                UpdateInterferencePattern();
            }
            else
            {
                showIntPattern = true;
                UpdateInterferencePattern();
            }
        }
        public void ShowProbabilityWave()
        {

            if (intPat.firstLineRenderer.enabled == true && intPat.secondLineRenderer.enabled == true)
            {
                intPat.firstLineRenderer.enabled = false;
                intPat.secondLineRenderer.enabled = false;
            }
            else
            {
                intPat.firstLineRenderer.enabled = true;
                intPat.secondLineRenderer.enabled = true;
            }
        }
    }
}
