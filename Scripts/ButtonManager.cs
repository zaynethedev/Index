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
        public static Material unselectedMaterial;
        public static Material selectedMaterial;
        public float debounceTime = 0.25f;
        public float touchTime;

        public void Start()
        {
            instance = this;

            Transform unselectedButton = Plugin.indexPanel.transform.Find("ShaderInit_UnselectedButton");
            Transform selectedButton = Plugin.indexPanel.transform.Find("ShaderInit_SelectedButton");

            unselectedMaterial = new Material(unselectedButton.GetComponent<MeshRenderer>().materials[0]);
            selectedMaterial = new Material(selectedButton.GetComponent<MeshRenderer>().materials[0]);

            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.layer = 18;
            gameObject.GetComponent<MeshRenderer>().material = unselectedMaterial;
        }

        protected void OnTriggerEnter(Collider collider)
        {
            if (!enabled || !(touchTime + debounceTime < Time.time))
                return;

            GorillaTriggerColliderHandIndicator component = collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
            if (component == null)
                return;

            touchTime = Time.time;
            ButtonActivation(component);
            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);
        }

        public void ButtonActivation(GorillaTriggerColliderHandIndicator hand)
        {
            GorillaTagger.Instance.StartVibration(hand.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
            string buttonName = gameObject.name;
            if (isCooldown)
                return;

            if (int.TryParse(buttonName, out int modID))
            {
                ModHandler mod = Plugin.initMods.FirstOrDefault(m => m.modID == modID);
                if (mod != null)
                {
                    if (mod.enabled)
                        mod.OnModDisabled();
                    else
                        mod.OnModEnabled();
                }
            }
            else
            {
                StartCoroutine(ChangeMaterialWithDelay(gameObject, selectedMaterial, unselectedMaterial, 0.1f));

                switch (buttonName)
                {
                    case "NextPage":
                        ChangePage(PageHandler.pageIndex + 1);
                        break;

                    case "PreviousPage":
                        if (PageHandler.pageIndex > 1)
                        {
                            ChangePage(PageHandler.pageIndex - 1);
                        }
                        break;
                }
            }
        }

        private void ChangePage(int newPageIndex)
        {
            Transform currentPage = Plugin.indexPanel.transform.Find($"Mods/page{PageHandler.pageIndex}");
            Transform nextPage = Plugin.indexPanel.transform.Find($"Mods/page{newPageIndex}");

            if (nextPage != null)
            {
                if (currentPage != null)
                {
                    currentPage.gameObject.SetActive(false);
                }
                nextPage.gameObject.SetActive(true);
                PageHandler.pageIndex = newPageIndex;
            }
        }

        private IEnumerator ChangeMaterialWithDelay(GameObject obj, Material selected, Material unselected, float delaySeconds)
        {
            obj.GetComponent<MeshRenderer>().material = selected;
            yield return new WaitForSeconds(delaySeconds);
            obj.GetComponent<MeshRenderer>().material = unselected;
        }
    }
}