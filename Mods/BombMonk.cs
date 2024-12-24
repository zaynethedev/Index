using Index.Resources;
using UnityEngine;
using System.Collections.Generic;

namespace Index.Mods
{
    [IndexMod("Bomb Monke", "It's a bomb. Press grip to spawn, trigger to explode.", "BombMonk", 14)]
    public class BombMonk : ModHandler
    {
        public static BombMonk instance;
        private List<GameObject> bombs = new List<GameObject>(); // Track all bombs
        private Rigidbody rb;

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            bool bo = false;

            if ((ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || ControllerInputPoller.instance.leftControllerGripFloat > 0.1f) && bo)
            {
                GameObject newBomb = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newBomb.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
                newBomb.transform.localScale = new Vector3(0.25f, 0.125f, 0.15f);
                newBomb.GetComponent<Renderer>().material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = Color.gray };
                bombs.Add(newBomb); // Add bomb to the list
                bo = false;
            }
            else if ((ControllerInputPoller.instance.rightControllerGripFloat < 0.1f || ControllerInputPoller.instance.leftControllerGripFloat < 0.1f) && !bo)
            {
                bo = true;
            }

            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || ControllerInputPoller.instance.leftControllerIndexFloat > 0.1f)
            {
                ExplodeAll(); // Trigger all bombs to explode
            }
        }

        private void ExplodeAll()
        {
            foreach (var bomb in bombs)
            {
                if (bomb != null)
                {
                    GameObject explosion = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    explosion.GetComponent<Collider>();
                    explosion.transform.position = bomb.transform.position;
                    explosion.transform.localScale = Vector3.one * 0.1f;
                    explosion.GetComponent<Renderer>().material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = Color.red };
                    GameObject.Destroy(bomb);

                    Bobmexplod.Instance.StartCoroutine(Eplosion(explosion));
                }
            }

            bombs.Clear(); // Clear the list of bombs after they explode
        }

        private System.Collections.IEnumerator Eplosion(GameObject explosion)
        {
            float duration = 1.0f;
            float elapsedTime = 0f;
            Vector3 startScale = explosion.transform.localScale;
            Vector3 endScale = Vector3.one * 5f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                explosion.transform.localScale = Vector3.Lerp(startScale, endScale, t);

                rb = GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody;
                if (rb != null)
                {
                    Vector3 direction = (rb.transform.position - explosion.transform.position).normalized;
                    float force = Mathf.Lerp(50f, 10f, Vector3.Distance(explosion.transform.position, rb.transform.position) / explosion.transform.localScale.x);
                    rb.AddForce(direction * force, ForceMode.Impulse);
                }

                yield return null;
            }

            GameObject.Destroy(explosion);
        }

        public override void OnModDisabled()
        {
            foreach (var bomb in bombs)
            {
                GameObject.Destroy(bomb);
            }
            bombs.Clear();
            base.OnModDisabled();
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
        }
    }

    public class Bobmexplod : MonoBehaviour
    {
        private static Bobmexplod _instance;

        public static Bobmexplod Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject helperObject = new GameObject("bobmexplod");
                    _instance = helperObject.AddComponent<Bobmexplod>();
                    GameObject.DontDestroyOnLoad(helperObject);
                }
                return _instance;
            }
        }
    }
}
