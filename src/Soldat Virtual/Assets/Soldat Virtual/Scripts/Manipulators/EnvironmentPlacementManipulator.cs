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
    public class EnvironmentPlacementManipulator : Manipulator
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        public GameObject Environment;

        public GameObject Soldier;

        /// <summary>
        /// Returns true if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>True if the manipulation can be started.</returns>
        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            if (gesture.TargetObject == null && Environment.activeInHierarchy == false)
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
            if (gesture.WasCancelled)
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

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
            PrintTheEnvironment(hit);

            // Initialize the Soldier destination to the current Soldier position
            SoldierMovement.Destination = Soldier.transform.position;
        }

        private void PrintTheEnvironment(TrackableHit hit)
        {
            Environment.SetActive(true);

            // Create a new Anchor
            Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
            // Get the camera position
            Vector3 cameraPosition = FirstPersonCamera.transform.position;

            MessageManager.ShowAndroidToastMessage("Camera Position: " + cameraPosition.ToString());

            // The soldier should only rotate around the Y axis
            cameraPosition.y = hit.Pose.position.y;

            // Set the gameobject position
            Environment.transform.position = hit.Pose.position;
            Environment.transform.rotation = hit.Pose.rotation;

            // Rotate the gameobject to face the camera
            Environment.transform.LookAt(cameraPosition, Environment.transform.up);

            // Need to attach our gameobject to the anchor
            Environment.transform.parent = anchor.transform;
        }
    }
}