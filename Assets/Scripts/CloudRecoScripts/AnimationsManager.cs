/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections;

/// <summary>
/// This class handles the animations of the augmentation
/// </summary>
public class AnimationsManager : MonoBehaviour
{
    #region PUBLIC_MEMBER_VARIABLES

    // Reference to the 2D Overlay position
    public GameObject OverlayPosition;

    #endregion // PUBLIC_MEMBER_VARIABLES



    #region PRIVATE_MEMBER_VARIABLES

    // Reference to the augmentation object to animate
    private GameObject mAugmentationObject;
    
    private bool mIsTracking = false;
    private bool mDoAnimationTo2D = false;
    private bool mDoAnimationTo3D = false;
    private bool mIsShowingOverlay = false;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region UNITY_MONOBEHAVIOUR_METHODS

    void Start () 
    {    
        if(Screen.dpi > 260)
            OverlayPosition.transform.localPosition = new Vector3(0,0, 550);
    }
    
    // Update is called once per frame
    void Update () 
    {
        if(mAugmentationObject == null )
        {
            mDoAnimationTo2D = false;
            mDoAnimationTo3D = false;
            return;
        }
    
        // Performs Animation to 2D
        if(mDoAnimationTo2D )
        {
            // Updates AugmentedObject position and rotation in every frame
            mAugmentationObject.transform.position = Vector3.Lerp(mAugmentationObject.transform.position, OverlayPosition.transform.position,   Time.deltaTime * 5.0f);
            mAugmentationObject.transform.rotation = Quaternion.Slerp(mAugmentationObject.transform.rotation, OverlayPosition.transform.rotation, Time.deltaTime * 5.0f);
            
            // Checks for object distance to check animation finish
            if(Vector3.Distance(mAugmentationObject.transform.position, OverlayPosition.transform.position) < 1)
            {
                mDoAnimationTo2D = false;
            }
        }
        
        // Performs Animation to 2D
        if(mDoAnimationTo3D)
        {
            // Updates AugmentedObject position and rotation in every frame
            mAugmentationObject.transform.localPosition = Vector3.Lerp( mAugmentationObject.transform.localPosition, new Vector3(0,0,0),   Time.deltaTime * 5.0f);
            mAugmentationObject.transform.localRotation = Quaternion.Slerp( mAugmentationObject.transform.localRotation, Quaternion.identity, Time.deltaTime * 5.0f);
            
            // Checks for object distance to check animation finish
            if(Vector3.Distance(mAugmentationObject.transform.localPosition, new Vector3(0,0,0)) < 0.01f)
            {
                mDoAnimationTo3D = false;
            }
        }
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS



    #region PUBLIC_METHODS

    // Starts playing animation to 2D
    public void PlayAnimationTo2D(GameObject augmentationObject)
    {
        mAugmentationObject = augmentationObject;
        
        // Checks that the system is already tracking
        if(mIsTracking)
        {
            mDoAnimationTo2D = true;
            mIsShowingOverlay = true;
        }
        
        // Updates state variables
        mDoAnimationTo3D = false;
        mIsTracking = false;
        
    }
    
    // Starts playing animation to 3D
    public void PlayAnimationTo3D( GameObject augmentedObject )
    {
        mAugmentationObject = augmentedObject;
        mDoAnimationTo2D = false;
        
        // Checks that the system is showing the overlay right now
        if(mIsShowingOverlay)
        {
            mDoAnimationTo3D = true;
            
            // Updates the augmented object initial position to the overlay position.
            // Since the overlayPosition is child of the ARCamera, once the trcking
            // starts again the ARCamera position is updated relative to the target position
            augmentedObject.transform.position = OverlayPosition.transform.position;
        }
        
        // Updates state variables
        mIsShowingOverlay = false;
        mIsTracking = true;
    }

    #endregion // PUBLIC_METHODS
}
