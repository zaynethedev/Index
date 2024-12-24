using System.Linq;
using UnityEngine;
using System.Collections;

namespace Index.Resources
{
    internal class ButtonManager : MonoBehaviour
    {
        public static ButtonManager instance;
        public static bool isCooldown = false;
        public float cooldownTime = 0.1f;
        public int modIDSettings = 1;
        public static Material unselectedMaterial = new Material(Plugin.indexPanel.transform.Find("ShaderInit_UnselectedButton").GetComponent<MeshRenderer>().materials[0]);
        public static Material selectedMaterial = new Material(Plugin.indexPanel.transform.Find("ShaderInit_SelectedButton").GetComponent<MeshRenderer>().materials[0]);
        public float debounceTime = 0.25f;
        public float touchTime;

        public void Start()
        {
            instance = this;
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.layer = 18;
            gameObject.GetComponent<MeshRenderer>().material = unselectedMaterial;
        }

        protected void OnTriggerEnter(Collider collider)
        {
            if (!enabled || !(touchTime + debounceTime < Time.time) || collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
            {
                return;
            }

            touchTime = Time.time;
            GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
            ButtonActivation();
            if (!(component == null))
            {
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);
                GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
            }
        }

        public void ButtonActivation()
        {
            string buttonName = gameObject.name;
            if (isCooldown)
            {
                return;
            }

            if (int.TryParse(buttonName, out int modID))
            {
                ModHandler mod = Plugin.mods.FirstOrDefault(m => m.modID == modID);

                if (mod != null)
                {
                    if (mod.enabled)
                    {
                        mod.OnModDisabled();
                    }
                    else
                    {
                        mod.OnModEnabled();
                    }
                }
            }
            else
            {
                StartCoroutine(ChangeMaterialWithDelay(gameObject, selectedMaterial, unselectedMaterial, 0.1f));
                switch (gameObject.name)
                {
                    case "Page1":
                        if (!Plugin.indexPanel.transform.Find("Mods/page1").gameObject.activeSelf)
                        {
                            Plugin.indexPanel.transform.Find("Mods/page1").gameObject.SetActive(true);
                            Plugin.indexPanel.transform.Find("Mods/page2").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("SettingsPage").gameObject.SetActive(false);
                        }
                        break;
                    case "Page2":
                        if (!Plugin.indexPanel.transform.Find("Mods/page2").gameObject.activeSelf)
                        {
                            Plugin.indexPanel.transform.Find("Mods/page1").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("Mods/page2").gameObject.SetActive(true);
                            Plugin.indexPanel.transform.Find("SettingsPage").gameObject.SetActive(false);
                        }
                        break;
                }
            }
        }

        private IEnumerator ChangeMaterialWithDelay(GameObject obj, Material selected, Material unselected, float delaySeconds)
        {
            var meshRenderer = obj.GetComponent<MeshRenderer>();
            meshRenderer.material = selected;
            yield return new WaitForSeconds(delaySeconds);
            meshRenderer.material = unselected;
        }
    }
}
