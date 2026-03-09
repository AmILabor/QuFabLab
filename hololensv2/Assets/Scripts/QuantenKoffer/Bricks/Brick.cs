using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using QuantenKoffer.Case;
using QuantenKoffer.Dialog;
using QuantenKoffer.Laser;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace QuantenKoffer.Bricks
{
    /// <summary>
    /// Basisklasse für Funktionalitäten im Quantenkoffer. 
    /// </summary>
    /// 
    /// <remarks>
    /// Bricks werden auch als Spielsteine bezeichnet.
    /// 
    /// Eigentlich eine abstract Klasse, von der nur geerbt wird.
    /// 
    /// Beinhaltet Methoden zur Bearbeitung von Lasern, für die spätere
    /// Interferenzberechnung, zum Bewegen der Steine und zum Rotieren der Steine.
    /// 
    /// Jeder Brick hat ein Child-Objekt center. Center ist manuell gesetzt, weil es sont rechenaufwändiger ist
    /// und man sicher sein kann, wo genau der center ist. Deswegen auch 0.15-y-offset.
    /// 
    /// Der Typ ist eigentlich Attribut eines Bricks, aber im Moment ist ein Brick sozusagen sein Typ.
    ///
    /// Nehme folgendes an: Der Typ ist Klasse, welcher für seinen Typ eine eigene Implementation der Brick-Methoden
    /// liefert. Der Typ ist einen Composite-Relation mit dem Brick. Dann wäre es halt ebenso unangemessen zu sagen,
    /// dass ein Typ Funktionen hat. Ein Typ stellt eigentlich ein primitives Datenobjekt
    /// dar.
    ///
    /// Was wir als Typ-Attribut bezeichnen in der Klasse, ist eigentlich dessen Typbeschreibung als Scriptable Objekt.
    /// Das war notwendig um lokalisierte Beschreibungen und Icons etc. abzulegen.
    /// </remarks>
    public class Brick : MonoBehaviour
    {
        private bool
            initialized =
                false; //!< Wahrweitswert, der beschreibt, ob der Brick aufgesetzt ist. Kann eventuell gelöscht werden

        protected Transform
            centerTransform; //!< Position von unterliegenden GameObject Center. Ziel für spätere Laserfortführung

        private LayerMask
            ignoreLayer; //!< \see GetNextBrickInDirection(Vector3 direction)

        [SerializeField] private QuBrickType type;
        //!< Typ eines Bricks (refactoring) vielleicht zu typeDescription umbennenen

        [SerializeField] private ErrorHighlighter
            _highlighter; //< Referenz auf Klasse, die Fehler in der Konfiguration des Spielbretts anzeigt

        protected GameObject dialogReference;
        //!< \see SetDialogueReference()

        public Vector2Int gridPos = new Vector2Int(4096, 4096);
        //!< Startposition des Bricks

        private int rotation = 0;
        //!< \see GetRotation()

        protected float SettingValue = 0.5f;
        //!< \see GetSetting(), \see ApplySetting(), \see Bricks::Brick90

        protected Quaternion Rotation;
        //!< Rotation bei der Initialisierung und vor dem Drehen


        /// <summary>
        /// Setzt den Brick auf und holt die nötigen Referenzen.
        /// </summary>
        ///
        /// <details>
        /// Wird in der Lebenszeit nur einmal augerufen, wenn das Skript aktiviert wird.
        /// </details>
        // (refactoring), gridPos muss nicht gesetzt werden und kann Start() statt Awake() sein?
        public void Awake()
        {
            if (!initialized)
            {
                gridPos.x = 4096;
                gridPos.y = 4096;
                centerTransform = gameObject.GetNamedChild("Center").transform;
                ignoreLayer = LayerMask.NameToLayer("Laser");
                initialized = true;
                try
                {
                    gameObject.GetNamedChild("Icon").GetComponentInChildren<SpriteRenderer>().sprite = type.sprite;
                }
                catch (Exception e)
                {
                    AMI.Util.Console.Log("Could not set Icon for " + gameObject.transform.parent.name + " ... " +
                                         e.Message);
                }
            }
        }

        /// <summary>
        /// nicht implementiert. Nicht mehr relevant. Vorher waren Bricks kein Child vom Koffer, weswegen beim drehen
        /// die Bricks nicht mit rotiert wurden.
        /// </summary>
        public void CalculateRotation()
        {
            //throw new NotImplementedException("Die Funktion CalculateRotation() muss noch geschrieben werden.");
        }

        /// <summary>
        /// Returnt true, wenn der Stein auf dem Grid ist
        /// </summary>
        /// 
        /// <details>
        /// Prüft nur, ob der Brick nicht am SpawnPoint liegt, denn dieser
        /// ist der einzige Ort, an dem ein Brick nicht automatisch zum
        /// Board gesnappt wird. 
        /// </details>
        /// 
        /// <returns>sehe summary</returns>
        public bool IsOnGrid()
        {
            return (gridPos.x != 4096 && gridPos.y != 4096);
        }

        /// <summary>
        /// Jeder Brick sollte einen Type habe, welcher hier geholt wird. 
        /// </summary>
        /// <returns></returns>
        public QuBrickType GetType()
        {
            return type;
        }

        public QuBrickType GetBrickType()
        {
            return type;
        }

        /// <summary>
        /// Kompatibilität zu Ralfs-Code. Directions sind in seinen Code per integer codiert.
        /// Schnittstellenkompatibitlität für WebSockets. Python codiert über integers.
        /// </summary>
        /// <returns>
        /// Rotation als Enum GridDirectino
        /// </returns>
        public GridDirection GetRotation()
        {
            switch (rotation)
            {
                case 0: return GridDirection.N;
                case 1: return GridDirection.E;
                case 2: return GridDirection.S;
                case 3: return GridDirection.W;
            }

            return GridDirection.E;
        }

        public void SetRotation(GridDirection  new_rotation)
        {
            
            switch (new_rotation)
            {
                case GridDirection.N: rotation = 0; break; 
                case GridDirection.E: rotation = 1; break;
                case GridDirection.S: rotation = 2; break;
                case GridDirection.W: rotation = 3; break;
            }
        }

        /// <summary>
        /// Zerstört den Brick UND das zugehörige ErrorHighlight
        /// </summary>
        public void Destroy()
        {
            Destroy(transform.parent.gameObject);
        }

        /// <summary>
        /// Wird durch Rotationsbutton im DialogHandler aufgerufen.
        /// Rotiert in direction je nach Knopfdruck. Kann nicht per Handgeste gedreht werden.
        /// WebSocket verwendet TestCaseCreator. Diese unterstützt auch ein 180° Rotation. Das Basisprogramm nicht.
        /// </summary>
        /// <param name="direction">Richtung</param>
        public void RotateTo(GridDirection direction)
        {
            switch (direction)
            {
                case GridDirection.E:
                    RotateRight();
                    break;
                case GridDirection.W:
                    RotateLeft();
                    break;
                case GridDirection.S:
                    transform.Rotate(Vector3.up, -180);
                    rotation = 2;
                    break;
            }
        }

        /// <summary>
        /// Updated die zu prüfenden Bedingungen für die Anzeige und zeigt einen Fehler an, wenn ein Stein falsch plaziert ist
        /// </summary>
        /// <param name="rotation">Error-Value-Rotation</param>
        /// <param name="type">Error-Value-Type</param>
        /// <param name="place">Error-Value-Place</param>
        public void SetErrors(bool rotation, bool type, bool place)
        {
            if (_highlighter == null)
                _highlighter = gameObject.transform.parent.gameObject.GetComponentInChildren<ErrorHighlighter>();
            _highlighter.UpdateErrors(place, type, rotation);
        }

        /// <summary>
        /// Wird von der Factory aufgerufen, damit alle Prefabs eine Referenz auf den einen DialogHandler haben.
        /// </summary>
        /// <details>
        /// Für das Verwenden des DialogHandlers muss die entsprechend Komponente DialogHandler aus dem dialogReference
        /// geladen werden.
        /// </details>
        /// <param name="dialog">Referenz auf Dialog-GameObjekt</param>
        public void setDialogReference(GameObject dialog)
        {
            dialogReference = dialog;
        }

        /// <summary>
        /// Setting ist primivite float, da der einzige Spielsteine mit Settings der 90° Spiegel mit seiner Phasenverschiebung.
        /// </summary>
        /// <param name="setting">Offset für die Phasenverschiebung der Welle </param>
        // (refactoring) muss wenn mehr Spielsteine unterschiedliche Settings haben polymorph gehandhabt werden
        // throw new NotImplementedException("Dieser Spielstein unterstüzt keine Einstellungsoptionen");
        public virtual void ApplySetting(float setting)
        {
        }

        /// <summary>
        /// Gibt den Setting-Wert zurück.
        /// </summary>
        /// <details>
        /// Return den Phasenverschiebungswert in seiner momentanen Implementation. \see ApplySetting(float setting)
        /// </details>
        /// <returns> siehe summary </returns>
        public virtual float GetSetting()
        {
            return SettingValue;
        }

        /// <summary>
        /// Registriert den Spielstein beim DialogHandler und zeigt diesen auch direkt an. 
        /// </summary>
        /// <details>
        /// Einige Buttons sollten nach je nach Implementation nicht angezeigt werden. Wenn ein Stein kein Settings
        /// hat zum Beispiel, sollten keine Setting-Optionen angezeigt werden.
        /// </details>
        public virtual void ShowDialog()
        {
            dialogReference.GetComponent<DialogHandler>().SetCurrentBrick(this);
        }

        /// <summary>
        /// Wird aufgerufen, wenn du den hochhebst. Sendet eine Message Upstream (KofferGrid), die vom
        /// ersten Objekt abgefangen wird, welche die Methode SetTrackerPreviewObject(Transform t) implementiert.
        /// Diese zeigt den "Schatten" unter dem hochgehobenen Brick an.
        /// </summary>
        public void SetThisBrickMoving()
        {
            Rotation = transform.rotation;
            gameObject.SendMessageUpwards("SetTrackedPreviewObject", this.transform.parent.transform);
            //grid.SetTrackedPreviewObject(this.transform);
        }

        /// <summary>
        /// Wenn man den Stein losslässt, snapt der zum Grid.
        /// </summary>
        // die Zuweisung hätte man sich sparen können, die ist unten in der nächsten Methode (refactoring)
        public void SetThisBrickDoneMoving()
        {
            transform.rotation = Rotation;
            gameObject.SendMessageUpwards("SnapToGrid", this);
        }

        /// <summary>
        /// Sorgt dafür, dass du beim Hochheben nicht aufpassen musst, den Stein nicht zu drehen.
        /// Die Rotation wird beim Loslassen auf das zurückgesetzt, was sie vor dem Hochheben war.
        /// </summary>
        // wäre für den Zusammenhalt gut, wenn man das Restoren aus dem SnapToGrid rauslöst. (refactoring)
        public void RestoreRotation()
        {
            transform.rotation = Rotation;
        }

        /// <summary>
        /// Es wurde um die y-Axe einmal nach links rotiert und die Roation ist von 0..3 kodiert.
        /// Nach einer Rotation wird dieser Integer-Wert immer um 1 reduziert, bis der dann kleiner 0 wird
        /// und auf 4-1 = 3 gesetzt wird.
        /// </summary>
        // Modulo % 4 kann eigentlich weg (refactoring)
        public void RotateLeft()
        {
            transform.Rotate(Vector3.up, -90);
            if (rotation - 1 < 0) rotation = 4;
            rotation = (rotation - 1) % 4;
            Rotation = transform.rotation;
        }

        /// <summary>
        /// \see RotateLeft()
        /// </summary>
        public void RotateRight()
        {
            transform.Rotate(Vector3.up, 90);
            rotation = (rotation + 1) % 4;
            Rotation = transform.rotation;
        }

        /// <summary>
        /// Bearbeitet den eingehenden Strahl in der Unterklasse.
        /// </summary>
        ///
        /// <param name="beam"> Der eingehende Laserstrahl</param>
        /// <returns> Die generierten ausgehenden Laserstrahlen. Vermutlich nicht verwendet </returns>
        public virtual LaserBeam[] HandleLaser(LaserBeam beam)
        {
            throw new Exception("HandleLaser must be implemented in derived class.");
        }

        /// <summary>
        /// Wenn der Laser an einem Stein ankommt, wird aus dem Laser dessen Brickquelle entnommen und
        /// die Event-Funktion NotifySourceOnNextHit(LaserBeam beam) aufgerufen. Danach wird der Laser verarbeitet.
        /// \see NotifySourceOnNextHit(LaserBeam beam)
        /// </summary>
        /// <param name="beam"> Kollidierender Beam </param>
        public virtual void HandleColision(LaserBeam beam)
        {
            Brick sourceBrick = beam.from.gameObject.GetComponentInParent<Brick>();
            if (sourceBrick != null)
            {
                sourceBrick.NotifySourceOnNextHit(beam);
            }

            HandleLaser(beam);
            //throw new Exception("HandleColision must be implemented in derived class.");
        }

        /// <summary>
        /// Berechnet die Reflexionsrichtung für einen eingehenden Strahl
        /// </summary>
        ///
        /// <param name="inVector"> Der Vektor des Strahls, für den dessen Reflektionsrichtung zu berechnen ist
        /// </param>
        /// <returns> Zwei Vektoren, die orhogonal zueinander sind </returns>
        protected virtual Vector3[] getOutVectors(Vector3 inVector)
        {
            throw new Exception("Must be implemented in derived brick");
        }

        /// <summary>
        /// Wir haben einen Child-Objekt namens Center. Das ist in der Mitte des Prefabs platziert.
        /// Diese Funktion ist ein Getter für das Attribut centerTransform, welches die Referenz
        /// hat.
        /// </summary>
        /// <returns>Transform des Child-Objektes center </returns>
        public Transform getCenterTransform()
        {
            return centerTransform;
        }

        /// <summary>
        /// Leitet den Strahl weiter und vermerkt diesen für Interferenzberechnung
        /// </summary>
        ///
        /// <param name="beam"> Der eingehende Laserstrahl</param>
        /// <param name="outDirections"> Die Richtung der auszugehenden Strahlen</param>
        /// <returns> Die generierten ausgehenden Laserstrahlen </returns>
        public LaserBeam[] HandleLaserBase(LaserBeam beam, Vector3[] outDirections)
        {
            //LaserBeam[] outBeams = { };
            List<LaserBeam> outBeams = new List<LaserBeam>();
            foreach (var direction in outDirections)
            {
                Brick targetBrick = getNextBrickInDirection(direction);
                if (targetBrick == null) continue;
                LaserBeam new_beam = beam.Clone(this, targetBrick);
                targetBrick.NotifyNextHit(new_beam);
                outBeams.Add(new_beam);
            }

            return outBeams.ToArray();
        }

        /// <summary>
        /// Gibt eine Referenz auf den eingehenden Strahl zum nächsten Baustein, den dieser treffen würde.
        /// </summary>
        ///
        /// <param name="incomingBeam"> Der eingehende Strahl </param>
        /// <returns> Nichts </returns>
        protected virtual void NotifyNextHit(LaserBeam incomingBeam)
        {
            return;
        }

        /// <summary>
        /// \see Periscope::NotifySourceOnNextHit(LaserBeam outgoingBeam)
        /// </summary>
        /// <param name="outgoingBeam"></param>
        protected virtual void NotifySourceOnNextHit(LaserBeam outgoingBeam)
        {
            return;
        }

        /// <summary>
        /// Normalisiert die Vektorrotation um den Ursprung des Spielsteins
        /// und behandelt Rundungsdifferenzen bei inVectoren.
        /// </summary>
        ///
        /// <details>
        /// inVector auf Spielsteinrotation gesetzt. Der inVector kommt immernur von einer der sechs Richtungen.
        /// Wenn der Vektor in der Richtung nicht normalisiert, wird dieser normalisiert. Es wird zu erst davon
        /// ausgegangen, dass der Vektor von rechts oder links kommt, wenn nicht dann von oben oder unten.
        /// Wenn der Vektor nicht die Vektorstärke |0.5| überschreitet, wird er nicht behandelt, da nicht
        /// gesagt werden kann, ob er eindeutig in die Richtung geht.
        /// </details>
        /// <param name="inVector"></param>
        /// <returns></returns>
        // 0.5 zur Variable auslagern
        // (refactor) in eine HelperClass auslagern? wird von zumindestens
        // LaserBeam::NormalizeBeamDirection(Vector3 inVector) fast identisch verwendet 
        protected Vector3 NormalizeInDirection(Vector3 inVector)
        {
            Vector3 normalizedVector = inVector.RotateAround(Vector3.zero, -transform.rotation.eulerAngles);
            if (normalizedVector.x < -0.5) return Vector3.left;
            if (normalizedVector.x > 0.5) return Vector3.right;
            if (normalizedVector.y < -0.5) return Vector3.down;
            if (normalizedVector.y > 0.5) return Vector3.up;
            if (normalizedVector.z < -0.5) return Vector3.back;
            if (normalizedVector.z > 0.5) return Vector3.forward;
            AMI.Util.Console.LogError("Beam", $"Encountered Strange inVector: {inVector}");
            return Vector3.zero;
        }

        /// <summary>
        /// Setzt die Rotation des inVectors zurück auf den Wert vor der Normalisierung.
        /// </summary>
        /// <param name="inVector"> Normalisierter Vektor </param>
        /// <returns> Entnormalisierter Vektor </returns>
        protected Vector3 DenormalizeInVector(Vector3 inVector)
        {
            return inVector.RotateAround(Vector3.zero, transform.rotation.eulerAngles);
        }

        /// <summary>
        /// Schießt eine Raycast-Strahl auf in direction und gibt den getroffenen Brick zurück.
        /// </summary>
        ///
        /// <details>
        /// Der Hit trifft einen Collider, der über transform.gameObject aufzurufen ist. Der Collider ist Child vom
        /// wirklichen Brick, weswegen wir noch GetComponentInParent verwenden müssen.
        /// Es wird eine Referenz auf Layer verwendet, damit Raycast nicht mit bestehenden Laser kollidiert.
        /// </details>
        /// <param name="direction"> Richtung zum Raycasten </param>
        /// <returns>Null, wenn nichts getroffen. Brick, wenn erfolgreich. </returns>
        protected Brick getNextBrickInDirection(Vector3 direction)
        {
            RaycastHit hit;

            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(centerTransform.position, direction, out hit, Mathf.Infinity, layerMask: ignoreLayer))
            {
                Debug.DrawRay(centerTransform.position, direction * hit.distance, Color.green, 10.0f);
            }
            else
            {
                Debug.DrawRay(centerTransform.position, direction * 1000, Color.red, 10.0f);
                return null;
            }

            Brick nextBrick = hit.transform.gameObject.GetComponentInParent<Brick>();
            return nextBrick;
        }
    }
}