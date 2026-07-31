/// <summary>
/// Erstellt Bausteine auf dem Spielfeld basierend auf einer Anleitung (Instruction) oder WebSocket-Daten.
/// Unterstützt verzögerte und sofortige Erstellung sowie das Entfernen und Ändern von Bausteinen.
/// </summary>
using System.Collections;
using QuantenKoffer.Bricks;
using QuantenKoffer.Case;
using QuantenKoffer.Instructions;
using QuantenKoffer.WebsocketInterface;
using UnityEngine;
using UnityEngine.Events;

namespace QuantenKoffer
{
    /// <summary>
    /// Creates bricks on the gameboard from an Instruction
    /// </summary>
    public class TestCaseCreator : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private BrickFactory factory;

        /// <summary>
        /// Reference to the gameboard grid
        /// </summary>
        [Tooltip("Reference to the gameboard grid")] [SerializeField]
        private KofferGrid caseGrid;

        [Header("Settings")]
        /// <summary>
        /// The Instruction from which the boardstate is created
        /// </summary>
        [Tooltip("The Instruction from which the boardstate is created")]
        [SerializeField]
        Instruction instruction;

        /// <summary>
        /// Should CreateFromInstruction be called on start?
        /// </summary>
        [Tooltip("Should CreateFromInstruction be called on start?")] [SerializeField]
        bool spawnTestCasesOnStart = false;

        /// <summary>
        /// Delay between placings of single QuBricks 
        /// </summary>
        [Tooltip("Delay between placings of single QuBricks ")] [SerializeField]
        float buildingDelay = 1f;

        [SerializeField] int buildToStep = -1;
        [SerializeField] UnityEvent brickCreatedAction;
        [SerializeField] UnityEvent brickRemovedAction;

        public float BuildingDelay
        {
            get => buildingDelay;
            set => buildingDelay = value;
        }

        private Vector3 gridPos;
        Coroutine createDelayedCoroutine = null;


        public void StartLasers()
        {
            caseGrid.periscopeContainer.StartBeams();
        }

        void Start()
        {
            if (spawnTestCasesOnStart)
            {
                StartCoroutine(LateStart(1f));
            }
        }

        IEnumerator LateStart(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            CreateFromInstruction();
        }


        /// <summary>
        /// Creates a new Brick and places it onto the grid
        /// </summary>
        /// <param name="type">QuBrickType</param>
        /// <param name="rotation">QuBrick-Rotation</param>
        /// <param name="gridPositionX">X-Position on the Grid</param>
        /// <param name="gridPositionY">Y-Position on the Grid</param>
        /// <returns>A Reference to the newly created Brick</returns>
        public Brick CreateBrickAtPosition(QuBrickType type, GridDirection rotation, int gridPositionX,
            int gridPositionY)
        {
            Brick newBrick = null;
            var position = new Vector2Int(gridPositionX, gridPositionY);
            if (position == new Vector2Int(-1, 0))
            {
                AMI.Util.Console.Log("TestCaseCreator", "Set Laser TopLeft active");
                caseGrid.TogglePeriscope(0);
            }
            else if (position == new Vector2Int(caseGrid.GridSize.x, 0))
            {
                AMI.Util.Console.Log("TestCaseCreator", "Set Laser TopRight active");
                caseGrid.TogglePeriscope(1);
            }
            else if (position == new Vector2Int(-1, caseGrid.GridSize.y - 1))
            {
                AMI.Util.Console.Log("TestCaseCreator", "Set Laser BottomLeft active");
                caseGrid.TogglePeriscope(3);
            }
            else if (position == new Vector2Int(caseGrid.GridSize.x, caseGrid.GridSize.y - 1))
            {
                AMI.Util.Console.Log("TestCaseCreator", "Set Laser BottomRight active");
                caseGrid.TogglePeriscope(2);
            }
            else if (position == new Vector2Int(caseGrid.GridSize.x + 1, caseGrid.GridSize.y / 2))
            {
                //Brick Spawn position - Do Nothing
                AMI.Util.Console.LogWarning("TestCaseCreator", "Spawnposition is at BrickSpawnPosition -> Do nothing");
            }
            else
            {
                if (position.x == -1 && position.y == -1)
                    return null;
                if (!caseGrid.GetField(position))
                {
                    AMI.Util.Console.Log("TestCaseCreator", $"created QuBrick {type} at {rotation} ({position})");
                    GameObject brickGo = factory.CreateBrickFromEnum(type.TypeEnum);
                    newBrick = brickGo.GetComponentInChildren<Brick>();
                    gridPos = caseGrid.PositionFromGridPos(gridPositionX, gridPositionY);
                    brickGo.transform.localPosition = gridPos;
                    caseGrid.SnapToGrid(newBrick);
                    newBrick.RotateTo(rotation);
                }
                else
                {
                    //,caseGrid.GetField(position).gameObject
                    AMI.Util.Console.Log("TestCaseCreator",
                        $"could not created QuBrick at {position} because there already is one ");
                }
            }

            brickCreatedAction.Invoke();
            return newBrick;
        }

