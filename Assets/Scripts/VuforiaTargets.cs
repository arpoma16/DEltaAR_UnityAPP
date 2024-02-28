/*==============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using Vuforia;
using UnityEngine.UI;
/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class VuforiaTargets : MonoBehaviour
{
    public ARmanager ARmanagerControlguie;
    public int targetnumber; /// identificacion de cada targeta 
    private bool stateTarget;
    private Vector3 LastPosition;
    private bool flatposition;


    private ObserverBehaviour mObserverBehaviour;
    private Status  m_PreviousStatus;
    private Status m_NewStatus;

    void Awake()
    {

        LastPosition = gameObject.GetComponent<Transform>().position;
        flatposition = false;
        stateTarget = false;

        ObserverBehaviour mObserverBehaviour = GetComponent<ObserverBehaviour>();

        if (mObserverBehaviour != null)
            mObserverBehaviour.OnTargetStatusChanged += OnStatusChanged;
    }
    void OnDestroy()
    {
        if (mObserverBehaviour != null)
            mObserverBehaviour.OnTargetStatusChanged -= OnStatusChanged;
    }
    
    
    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        Debug.LogFormat("TargetName: {0}, Status is: {1}, StatusInfo is: {2}", behaviour.TargetName, status.Status, status.StatusInfo);
         Status mystatus = behaviour.TargetStatus.Status;
        m_PreviousStatus = m_NewStatus;
        m_NewStatus = mystatus;
        //if (mystatus == Status.DETECTED || newStatus == ObserverBehaviour.Status.TRACKED )
        if (mystatus == Status.TRACKED  )
        {
            // targeta encontrado
            stateTarget = true;   
            Debug.Log("Trackable "+targetnumber+"-" +  behaviour.TargetName + "  find");

            ARmanagerControlguie.TargetState(targetnumber, 1);//ARmanagerControlguie.ChangePosition(targetnumber);
            OnTrackingFound();


        }else if (mystatus == Status.EXTENDED_TRACKED)
        {
            // targeta encontrado
            Debug.Log("extended  "+targetnumber+"-" +  behaviour.TargetName + " extended");

            stateTarget = true;
            ARmanagerControlguie.TargetState(targetnumber, 2);
            //ARmanagerControlguie.ChangePosition(targetnumber);
            OnTrackingFound();

        }
        // else if (previousStatus == ObserverBehaviour.Status.TRACKED && newStatus == ObserverBehaviour.Status.NO_POSE)
        else if (mystatus == Status.NO_POSE || mystatus == Status.LIMITED )
        {
            // targeta perdida
            stateTarget = false;
            //ARmanagerControlguie.TargetState(targetnumber, stateTarget);
            ARmanagerControlguie.TargetState(targetnumber, 0);
            Debug.Log("Trackable "+targetnumber+"-" +  behaviour.TargetName + " lose ");

            //Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
             OnTrackingLost();
            flatposition = false;
        }else
        {
            Debug.Log("Trackable " +  behaviour.TargetName + " lose2");

            // trageta nunca se a encontrado 
            // For combo of previousStatus = UNKNOWN + newStatus= UNKNOWN | NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
             OnTrackingLost();//to hide the augmentations
        }
    }

        

    #region PROTECTED_METHODS

    protected virtual void OnTrackingFound()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;
    }


    protected virtual void OnTrackingLost()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;
    }

    #endregion // PROTECTED_METHODS



}
