using UnityEngine;

namespace Flawless
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        private static readonly object Lock = new object();
        private static bool _applicationIsQuitting;

        protected virtual bool ShouldRename => false;

        public static T instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance)
                        return _instance;

                    _instance = (T) FindObjectOfType(typeof(T));

                    if (!_instance)
                    {
                        _instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                        if (!_instance)
                        {
                            Debug.LogError("[MonoSingleton]Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                        }

                        Debug.Log($"[MonoSingleton]An instance of {typeof(T)} is needed in the scene, so '{_instance.name}' was created with DontDestroyOnLoad.");
                    }

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError(
                            $"[MonoSingleton]Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                    }

                    return _instance;
                }
            }
        }

        #region Mono

        protected virtual void Awake()
        {
            if (_instance &&
                _instance != this)
            {
                Debug.LogWarning($"{typeof(T)} already exist!");
                Destroy(gameObject);
                return;
            }
            
            if (!_instance)
            {
                _instance = (T) this;
            }
            
            if (ShouldRename)
            {
                name = typeof(T).ToString();
            }
            
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        #endregion

        public void EmptyMethod()
        {
        }
    }
}
