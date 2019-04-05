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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}