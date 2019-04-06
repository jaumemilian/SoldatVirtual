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
    public class TapDetector : Manipulator
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
            if (gesture.TargetObject == null)
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

            // If gesture is targeting an existing object we are done.
            if (gesture.TargetObject != null)
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
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
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
            if (Soldier.activeInHierarchy)
            {
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
            else
            {
                //PrintTheGameObjects(hit);
                PrintTheEnvironment(hit);

                var components = GameObject.FindGameObjectsWithTag("Rescale");

                MessageManager.ShowAndroidToastMessage("Components: " + components.Length);

                foreach (var comp in components)
                {
                    MessageManager.ShowAndroidToastMessage("Name: " + comp.name);
                    MessageManager.ShowAndroidToastMessage("Position: " + comp.transform.localPosition);
                }

                // Initialize the Soldier destination
                SoldierMovement.Destination = hit.Pose.position;

                MessageManager.ShowAndroidToastMessage("Components: " + components.Length);

                foreach (var comp in components)
                {
                    MessageManager.ShowAndroidToastMessage("Name: " + comp.name);
                    MessageManager.ShowAndroidToastMessage("Position: " + comp.transform.localPosition);
                }
            }
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