using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Silentor.TB.Client.Tools.UnityThreading
{
    public class UnityThreadHelper : MonoBehaviour
    {
        private static UnityThreadHelper instance = null;

        public static void EnsureHelper()
        {
            if (null == (object)instance)
            {
                WaitOneExtension.isWebPlayer = Application.isWebPlayer;
                instance = FindObjectOfType(typeof(UnityThreadHelper)) as UnityThreadHelper;
                if (null == (object)instance)
                {
                    var go = new GameObject("[UnityThreadHelper]");
                    go.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                    instance = go.AddComponent<UnityThreadHelper>();
                    instance.EnsureHelperInstance();
                }
            }
        }

        private static UnityThreadHelper Instance
        {
            get
            {
                EnsureHelper();
                return instance;
            }
        }

        /// <summary>
        /// Returns the GUI/Main Dispatcher.
        /// </summary>
        public static global::Silentor.TB.Client.Tools.UnityThreading.Dispatcher Dispatcher
        {
            get
            {
                return Instance.CurrentDispatcher;
            }
        }

        /// <summary>
        /// Returns the TaskDistributor.
        /// </summary>
        public static global::Silentor.TB.Client.Tools.UnityThreading.TaskDistributor TaskDistributor
        {
            get
            {
                return Instance.CurrentTaskDistributor;
            }
        }

        private global::Silentor.TB.Client.Tools.UnityThreading.Dispatcher dispatcher;
        public global::Silentor.TB.Client.Tools.UnityThreading.Dispatcher CurrentDispatcher
        {
            get
            {
                return dispatcher;
            }
        }

        private global::Silentor.TB.Client.Tools.UnityThreading.TaskDistributor taskDistributor;
        public global::Silentor.TB.Client.Tools.UnityThreading.TaskDistributor CurrentTaskDistributor
        {
            get
            {
                return taskDistributor;
            }
        }

        private void EnsureHelperInstance()
        {
            if (dispatcher == null)
                dispatcher = new global::Silentor.TB.Client.Tools.UnityThreading.Dispatcher();

            if (taskDistributor == null)
                taskDistributor = new global::Silentor.TB.Client.Tools.UnityThreading.TaskDistributor();
        }

        /// <summary>
        /// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The action which the new thread should run.</param>
        /// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static global::Silentor.TB.Client.Tools.UnityThreading.ActionThread CreateThread(System.Action<ActionThread> action, bool autoStartThread)
        {
            Instance.EnsureHelperInstance();

            System.Action<ActionThread> actionWrapper = currentThread =>
            {
                try
                {
                    action(currentThread);
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                }
            };
            var thread = new global::Silentor.TB.Client.Tools.UnityThreading.ActionThread(actionWrapper, autoStartThread);
            Instance.RegisterThread(thread);
            return thread;
        }

        /// <summary>
        /// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The action which the new thread should run.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static global::Silentor.TB.Client.Tools.UnityThreading.ActionThread CreateThread(System.Action<ActionThread> action)
        {
            return CreateThread(action, true);
        }

        /// <summary>
        /// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The action which the new thread should run.</param>
        /// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static global::Silentor.TB.Client.Tools.UnityThreading.ActionThread CreateThread(System.Action action, bool autoStartThread)
        {
            return CreateThread((thread) => action(), autoStartThread);
        }

        /// <summary>
        /// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The action which the new thread should run.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static global::Silentor.TB.Client.Tools.UnityThreading.ActionThread CreateThread(System.Action action)
        {
            return CreateThread((thread) => action(), true);
        }

        #region Enumeratable

        /// <summary>
        /// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The enumeratable action which the new thread should run.</param>
        /// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static global::Silentor.TB.Client.Tools.UnityThreading.ThreadBase CreateThread(System.Func<ThreadBase, IEnumerator> action, bool autoStartThread)
        {
            Instance.EnsureHelperInstance();

            var thread = new global::Silentor.TB.Client.Tools.UnityThreading.EnumeratableActionThread(action, autoStartThread);
            Instance.RegisterThread(thread);
            return thread;
        }

        /// <summary>
        /// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The enumeratable action which the new thread should run.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static global::Silentor.TB.Client.Tools.UnityThreading.ThreadBase CreateThread(System.Func<ThreadBase, IEnumerator> action)
        {
            return CreateThread(action, true);
        }

        /// <summary>
        /// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The enumeratable action which the new thread should run.</param>
        /// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static global::Silentor.TB.Client.Tools.UnityThreading.ThreadBase CreateThread(System.Func<IEnumerator> action, bool autoStartThread)
        {
            System.Func<ThreadBase, IEnumerator> wrappedAction = (thread) => { return action(); };
            return CreateThread(wrappedAction, autoStartThread);
        }

        /// <summary>
        /// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The action which the new thread should run.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static global::Silentor.TB.Client.Tools.UnityThreading.ThreadBase CreateThread(System.Func<IEnumerator> action)
        {
            System.Func<ThreadBase, IEnumerator> wrappedAction = (thread) => { return action(); };
            return CreateThread(wrappedAction, true);
        }

        #endregion

        List<ThreadBase> registeredThreads = new List<ThreadBase>();
        public void RegisterThread(global::Silentor.TB.Client.Tools.UnityThreading.ThreadBase thread)
        {
            if (registeredThreads.Contains(thread))
            {
                return;
            }

            registeredThreads.Add(thread);
        }

        void OnDestroy()
        {
            foreach (var thread in registeredThreads)
                thread.Dispose();

            if (dispatcher != null)
                dispatcher.Dispose();
            dispatcher = null;

            if (taskDistributor != null)
                taskDistributor.Dispose();
            taskDistributor = null;

            if (instance == this)
                instance = null;
        }

        void Update()
        {
            if (dispatcher != null)
                dispatcher.ProcessTasks();

            var finishedThreads = registeredThreads.Where(thread => !thread.IsAlive).ToArray();
            foreach (var finishedThread in finishedThreads)
            {
                finishedThread.Dispose();
                registeredThreads.Remove(finishedThread);
            }
        }
    }
}