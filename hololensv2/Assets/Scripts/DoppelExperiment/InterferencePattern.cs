/// <summary>
/// Enthält die Klasse zur Berechnung und Darstellung des Interferenzmusters beim Doppelspalt-Experiment.
/// </summary>
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using static System.Net.Mime.MediaTypeNames;


namespace AMI.DoppelExperiment{  
    public class InterferencePattern : MonoBehaviour
    {
        public LineRenderer firstLineRenderer;
        public LineRenderer secondLineRenderer;

        [SerializeField] GameObject wall;

        [SerializeField]
        private int screenWidthInPixels, screenHeightInPixels, slitDistance, slitWidth, wavelength;
        private int points;

        [SerializeField]
        private float scalingFactor;
        [SerializeField] Color pointColor = Color.yellow;
        [SerializeField] Color wallColor = Color.white;
        [SerializeField]
        private Vector2[] tmpArr;
        private Vector2[] valueArr;

        [SerializeField]
        private List<Tuple<Vector3, Vector3>> pixelPos = new List<Tuple<Vector3, Vector3>>();


        [SerializeField]
        float INull;

        // public get and set methods for other classes to use
        public int ScreenWidthInPixels
        {
            get
            {
                return this.screenWidthInPixels;
            }
            set
            {
                screenWidthInPixels = value;
            }
        }
        public int ScreenHeightInPixels
        {
            get
            {
                return this.screenHeightInPixels;
            }
            set
            {
                screenHeightInPixels = value;
            }
        }
        public int SlitDistance
        {
            get
            {
                return this.slitDistance;
            }
            set
            {

                slitDistance = value;
            }
        }
        public int SlitWidth
        {
            get
            {
                return this.slitWidth;
            }
            set
            {
                slitWidth = value;
            }
        }

        public int Wavelength
        {
            get
            {
                return this.wavelength;
            }
            set
            {
                wavelength = value;
            }
        }

        public Vector2[] ValueArr
        {
            get
            {
                return this.valueArr;
            }
            set
            {
                valueArr = value;
            }
        }


    /*  public int Points
        {
            get
            {
                return this.points;
            }
            set
            {
                points = value;
            }
        }
    */

        public List<Tuple<Vector3, Vector3>> PixelPos
        {
            get
            {
                return this.pixelPos;
            }
            set
            {
                pixelPos = value;
            }
        }



        void Start()
        {
            points = (int)(screenWidthInPixels / 2);
            Debug.Log("Points: " + points);
            
            valueArr = new Vector2[points];
            valueArr = calculateInterferencePattern();

            Texture2D tex = WriteArrayTotexture2D(ValueArr, screenWidthInPixels, screenHeightInPixels, wallColor);


            drawInterferencePattern(valueArr);
            drawDistributionCurve(valueArr);
        }


        float calculateYCoordinate(float alpha)
        {
            float I_alpha;
            float I_null = INull; //1.1f;
            int lambda = wavelength; // 569;
            int d = slitDistance; // 5000;
            int b = slitWidth;  //500;
            float sin = Mathf.Sin(alpha);


            float aa = Mathf.Sin(2 * Mathf.PI * d * sin / lambda);
            float bb = Mathf.Sin(Mathf.PI * d * sin / lambda);
            float cc = Mathf.Sin(Mathf.PI * b * sin / lambda);
            float dd = Mathf.PI * b * sin / lambda;
            I_alpha = I_null * Mathf.Pow(aa / bb, 2) * Mathf.Pow(cc / dd, 2);

            return I_alpha;

            // Source: https://www.leifiphysik.de/optik/beugung-und-interferenz/grundwissen/doppelspalt

        }

        Vector2[] calculateInterferencePattern()
        {
            float x;
            float y;
            int[] maxArr = new int[points];
            firstLineRenderer.positionCount = points;
            secondLineRenderer.positionCount = points;
            Debug.Log(points);
            Vector2[] valArr = new Vector2[points];

            Debug.Log(valArr);
            // Set valArr[0] and Draw for the x-Coordinate 0 because otherwise it is NaN
            valArr[0] = new Vector2(0.0f, INull * 4.0f);
            Debug.Log("Mit null");
            Debug.Log(valArr);

            for (int currentPoint = 1; currentPoint < points; currentPoint++)
            {
                // Calculate x-coordinate of the Interference-Wave
                float progress = (float)currentPoint / points;
                x = Mathf.Lerp(0, 1, progress); // Get x-value thorugh interpolation

                // Calculate y-coordinate of the Interference-Wave
                y = calculateYCoordinate(x);

                // Save values in array to draw the Interference Pattern later
                // Only half the Interference Pattern is calculated because the entire Pattern is y-axis-mirrored at the center
                // Look at https://www.leifiphysik.de/optik/beugung-und-interferenz/grundwissen/doppelspalt for reference (val Arr contains the (+x,+y) quadrant)
                valArr[currentPoint] = new Vector2(x, y); 
            }

            return valArr;
        }


