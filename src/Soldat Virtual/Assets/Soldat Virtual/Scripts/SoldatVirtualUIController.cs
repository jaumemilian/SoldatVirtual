using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoldatVirtual.Scripts
{
#if UNITY_EDITOR
    using Input = GoogleARCore.InstantPreviewInput;
#endif
    public class SoldatVirtualUIController : MonoBehaviour
    {

        /// <summary>
        /// Scenario1 Button for toggling model
        /// </summary>
        public Button RunModeButton;

        /// <summary>
        /// Scenario1 Button for toggling model
        /// </summary>
        public Button ShotModeButton;

        public static bool _isInShotMode;

        // Start is called before the first frame update
        void Start()
        {
            _isInShotMode = false;

            RunModeButton.interactable = false;
            RunModeButton.enabled = false;

            ShotModeButton.interactable = false;
            ShotModeButton.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
        }

        public static bool IsInShotMode { get { return _isInShotMode; } }

        public void ShotModeEnabled()
        {
            _isInShotMode = true;

            RunModeButton.interactable = true;
            RunModeButton.enabled = true;

            ShotModeButton.interactable = false;
            ShotModeButton.enabled = false;
        }

        public void RunModeEnabled()
        {
            _isInShotMode = false;

            RunModeButton.interactable = false;
            RunModeButton.enabled = false;

            ShotModeButton.interactable = true;
            ShotModeButton.enabled = true;
        }
    }
}