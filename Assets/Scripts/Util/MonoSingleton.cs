

using UnityEngine;

namespace Assets.Scripts.Util
{
    /// <summary>
    /// Singleton abstract class to reduce boilerplate.
    /// https://medium.com/@eveciana21/monosingleton-cecef6ce60f5
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        public static T Inst {
            get 
            {
                if (_instance == null)
                    Debug.LogError(typeof(T).ToString() + "is null.");
                return _instance;
            } 
        }

        private void Awake()
        {
            _instance = this as T;
            Init();
        }

        public virtual void Init()
        {

        }
    }
}
