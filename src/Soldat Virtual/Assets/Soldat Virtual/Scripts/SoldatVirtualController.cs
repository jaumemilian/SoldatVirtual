using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;
using UnityEngine.AI;
using UnityEngine.UI;

namespace SoldatVirtual.Scripts
{
#if UNITY_EDITOR
    using Input = GoogleARCore.InstantPreviewInput;
#endif

    public class SoldatVirtualController : MonoBehaviour
    {
        public GameObject Environment;

        public GameObject Soldier;

        public GameObject Enemy;

        public SoldatVirtualUIController UIController;

        public SoldierMovement SoldierMovement;

        public Text EnvironmentText;
        public Text SoldierText;

        public Text EnemyText;

        public float ModelScalingFactor = 0.005f;

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        public void Start()
        {
            float newXScale = (float)(ModelScalingFactor);
            float newYScale = (float)(ModelScalingFactor);
            float newZScale = (float)(ModelScalingFactor);

            Environment.transform.localScale = new Vector3(newXScale, newYScale, newZScale);
            Environment.SetActive(false);

            // Initialize the Canvas
            UIController.RunModeEnabled();
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            EnvironmentText.text = "Environment Position: " + Environment.transform.position.ToString();
            EnvironmentText.text += ". Rotation: " + Environment.transform.rotation.ToString();

            SoldierText.text = "Soldat Position: " + Soldier.transform.position.ToString();
            SoldierText.text += ". Rotation: " + Soldier.transform.rotation.ToString();
            SoldierText.text += ". Destination: " + SoldierMovement.Destination;

            EnemyText.text = "Enemy Position: " + Enemy.transform.position.ToString();
            EnemyText.text += ". Rotation: " + Enemy.transform.rotation.ToString();

            _UpdateApplicationLifecycle();
        }

        public void OnRunModeButtonClick()
        {
            if (!SoldierMovement.IsSoldierStopped)
                return;

            UIController.RunModeEnabled();
            SoldierMovement.SetRunMode();
        }

        public void OnShotModeButtonClick()
        {
            if (!SoldierMovement.IsSoldierStopped)
                return;

            UIController.ShotModeEnabled();
            SoldierMovement.SetShotMode();
        }

        public void OnRestartButtonClick()
        {
            Start();
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
}