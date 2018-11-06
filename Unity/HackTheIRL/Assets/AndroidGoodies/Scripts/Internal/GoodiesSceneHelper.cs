#if UNITY_ANDROID
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace DeadMosquito.AndroidGoodies.Internal
{
    class GoodiesSceneHelper : MonoBehaviour
    {
        private static GoodiesSceneHelper instance;
        private static System.Object initLock = new System.Object();
        private readonly System.Object queueLock = new System.Object();
        private readonly List<Action> queuedActions = new List<Action>();
        private readonly List<Action> executingActions = new List<Action>();

        public static GoodiesSceneHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    Init();
                }
                return instance;
            }
        }

        public static bool IsInImmersiveMode { set; private get; }

        public Texture2D LastTekenScreenshot { get; private set; }

        internal static void Init()
        {
            lock (initLock)
            {
                if (object.ReferenceEquals(instance, null))
                {
                    var instances = FindObjectsOfType<GoodiesSceneHelper>();

                    if (instances.Length > 1)
                    {
                        Debug.LogError(typeof(GoodiesSceneHelper) + " Something went really wrong " +
                            " - there should never be more than 1 " + typeof(GoodiesSceneHelper) +
                            " Reopening the scene might fix it.");
                    }
                    else if (instances.Length == 0)
                    {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<GoodiesSceneHelper>();
                        singleton.name = "[singleton] " + typeof(GoodiesSceneHelper);

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(GoodiesSceneHelper) +
                            " is needed in the scene, so '" + singleton +
                            "' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using instance already created: " + instance.gameObject.name);
                    }
                }
            }
        }

        private GoodiesSceneHelper()
        {
        }

        internal static void Queue(Action action)
        {
            if (action == null)
            {
                Debug.LogWarning("Trying to queue null action");
                return;
            }

            lock (instance.queueLock)
            {
                instance.queuedActions.Add(action);
            }
        }

        void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus && IsInImmersiveMode)
            {
                AGUIMisc.EnableImmersiveMode();
            }
        }

        void Update()
        {
            MoveQueuedActionsToExecuting();

            while (executingActions.Count > 0)
            {
                Action action = executingActions[0];
                executingActions.RemoveAt(0);
                action();
            }
        }

        private void MoveQueuedActionsToExecuting()
        {
            lock (queueLock)
            {
                while (queuedActions.Count > 0)
                {
                    Action action = queuedActions[0];
                    executingActions.Add(action);
                    queuedActions.RemoveAt(0);
                }
            }
        }

        #region share_screenshot
        public void SaveScreenshotToGallery(Action<string> onScreenSaved)
        {
            StartCoroutine(TakeScreenshot(Screen.width, Screen.height, onScreenSaved));
        }

        public IEnumerator TakeScreenshot(int width, int height, Action<string> onScreenSaved)
        {
            yield return new WaitForEndOfFrame();
            var texture = new Texture2D(width, height, TextureFormat.RGB24, true);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();
            LastTekenScreenshot = texture;
            var imageTitle = "Screenshot-" + DateTime.Now.ToString("yy-MM-dd-hh-mm-ss");
            var uri = AndroidPersistanceUtilsInternal.InsertImage(LastTekenScreenshot, imageTitle, "My awesome screenshot");
            onScreenSaved(uri);
        }
        #endregion
    }
}
#endif