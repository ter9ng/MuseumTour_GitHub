/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections;

/// <summary>
/// This class manages the content displayed on top of cloud reco targets in this sample
/// </summary>
public class ContentManager : MonoBehaviour, ITrackableEventHandler
{
    #region EXPOSED_PUBLIC_VARIABLES

    /// <summary>
    /// The spinning wheel rendered while data is fetched from the server
    /// </summary>
    public Texture LoadingSpinner;

    /// <summary>
    /// The background texture behind the spinning wheel
    /// </summary>
    public Texture LoadingBackground;
    
    /// <summary>
    /// The root gameobject that serves as an augmentation for the image targets created by search results
    /// </summary>
    public GameObject AugmentationObject;
    
    /// <summary>
    /// Reference to the script handling animations between 2D and 3D.
    /// </summary>
    public AnimationsManager AnimationsManager;

    /// <summary>
    /// the URL the JSON data should be fetched from
    /// </summary>
    public string JsonServerUrl;

    #endregion  // EXPOSED_PUBLIC_VARIABLES



    #region PRIVATE_MEMBER_VARIABLES

    private bool mIsShowingBookData = false;
    private bool mIsLoadingBookData = false;
    private bool mIsLoadingBookThumb = false;
    
    private WWW mJsonBookInfo;
    private WWW mBookThumb;
        
    private string mLastBookMetadata = "";
    private BookData mBookData;
    private bool mIsJSONRequested = false;
    private bool mIsBookThumbRequested = false;
    private BookInformationParser mBookInformationParser;
    private bool mIsShowingMenu = false;

    private Touch mTouch;
    private RaycastHit mHit;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region UNTIY_MONOBEHAVIOUR_METHODS

    void Start ()
    {
        // setup BookInformationParser 
        mBookInformationParser = new BookInformationParser();
        mBookInformationParser.SetBookObject(AugmentationObject);

        TrackableBehaviour trackableBehaviour = AugmentationObject.transform.parent.GetComponent<TrackableBehaviour>();
        if (trackableBehaviour)
        {
            trackableBehaviour.RegisterTrackableEventHandler(this);
        }
        
        HideObject();
    }
    
    void Update () 
    {
        if( mIsShowingBookData )
        {
            if(Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast (ray, out mHit, 1000.0f)) {

                    if(mHit.collider.tag == "BookInformation" )                        
                    {
                        if (mBookData != null && mIsShowingMenu == false)
                        {
                            Application.OpenURL(mBookData.BookDetailUrl);
                        }
                    }
                }
            }
        }
        
        // Back Key opens the Abouts Screen
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {    
            Application.LoadLevel("Vuforia-2-AboutScreen");
        }

        if( mIsLoadingBookData )
        {    
            // Animates the loadingSpinner
            LoadJSONBookData(mLastBookMetadata);
        }

