/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System;
using System.Collections.Generic;


/// <summary>
/// Menu that appears on double tap, enables and disables the AutoFocus on the camera.
/// </summary>
[RequireComponent(typeof(CloudRecoBehaviour))]
public class CloudRecoSampleMenu : MonoBehaviour, ITrackerEventHandler
{
    #region PRIVATE_MEMBER_VARIABLES

    private const string AUTOFOCUS_ON = "Autofocus On";
    private const string AUTOFOCUS_OFF = "Autofocus Off";

    // Check if a menu button has been pressed.
    private bool mButtonPressed = false;

    // If the menu is currently open
    private bool mMenuOpen = false;

    // Contains if the device supports continous autofocus
    private bool mContinousAFSupported = true;

    // Contains the currently set auto focus mode.
    private CameraDevice.FocusMode mFocusMode =
        CameraDevice.FocusMode.FOCUS_MODE_NORMAL;

    // this is used to distinguish single and double taps
    private bool mWaitingForSecondTap;
    private Vector3 mFirstTapPosition;
    private DateTime mFirstTapTime;
    // the maximum distance that is allowed between two taps to make them count as a double tap
    // (relative to the screen size)
    private const float MAX_TAP_DISTANCE_SCREEN_SPACE = 0.1f;
    private const int MAX_TAP_MILLISEC = 500;

    // reference to the CloudRecoBehaviour
    private CloudRecoBehaviour mCloudRecoBehaviour;

    // reference to the ContentManager in this sample
    ContentManager mContentManager;
    
    // Unity GUI Skin containing settings for font and custom image buttons
    private GUISkin mUISkin;
    
    // dictionary to hold gui styles, fetching them each time a button is drawn is slow
    private Dictionary<string, GUIStyle> mButtonGUIStyles;

    private string mAutoFocusText = "";


    /// <summary>
    /// This float returns a resolution dependent scale factor.
    /// Using this, elements can be drawn as if the resolution was 480 (smaller dimension)
    /// on every device.
    /// </summary>
    private static float DeviceDependentScale
    {
        get 
        {
            if ( Screen.width > Screen.height)
                return Screen.height / 480f;
            else 
                return Screen.width / 480f; 
        }
    }
    
    #endregion // PRIVATE_MEMBER_VARIABLES



    #region UNTIY_MONOBEHAVIOUR_METHODS

    public void Start()
    {
        // register for the OnInitialized event at the QCARBehaviour
        QCARBehaviour qcarBehaviour = (QCARBehaviour)FindObjectOfType(typeof(QCARBehaviour));
        if (qcarBehaviour)
        {
            qcarBehaviour.RegisterTrackerEventHandler(this);
        }

        // obtain reference to the cloud reco behaviour:
        mCloudRecoBehaviour = (CloudRecoBehaviour)FindObjectOfType(typeof(CloudRecoBehaviour));

        // obtaon reference to the content manager:
        mContentManager = (ContentManager)FindObjectOfType(typeof(ContentManager));
        
        // load and set gui style
        mUISkin = Resources.Load("UserInterface/ButtonSkins") as GUISkin;
        
        // remember all custom styles in gui skin to avoid constant lookups
        mButtonGUIStyles = new Dictionary<string,GUIStyle>();
        foreach (GUIStyle customStyle in mUISkin.customStyles) 
            mButtonGUIStyles.Add(customStyle.name, customStyle);
    }
    


    public void Update()
    {		
		 if (Application.platform == RuntimePlatform.Android)
        {

            if (Input.GetKey(KeyCode.Escape))
            {

                Application.LoadLevel(1);

            }

        }
    }


    // Draw menus.
    public void OnGUI()
    {
        
        
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS



    #region ITrackerEventHandler_IMPLEMENTATION

    /// <summary>
    /// This method is called when QCAR has finished initializing
    /// </summary>
    public void OnInitialized()
    {
        // try to set continous auto focus as default
        if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO))
        {
            mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO;
            mAutoFocusText = AUTOFOCUS_OFF;
        }
        else
        {
            Debug.LogError("could not switch to continuous autofocus");
            mContinousAFSupported = false;
            mAutoFocusText = "Cont. Auto Focus not supported";
        }
    }

    public void OnTrackablesUpdated()
    {
        // not used
    }

    #endregion //ITrackerEventHandler_IMPLEMENTATION



    #region PRIVATE_METHODS

    private void HandleSingleTap()
    {
        mWaitingForSecondTap = false;

        if (mMenuOpen)
            mMenuOpen = false;
        else
        {
            // trigger focus once
            if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO))
            {
                mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO;
                mAutoFocusText = AUTOFOCUS_ON;
            }
        }
    }

    private void HandleDoubleTap()
    {
        mWaitingForSecondTap = false;
        mMenuOpen = true;
    }

    #endregion // PRIVATE_METHODS
}