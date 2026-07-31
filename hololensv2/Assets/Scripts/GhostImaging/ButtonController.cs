/// <summary>
/// Enthält Steuerungsklassen für das Ghost-Imaging-Experiment.
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using GhostImaging.Bricks;
using Unity.XR.CoreUtils;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

namespace QuantenKoffer.Bricks
{
    /// <summary>
    /// Mit Hilfe dieser Klasse wird im Ghost-Imaging-Experiment die Funktionalitäten
    /// der das Experiment-kontrollierenden Knöpfe verwaltet.
    /// </summary>
    /// <remarks>
    ///Diese sind dazu da das Experiment zu starten, die Shapes vor dem Bucket-Detektor zu verwalten,
    /// den Zielmodus auszuwählen, festzustellen ob das Experiment gerade im Gange ist
    /// und später für die Funktion der einzelnen Knöpfe Erklärungen zu liefern.
    /// In der Szene hängt dieses Skript am GameObject "Controls".
    /// </remarks>
    public class ButtonController: MonoBehaviour
    {
        private int
            CurrentActiveShapeIndex =
                0; /*! Shapes sind Objekte, welche im Ghost-Imaging-Experiment vor dem Bucket-Detektor liegen.
                    * Der Index bestimmt, welche der Shapes aktiviert oder deaktiviert werden soll. */

        public Pump
            Pump; //! Ein im Editor hinterlegter Verweis auf das den Strahl erzeugende GameObject. In der Szene ist das der kleine schwarze Kasten vorne auf dem Tisch.

        public SPDCBrick
            Spdc; //! Ein im Editor hinterlegter Verweis auf das GameObject, welches Photonenpaare erzeugt. In der Szene ist das der kleine schwarze Kasten hinten auf dem Tisch.

        public ResultImageHandler
            ImageDisplayHandler; //! Ein im Editor hinterlegeter Verweis auf das GameObject, welches die Visuals des ArrayDetektors hanhabt.

        public GameObject
            ShapeContainer; /*! Ein im Editor hinterlegeter Verweis auf das übergeordnete GameObject, welches die relative Position der einzelnen Shapes handhabt. Shapes waren
                             * die Objekte, welche vor dem BucketDetektor liegen und den Rahmen festlegen, wo der Bucket-Detektor getroffen wird.*/

        // disable for now; public TextMeshPro ChatContent; //! Ein im Editor hinterlegter Verweis auf ein ChatFenster zum Anzeigen von Erklärungsdialogen
        private List<GameObject>
            Shapes = new List<GameObject>(
                3); //! Liste von GameObjects, welche später Shapes beinhalten soll und in der Start() belegt wird.

        private bool IsRunning = false; //! Boolean, der vom Namen her feststellt, ob das Experiment am Laufen ist.
                                        //Eigentlich wird bestimmt ob zuletzt ein Photon geschossen wurde bzw. �ber Timer feststellt ob ein Photon noch im Durchlauf ist.

        /*!
         * <summary>
         * Finde Shapes und versetze die erste Shape in den aktiven Zustand. 
         * </summary>
         * 
         * <returns> Nichts </returns>
         * 
         */
        private void Start()
        {
            ShapeContainer.GetChildGameObjects(Shapes);
            DisableAllShapes();
            EnableShape(0);
        }

        /*!
         * <summary>
         * Finde Shapes und versetze die erste Shape in den aktiven Zustand. 
         * </summary>
         * 
         * <returns> Nichts </returns>
         */
        public void DisableAllShapes()
        {
            foreach (var shape in Shapes)
            {
                shape.SetActive(false);
            }
        }

        /*!
         * <summary>
         * Aktiviere die Shape mit der id: idx.
         * </summary>
         *
         * <returns> Nichts </returns>
         */
        private void EnableShape(int idx)
        {
            Shapes[idx].SetActive(true);
        }

        /*!
         * <summary>
         * Addiere den ShapeIndex um 1.
         * </summary>
         *
         * <returns> Nichts </returns>
         */
        public void NextShape()
        {
            if (CurrentActiveShapeIndex + 1 <= Shapes.Count)
            {
                CurrentActiveShapeIndex += 1;
            }
            else
            {
                CurrentActiveShapeIndex = 0;
            }

            DisableAllShapes();
            if (CurrentActiveShapeIndex == 0)
            {
                return;
            }


            EnableShape(CurrentActiveShapeIndex - 1);
        }

        /*!
         * <summary>
         * Erlösche alle roten Knöpfe auf dem ArraySensor.
         * </summary>
         *
         * <returns> Nichts </returns>
         */
        public void ClearDisplay()
        {
            ImageDisplayHandler.Clear();
            IsRunning = false;
        }

        /*!
         * <summary>
         * Setze die isRunning-Variable auf false. Stoppt eigentlich nichts?
         * </summary>
         *
         * <returns> Nichts </returns>
         */
        public void Stop()
        {
            IsRunning = false;
        }

        /*!
         * <summary>
         * Schießt zufällige Strahlen auf den BucketDetektor und den ArrayDetektor.
         * </summary>
         *
         * <returns> Nichts </returns>
         */
        public void ShootRandomBeam()
        {
            if (IsRunning)
                return;
            IsRunning = true;
            int x = Random.Range(0, 4);
            int y = Random.Range(0, 4);
            SetSPDCTarget(x, y);
            Pump.CallStartBeam();
            IsRunning = false;
        }

        /*!
         * <summary>
         * Startet eine Coroutine, die den Detektor von oben links nach unten rechts mit Strahlen abtastet. Bricht ab, wenn isRunning auf false ist.
         * </summary>
         *
         * <returns> Nichts </returns>
         */
        public void StartRaster()
        {
            if (IsRunning)
                return;
            IsRunning = true;
            StartCoroutine(RasterCoroutine());
        }

        /*!
         * <summary>
         * Setzt die relative Position im Raster des anzuvisierenden Rasterelementes.
         * </summary>
         *
         * <returns> Nichts </returns>
         */

        // Warum ist das hier? Gehört eigentlich in die Klasse SPDCBrick. 
        private void SetSPDCTarget(int x, int y)
        {
            Spdc.targetPosition.x = x;
            Spdc.targetPosition.y = y;
        }

        /*!
         * <summary>
         * Tastet das Raster mit Strahlen ab.
         * </summary>
         *
         * <details>
         * Wartet jeweils 2 Sekunden. Bestimmt aus der Nummer der Reihenfolge die x und y Coordinates im Raster für das Zielobjekt.
         * </details>
         *
         * <returns> Nichts? </returns>
         */

        // nicht ganz verstanden wie das funktioniert
        private IEnumerator RasterCoroutine()
        {
            int i = 0;
            int x = 0;
            int y = 0;
            SetSPDCTarget(x, y);
            while (i < 25)
            {
                if (!IsRunning)
                    break;
                Pump.CallStartBeam();
                yield return new WaitForSeconds(2f);
                i += 1;
                x = (int)(i / 5);
                y = i % 5;
                SetSPDCTarget(x, y);
            }

            IsRunning = false;
        }
    }
}