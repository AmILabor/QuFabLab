/// <summary>
/// Fabrikklasse zum Erstellen von Brick-Instanzen (Spiegel 45°, Spiegel 90°, Strahlteiler, Periskope).
/// Enthält die Erstellungslogik und das Prefab-Management für alle Baustein-Typen.
/// </summary>
using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Localization.Settings;
using UnityEngine.Networking;

namespace QuantenKoffer.Bricks
{
    /// <summary>
    /// Enum der verfügbaren Baustein-Typen in der Fabrik.
    /// </summary>
    public enum BrickTypes
    {
        Mirror45,
        Mirror90,
        BeamSplitter
    }

    /// <summary>
    /// Erstellt Bricks
    /// </summary>
    public class BrickFactory : MonoBehaviour
    {
        [SerializeField] public GameObject Mirror45Prefab;
        [SerializeField] public GameObject Mirror90Prefab;
        [SerializeField] public GameObject BeamSplitterPrefab;
        [SerializeField] public GameObject PeriscopePrefab;
        [SerializeField] public Transform DefaultSpawnPosition;
        [SerializeField] public GameObject DialogReference;
        [SerializeField] public Transform BrickHolder;
        [SerializeField] public Vector3 AdditionalBrickRotation = new Vector3(90, 0, 0);
        [SerializeField] public float scaler = 0.375f;
        [SerializeField] public bool showRenderer = true;
        [SerializeField] public bool showIconsIfNotRendered = true;

        [Header("Periscope Positions")] [SerializeField]
        public Transform PeriscopePositionTopLeft;

        public Transform PeriscopePositionBottomLeft;
        public Transform PeriscopePositionTopRight;
        public Transform PeriscopePositionBottomRight;

        private int id = 0; //!< id ersteller Bricks

        private Dictionary<QuBrickTypeEnum, Func<GameObject>> lookup =
            new Dictionary<QuBrickTypeEnum, Func<GameObject>>();


        /// <summary>
        /// Registriert im Dictionary die Create-Methode für ein Brick
        /// </summary>
        private void Start()
        {
            lookup[QuBrickTypeEnum.Mirror45] = this.CreateMirror45;
            lookup[QuBrickTypeEnum.Mirror90] = this.CreateMirror90;
            lookup[QuBrickTypeEnum.BeamSplitter] = this.CreateBeamSplitter;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("de");
        }

        /// <summary>
        /// Verwendet den Enum-Typen um aus dem Lookup-Dict die create-Methode auszurufen und auszuführen.
        /// </summary>
        /// <param name="type"> Der zu erstellende Brick-Typ </param>
        /// <returns> Ein Brick </returns>
        public GameObject CreateBrickFromEnum(QuBrickTypeEnum type)
        {
            AMI.Util.Console.Log("Factory", $"Creating {type}");
            if (lookup.ContainsKey(type))
                return lookup[type]();
            AMI.Util.Console.LogError("Factory", $"Could not create {type} - seems to be not registered");
            return null;
        }

        /// <summary>
        /// Erstellt einen 45° Spiegel-Brick
        /// </summary>
        /// <returns> Ein 45° Spiegel-Brick </returns>
        public GameObject CreateDefaultBrick()
        {
            GameObject go = CreatePrefab(Mirror45Prefab);
            go.transform.position = DefaultSpawnPosition.transform.position;
            return go;
        }

        /// <summary>
        /// Erstellt ein Periskop an der übergebenen Position
        /// </summary>
        /// <param name="positionTransform">Übergebne Position</param>
        /// <returns>Ein Periskop</returns>
        public GameObject CreatePeriscopeAtPosition(Transform positionTransform)
        {
            GameObject go = CreatePrefab(PeriscopePrefab);
            //float yAngle =positionTransform.rotation.eulerAngles.y;
            //go.transform.RotateAround(Vector3.up,yAngle);
            go.transform.position = positionTransform.position;
            go.transform.parent = positionTransform.parent;
            return go;
        }

