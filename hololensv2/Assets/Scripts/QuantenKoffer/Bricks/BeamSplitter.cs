using System.Collections.Generic;
using QuantenKoffer.Laser;
using UnityEngine;

namespace QuantenKoffer.Bricks
{
    /*!
     * <summary>
     * Klasse für Bausteine, welche einen eingehenden Strahl in zwei ausgehende Strahlen aufteilen.
     * </summary>
     *
     * <remarks>
     * Die Methoden der Klasse drehen sich im Wesentlichen um das Feststellen von Ausgangsvektoren und das Vorbereiten
     * von Daten für die Interferenzberechnung. Die Interferenzberechnung findet
     * im Wesentlichen durch geerbte Methoden von der Elternklasse Brick statt.
     * </remarks>
     */
    /// <summary>
    /// Klasse für Bausteine, welche einen eingehenden Strahl in zwei ausgehende Strahlen aufteilen.
    /// </summary>
    /// <remarks>
    /// Die Methoden der Klasse drehen sich im Wesentlichen um das Feststellen von Ausgangsvektoren und das Vorbereiten
    /// von Daten für die Interferenzberechnung. Die Interferenzberechnung findet
    /// im Wesentlichen durch geerbte Methoden von der Elternklasse Brick statt.
    /// </remarks>
    public class BeamSplitter : Brick
    {
        private Dictionary<Transform, List<LaserBeam>> interferenceMemory =
            new Dictionary<Transform, List<LaserBeam>>();
        //!< Speichert die Eigenschaften des eingehenden LaserBeams für spätere Interferenzberechnung 

        private List<Transform> alreadyInterfered = new List<Transform>();
        //!< Merkt sich welche Strahlen schon miteinander interferriert haben um nach deren Darstellung diese zu löschen?


        /// <summary>
        /// \see Brick::HandleLaser(LaserBeam beam)
        /// </summary>
        ///
        /// <details>
        /// Wenn ein eingehender Strahl mit einem Baustein kollidiert, wird vor dem Rendern die Richtung des
        /// Ausgangsstrahls bestimmt. Ein neuer Strahl wird erzeugt, welcher auf den nächsten Baustein in
        /// Ausgangsrichtung zeigt. In der Funktion HandleLaserBase(LaserBeam beam, Vector3[] outDirections) wird
        /// der Strahl über NotifyNextHit(LaserBeam incomingBeam) durch interferenceMemory beim nächsten Baustein
        /// vermerkt. Der erste weitergeleitete Strahl wird normal gezeichnet.
        /// Er hat nicht interferriert. Alle nachliegenden Strahlen werden interferenztechnisch behandelt,
        /// aber nicht gezeichnet, sofern sie dasselbe Ziel haben. Sollte dies nicht der Fall sein, wird der
        /// outgoingBeam gerendert. Danach wird das Dictionary interferenceMemory geleert und der neue Strahl
        /// zurückgegeben. Momentan unterstützt die Funktion nur zwei Strahlen, die in eine Richtung miteinander
        /// interferrieren können.
        /// </details>
        /// 
        /// <param name="beam">Eingehender Strahl </param>
        /// <returns> Array ausgehender Strahlen </returns>
        public override LaserBeam[] HandleLaser(LaserBeam beam)
        {
            LaserBeam[] outgoingBeams = HandleLaserBase(beam, getOutVectors(beam.direction));
            foreach (var outgoingBeam in outgoingBeams)
            {
                if (AlreadyInterfered(outgoingBeam)) continue;
                //ausgeführt wenn else?
                HandleInterference(beam, outgoingBeam);
                outgoingBeam.Draw();
            }

            interferenceMemory.Clear();
            return outgoingBeams;
        }
        
