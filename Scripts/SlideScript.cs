using System.Collections;
using UnityEngine;

namespace Index.Scripts
{
    public class SlideScript : MonoBehaviour
    {

        public float secs = 1.0f;
        void Start() => StartCoroutine(Break());

        public IEnumerator Break()
        {
            yield return new WaitForSeconds(secs);
            Destroy(gameObject);
        }

        
    }
}