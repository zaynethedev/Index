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
        public static Material unselectedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        public static Material selectedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        public override void Start()
        {
            instance = this;
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.layer = 18;
            unselectedMaterial.color = new Color(.75f, .75f, .75f);
            selectedMaterial.color = new Color(1, 0, 0);
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
                if (buttonName == "Page1" && !Plugin.indexPanel.transform.Find("Mods/page1").gameObject.activeSelf)
                {
                    Plugin.indexPanel.transform.Find("Mods/page1").gameObject.SetActive(true);
                    Plugin.indexPanel.transform.Find("Mods/page2").gameObject.SetActive(false);
                    StartCoroutine(ChangeMaterialWithDelay(gameObject, selectedMaterial, unselectedMaterial, 0.1f));
                }
                else if (buttonName == "Page2" && !Plugin.indexPanel.transform.Find("Mods/page2").gameObject.activeSelf)
                {
                    Plugin.indexPanel.transform.Find("Mods/page1").gameObject.SetActive(false);
                    Plugin.indexPanel.transform.Find("Mods/page2").gameObject.SetActive(true);
                    StartCoroutine(ChangeMaterialWithDelay(gameObject, selectedMaterial, unselectedMaterial, 0.1f));
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
