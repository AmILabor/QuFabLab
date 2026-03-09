using System.Collections.Generic;
using UnityEngine;

namespace QuantenKoffer.Case
{
    /// <summary>
    /// Highlights one or more MeshRenderer and/or SpriteRenderer with a material which is decided by a bool(good or bad)
    /// </summary>
    public class Highlight : MonoBehaviour
    {
        /// <summary>
        /// List of all MeshRenderer to be highlighted
        /// </summary>
        [Tooltip("List of all MeshRenderer to be highlighted")] [SerializeField]
        List<MeshRenderer> meshRenderer;

        /// <summary>
        /// List of all SpriteRenderer to be highlighted
        /// </summary>
        [Tooltip("List of all SpriteRenderer to be highlighted")] [SerializeField]
        List<SpriteRenderer> spriteRenderer;

        /// <summary>
        /// Material for highlighting by bool (materialGood = true,materialBad = false)
        /// </summary>
        [Tooltip("List of all SpriteRenderer to be highlighted")] [SerializeField]
        Material materialGood, materialBad;

        /// <summary>
        /// Sets the material of all MeshRenderer and/or SpriteRenderer to the material corresponding to the given bool
        /// </summary>
        /// <param name="good">use materialGood(true) or use materialBad(false)</param>
        public void SetColor(bool good)
        {
            foreach (var item in meshRenderer)
            {
                if (good)
                {
                    item.material = materialGood;
                }
                else
                {
                    item.material = materialBad;
                }
            }

            foreach (var item in spriteRenderer)
            {
                Color color;
                if (good)
                {
                    color = materialGood.color;
                }
                else
                {
                    color = materialBad.color;
                }

                color.a += 75;
                item.color = color;
            }
        }
    }
}