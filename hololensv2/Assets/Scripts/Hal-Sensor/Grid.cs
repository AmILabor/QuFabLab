using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AMI.HAL{
    public class Grid : MonoBehaviour{

        /// <summary>
        /// Size of the Grid
        /// </summary>
        [Tooltip("Size of the Grid")]
        [SerializeField] Vector2Int gridSize = new Vector2Int(8,8); 

        /// <summary>
        /// Offset between grid elements
        /// </summary>
        [Tooltip("Offset between grid elements")]
        [SerializeField] Vector2 gridRowOffset = new Vector2(0.01f,0.01f);

        /// <summary>
        /// Prefab to be instantiated as grid elements
        /// </summary>
        [Tooltip("Prefab to be instantiated as grid elements")]
        [SerializeField] MagnetInfluenceColoring prefabToInstantiate;

        /// <summary>
        /// Transform at whose position the first Element will be Instantiated
        /// </summary>
        [Tooltip("Transform at whose position the first Element will be Instantiated")]
        [SerializeField] Transform startPosition;

        /// <summary>
        /// Magnet Reference
        /// </summary>
        [Tooltip("Magnet Reference")]
        [SerializeField] Magnet magnet;
        /// <summary>
        /// Invert the x direction in which the grid will be laid out?
        /// </summary>
        [Tooltip("Invert the x direction in which the grid will be laid out?")]
        [SerializeField] bool invertOrderX= true;
        /// <summary>
        /// Invert the y direction in which the grid will be laid out?
        /// </summary>
        [Tooltip("Invert the y direction in which the grid will be laid out?")]
        [SerializeField] bool invertOrderY= true;

        /// <summary>
        /// Populate the Grid on Entering Playmode?
        /// </summary>
        [Tooltip("Populate the Grid on Entering Playmode?")]
        [SerializeField] bool PopulateGridOnStart = true;


        /// <summary>
        /// Array holding all grid Elements
        /// </summary>
        MagnetInfluenceColoring[,] fields;

        private void Start() {
            if(PopulateGridOnStart) {
                PopulateGrid();
            }
        }

        /// <summary>
        /// Populate the Grid with Instantiated Prefabs
        /// </summary>
        [ContextMenu("PopulateGrid")] // Makes this Method executable from Inspector (select it from the Menu with the three dots in the Inspector) 
        public void PopulateGrid(){
            ClearGrid();
            fields = new MagnetInfluenceColoring[gridSize.x,gridSize.y];
            for (int x = 0; x < gridSize.x; x++){
                for (int y = 0; y < gridSize.y; y++){
                    fields[x,y] = Instantiate<MagnetInfluenceColoring>(prefabToInstantiate,transform);
                    fields[x,y].name = $"({x},{y})";
                    fields[x,y].transform.localPosition = GetPosition(x,y);
                    fields[x,y].magnet = magnet;
                }
            }
        }

        /// <summary>
        /// Clear the grid's Elements
        /// </summary>
        [ContextMenu("ClearGrid")]
        public void ClearGrid(){
            if(fields != null){
                for (int x = 0; x < gridSize.x; x++){
                    for (int y = 0; y < gridSize.y; y++){
                        if(fields[x,y] != null){
                            if(Application.isPlaying){
                                Destroy(fields[x,y].gameObject);         // Destroy while playing
                            }else{
                                DestroyImmediate(fields[x,y].gameObject);// Destroy while in Editor
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Get the Element at a given position
        /// </summary>
        /// <param name="position">position of the Element requested </param>
        public MagnetInfluenceColoring GetField(Vector2Int position){
            return fields[position.x,position.y];
        }
        /// <summary>
        /// Get the given Elements position
        /// </summary>
        /// <param name="x">x coordinate of the Element </param>
        /// <param name="y">y coordinate of the Element </param>
        public Vector3 GetPosition(int x,int y){
            float newX = x;
            float newY = y;
            if(invertOrderX){
                newX = -x;
            }
            if(invertOrderY){
                newY = -y;
            }
            return startPosition.localPosition + new Vector3(newX * gridRowOffset.x * transform.localScale.x,newY* gridRowOffset.y* transform.localScale.y,0);
        }
        
    }
}
