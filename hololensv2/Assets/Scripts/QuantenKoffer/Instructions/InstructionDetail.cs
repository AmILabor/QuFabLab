/// <summary>
/// Enthält alle Informationen für einen einzelnen Schritt einer Anleitung.
/// Definiert Beschreibung, Positionen und Prüflogik für einen Schritt.
/// </summary>
using System.Collections.Generic;
using System.Linq;
using QuantenKoffer.Bricks;
using QuantenKoffer.Case;
using UnityEngine;
using UnityEngine.Localization;

namespace QuantenKoffer.Instructions
{
    /// <summary>
    /// Contains all Information for a single step of an Instruction
    /// </summary>
    [System.Serializable]
    public class InstructionDetail
    {
        /// <summary>
        /// description of this step with translation/s
        /// </summary>
        [Tooltip("description of this step with translation/s")] [SerializeField]
        LocalizedString description;

        public LocalizedString Description
        {
            get => description;
            set => description = value;
        }

        /// <summary>
        /// List of all InstructionDetailPosition required to complete this step
        /// </summary>
        [Tooltip("List of all InstructionDetailPosition required to complete this step")] [SerializeField]
        List<InstructionDetailPosition> highlights;

        public List<InstructionDetailPosition> Highlights
        {
            get => highlights;
            set => highlights = value;
        }

        public int BrickAmount
        {
            get => highlights.Count;
        }

        /// <summary>
        /// Is this step completet? (are all InstructionDetailPosition substeps completed?)
        /// </summary>
        /// <param name="grid">Reference to KofferGrid</param>
        /// <returns>If this InstructionDetail is completed</returns>
        public bool IsCompleted(KofferGrid grid)
        {
            bool completed = true;
            foreach (var item in Highlights)
            {
                Brick brick = grid.GetField(item.Position);
                CheckBrick(brick);
                completed = completed && item.IsCompleted(grid);
            }

            return completed;
        }

        public void HideAllHighlights(KofferGrid grid)
        {
            Brick brick;
            int periscopeIndex;
            foreach (var item in Highlights)
            {
                periscopeIndex = grid.GetPeriscopeIndexByPosition(item.Position);
                if (periscopeIndex == -1)
                {
                    brick = grid.Fields[item.Position.x, item.Position.y];
                }
                else
                {
                    brick = grid.periscopeContainer.GetPeriscopeByIndex(periscopeIndex);
                }

                brick.SetErrors(false, false, false);
            }
        }

        /// <summary>
        /// Checks a given QuBrick if it matches an InstructionDetailPosition and sets the errorDialoge for it if not
        /// </summary>
        /// <param name="brick">QuBrick to be checked</param>
        public bool CheckBrick(Brick brick)
        {
            bool wrongRotation = true, wrongType = true;
            if (brick == null)
                return false;

            InstructionDetailPosition detailItem = Highlights.Where(
                item => item.Position == brick.gridPos).DefaultIfEmpty(null).First();

            if (detailItem == null)
            {
                brick.SetErrors(false, false, true);
                return false;
            }

            GridDirection brick_rotation = brick.GetRotation();
            GridDirection detailItem_rotation = detailItem.Rotation;
            wrongRotation = detailItem.Rotation != brick.GetRotation();
            wrongType = detailItem.Type != brick.GetBrickType();

            if (BrickAmount != 0)
                brick.SetErrors(wrongRotation, wrongType, false);
            return !(wrongRotation || wrongType);
        }
    }
}