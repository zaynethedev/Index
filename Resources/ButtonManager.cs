using System.Linq;
using UnityEngine;
using System.Collections;

namespace Index.Resources
{
    internal class ButtonManager : GorillaPressableButton
    {
        public static ButtonManager instance;
        public static bool isCooldown = false;
        public float cooldownTime = 0.1f;
        public static Material unselectedMaterial = new Material(Plugin.indexPanel.transform.Find("ShaderInit_UnselectedButton").GetComponent<MeshRenderer>().materials[0]);
        public static Material selectedMaterial = new Material(Plugin.indexPanel.transform.Find("ShaderInit_SelectedButton").GetComponent<MeshRenderer>().materials[0]);

        public override void Start()
        {
            instance = this;
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.layer = 18;
            gameObject.GetComponent<MeshRenderer>().material = unselectedMaterial;
        }

        public override void ButtonActivation()
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
                        StartCoroutine(Cooldown());
                    }
                    else
                    {
                        mod.OnModEnabled();
                        StartCoroutine(Cooldown());
                    }
                }
            }
            else
            {
                switch (gameObject.name)
                {
                    case "Page1":
                        if (!Plugin.indexPanel.transform.Find("Mods/page1").gameObject.activeSelf)
                        {
                            Plugin.indexPanel.transform.Find("Mods/page1").gameObject.SetActive(true);
                            Plugin.indexPanel.transform.Find("Mods/page2").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("Mods/page3").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("SettingsPage").gameObject.SetActive(false);
                            StartCoroutine(ChangeMaterialWithDelay(gameObject, selectedMaterial, unselectedMaterial, 0.1f));
                        }
                        break;
                    case "Page2":
                        if (!Plugin.indexPanel.transform.Find("Mods/page2").gameObject.activeSelf)
                        {
                            Plugin.indexPanel.transform.Find("Mods/page1").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("Mods/page2").gameObject.SetActive(true);
                            Plugin.indexPanel.transform.Find("Mods/page3").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("SettingsPage").gameObject.SetActive(false);
                            StartCoroutine(ChangeMaterialWithDelay(gameObject, selectedMaterial, unselectedMaterial, 0.1f));
                        }
                        break;
                    case "Page3":
                        if (!Plugin.indexPanel.transform.Find("Mods/page3").gameObject.activeSelf)
                        {
                            Plugin.indexPanel.transform.Find("Mods/page1").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("Mods/page2").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("Mods/page3").gameObject.SetActive(true);
                            Plugin.indexPanel.transform.Find("SettingsPage").gameObject.SetActive(false);
                            StartCoroutine(ChangeMaterialWithDelay(gameObject, selectedMaterial, unselectedMaterial, 0.1f));
                        }
                        break;
                    case "Settings":
                        if (!Plugin.indexPanel.transform.Find("SettingsPage").gameObject.activeSelf)
                        {
                            Plugin.indexPanel.transform.Find("Mods/page1").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("Mods/page2").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("Mods/page3").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("SettingsPage").gameObject.SetActive(true);
                            StartCoroutine(ChangeMaterialWithDelay(gameObject, selectedMaterial, unselectedMaterial, 0.1f));
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

        public IEnumerator Cooldown()
        {
            isCooldown = true;
            yield return new WaitForSeconds(cooldownTime);
            isCooldown = false;
        }
    }
}