        /// <summary>
        /// Removes a brick from the grid or deactivates the laser at that position
        /// </summary>
        /// <param name="brick">brick to be removed</param>
        public void RemoveBrick(Brick brick, Vector2Int position)
        {
            if (brick)
            {
                caseGrid.RemoveFromGrid(brick);
                Destroy(brick.gameObject);
                AMI.Util.Console.Log("TestCaseCreator", $"removed QuBrick of type {brick} at {brick.gridPos}");
                brickRemovedAction.Invoke();
                caseGrid.RecalculateAllLaser();
            }
            else
            {
                if (position == new Vector2Int(-1, 0))
                {
                    AMI.Util.Console.Log("TestCaseCreator", "Set Laser TopLeft inactive", gameObject);
                    //caseGrid.SetLaserTopLeftActive(false);
                    caseGrid.TogglePeriscope(0);
                    brickRemovedAction.Invoke();
                }
                else if (position == new Vector2Int(caseGrid.GridSize.x, 0))
                {
                    AMI.Util.Console.Log("TestCaseCreator", "Set Laser TopRight inactive", gameObject);
                    //caseGrid.SetLaserTopRightActive(false);
                    caseGrid.TogglePeriscope(1);

                    brickRemovedAction.Invoke();
                }
                else if (position == new Vector2Int(-1, caseGrid.GridSize.y - 1))
                {
                    AMI.Util.Console.Log("TestCaseCreator", "Set Laser BottomLeft inactive", gameObject);
                    caseGrid.TogglePeriscope(3);

                    //caseGrid.SetLaserBottomLeftActive(false);
                    brickRemovedAction.Invoke();
                }
                else if (position == new Vector2Int(caseGrid.GridSize.x, caseGrid.GridSize.y - 1))
                {
                    AMI.Util.Console.Log("TestCaseCreator", "Set Laser BottomRight inactive", gameObject);
                    //caseGrid.SetLaserBottomRightActive(false);
                    caseGrid.TogglePeriscope(2);

                    brickRemovedAction.Invoke();
                }
                else if (position == new Vector2Int(caseGrid.GridSize.x + 1, caseGrid.GridSize.y / 2))
                {
                    //Brick Spawn position - Do Nothing
                    AMI.Util.Console.LogWarning("TestCaseCreator",
                        "Remove Brick position is at BrickSpawnPosition -> Do nothing", gameObject);
                }
                else
                {
                    AMI.Util.Console.LogError("TestCaseCreator", $"cannot remove brick because it is null", gameObject);
                }
            }
        }