        /// <summary>
        /// Prüft, ob ein Beam schon für die Interferenzberechnung verwendet wurde.
        /// </summary>
        /// <details>
        /// Es wird gecheckt, ob in alreadyInterfered schon der outgoingBeam vorhanden ist.
        /// Wenn ja, dann sollen der vorhandene Beam und der neue Beam interferrieren und true
        /// wird zurückgegeben. Danach wird der neue Beam entfernt. Ansonsten wird false zurückgegeben.
        /// Der erste eingehende Strahl trägt die interferrierte Amplitude. Dieser Strahl wird
        /// dafür gelöscht und neu erstellt.
        /// </details>
        /// <param name="outgoingBeam">
        /// Ein ausgehender Laserstrahl
        /// </param>
        /// <returns> Ein boolscher Wert, der beschreibt ob der Beam schon für die
        /// Interferenzberechnung verwendet wurde
        /// </returns>
        private bool AlreadyInterfered(LaserBeam outgoingBeam)
        {
            if (alreadyInterfered.Contains(outgoingBeam.to))
            {
                alreadyInterfered.Remove(outgoingBeam.to);
                outgoingBeam.DestroyWhenDone();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Stößt die Interferenzberechnung an für markierte Strahlen
        /// </summary>
        ///
        /// <details>
        /// Aus dem interferenceMemory werden die zu interferrierenden Strahlen entnommen und diese dann an
        /// PerformInterference(LaserBeam other) weitergegeben. Nach erfolgreicher
        /// Abhandlung der Interferenz wird der outgoingBeam in alreadyInterfered eingetragen.
        /// </details>
        ///
        /// <param name="currentBeam"> Der eingehende Strahl </param>
        /// <param name="outgoingBeam"> Ein ausgehender Strahl </param>
        ///
        private void HandleInterference(LaserBeam currentBeam, LaserBeam outgoingBeam)
        {
            List<LaserBeam> interferingBeams =
                interferenceMemory.GetValueOrDefault(outgoingBeam.to, new List<LaserBeam>());
            foreach (var interferingBeam in interferingBeams)
            {
                if (interferingBeam != currentBeam)
                {
                    outgoingBeam.PerformInterference(interferingBeam);
                    alreadyInterfered.Add(outgoingBeam.to);
                }
            }
        }

        /// <summary>
        /// Gibt eine Referenz auf den eingehenden Strahl zum nächsten Baustein, den dieser treffen würde.
        /// </summary>
        ///
        /// <details>
        /// Die erste Funktion, die bei der Strahlenbearbeitung durchgeführt wird über Raycasting.
        /// Berechnet über die Funktion getOutVectors(Vector3 inVector) die Richtung des nächsten Strahlenganges.
        /// Für jeden dieser Richtungen wird geprüft, ob ein Spielstein in deren Richtung liegt. Wenn ja, wird nextTarget
        /// auf die Mitte des nächsten Spielsteins gesetzt und bei diesem in das interferenceMemory
        /// der Strahl abgelegt, wenn derselbe Strahl nicht schon im Dictionary ist. Ansonsten, wenn kein Spielstein in
        /// der Richtung ist, geschieht nichts.
        /// </details>
        ///
        /// <param name="incomingBeam"> Der eingehende Strahl </param>
        /// <returns> Nichts </returns>
        protected override void NotifyNextHit(LaserBeam incomingBeam)
        {
            Vector3[] outVectors = getOutVectors(incomingBeam.direction);
            foreach (var vec in outVectors)
            {
                Brick next_brick = getNextBrickInDirection(vec);
                if (next_brick == null) continue;
                Transform nextTarget = next_brick.getCenterTransform();
                bool found = interferenceMemory.ContainsKey(nextTarget);
                if (!found) interferenceMemory[nextTarget] = new List<LaserBeam>();
                interferenceMemory[nextTarget].Add(incomingBeam);
            }
        }

        /// <summary>
        /// \see Brick::getOutVectors(Vector3 inVector)
        /// </summary>
        ///
        /// <details>
        /// Die Funktion, welche bestimmt in welche Richtung ein einhergehender Strahl gespiegelt wird.
        /// Um die Rotation unabhängig vom Koordinatensystem zu handhaben,
        /// wird durch die Funktion NormalizeInDirection(Vector3 inVector) der Baustein
        /// und der Strahl gedanklich auf den Ursprung gesetzt und auf 0 Grad rotiert.
        /// Aus dieser Position heraus wird dann über mehrere if-Clauses bestimmt in welche
        /// Richtung der ausgehende Strahl sich zu bewegen hat. Schließlich wird
        /// der Baustein und Strahl in seine ursprüngliche Rotation und Position zurückbewegt
        /// und der ausgehende Richtungsvektor zurückgegeben.
        ///</details>
        /// <param name="inVector"></param>
        /// <returns></returns>
        //! \see Brick::getOutVectors(Vector3 inVector)
        protected Vector3[] getOutVectors(Vector3 inVector)
        {
            Vector3 normalizedVector = NormalizeInDirection(inVector);
            List<Vector3> outDirs = new List<Vector3>();
            if (normalizedVector == Vector3.back)
            {
                outDirs.Add(Vector3.forward);
                outDirs.Add(Vector3.left);
            }
            if (normalizedVector == Vector3.forward){
                outDirs.Add(Vector3.back);
                outDirs.Add(Vector3.right);
            }
            if (normalizedVector == Vector3.right)
            {
                outDirs.Add(Vector3.left);
                outDirs.Add(Vector3.forward);
            }
            if (normalizedVector == Vector3.left)
            {
                outDirs.Add(Vector3.right);
                outDirs.Add(Vector3.back);
            }
            for (int i = 0; i < outDirs.Count; i++)
            {
                outDirs[i] = DenormalizeInVector(outDirs[i]);
            }
            return outDirs.ToArray();

        }
    }
}