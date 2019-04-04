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

        private readonly int hashSpeedParameter = Animator.StringToHash("Speed");

        private void Start()
        {

        }

        private void Update()
        {
            if (!Soldier.activeInHierarchy) // If soldier is not active, return
                return;

            float speed = 0;

            if (Vector3.Distance(Soldier.transform.position, Destination) <= 0.01)
            {
                if (!IsSoldierStopped)
                {
                    MessageManager.ShowAndroidToastMessage("Stopping Soldier ");

                    // Set the player's position to the destination.
                    Soldier.transform.position = Destination;

                    // Set the speed (which is what the animator will use) to zero.
                    speed = 0f;
                    IsSoldierStopped = true;
                }
            }
            else
            {
                speed = 1f;

                MessageManager.ShowAndroidToastMessage("Moving from main." +
                "Speed: " + speed + " > " + 0 + ". " +
                "Position: " + Soldier.transform.position + ". " +
                "Destination: " + Destination);

                // Rotate the soldier to face the destination
                Soldier.transform.LookAt(Destination, Soldier.transform.up);
                // Move the soldier
                Soldier.transform.position = Vector3.MoveTowards(Soldier.transform.position, Destination, 0.01f);
            }

            // Set the animator's Speed parameter based on the (possibly modified) speed that the nav mesh agent wants to move at.
            SoldierAnimator.SetFloat(hashSpeedParameter, speed, 0.05f, Time.deltaTime);
        }
    }
}