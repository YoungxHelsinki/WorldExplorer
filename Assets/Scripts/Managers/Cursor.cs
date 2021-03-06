﻿
using HoloToolkit.Unity;
using Symbols;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class Cursor : CursorManager
{
    // private MeshRenderer meshRenderer;
    // Represents the hologram that is currently being gazed at.
    public GameObject FocusedObject { get; private set; }
    private GameObject prevTarget;
    GestureRecognizer recognizer;
  //  protected readonly SelectionHandler selectionHandler = SelectionHandler.Instance;
    private GameObject oldFocusObject;
    private bool mouse = false;

    // Use this for initialization
    void Start()
    {
        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            // Send an OnSelect message to the focused object and its ancestors.
            oldFocusObject = FocusedObject;
            //if (GazeManager.Instance.HitInfo.transform != null)
            //{
            //    GameObject hitInfo = GazeManager.Instance.HitInfo.transform.gameObject;
            //    if (hitInfo.tag == "symbol")
            //    {
            //        // Send an OnSelect message to the focused object and its ancestors.
            //     //   if (oldFocusObject != hitInfo.transform.gameObject && selectionHandler.GameObjectIsSelected(hitInfo.transform.gameObject))
            //       // { FocusedObject = hitInfo.transform.gameObject;

            //      //  }
            //     // /  else
            //     //       FocusedObject = null;
            //    }//
            //}
            if (FocusedObject != oldFocusObject)
            {
                if (oldFocusObject != null)
                    oldFocusObject.BroadcastMessage("OnSelect", SendMessageOptions.RequireReceiver);
                if (FocusedObject != null)
                    FocusedObject.BroadcastMessage("OnSelect", SendMessageOptions.RequireReceiver);
            }
        };
        recognizer.StartCapturingGestures();
    }

    // private void OnTriggerEnter(Collider other)
    // {
    // }

    //private float nextActionTime = 1.0f;
    // in seconds
    //private float period = 1.0f;
    // Update is called once per frame
    void Update()
    {
        // Do a raycast into the world based on the user's
        // head position and orientation.
        if (GazeManager.Instance.HitInfo.transform != null)
        {
            GameObject hitInfo = GazeManager.Instance.HitInfo.transform.gameObject;
            if (hitInfo.tag == "symbol")
            {
                if (prevTarget == null)
                    hitInfo.transform.gameObject.GetComponent<SymbolTargetHandler>().Show();
                else
                    if (prevTarget != hitInfo.transform.gameObject)
                {
                    prevTarget.GetComponent<SymbolTargetHandler>().Hide();
                    hitInfo.transform.gameObject.GetComponent<SymbolTargetHandler>().Show();
                }
                prevTarget = hitInfo.transform.gameObject;
                if (Input.GetMouseButtonDown(0)) 
                {
                    mouse = true;
                    oldFocusObject = FocusedObject;
                    if (oldFocusObject != hitInfo.transform.gameObject)
                        FocusedObject = hitInfo.transform.gameObject;
                    else
                        FocusedObject = null;
                }
            }
            else
            {
                if (prevTarget != null)
                {
                    prevTarget.GetComponent<SymbolTargetHandler>().Hide();
                    prevTarget = null;
                }
            }
        }

        // If the focused object changed this frame,
        // start detecting fresh gestures again.
        if (mouse)
        {
            if (FocusedObject != oldFocusObject)
            {
                if (oldFocusObject != null)
                    oldFocusObject.BroadcastMessage("OnSelect");
                if (FocusedObject != null)
                    FocusedObject.BroadcastMessage("OnSelect");
                recognizer.CancelGestures();
                recognizer.StartCapturingGestures();
            }
            mouse=false;
        }
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
    }
}