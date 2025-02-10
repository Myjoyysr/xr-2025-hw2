using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomGrab : MonoBehaviour
{
    // This script should be attached to both controller objects in the scene
    // Make sure to define the input in the editor (LeftHand/Grip and RightHand/Grip recommended respectively)
    CustomGrab otherHand = null;
    public List<Transform> nearObjects = new List<Transform>();
    public Transform grabbedObject = null;
    public InputActionReference action;
    bool grabbing = false;

    private Vector3 lastPosition;
    private Quaternion lastRotation;
    //private Vector3 lastLocalPosition;

    public InputActionReference doubleRotationAction;
    private bool doubleRotation;

    private void Start()
    {
        action.action.Enable();
        doubleRotationAction.action.Enable();

        // Find the other hand
        foreach(CustomGrab c in transform.parent.GetComponentsInChildren<CustomGrab>())
        {
            if (c != this)
                otherHand = c;
        }

    }
    
    void Update()
    {
        grabbing = action.action.IsPressed();
        doubleRotation = doubleRotationAction.action.IsPressed();

        if (grabbing)
        {
            Debug.Log("grabbing");

            // Grab nearby object or the object in the other hand
            if (!grabbedObject){
                grabbedObject = nearObjects.Count > 0 ? nearObjects[0] : otherHand.grabbedObject;
            }
            
            if (grabbedObject)
            {
                // Change these to add the delta position and rotation instead
                // Save the position and rotation at the end of Update function, so you can compare previous pos/rot to current here
                //grabbedObject.position = transform.position;
                //grabbedObject.rotation = transform.rotation;
                // 1. calculate the deltas:
                Vector3 deltaPosition = transform.position - lastPosition;
                Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);

                if (otherHand.grabbedObject==grabbedObject && otherHand.grabbing){
                    //rotation
                    //deltaRotation = deltaRotation * Quaternion.Inverse(otherHand.lastRotation) * otherHand.transform.rotation;
                    //deltaRotation = Quaternion.Slerp((Quaternion.Inverse(otherHand.lastRotation) * otherHand.transform.rotation), deltaRotation, 0.5f);

                    Vector3 deltaPositionBoth = deltaPosition + (otherHand.transform.position -  otherHand.lastPosition);// * 0.5f;
                    grabbedObject.position = grabbedObject.position + deltaPositionBoth * 0.5f;

                }else{
                    grabbedObject.position = grabbedObject.position+ deltaPosition;
                }
      
                //grabbedObject.position = grabbedObject.position+ deltaPosition;

                if (!doubleRotation){
                    // 2. calculate and apply rotation
                    grabbedObject.rotation = deltaRotation * grabbedObject.rotation;

                }else{

                    // 2. calculate and apply rotation (double angle)
                    deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
                    grabbedObject.rotation = Quaternion.AngleAxis(angle * 2.0f, axis) * grabbedObject.rotation;
                }
 
                // 3. calculate and apply new position related to the the controller
                Vector3 controllerVector = grabbedObject.position - transform.position;
                controllerVector = deltaRotation * controllerVector;

                if (otherHand.grabbedObject == grabbedObject && otherHand.grabbing){
                    grabbedObject.position = transform.position + (controllerVector);
                    
                }else{
                    grabbedObject.position = transform.position + controllerVector;
                }

            }
        }
        // If let go of button, release object
        else if (grabbedObject)
            grabbedObject = null;

        // Should save the current position and rotation here
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Make sure to tag grabbable objects with the "grabbable" tag
        // You also need to make sure to have colliders for the grabbable objects and the controllers
        // Make sure to set the controller colliders as triggers or they will get misplaced
        // You also need to add Rigidbody to the controllers for these functions to be triggered
        // Make sure gravity is disabled though, or your controllers will (virtually) fall to the ground

        Debug.Log("Collision");

        Transform t = other.transform;
        if(t && t.tag.ToLower()=="grabbable")
            nearObjects.Add(t);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Collision EXIT");
        Transform t = other.transform;
        if( t && t.tag.ToLower()=="grabbable")
            nearObjects.Remove(t);
    }
}