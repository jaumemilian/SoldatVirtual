using GoogleARCore;
using GoogleARCore.Examples.ObjectManipulation;
using UnityEngine;

namespace SoldatVirtual.Scripts
{
#if UNITY_EDITOR
    using Input = GoogleARCore.InstantPreviewInput;
#endif

    /// <summary>
    /// Controls the placement of Plane objects via a tap gesture.
    /// </summary>
    public class SoldierMovementManipulator : Manipulator
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject Soldier;

        public GameObject Environment;

        public SoldierMovement SoldierMovement;

        /// <summary>
        /// Returns true if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>True if the manipulation can be started.</returns>
        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            if (gesture.TargetObject != null && Environment.activeInHierarchy)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Function called when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnEndManipulation(TapGesture gesture)
        {
            MessageManager.ShowAndroidToastMessage("JMMMM SoldierMovementManipulator");

            if (gesture.WasCancelled)
            {
                return;
            }

            // If gesture is not targeting an existing object we are done.
            if (gesture.TargetObject == null || gesture.TargetObject.name != "Ground")
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinInfinity;

            if (Frame.Raycast(gesture.StartPosition.x, gesture.StartPosition.y, raycastFilter, out hit))
            {
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    _OnTapHandler(hit);
                }
            }
        }

        private void _OnTapHandler(TrackableHit hit)
        {
            Vector3 finalDestination = new Vector3(hit.Pose.position.x, hit.Pose.position.y, hit.Pose.position.z);

            if (SoldatVirtualUIController.IsInShotMode)
            {
                SoldierMovement.Shot(hit.Pose.position);
            }
            else
            {
                // Set the destination of the Soldier
                SoldierMovement.Destination = hit.Pose.position;
                SoldierMovement.IsSoldierStopped = false;
            }
        }
    }
}