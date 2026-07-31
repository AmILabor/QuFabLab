using System.Collections;
using System.Collections.Generic;
using QuantenKoffer.Bricks;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace QuantenKoffer.Case
{
    /// <summary>
    /// Spiegelt das Spielfeld wider. Beinhaltet alle Methoden, die sich um das Positionieren
    /// von Spielsteinen auf dem Grid beziehen. 
    /// </summary>
    /// 
    /// <remarks>
    /// Die Periskope und der Spawn liegen nicht in fields.
    /// Skript angeheftet an GameObject QR_Tracker/TrackerHolder/Quantenkoffer/Einsatzfeld.
    /// </remarks>
    public class KofferGrid : MonoBehaviour, INextFrameUnityEventInvoker
    {
        /// <summary>
        /// Size of the Grid in its own local space
        /// </summary>
        [Tooltip("Size of the Grid")] [SerializeField]
        protected Vector2Int gridSize = new Vector2Int(11, 8);

        public Vector2Int GridSize
        {
            get => gridSize;
        }

        /// <summary>
        /// Offset between grid elements
        /// </summary>
        [Tooltip("Offset between grid elements")] [SerializeField]
        protected Vector2 gridRowOffset = new Vector2(0.01f, 0.01f);

        /// <summary>
        /// Prefab to be instantiated when previewing a field
        /// </summary>
        [Tooltip("Prefab to be instantiated when previewing a field")] [SerializeField]
        Highlight previewPrefab;

        /// <summary>
        /// Prefab to be instantiated when highlighting a field
        /// </summary>
        [Tooltip("Prefab to be instantiated when highlighting a field")] [SerializeField]
        Highlight highlightPrefab;

        /// <summary>
        /// Reference to the brickSpawnPosition where new bricks are created
        /// </summary>
        [Tooltip("Reference to the brickSpawnPosition where new bricks are created")] [SerializeField]
        Transform brickSpawnPosition;

        /// <summary>
        /// Standardwert für die Rotation eines Bricks, wenn dieser erstellt wird.
        /// </summary>
        [SerializeField] protected Vector3 brickBaseRotation = new Vector3(-90, 180, 0);

        [SerializeField] protected Vector3 brickScale = Vector3.one;
        public bool allowManualBrickMovement = true;

        /// <summary>
        /// Vector3 positions of each corner of the grid
        /// </summary>
        // (refactoring) werden nicht verwendet
        [Tooltip("Vector3 positions of each corner of the grid")]
        protected Vector3 leftTopStart, rightTopStart, leftBottomStart, rightBottomStart;

        /// <summary>
        /// 2D array of all QuBricks on the field, indexed at their corresponding gridPosition
        /// </summary>
        [Tooltip("2D array of all QuBricks on the field, indexed at their corresponding gridPosition")]
        protected Brick[,] fields;

        /// <summary>
        /// 2D-Array für die vier Periskope mit deren Index in ihrem eigenen Space 
        /// </summary>
        protected Brick[,] periscopeFields;

        // (refactoring) gibt es schon als fields
        public Brick[,] Fields
        {
            get => fields;
            set => fields = value;
        }

        // (refactoring) gibt es schon als periscopeFields
        public Brick[,] PeriscopeFields
        {
            get => periscopeFields;
            set => periscopeFields = value;
        }

        /// <summary>
        /// Position des gegriffenen Steines?. \see PreviewCoroutine(Transform trackedObject)
        /// </summary>
        // (refactoring) umbenennen zu trackedPreviewTransform?
        Transform trackedPreviewObject;

        /// <summary>
        /// Highlight-Objekt des gegriffenen Steines
        /// </summary>
        Highlight previewObject;

        /// <summary>
        /// beschreibt, ob ein gegriffener Stein schon seinen "Schatten" als Highlight hat
        /// </summary>
        bool previewState = false;

        /// <summary>
        /// Instruction-Dialog-Highlights
        /// </summary>
        protected List<Highlight> highlights;

        // (refactoring) wird nicht mehr gebraucht
        [SerializeField] public bool recalculateLasers = false;

        // (refactoring) wird nur verwendet für HasNewBrick, was wieder nicht verwendet wird
        [SerializeField] public Transform brickParent;

        // (refactoring) umbenennen zu periscopeHandler
        // (refactoring) Referenzen stattdessen auf periscopeFields oder halt hier, nicht beides
        [SerializeField] public PeriscopeHandler periscopeContainer;

        /// <summary>
        /// Custom-Unity-Event der im Moment InstructionDialogue::IsCurrentStepCompleted() aufruft
        /// </summary>
        [SerializeField] public UnityEvent<Brick> OnBrickAction;

        // (refactoring) wird nicht verwendet
        public bool HasNewBrick
        {
            get => brickSpawnPosition.childCount > 2 || brickParent.childCount > 0;
        }

        // (refactoring) wird nicht verwendet
        // (refactoring) entweder ist ein Attribut protected oder public, nicht beides
        //    GridRowOffset umgeht das protected keyword, was "unschön" ist
        public Vector2 GridRowOffset
        {
            get => gridRowOffset;
            set => gridRowOffset = value;
        }

        /// <summary>
        /// 
        /// </summary>
        private bool spawnPositionIsOccupied = false;

        /// <summary>
        /// Ruft Initialize() auf.
        /// </summary>
        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes arrays and calculates corner positions
        /// </summary>
        public virtual void Initialize()
        {
            fields = new Brick[gridSize.x, gridSize.y];
            periscopeFields = new Brick[2, 2];
            leftTopStart = transform.TransformPoint(new Vector3(gridRowOffset.x, 0, 0));
            rightTopStart = transform.TransformPoint(new Vector3(-(gridRowOffset.x * gridSize.x), 0, 0));
            leftBottomStart =
                transform.TransformPoint(new Vector3(gridRowOffset.x, -(gridRowOffset.y * (gridSize.y - 1)), 0));
            rightBottomStart = transform.TransformPoint(new Vector3(-(gridRowOffset.x * gridSize.x),
                -(gridRowOffset.y * (gridSize.y - 1)), 0));
            highlights = new List<Highlight>();
        }

        // (refactoring) is not used anymore?
        private void Update()
        {
            if (recalculateLasers)
            {
                AMI.Util.Console.Log("KofferGrid", "I should recalculate lasers.");
                recalculateLasers = false;
            }
        }

        public void SetSpawnPositionOccupied(bool occupied)
        {
            spawnPositionIsOccupied = occupied;
        }

        public bool IsSpawnPositionOccupied()
        {
            return spawnPositionIsOccupied;
        }

        public bool IsPeriscopeActive(int index)
        {
            return periscopeContainer.IsPeriscopeActive(index);
        }

        public int GetPeriscopeIndexByPosition(Vector2Int position)
        {
            if (position.x == -1)
            {
                if (position.y == 0) return 0;
                if (position.y == GridSize.y - 1) return 3;
            }

            if (position.x == GridSize.x)
            {
                if ((position.y == 0)) return 1;
                if (position.y == GridSize.y - 1) return 2;
            }

            return -1;
        }

        public Brick TogglePeriscope(int idx)
        {
            if (!periscopeContainer.IsPeriscopeActive(idx))
                return periscopeContainer.CreatePeriscope(idx);
            periscopeContainer.ClearPeriscope(idx);
            return null;
        }

        /// <summary>
        /// Get the Vector3 position of a gridField
        /// </summary>
        /// <param name="x">X-Coordinate of the gridField</param>
        /// <param name="y">Y-Coordinate of the gridField</param>
        /// <returns>Vector3 position of the field</returns>
        public Vector3 FieldPosition(int x, int y)
        {
            return new Vector3(x * gridRowOffset.x, y * gridRowOffset.y);
        }

        /// <summary>
        /// Get the QuBrick at a gridField
        /// </summary>
        /// <param name="position">Vector2Int Coordinates of the gridField</param>
        /// <returns>QuBrick at that field or null if there is none</returns>
        public Brick GetField(Vector2Int position)
        {
            try
            {
                if (fields[position.x, position.y])
                {
                    return fields[position.x, position.y];
                }
            }
            catch
            {
                AMI.Util.Console.LogError("Brick-GetField", $"Could not find Position {position.x},{position.y}");
            }

            return null;
        }

        /// <summary>
        /// Recalculates the pathing of all Laser objects
        /// </summary>
        // (refactoring) vermutlich nicht mehr benutzt
        public void RecalculateAllLaser()
        {
            recalculateLasers = true;
        }

        /// <summary>
        /// Returns the Vector2Int gridPosition of a given QuBrick
        /// </summary>
        /// <param name="brick">brick we want the position of</param>
        /// <returns>Vector2Int gridPosition of the brick (-1,-1) if not found</returns>
        // (refactoring) Information liegt beim Brick schon vor
        public Vector2Int GetBrickGridPosition(Brick brick)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (fields[x, y] == brick)
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }

            return new Vector2Int(-1, -1);
        }

        /// <summary>
        /// Sets the trackedPreviewObject
        /// </summary>
        /// <param name="obj">new trackedPreviewObject</param>
        public void SetTrackedPreviewObject(Transform obj)
        {
            if (!allowManualBrickMovement) return;
            trackedPreviewObject = obj;
            SetPreviewState(true);
        }

        /// <summary>
        /// Sets the preview State (visibility of previewObject)
        /// </summary>
        /// <param name="state">new previewState</param>
        public void SetPreviewState(bool state)
        {
            if (!allowManualBrickMovement) return;

            if (state && !previewState)
            {
                previewState = true;
                StartCoroutine(PreviewCoroutine(trackedPreviewObject));
            }
            else if (!state && previewState)
            {
                previewState = false;
                if (previewObject)
                {
                    previewObject.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Instantiates the previewObject and snaps it to a trackedObject every frame
        /// </summary>
        /// <param name="trackedObject">object to snap to</param>
        // (refactoring) testen ob Rotation-Setzen noch notwendig ist
        IEnumerator PreviewCoroutine(Transform trackedObject)
        {
            if (previewObject == null)
            {
                previewObject = Instantiate<Highlight>(previewPrefab);
                previewObject.transform.SetParent(transform);
                // Since the Quantenkoffer can be rotated differently we have to apply the base rotation here (independent from the Qubricks Rotation)
                previewObject.transform.localRotation = Quaternion.Euler(brickBaseRotation);
            }

            if (!previewObject.gameObject.activeSelf)
            {
                previewObject.gameObject.SetActive(true);
            }

            while (previewState)
            {
                previewObject.transform.localPosition = SnapPosition(trackedObject);
                yield return null;
            }
        }

        /// <summary>
        /// Highlights field (1,1) for testing purposes (can be accessed through the ContextMenu)
        /// </summary>
        [ContextMenu("Test Highlight")]
        public void TestHighlight()
        {
            HighlightField(1, 1);
        }

        /// <summary>
        /// Highlights a given gridField
        /// </summary>
        /// <param name="x">X-Coordinate of the field</param>
        /// <param name="y">Y-Coordinate of the field</param>
        /// <param name="good">sets the color of the previewObject to green or red</param>
        public void HighlightField(int x, int y, bool good = true, bool usePreviewPrefab = false)
        {
            Highlight obj;
            if (usePreviewPrefab)
            {
                obj = Instantiate<Highlight>(previewPrefab, transform);
            }
            else
            {
                obj = Instantiate<Highlight>(highlightPrefab, transform);
            }

            obj.transform.localPosition = PositionFromGridPos(x, y);
            obj.SetColor(good);
            highlights.Add(obj);
        }

        /// <summary>
        /// Highlights a given gridField
        /// </summary>
        /// <param name="field">X and Y-Coordinate of the field</param>
        /// <param name="good">sets the color of the previewObject to green or red</param>
        public void HighlightField(Vector2Int field, bool good = true)
        {
            HighlightField(field.x, field.y, good);
        }

        /// <summary>
        /// Clears all highlighted fields and empties the highlights list
        /// </summary>
        // (refactoring) trackedHighlightObject as it's own class/type/whatever
        [ContextMenu("Clear Highlights")]
        public void ClearHighlights()
        {
            for (int i = 0; i < highlights.Count; i++)
            {
                Destroy(highlights[i].gameObject);
            }

            highlights = new List<Highlight>();
        }

        /// <summary>
        /// Returns the Vector3 position of a given gridCoordinate
        /// </summary>
        /// <param name="gridPos">X and Y-Coordinate of the field</param>
        /// <returns>Vector3 position of the gridField</returns>
        public Vector3 PositionFromGridPos(Vector2Int gridPos)
        {
            return PositionFromGridPos(gridPos.x, gridPos.y);
        }

        /// <summary>
        /// Returns the Vector3 position of a given gridCoordinate
        /// </summary>
        /// <param name="x">X-Coordinate of the field</param>
        /// <param name="y">Y-Coordinate of the field</param>
        /// <returns>Vector3 position of the gridField</returns>
        // (refactoring) auslagern in Instruction?
        public virtual Vector3 PositionFromGridPos(int x, int y)
        {
            float xPos = -(x * gridRowOffset.x);
            float yPos = -(gridRowOffset.y * y);
            if (x == -1) // Index for Periscope spawn left side
            {
                Vector3 periscopePos = periscopeContainer.GetLeftPeriscopePosition();
                periscopePos = transform.InverseTransformPoint(periscopePos);
                xPos = periscopePos.x;
            }
            else if (x == 11) // Index for Periscope spawn right side
            {
                Vector3 periscopePos = periscopeContainer.GetRightPeriscopePosition();
                periscopePos = transform.InverseTransformPoint(periscopePos);
                xPos = periscopePos.x;
            }

            return new Vector3(xPos, yPos, 0);
        }

        /// <summary>
        /// Returns the nearest gridField of a given transform
        /// </summary>
        /// <param name="toSnap">transform to get the nearest position from</param>
        /// <returns>Vector3 nearest gridPosition</returns>
        // (refactoring) virtual entfernen?
        public virtual Vector3 SnapPosition(Transform toSnap)
        {
            Vector3 snapPosition;
            //toSnap.SetParent(transform);

            Vector2 gridPos;
            gridPos.x = Mathf.Round(toSnap.localPosition.x / gridRowOffset.x);

            if (gridPos.x <= -gridSize.x)
            {
                gridPos.x = -(gridSize.x - 1);
            }

            if (gridPos.x > 0)
            {
                gridPos.x = 0;
            }

            gridPos.y = Mathf.Round(toSnap.localPosition.y / gridRowOffset.y);

            if (gridPos.y <= -gridSize.y)
            {
                gridPos.y = -(gridSize.y - 1);
            }

            if (gridPos.y > 0)
            {
                gridPos.y = 0;
            }

            //toDo: Check if position is occupied 
            //Debug.Log(gridPos);
            /* if(fields[(int)gridPos.x,(int)gridPos.y] != null){
                if(gridPos.x > 0){
                    gridPos.x -= 1;
                }else{
                    gridPos.x += 1;
                }

            } */

            snapPosition.x = gridPos.x * gridRowOffset.x;
            snapPosition.z = transform.parent.transform.localPosition.z;
            snapPosition.y = gridPos.y * gridRowOffset.y;

            AMI.Util.Console.Log("Koffergrid Snap", $"SnapPosition: {snapPosition}", toSnap.gameObject);

            //toSnap.SetParent(null);

            return snapPosition;
        }

        /// <summary>
        /// Returns the gridCoordinate of a given vector3 position
        /// </summary>
        /// <param name="position">Vector3 position to get the gridCoordinate of</param>
        /// <returns>Vector2Int gridCoordinate</returns>
        protected virtual Vector2Int GridPosFromPosition(Vector3 position)
        {
            Vector2Int gridPos = new Vector2Int();

            gridPos.x = Mathf.Abs(Mathf.RoundToInt(position.x / gridRowOffset.x));
            gridPos.y = Mathf.Abs(Mathf.RoundToInt(position.y / gridRowOffset.y));

            return gridPos;
        }

        /// <summary>
        /// Places a given QuBrick onto the nearest gridField
        /// </summary>
        ///
        /// <details>
        /// Bewegt das Parent-Objekt als solches um den Error-Highlighter mitzubewegen
        /// </details>
        /// <param name="toSnap">QuBrick to snap to the Grid</param>
        // (refactoring) ErrorHighlighter unter Brick-Parent packen wenn möglich
        // (refactoring) collision check wenn du Stein auf Stein setzt
        public void SnapToGrid(Brick toSnap)
        {
            //,bool recalculateLasers=true
            if (toSnap.gridPos.x == 4096) SetSpawnPositionOccupied(false);
            SetPreviewState(false);
            toSnap.RestoreRotation();
            Transform brickContainer = toSnap.transform.parent.transform;
            Vector3 snapPosition = SnapPosition(brickContainer);
            brickContainer.localPosition = snapPosition;
            var gridPos = GridPosFromPosition(snapPosition);
            if (toSnap.gridPos.x != 4096)
                fields[toSnap.gridPos.x, toSnap.gridPos.y] = null;
            fields[gridPos.x, gridPos.y] = toSnap;
            toSnap.gridPos = gridPos;

            AMI.Util.Console.Log("KofferGrid",
                $"QuBrick has been added to the grid at position ({gridPos.x},{gridPos.y})");
            toSnap.SetErrors(false, false, false);
            // Since the Quantenkoffer can be rotated differently we have to apply the base rotation here (independent from the Qubricks Rotation)
            //brickContainer.localRotation = Quaternion.Euler(brickBaseRotation);

            //toSnap.CalculateRotation();

            brickContainer.localScale = brickScale;

            if (recalculateLasers)
                RecalculateAllLaser();
            StartCoroutine(((INextFrameUnityEventInvoker)this).InvokeNextFrame<Brick>(OnBrickAction, toSnap));

            //OnBrickAction.Invoke();
        }

        /// <summary>
        /// Removes a brick from the Grid then recalculates all Laser
        /// Used on Manipulation start to remove the manipulated QuBrick from the Grid which will be added back by SnapToGrid at the correct Position
        /// Can be optimized : Maybe we should use a dictionary with brick --> X/Y to remove it form fields. mem vs space.
        /// </summary>
        /// <param name="brick">QuBrick to remove</param>
        // (refactoring) redundant durch Position im Brick, einfach an der Stelle vom Brick löschen
        public void RemoveFromGrid(Brick brick)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (fields[x, y] == brick)
                    {
                        fields[x, y].Destroy();
                        fields[x, y] = null;
                        AMI.Util.Console.Log("Koffergrid",
                            $"QuBrick has been removed from the grid at position ({x},{y})");
                        StartCoroutine(((INextFrameUnityEventInvoker)this).InvokeNextFrame<Brick>(OnBrickAction, null));
                        return;
                    }
                }
            }

            RecalculateAllLaser();
        }

        /// <summary>
        /// Removes all bricks from the Grid and deactivates all Laser, then recalculates all Laser
        /// Can be optimized : Maybe we should use a dictionary with brick --> X/Y to remove it form fields. mem vs space.
        /// </summary>
        /// 
        /// <details>
        /// OnBrickAction wird der InstructionDialog resetet
        /// </details>
        [ContextMenu("Clear Grid")]
        public void ClearGrid()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (fields[x, y] != null)
                    {
                        try
                        {
                            fields[x, y].Destroy();
                            //Destroy(fields[x,y].gameObject);
                        }
                        catch (System.Exception)
                        {
                            throw;
                        }

                        fields[x, y] = null;
                    }
                }
            }

            AMI.Util.Console.Log("KofferGrid", "Grid has been cleared", gameObject);
            ClearHighlights();
            RecalculateAllLaser();
            ClearBrickSpawn();
            ClearPeriscopes();
            StartCoroutine(((INextFrameUnityEventInvoker)this).InvokeNextFrame<Brick>(OnBrickAction, null));
        }

        /// <summary>
        /// Löscht alle Objekt die noch übrig sind, was die Objekte auf dem Spawn hoffentlich sind
        /// </summary>
        ///
        /// <details>
        /// Periskope liegen unter einem anderen GameObjekt
        /// </details>
        private void ClearBrickSpawn()
        {
            Brick[] children = gameObject.GetComponentsInChildren<Brick>();
            foreach (var child in children)
            {
                child.Destroy();
            }
        }

        /// <summary>
        /// Wrapper für das Löschen durch den PeriscopeHandler
        /// </summary>
        private void ClearPeriscopes()
        {
            periscopeContainer.ClearPeriscopes();
        }
    }
}