        /// <summary>
        /// Erstellt ein Periskop oben links
        /// </summary>
        // unnötiger Methoden-Zuwachs? lieber CreatePeriscope(pos) und pos übergeben statt das?
        // (refactoring) Methoden nicht verwendet?
        public void CreatePeriscopeTopLeft()
        {
            CreatePeriscopeAtPosition(PeriscopePositionTopLeft);
        }

        /// <summary>
        /// Erstellt ein Periskop unten links
        /// </summary>
        public void CreatePeriscopeBottomLeft()
        {
            CreatePeriscopeAtPosition(PeriscopePositionBottomLeft);
        }

        /// <summary>
        /// Erstellt ein Periskop oben rechts
        /// </summary>
        public void CreatePeriscopeTopRight()
        {
            CreatePeriscopeAtPosition(PeriscopePositionTopRight);
        }

        /// <summary>
        /// Erstellt ein Periskop unten rechts
        /// </summary>
        public void CreatePeriscopeBottomRight()
        {
            CreatePeriscopeAtPosition(PeriscopePositionBottomRight);
        }

        /// <summary>
        /// Erstellt einen 45° Spiegel-Brick
        /// </summary>
        /// <returns> Ein 45° Spiegel-Brick </returns>
        /*
         * (refactoring) Würde mich entscheiden, entweder über Enum oder manuell Bricks zu erstellen
         * Beides zu machen ist einfach redundant und macht den Code schwerer zu warten, but w/e
         */
        [ContextMenu("CreateMirror45")]
        public GameObject CreateMirror45()
        {
            return CreatePrefab(Mirror45Prefab);
        }

        /// <summary>
        /// Erstellt einen 90° Spiegel-Brick
        /// </summary>
        /// <returns> Ein 90° Spiegel-Brick </returns>
        [ContextMenu("CreateMirror90")]
        public GameObject CreateMirror90()
        {
            return CreatePrefab(Mirror90Prefab);
        }

        /// <summary>
        /// Erstellt einen Beamsplitter-Brick
        /// </summary>
        /// <returns> Ein Beamsplitter-Brick </returns>
        [ContextMenu("CreateBeamSplitter")]
        public GameObject CreateBeamSplitter()
        {
            return CreatePrefab(BeamSplitterPrefab);
        }

        /// <summary>
        /// Instanziiert ein Prefab des gewünschten Brick-Typen
        /// </summary>
        /// <param name="prefab">Zu instanziierender Prefab</param>
        /// <param name="rotate">Ob ein Rotation-Offset gegeben ist</param>
        /// <returns> Ein Brick-Prefab </returns>
        private GameObject CreatePrefab(GameObject prefab, bool rotate = true)
        {
            id += 1;
            GameObject go = GameObject.Instantiate(prefab);
            go.name = $"{id}_{prefab.name.Split(" ")[0]}";
            go.transform.parent = BrickHolder;
            if (rotate)
            {
                go.transform.rotation = BrickHolder.transform.rotation;
                if (AdditionalBrickRotation.x != 0) go.transform.Rotate(Vector3.right, AdditionalBrickRotation.x);
                if (AdditionalBrickRotation.z != 0) go.transform.Rotate(Vector3.forward, AdditionalBrickRotation.z);
                if (AdditionalBrickRotation.y != 0) go.transform.Rotate(Vector3.up, AdditionalBrickRotation.y);
            }

            go.GetComponentInChildren<Brick>().setDialogReference(DialogReference);
            go.transform.localScale *= scaler;
            bool isPeriscope = go.GetComponentInChildren<Periscope>() != null;
            if (!showRenderer && !isPeriscope)
            {
                foreach (MeshRenderer render in go.GetComponentsInChildren<MeshRenderer>())
                {
                    render.enabled = false;
                }

                foreach (NearInteractionGrabbable grabbable in go.GetComponentsInChildren<NearInteractionGrabbable>())
                {
                    grabbable.enabled = false;
                }

                go.GetNamedChild("BrickButton")?.SetActive(false);
                if (!showIconsIfNotRendered)
                    go.GetNamedChild("Icon")?.SetActive(false);
            }

            return go;
        }
    }
}