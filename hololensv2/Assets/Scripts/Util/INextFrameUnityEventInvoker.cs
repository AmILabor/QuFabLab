/// <summary>
/// Enthält ein Interface zur verzögerten Ausführung von Unity-Events im nächsten Frame.
/// </summary>
using System.Collections;
using UnityEngine.Events;

namespace Util
{
    /// <summary>
    /// Interface um Event-Methoden, die im nächsten Frame geschehen sollen, bereitzustellen
    /// </summary>
    public interface INextFrameUnityEventInvoker
    {
        /// <summary>
        /// Führt im nächsten Frame alle Methoden unter dem jeweiligen Event aus
        /// </summary>
        ///
        /// <details>
        /// Eigene Event-Handler mit Methoden haben bisher DialogHandler und KofferGridNew implementiert.
        /// \see DialogHandler::ChangeType(string type), DialogHandler::DestroyCurrentBrick()
        /// </details>
        /// 
        /// <param name="evt">Der zu triggerende Event</param>
        /// <param name="arg"> Von den Methoden des Events benötigte Parameter </param>
        /// <typeparam name="T"> Beliebiger Typ </typeparam>
        /// <returns> Nichts </returns>
        public IEnumerator InvokeNextFrame<T>(UnityEvent<T> evt, T arg)
        {
            yield return null;
            evt.Invoke(arg);
        }
    }
}