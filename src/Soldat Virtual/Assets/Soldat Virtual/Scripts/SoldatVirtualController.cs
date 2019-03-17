using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;
using UnityEngine.AI;

#if UNITY_EDITOR
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class SoldatVirtualController : MonoBehaviour
{
    public GameObject Soldier;

    public Animator animator;

    public GameObject ArCamera;

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;

    public float turnSmoothing = 15f;           // The amount of smoothing applied to the player's turning using spherical interpolation.

    private Vector3 destination;

    private readonly int hashSpeedPara = Animator.StringToHash("Speed");

    private bool soldierIsStopped = true;

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        _UpdateApplicationLifecycle();
        //TODO: JMMPEND: Show here to manage the first touch, and display the whole scenario

        _UpdateSoldier();        
        _OnTapHandler();
    }

    private void _UpdateSoldier()
    {
        float speed = 0;

        if (Soldier.activeInHierarchy)
        {
            if (Vector3.Distance(Soldier.transform.position, destination) <= 0.01)
            {
                if (!soldierIsStopped)
                {
                    MessageManager.ShowAndroidToastMessage("Stopping from main: ");
                    
                    // Set the player's position to the destination.
                    Soldier.transform.position = destination;

                    // Set the speed (which is what the animator will use) to zero.
                    speed = 0f;

                    soldierIsStopped = true;
                }
            }
            else
            {
                speed = 1f;

                MessageManager.ShowAndroidToastMessage("Moving from main." + 
                "Speed: " + speed +" > " + 0 + ". " +
                "Position: " + Soldier.transform.position + ". " + 
                "Destination: " + destination);

                 // Rotate the soldier to face the camera
                Soldier.transform.LookAt(destination, Soldier.transform.up);

                Soldier.transform.position = Vector3.MoveTowards(Soldier.transform.position, destination, 0.01f);
            }

            // Set the animator's Speed parameter based on the (possibly modified) speed that the nav mesh agent wants to move at.
            animator.SetFloat(hashSpeedPara, speed, 0.1f, Time.deltaTime);
        }
    }

    private void _OnTapHandler()
    {
        // Detect if a touch occurs
        Touch touch;

        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Check if the user touched any tracked plane
        TrackableHit hit;
        if (Frame.Raycast(touch.position.x, touch.position.y, TrackableHitFlags.PlaneWithinPolygon,out hit))
        {
            if (Soldier.activeInHierarchy)
            {
                // Set the destination
                destination = hit.Pose.position;
                soldierIsStopped = false;
                MessageManager.ShowAndroidToastMessage("Mueve al soldado: " + hit.Pose.position);
            }
            else
            {
                // Enable the soldier
                Soldier.SetActive(true);

                // Create a new Anchor
                Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
                
                // Set the position of the soldier
                Soldier.transform.position = hit.Pose.position;
                
                MessageManager.ShowAndroidToastMessage("Muestra el soldado: " + hit.Pose.position);

                Soldier.transform.rotation = hit.Pose.rotation;

                // Show the soldier to face the camera
                Vector3 cameraPosition = ArCamera.transform.position;

                // The soldier should only rotate around the Y axis
                cameraPosition.y = hit.Pose.position.y;

                // Rotate the soldier to face the camera
                Soldier.transform.LookAt(cameraPosition, Soldier.transform.up);

                // Need to attach our Soldier to the anchor
                Soldier.transform.parent = anchor.transform;

                destination = Soldier.transform.position;
            }
        }
    }

    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 30;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            MessageManager.ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            MessageManager.ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }
}