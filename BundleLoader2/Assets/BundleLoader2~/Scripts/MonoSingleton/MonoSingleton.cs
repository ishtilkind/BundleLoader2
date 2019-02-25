using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NG.TRIPSS.CORE
{
    public abstract class MonoSingleton<T> : MonoBehaviour
    {
        private static T instance;

        protected static T Instance { get; }

        private void Init()
        {

        }
    }
}
