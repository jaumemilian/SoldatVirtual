using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class SoldierMovement : MonoBehaviour
{
    private void Start()
    {
        MessageManager.ShowAndroidToastMessage("Soldier Movement Start");
    }

    private void Update()
    {
        
    }

    // This function is called by the EventTrigger on the scene's ground when it is clicked on.
    public void OnGroundClick(BaseEventData data)
    {
        MessageManager.ShowAndroidToastMessage("Soldier Movement - OnGroundClick");
    }
}