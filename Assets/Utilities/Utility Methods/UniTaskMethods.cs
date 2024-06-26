using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UtilityMethods
{
    public static class UniTaskMethods
    {
        public static async UniTask DelayedFunction(Action action, float secondsToWait)
        {
            int millisToWait = (int)(secondsToWait * 1000);
            await UniTask.Delay(millisToWait);
            action.Invoke();
        }

        public static async UniTask ExecuteInBatches(IEnumerable<UniTask> taskList, int batchSize, VariableProgress progress = null)
        {
            if (taskList == null || taskList.Count() == 0) return;
            if (batchSize <= 0)
            {
                if (progress == null)
                {
                    await UniTask.WhenAll(taskList);
                }
                else
                {
                    await WhenAllWithReporting(taskList, progress);
                }
                return;
            }
            List<UniTask> tempTaskList = new List<UniTask>();
            foreach (UniTask task in taskList)
            {
                tempTaskList.Add(task);
                if (tempTaskList.Count == batchSize)
                {
                    if (progress == null)
                    {
                        await UniTask.WhenAll(tempTaskList);
                    }
                    else
                    {
                        await WhenAllWithReporting(tempTaskList, progress);
                    }
                    tempTaskList.Clear();
                }
            }
            if (tempTaskList.Count <= 0) return;
            if (progress == null)
            {
                await UniTask.WhenAll(tempTaskList);
            }
            else
            {
                await WhenAllWithReporting(tempTaskList, progress);
            }
            tempTaskList.Clear();
        }

        public static async UniTask WhenAllWithReporting(IEnumerable<UniTask> tasks, VariableProgress progress)
        {
            HashSet<UniTask> tasksToWaitFor = new HashSet<UniTask>();
            foreach (UniTask task in tasks)
            {
                tasksToWaitFor.Add(ExecuteWithReporting(task, progress));
            }
            await UniTask.WhenAll(tasksToWaitFor);
        }

        public static async UniTask ExecuteWithReporting(UniTask task, VariableProgress progress)
        {
            progress.AddToTotal();
            await task;
            progress.AddToCompleted();
        }


        public static async UniTask<T> ExecuteAndRaiseCancelIfValue<T>(UniTask<T> task, CancellationTokenSource cancellationToken, T valueToCancelOn)
        {
            var result = await task;
            if (EqualityComparer<T>.Default.Equals(result, valueToCancelOn))
            {
                cancellationToken.Cancel();
                throw new OperationCanceledException(cancellationToken.Token);
            }
            return result;
        }

        /// <summary>
        /// Executes a series of tasks, and if one of them returns the given value valueToCancelOn,
        /// cancels any remaining tasks
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <param name="valueToCancelOn"></param>
        /// <returns></returns>
        public static async UniTask WhenAllUnlessValue<T>(IEnumerable<UniTask<T>> tasks, T valueToCancelOn)
        {
            var cts = new CancellationTokenSource();

            foreach (UniTask<T> task in tasks)
            {
                await ExecuteAndRaiseCancelIfValue<T>(task, cts, valueToCancelOn);
            }

            try
            {
                await UniTask.WhenAll(tasks);
                // If we reach here, both tasks completed successfully without null results
            }
            catch (OperationCanceledException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Returns a UniTask of the second type. Example: UniTask<Cat> -> UniTask<Animal>
        /// </summary>
        /// <typeparam name="TOriginal"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static UniTask<TResult> WrapTaskWithCast<TOriginal, TResult>(UniTask<TOriginal> task) where TOriginal : TResult
        {
            return task.ContinueWith(t => (TResult)t);
        }
    }
}
