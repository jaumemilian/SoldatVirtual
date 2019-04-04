namespace SoldatVirtual.Scripts
{
    using GoogleARCore;
    using GoogleARCore.Examples.ObjectManipulation;
    using UnityEngine;

#if UNITY_EDITOR
    using Input = GoogleARCore.InstantPreviewInput;
#endif

    /// <summary>
    /// Controls the placement of Plane objects via a tap gesture.
    /// </summary>
    public class SoldierPlacementManipulator : Manipulator
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
                // Set the destination of the Soldier
                SoldierMovement.Destination = hit.Pose.position;
                SoldierMovement.IsSoldierStopped = false;
            }
            else
            {
                // Enable the soldier
                Soldier.SetActive(true);

                // Create a new Anchor
                Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Set the position of the soldier
                Soldier.transform.position = hit.Pose.position;
                Soldier.transform.rotation = hit.Pose.rotation;

                // Show the soldier to face the camera
                Vector3 cameraPosition = FirstPersonCamera.transform.position;

                // The soldier should only rotate around the Y axis
                cameraPosition.y = hit.Pose.position.y;

                // Rotate the soldier to face the camera
                Soldier.transform.LookAt(cameraPosition, Soldier.transform.up);

                // Need to attach our Soldier to the anchor
                Soldier.transform.parent = anchor.transform;

                SoldierMovement.Destination = Soldier.transform.position;


                // Do the same with the environment

                Environment.SetActive(true);

                // Rotate the environment to face the camera
                Environment.transform.LookAt(cameraPosition, Environment.transform.up);

                // Set the position of the environment
                Environment.transform.position = hit.Pose.position;
                Environment.transform.rotation = hit.Pose.rotation;

                // Need to attach our environment to the anchor
                Environment.transform.parent = anchor.transform;

            }
        }
    }
}