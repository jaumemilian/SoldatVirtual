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

        private readonly int isRunningParameter = Animator.StringToHash("IsRunning");

        private readonly int isInShotModeParameter = Animator.StringToHash("IsInShotMode");

        private void Start()
        {

        }

        private void Update()
        {
            if (!Soldier.activeInHierarchy) // If soldier is not active, return
                return;

            if (Vector3.Distance(Soldier.transform.position, Destination) <= 0.03)
            {
                if (!IsSoldierStopped)
                {
                    // Set the player's position to the destination.
                    Soldier.transform.position = Destination;

                    // Set the speed (which is what the animator will use) to zero.
                    IsSoldierStopped = true;
                }

                SoldierAnimator.SetBool(isRunningParameter, false);
            }
            else
            {
                // Rotate the soldier to face the destination
                Soldier.transform.LookAt(Destination, Soldier.transform.up);

                // Move the soldier
                Soldier.transform.position = Vector3.MoveTowards(Soldier.transform.position, Destination, 0.2f * Time.deltaTime);

                SoldierAnimator.SetBool(isRunningParameter, true);
            }
        }

        public void SetShotMode()
        {
            SoldierAnimator.SetBool(isInShotModeParameter, true);
        }

        public void SetRunMode()
        {
            SoldierAnimator.SetBool(isInShotModeParameter, false);
        }

        public void Shot(Vector3 shotDestination, GameObject targetObject)
        {
            // Rotate the soldier where it has been hit
            Soldier.transform.LookAt(shotDestination, Soldier.transform.up);
            SoldierAnimator.SetTrigger("Shot");

            if (targetObject.tag == "Enemy")
            {
                MessageManager.ShowAndroidToastMessage("Enemy " + targetObject.name + " Killed !");

                targetObject.transform.Rotate(new Vector3(90f, 0f, 0f));

                SoldatVirtualController.TotalEnemiesAlive -= 1;

                if (SoldatVirtualController.TotalEnemiesAlive == 0)
                    MessageManager.ShowAndroidToastMessage("You WIN !!!!");
            }
            else
            {
                MessageManager.ShowAndroidToastMessage("Failed: " + targetObject.name);
            }
        }
    }
}