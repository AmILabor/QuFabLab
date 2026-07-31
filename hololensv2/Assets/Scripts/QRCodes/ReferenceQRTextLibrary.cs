/// <summary>
/// Enthält die Bibliotheksklasse zur Zuordnung von QR-Code-Texten zu Prefabs.
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRTKExtensions.QRCodes
{
    /// <summary>
    /// Enthält eine Bibliothek von QR-Code-Texten mit zugehörigen Prefabs.
    /// </summary>
    [CreateAssetMenu(fileName = "new ReferenceQRTextLibrary", menuName = "ScriptableObjects/new ReferenceQRTextLibrary", order = 1)]
    public class ReferenceQRTextLibrary : ScriptableObject
    {
        public List<QRContentPrefab> QRContentPrefabs;
    }

    [Serializable]
    /// <summary>
    /// Verknüpft einen QR-Code-Text mit einem Prefab zur Instanziierung.
    /// </summary>
    public class QRContentPrefab
    {
        public string QRText;
        public GameObject Prefab;

    }
}
