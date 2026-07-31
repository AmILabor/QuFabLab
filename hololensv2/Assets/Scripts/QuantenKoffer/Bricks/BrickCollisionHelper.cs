/// <summary>
/// Hilfsklasse zur Erkennung von Laserstrahl-Kollisionen mit Bausteinen.
/// Leitet Kollisionsereignisse an den Brick-Kollisionshandler weiter.
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using QuantenKoffer.Bricks;
using QuantenKoffer.Laser;
using UnityEngine;

/// <summary>
/// Hilfsklasse zur Erkennung von Laserstrahl-Kollisionen mit Bausteinen.
/// Leitet Kollisionsereignisse an den Brick-Kollisionshandler weiter.
/// </summary>
public class BrickCollisionHelper : MonoBehaviour
{
    private Brick brick; //!< Komponente des überliegenden Game-Objekts
    
    /// <summary>
    /// Holt die Komponente Brick aus dem Parent GameObjekt
    /// </summary>
    private void Start()
    {
        brick = gameObject.GetComponentInParent<Brick>();
    }

    /// <summary>
    /// On Collision-Funktion. Ruft den Collisions-Handler des Bricks auf, wenn
    /// der Strahl von einem anderen Brick kommt und mit einem anderen Strahl kollidiert.
    /// </summary>
    /// <param name="other">Kollidierende Objekt, immer eigentlich LaserBeams</param>
    // (refactoring) Wäre es möglich gewesen, die HandleCollision-Funktion auf Beam-Ebene zu schreiben?
    // Oder vielleicht kann man die in Zwei teilen. Die Interferenzberechnung findet auf Laser-Ebene
    // statt und das Weiterleiten des Strahls auf Brick-Ebene.
    // Der Vorteile wäre, dass man die Interferenzberechnung weniger an die Weiterleitung koppelt. 
    // Der funktionale Ablauf kann dann unabhängig voneinander betrachtet werden. Es ist 
    // debatierbar, wie sinnvoll das ist. Btw. für sowas wären Design-Dokumente nice.
    private void OnTriggerEnter(Collider other) 
    {
        LaserBeam beam = other.gameObject.GetComponentInParent<LaserBeam>();
        if (beam == null) return; // We only want to process laserbeam collisions
        if (beam.from == brick.getCenterTransform()) return; // We dont want to handle colisions with source brick
        brick.HandleColision(beam);
    }
}