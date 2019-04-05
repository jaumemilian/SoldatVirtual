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

        private readonly int isInShotModeParameter = Animator.StringToHash("IsInShotMode");

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

                // Rotate the soldier to face the destination
                Soldier.transform.LookAt(Destination, Soldier.transform.up);
                // Move the soldier
                Soldier.transform.position = Vector3.MoveTowards(Soldier.transform.position, Destination, 0.01f);
            }

            SoldierAnimator.SetFloat(speedParameter, speedValue);
        }

        public void SetShotMode()
        {
            SoldierAnimator.SetBool(isInShotModeParameter, true);
        }

        public void SetRunMode()
        {
            SoldierAnimator.SetBool(isInShotModeParameter, false);
        }

        public void Shot(Vector3 shotDestination)
        {
            // Rotate the soldier where it has been hit
            Soldier.transform.LookAt(shotDestination, Soldier.transform.up);
            SoldierAnimator.SetTrigger("Shot");
        }
    }
}