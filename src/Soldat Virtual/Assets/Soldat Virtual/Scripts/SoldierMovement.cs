using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

namespace SoldatVirtual.Scripts
{
#if UNITY_EDITOR
    using Input = GoogleARCore.InstantPreviewInput;
#endif

    public class SoldierMovement : MonoBehaviour
    {
        public GameObject Soldier;

        public Animator SoldierAnimator;

        public static Vector3 Destination;

        public static bool IsSoldierStopped;

        private readonly int speedParameter = Animator.StringToHash("Speed");
        private readonly int isRunModeParameter = Animator.StringToHash("IsRunMode");

        private void Start()
        {

        }

        private void Update()
        {
            if (!Soldier.activeInHierarchy) // If soldier is not active, return
                return;

            float speedValue = 0f;

            if (Vector3.Distance(Soldier.transform.position, Destination) <= 0.01)
            {
                if (!IsSoldierStopped)
                {
                    MessageManager.ShowAndroidToastMessage("Stopping Soldier ");

                    // Set the player's position to the destination.
                    Soldier.transform.position = Destination;

                    // Set the speed (which is what the animator will use) to zero.
                    speedValue = 0f;
                    IsSoldierStopped = true;
                }
            }
            else
            {
                speedValue = 1f;

                MessageManager.ShowAndroidToastMessage("Moving from main." +
                "Speed: " + speedValue + " > " + 0 + ". " +
                "Position: " + Soldier.transform.position + ". " +
                "Destination: " + Destination);

                // Rotate the soldier to face the destination
                Soldier.transform.LookAt(Destination, Soldier.transform.up);
                // Move the soldier
                Soldier.transform.position = Vector3.MoveTowards(Soldier.transform.position, Destination, 0.01f);
            }

            // Set the animator's Speed parameter based on the (possibly modified) speed that the nav mesh agent wants to move at.
            //SoldierAnimator.SetFloat(speedParameter, speedValue, 0.05f, Time.deltaTime);
            SoldierAnimator.SetFloat(speedParameter, speedValue);
        }

        // Check if RunMode Button has been selected
        public void OnRunModeButtonClick()
        {
            if (!IsSoldierStopped)
                return;

            SoldierAnimator.SetBool(isRunModeParameter, true);
        }

        public void OnShotModeButtonClick()
        {
            if (!IsSoldierStopped)
                return;

            SoldierAnimator.SetBool(isRunModeParameter, false);
        }
    }
}