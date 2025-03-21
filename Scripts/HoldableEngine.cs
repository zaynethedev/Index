using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Index.Scripts
{
    public class HoldableEngine : MonoBehaviour
    {
        private float GrabRadius = 0.125f;
        private bool InHand = false;
        private bool InLeftHand = false;

        private ControllerInputPoller input;

        public void Start()
        {
            input = ControllerInputPoller.instance;
        }

        public void Update()
        {
            if (!InHand && input.leftControllerGripFloat >= 0.5f && Vector3.Distance(GTPlayer.Instance.leftControllerTransform.position, transform.position) < GrabRadius)
            {
                InHand = true;
                InLeftHand = true;
                transform.SetParent(GTPlayer.Instance.leftControllerTransform);
            }
            else if (InHand && InLeftHand && input.leftControllerGripFloat <= 0.5f)
            {
                InHand = false;
                transform.SetParent(null);
            }
            if (!InHand && input.rightControllerGripFloat >= 0.5f && Vector3.Distance(GTPlayer.Instance.rightControllerTransform.position, transform.position) < GrabRadius)
            {
                InHand = true;
                InLeftHand = false;
                transform.SetParent(GTPlayer.Instance.rightControllerTransform);
            }
            else if (InHand && !InLeftHand && input.rightControllerGripFloat <= 0.5f)
            {
                InHand = false;
                transform.SetParent(null);
            }
        }
    }
}