        if(mIsLoadingBookThumb )
        {
            LoadBookThumb();
        }
    }

    void OnGUI()
    {
        if(mIsLoadingBookData || mIsLoadingBookThumb)
        {
            // Draws the Loading background
            Rect backgroundRect = new Rect(Screen.width/2.0f -LoadingBackground.width/2f, Screen.height/2.0f - LoadingBackground.height/2f, LoadingBackground.width, LoadingBackground.height);
            GUI.DrawTexture(backgroundRect,LoadingBackground);
            
            // Draws the rotating loading spinner
            Matrix4x4 oldMatrix = GUI.matrix;
            float thisAngle = Time.frameCount * 4;

            Rect spinnerRect = new Rect(Screen.width/2.0f -LoadingSpinner.width/2f, Screen.height/2.0f - LoadingSpinner.height/2f, LoadingSpinner.width, LoadingSpinner.height);
        
            GUIUtility.RotateAroundPivot (thisAngle, spinnerRect.center);
            GUI.DrawTexture(spinnerRect, LoadingSpinner);
            GUI.matrix = oldMatrix;
        }
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS



    #region PUBLIC_METHODS
    
    /// <summary>
    /// Method called from the CloudRecoEventHandler
    /// when a new target is created
    /// </summary>
    public void TargetCreated(string targetMetadata)
    {
        // Initialize the showing book data variable
        mIsShowingBookData = true;
        mIsLoadingBookData = true;
            
        // initialize the last tracked book metadata
        mLastBookMetadata = targetMetadata;
            
        mIsJSONRequested = false;
        mIsBookThumbRequested = false;
            
        // Loads the JSON Book Data
        LoadJSONBookData(targetMetadata);
    }
    

    /// <summary>
    /// Method called when the Close button is pressed
    /// to clean the target Data
    /// </summary>
    public void TargetDeleted()
    {
        // Initialize the showing book data variable
        mIsShowingBookData = false;
        mIsLoadingBookData = false;
        mIsLoadingBookThumb = false;
        
        mBookData = null;
    }

    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED)
        {
            TargetFound();
        }
        else
        {
            TargetLost();
        }
    }

    /// <summary>
    /// hides the augmentation object
    /// </summary>
    public void HideObject()
    {
        Renderer[] rendererComponents = AugmentationObject.GetComponentsInChildren<Renderer>();
        Collider[] colliderComponents = AugmentationObject.GetComponentsInChildren<Collider>();

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = false;
        }

        // Enable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = false;
        }
    }


    /// <summary>
    /// Method to let the ContentManager know if the CloudReco
    /// SampleMenu is being displayed
    /// </summary>
    public void SetIsShowingMenu(bool isShowing)
    {
        mIsShowingMenu = isShowing;
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS


    /// <summary>
    /// Method called from the CloudReco Trackable Event Handler
    /// when a target has been found
    /// </summary>
    private void TargetFound()
    {
        // Checks tha the book info is displayed
        if (mIsShowingBookData)
        {
            // Starts playing the animation to 3D
            AnimationsManager.PlayAnimationTo3D(AugmentationObject);
        }
    }


    /// <summary>
    /// Method called from the CloudReco Trackable Event Handler
    /// when a target has been Lost
    /// </summary>
    private void TargetLost()
    {
        // Checks tha the book info is displayed
        if (mIsShowingBookData)
        {
            // Starts playing the animation to 2D
            AnimationsManager.PlayAnimationTo2D(AugmentationObject);
        }
    }

    /// <summary>
    /// fetches the JSON data from a server
    /// </summary>
    private void LoadJSONBookData(string jsonBookUrl)
    {
        // Gets the full book json url
        string fullBookURL = JsonServerUrl + jsonBookUrl;
    
        if(!mIsJSONRequested){

            // Gets the json book info from the url
            mJsonBookInfo = new WWW(fullBookURL);
            mIsJSONRequested = true;
        }
        
        if(mJsonBookInfo.progress >= 1)
        {
            if(mJsonBookInfo.error == null )
            {
                // Parses the json Object
                JSONParser parser = new JSONParser();
                
                BookData bookData = parser.ParseString(mJsonBookInfo.text);
                mBookData = bookData;
                
                // Updates state variables
                mIsLoadingBookData = false;
        
                // Updates the BookData info in the augmented object
                mBookInformationParser.UpdateBookData(bookData);
        
                mIsLoadingBookThumb = true;
                
            }else
            {
                Debug.LogError("Error downloading json");
                mIsLoadingBookData = false;
            }
        }
    }
    
    /// <summary>
    /// Loads the texture for the book thumbnail
    /// </summary>
    private void LoadBookThumb()
    {
        if(!mIsBookThumbRequested )            
        {
            if(mBookData != null )
            {
                mBookThumb = new WWW(mBookData.BookThumbUrl);
            
                mIsBookThumbRequested = true;
            }
        }
        
        if(mBookThumb.progress >=1)
        {
            if(mBookThumb.error == null && mBookData != null)
            {
                mBookInformationParser.UpdateBookThumb(mBookThumb.texture);
                mIsLoadingBookThumb = false;

                ShowObject();
            }
        }
    }
    
    /// <summary>
    /// shows the augmentation object
    /// </summary>
    private void ShowObject()
    {
        Renderer[] rendererComponents = AugmentationObject.GetComponentsInChildren<Renderer>();
        Collider[] colliderComponents = AugmentationObject.GetComponentsInChildren<Collider>();

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = true;
        }

        // Enable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = true;
        }
    }

    #endregion // PRIVATE_METHODS
}
