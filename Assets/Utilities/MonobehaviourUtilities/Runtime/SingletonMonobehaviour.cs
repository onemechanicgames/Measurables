using UnityEngine;

namespace OMG.Utilities.Monobehaviour.Runtime
{
    public class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static SingletonMonobehaviour<T> instance;

        protected void Awake() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else {
                Destroy(gameObject);
            }
        }
    }
}