/// <summary>
/// Ermöglicht die Bewegung eines GameObjects in alle Richtungen über Schieberegler oder Tasten.
/// Die Bewegungsgeschwindigkeit kann über einen Slider angepasst werden.
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

/// <summary>
/// Ermöglicht die Bewegung eines GameObjects in alle Richtungen über Schieberegler oder Tasten.
/// </summary>
public class CaseMover : MonoBehaviour
{
    public float minMoveValue = 1;
    public float maxMoveValue = 10;
    private float currentMoveValue = 1;

    public GameObject toMove;

    void Start()
    {
        currentMoveValue = CalculateMoveValue(0.5f);
    }

    /// <summary>
    /// Berechnet den Bewegungswert basierend auf einem normalisierten Eingabewert.
    /// </summary>
    /// <param name="moveValue">Normalisierter Eingabewert (0-1)</param>
    /// <returns>Skalierter Bewegungswert</returns>
    private float CalculateMoveValue(float moveValue)
    {
        return minMoveValue + ((maxMoveValue - minMoveValue) * moveValue);
    }

    /// <summary>
    /// Wird aufgerufen, wenn sich der Schiebereglerwert ändert. Aktualisiert den Bewegungswert.
    /// </summary>
    /// <param name="evtDAta">SliderEventData mit dem neuen Wert</param>
    public void OnMoveValueChanged(SliderEventData evtDAta)
    {
        if (evtDAta.NewValue != evtDAta.OldValue)
        {
            AMI.Util.Console.Log(evtDAta.NewValue);
            currentMoveValue = CalculateMoveValue(evtDAta.NewValue);
        }
    }

    /// <summary>
    /// Bewegt das Ziel-Objekt um den angegebenen Vektor.
    /// </summary>
    /// <param name="move">Bewegungsvektor</param>
    private void Move(Vector3 move)
    {
        AMI.Util.Console.Log("Moving " + toMove.transform.position + " to " + (toMove.transform.position + move));

        toMove.transform.position += move;
    }

    /// <summary>
    /// Bewegt das Ziel-Objekt nach links.
    /// </summary>
    public void MoveLeft()
    {
        Move(new Vector3(-currentMoveValue, 0, 0));
    }

    /// <summary>
    /// Bewegt das Ziel-Objekt nach rechts.
    /// </summary>
    public void MoveRight()
    {
        Move(new Vector3(currentMoveValue, 0, 0));
    }

    /// <summary>
    /// Bewegt das Ziel-Objekt nach oben.
    /// </summary>
    public void MoveUp()
    {
        Move(new Vector3(0, currentMoveValue, 0));
    }

    /// <summary>
    /// Bewegt das Ziel-Objekt nach unten.
    /// </summary>
    public void MoveDown()
    {
        Move(new Vector3(0, -currentMoveValue, 0));
    }

    /// <summary>
    /// Bewegt das Ziel-Objekt nach vorne.
    /// </summary>
    public void MoveFowrward()
    {
        Move(new Vector3(0, 0, currentMoveValue));
    }

    /// <summary>
    /// Bewegt das Ziel-Objekt nach hinten.
    /// </summary>
    public void MoveBackward()
    {
        Move(new Vector3(0, 0, -currentMoveValue));
    }
}