/// <summary>
/// Enthält Erweiterungsmethoden für asynchrone Aufgaben.
/// </summary>
using System;
using System.Threading.Tasks;

namespace MRKTExtensions.Utilities
{
    /// <summary>
    /// Stellt Erweiterungsmethoden für Task-Operationen bereit.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Führt eine asynchrone Aufgabe mit Zeitüberschreitung aus und ruft entsprechende Callbacks auf.
        /// </summary>
        /// <typeparam name="T">Der Rückgabetyp der Aufgabe.</typeparam>
        /// <param name="task">Die auszuführende Aufgabe.</param>
        /// <param name="timeout">Die Zeitüberschreitung in Millisekunden.</param>
        /// <param name="success">Callback bei erfolgreichem Abschluss innerhalb der Zeit.</param>
        /// <param name="error">Callback bei Zeitüberschreitung.</param>
        public static async Task AwaitWithTimeout<T>(this Task<T> task, int timeout, Action<T> success, Action error)
        {
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                success?.Invoke(task.Result);
            }
            else
            {
                error?.Invoke();
            }
        }
    }
}