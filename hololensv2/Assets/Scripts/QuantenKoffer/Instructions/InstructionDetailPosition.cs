/// <summary>
/// Enthält die Datenstruktur für eine einzelne Position innerhalb eines Anleitungsschritts.
/// Definiert die erforderliche Position, Rotation und den Typ eines QuBricks.
/// </summary>
using QuantenKoffer.Bricks;
using QuantenKoffer.Case;
using UnityEngine;

namespace QuantenKoffer.Instructions
{
    [System.Serializable]
    /// <summary>
    /// Contains Position,Type and Rotation required for a single QuBrick within an Instruction step
    /// </summary>
    public class InstructionDetailPosition
    {
        /// <summary>
        /// Required position of the QuBrick
        /// </summary>
        [Tooltip("Required position of the QuBrick")] [SerializeField]
        Vector2Int position;

        public Vector2Int Position
        {
            get => position;
            set => position = value;
        }

        /// <summary>
        /// Required type of the QuBrick
        /// </summary>
        [Tooltip("Required type of the QuBrick")] [SerializeField]
        QuBrickType type;

        public QuBrickType Type
        {
            get => type;
            set => type = value;
        }

        /// <summary>
        /// Dont Check this step to mark the InstrucionDetail as done.
        /// </summary>
        [Tooltip("Dont Check this step to mark the InstrucionDetail as done.")] [SerializeField]
        bool dontCheckForCompletion;

        public bool DontCheckForCompletion
        {
            get => dontCheckForCompletion;
            set => dontCheckForCompletion = value;
        }

        /// <summary>
        /// Required rotation of the QuBrick
        /// </summary>
        [Tooltip("Required rotation of the QuBrick")] [SerializeField]
        GridDirection rotation;

        public GridDirection Rotation
        {
            get => rotation;
            set => rotation = value;
        }

        private bool SpawnPositionInstructed = false;

        /// <summary>
        /// Is a QuBrick placed exactly how this InstructionDetailPosition requires it?
        /// </summary>
        /// <param name="grid">Reference to KofferGrid</param>
        /// <returns>If this InstructionDetailPosition is completed</returns>
        [Tooltip("Required position of the QuBrick")]
        public bool IsCompleted(KofferGrid grid)
        {
            int periscopeIndex = grid.GetPeriscopeIndexByPosition(position);
            if (periscopeIndex != -1)
            {
                return grid.IsPeriscopeActive(periscopeIndex);
            }

            if (position.x == grid.GridSize.x + 1 && position.y == grid.GridSize.y / 2)
            {
                if (SpawnPositionInstructed == false)
                {
                    SpawnPositionInstructed = grid.IsSpawnPositionOccupied();
                }

                return SpawnPositionInstructed;
            }

            Brick brick = grid.GetField(position);
            if (brick == null)
                return false;
            bool typeError = brick.GetBrickType() != type;
            bool rotationError = brick.GetRotation() != rotation;
            return !typeError && !rotationError;
        }
    }
}