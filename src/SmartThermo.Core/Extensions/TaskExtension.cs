using System;
using System.Threading.Tasks;

namespace SmartThermo.Core.Extensions
{
    public static class TaskExtension
    {
        /// <summary>
        /// Ожидает завершения выполнения задачи Task.
        /// </summary>
        /// <param name="task">Выбранная задача</param>
        /// <param name="completedCallback">Обратный вызов при успешном завершении Task.</param>
        /// <param name="errorCallback">Обратный вызов при неудачном завершении Task.</param>
        public static async void AwaitEx(this Task task, Action completedCallback, Action<Exception> errorCallback)
        {
            try
            {
                await task;
                completedCallback?.Invoke();
            }
            catch (Exception ex)
            {
                errorCallback?.Invoke(ex);
            }
        }
    }
}