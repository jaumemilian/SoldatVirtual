﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;
using UnityEngine.AI;

namespace SoldatVirtual.Scripts
{
#if UNITY_EDITOR
    using Input = GoogleARCore.InstantPreviewInput;
#endif

    public class SoldatVirtualController : MonoBehaviour
    {
        public GameObject Soldier;
        public GameObject Environment;
        public SoldatVirtualUIController UIController;
        public SoldierMovement SoldierMovement;

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        public void Start()
        {
            Soldier.SetActive(false);
            Environment.SetActive(false);
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
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