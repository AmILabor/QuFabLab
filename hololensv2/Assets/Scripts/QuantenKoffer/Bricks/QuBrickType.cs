using UnityEngine;
using UnityEngine.Localization;

namespace QuantenKoffer.Bricks
{
    /// <summary>
    /// Scripatble Object defining a QuBrick type
    /// </summary> 
    [CreateAssetMenu(fileName = "Mirror90", menuName = "QFAB/Quantenkoffer/BrickType", order = 2)]
    public class QuBrickType : ScriptableObject
    {
        /// <summary>
        /// the type as enum
        /// </summary> 
        [SerializeField] QuBrickTypeEnum typeEnum;

        /// <summary>
        /// Sprite image to be displayed on top of the QuBrick
        /// </summary> 
        [Tooltip("Sprite image to be displayed on top of the QuBrick")]
        public Sprite sprite;

        /// <summary>
        /// Name of the QuBrick type with translation
        /// </summary> 
        [Tooltip("Name of this QuBrick type with translation")]
        public LocalizedString brickName;

        /// <summary>
        /// Description of the QuBrick type with translation
        /// </summary> 
        [Tooltip("Description of this QuBrick type with translation")]
        public LocalizedString description;

        public QuBrickTypeEnum TypeEnum
        {
            get => typeEnum;
            set => typeEnum = value;
        }
    }
}