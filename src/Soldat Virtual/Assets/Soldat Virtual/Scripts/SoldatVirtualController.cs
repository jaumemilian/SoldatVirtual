using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;

#if UNITY_EDITOR
    using Input = GoogleARCore.InstantPreviewInput;
#endif

public class SoldatVirtualController : MonoBehaviour
{
    public GameObject GridPrefab;

    public GameObject Soldier;

    public GameObject ArCamera;

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        _UpdateApplicationLifecycle();

        Touch touch;

        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Check if the user touched any tracked plane
        TrackableHit hit;
        if (Frame.Raycast(touch.position.x, touch.position.y, TrackableHitFlags.PlaneWithinPolygon,out hit))
        {
            // Let's place the soldier 

            // Enable the soldier
            Soldier.SetActive(true);

            // Create a new Anchor
            Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
            
            // Set the position of the soldier
            Soldier.transform.position = hit.Pose.position;
            Soldier.transform.rotation = hit.Pose.rotation;

            // Show the soldier to face the camera
            Vector3 cameraPosition = ArCamera.transform.position;

            // The soldier should only rotate around the Y axis
            cameraPosition.y = hit.Pose.position.y;

            // Rotate the soldier to face the camera
            Soldier.transform.LookAt(cameraPosition, Soldier.transform.up);

            // Need to attach our Soldier to the anchor
            Soldier.transform.parent = anchor.transform;
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
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
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

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}