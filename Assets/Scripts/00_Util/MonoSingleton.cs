using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonoSingleton
{
    public class MonoSingleton<T> : MonoBehaviour where T : class
    {
        public static T Instance;

        protected virtual void Awake()
        {
            Instance = this as T;
        }
    }
}