        public Vector2[] updateInteferencePattern()
        {
            Vector2[] valueArr = new Vector2[points];
            return calculateInterferencePattern();
        }

        public void drawInterferencePattern(int x, int y)
        {

            Texture2D tex = WriteArrayTotexture2D(valueArr, ScreenWidthInPixels, ScreenHeightInPixels, pointColor);
            
            // Get MeshRenderer of Screen/Wall and apply the texture to it
            MeshRenderer wallRend = wall.GetComponent<MeshRenderer>();
            // Texture2D tex = drawPixel(x, y, Color.yellow);
            wallRend.material.mainTexture = tex;


        }


        public void drawInterferencePattern(Vector2[] valueArr)
        {

            Texture2D tex = WriteArrayTotexture2D(valueArr, ScreenWidthInPixels, ScreenHeightInPixels, pointColor);
            //Texture2D tex = drawPixel(x, y, Color.yellow);
            // Get MeshRenderer of Screen/Wall and apply the texture to it
            MeshRenderer wallRend = wall.GetComponent<MeshRenderer>();
            wallRend.material.mainTexture = tex;


        }

        // Replace Pattern with original Texture
        public void drawInterferencePattern(Vector2[] valueArr, bool showPattern)
        {
            // Get MeshRenderer of Screen/Wall and apply the texture to it
            MeshRenderer wallRend = wall.GetComponent<MeshRenderer>();
            Texture2D tex = wallRend.GetComponent<Texture2D>();
            wallRend.material.mainTexture = tex;
        }




        public void drawDistributionCurve(Vector2[] valArr)
        {
            firstLineRenderer.SetPosition(0, new Vector3(0, INull * 4, 0));
            secondLineRenderer.SetPosition(0, new Vector3(0, INull * 4, 0));
            valArr[0] = new Vector2(0.0f, INull * 4.0f);


            for (int currentPoint = 1; currentPoint < points; currentPoint++)
            {

                // Draw distribtution curve
                firstLineRenderer.SetPosition(currentPoint, new Vector3(valArr[currentPoint].x, valArr[currentPoint].y, 0.001f));
                secondLineRenderer.SetPosition(currentPoint, new Vector3(-valArr[currentPoint].x, valArr[currentPoint].y, 0.001f));
            }

        }
        Texture2D WriteArrayTotexture2D(Vector2[] arr, int width, int height, Color color)
        {
            // Color color = Color.yellow; // Set color
            Texture2D texture = new Texture2D(width, height); // Create New Texture
            float maxArr = arr[0].y; // Max Value in Array is always at arr[0]
            //color = Color.white;
            // Draw Right Side of the Interference Pattern
            for (int x = 0; x < texture.width / 2; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    //Debug.Log("In Color If arr.x: " + arr[x].x.ToString() + "::" + arr[x].y.ToString() + " on " + x + ":" + y);

                    if (UnityEngine.Random.Range(0.0f, maxArr) <= arr[x].y)
                    {
                        texture.SetPixel((texture.width / 2) - x, y, color); // Draw from center (x = width/2) to right edge
                        //pixelPos.Add(new Vector3((texture.width / 2) - x, y, 0));
                        pixelPos.Add(new Tuple<Vector3, Vector3>(arr[x], new Vector3((texture.width / 2) - x, y,0.0f)));
                    }
                    else
                    {
                        texture.SetPixel((texture.width / 2) - x, y, wallColor);
                    }
                }
            }

            // Draw Left Side of the Interference Pattern
            for (int x = texture.width / 2; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    //Debug.Log("In Color If arr.x: " + arr[x].x.ToString() + "::" + arr[x].y.ToString() + " on " + x + ":" + y);


                    if (UnityEngine.Random.Range(0.0f, maxArr) <= arr[x - texture.width / 2].y)
                    {
                        texture.SetPixel(x, y, color); // Draw from center (x = width/2) to left edge
                        //pixelPos.Add(new Vector3(x, y, 0));
                        pixelPos.Add(new Tuple <Vector3, Vector3>(new Vector3( - arr[x - texture.width / 2].x, arr[x - texture.width / 2].y, 0.0f), new Vector3(x,y,0.0f)));

                    }
                    else
                    {
                        texture.SetPixel(x, y, wallColor);
                    }
                }
            }

            texture.Apply();
            return texture;

        }
    }
}