        /// <summary>
        /// Instantly creates a setup of bricks on the grid from an Instruction
        /// </summary>
        /// <param name="instruction">Instruction to be built</param>
        /// <param name="fromStep">step from which on the Instruction should be built</param>
        public void CreateFromInstruction(Instruction instruction, int fromStep)
        {
            if (instruction)
            {
                for (int i = fromStep; i < instruction.StepCount; i++)
                {
                    if (buildToStep != -1 && i == buildToStep)
                    {
                        return;
                    }

                    InstructionDetail currentStep = instruction.getStep(i);
                    foreach (var item in currentStep.Highlights)
                    {
                        CreateBrickAtPosition(item.Type, item.Rotation, item.Position.x, item.Position.y);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a setup of bricks on the grid from an Instruction with delay between each step
        /// </summary>
        /// <param name="instruction">Instruction to be built</param>
        /// <param name="fromStep">step from which on the Instruction should be built</param>
        public void CreateFromInstructionWithDelay(Instruction instruction, int fromStep = 0)
        {
            if (createDelayedCoroutine == null)
            {
                createDelayedCoroutine =
                    StartCoroutine(CreateFromInstructionDelayed(instruction, fromStep, buildingDelay));
            }
        }

        /// <summary>
        /// Creates a setup of bricks on the grid from an Instruction with delay between each step
        /// </summary>
        /// <param name="instruction">Instruction to be built</param>
        public void CreateFromInstructionWithDelay(Instruction instruction)
        {
            CreateFromInstructionWithDelay(instruction, 0);
        }

        /// <summary>
        /// Creates a setup of bricks on the grid from the Instruction set within the Inspector with delay between each step
        /// </summary>
        [ContextMenu("Create from Instruction with delay")]
        public void CreateFromInstructionWithDelay()
        {
            CreateFromInstructionWithDelay(instruction);
        }

        IEnumerator CreateFromInstructionDelayed(Instruction instruction, int fromStep, float delayBetweenSteps)
        {
            if (instruction)
            {
                for (int i = fromStep; i < instruction.StepCount; i++)
                {
                    InstructionDetail currentStep = instruction.getStep(i);
                    foreach (var item in currentStep.Highlights)
                    {
                        CreateBrickAtPosition(item.Type, item.Rotation, item.Position.x, item.Position.y);
                        yield return new WaitForSeconds(delayBetweenSteps);
                    }
                }

                caseGrid.RecalculateAllLaser();
            }

            createDelayedCoroutine = null;
        }

        /// <summary>
        /// Changes the settings of a brick based on the data given
        /// </summary>
        /// <param name="brick">brick whose settings will be changed</param>
        /// <param name="data">data to change the brick with</param>
        public void ChangeBrickSettings(Brick brick, WebsocketDataProcessed data)
        {
            brick.ApplySetting(data.Value);
        }

        [ContextMenu("Create from Instruction")]
        /// <summary>
        /// Creates a setup of bricks on the grid from the instruction set in the inspector
        /// </summary>
        public void CreateFromInstruction()
        {
            CreateFromInstruction(instruction, 0);
        }

        [ContextMenu("Clear Grid")]
        /// <summary>
        /// Removes all bricks and lasers from the board
        /// </summary>
        public void ClearGrid()
        {
            caseGrid.ClearGrid();
        }

        /// <summary>
        /// Receives the processed Data from our Websocket and executes the corresponding Action
        /// </summary>
        /// <param name="data">WebsocketDataProcessed</param>
        public void ProcessWebsocketData(WebsocketDataProcessed data)
        {
            Vector2Int position = new Vector2Int(data.PosX, data.PosY);
            Brick brick;
            try
            {
                brick = caseGrid.GetField(position);
            }
            catch (System.Exception)
            {
                brick = null;
            }

            switch (data.Command)
            {
                case Commands.start:
                    StartLasers();
                    break;
                case Commands.place:
                    if (!brick)
                    {
                        CreateBrickAtPosition(data.Type, data.Rotation, data.PosX, data.PosY);
                    }
                    else
                    {
                        AMI.Util.Console.LogError("TestCaseCreator",
                            $"cannot create brick at position: {position} because there already is one", gameObject);
                    }

                    break;
                case Commands.remove:
                    RemoveBrick(brick, position);
                    break;
                case Commands.settings:
                    if (brick)
                    {
                        if (brick.GetType() != data.Type)
                        {
                            RemoveBrick(brick, position);
                            CreateBrickAtPosition(data.Type, data.Rotation, data.PosX, data.PosY);
                        }
                        else
                        {
                            if (data.Type.TypeEnum == QuBrickTypeEnum.Mirror90)
                            {
                                //Skaliere -1 ... 1 vom brick zu 0 - 1 von unity
                                data.Value = (data.Value + 1) / 2;
                            }

                            ChangeBrickSettings(brick, data);

                            AMI.Util.Console.Log("TestCaseCreator",
                                $"changed settings of QuBrick at {brick.gridPos} to {data.Value}", gameObject);
                        }
                    }
                    else
                    {
                        AMI.Util.Console.LogError("TestCaseCreator",
                            $"cannot change settings of brick at position: {position} because there is none",
                            gameObject);
                    }

                    break;
                default:
                    AMI.Util.Console.LogError("TestCaseCreator", "unknown command within WebSocketDataProcessed",
                        gameObject);
                    break;
            }
        }
    }
}