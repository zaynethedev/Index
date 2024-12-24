using UnityEngine;

namespace Index.Mods
{
    public class NoClipHelper : MonoBehaviour
    {
        public static NoClipHelper Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public T[] FindAllObjectsOfType<T>() where T : Object
        {
            return FindObjectsOfType<T>();
        }
    }
}