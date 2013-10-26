/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
==============================================================================*/

using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results as well as error messages
/// The current state is visualized and new results are enabled using the TargetFinder API.
/// </summary>
public class CloudRecoEventHandler : MonoBehaviour, ICloudRecoEventHandler
{
    #region PRIVATE_MEMBER_VARIABLES

    // CloudRecoBehaviour reference to avoid lookups
    private CloudRecoBehaviour mCloudRecoBehaviour;
    // ImageTracker reference to avoid lookups
    private ImageTracker mImageTracker;
    // reference to the cloud reco scene manager component:
    private ContentManager mContentManager;

    //  array that all textures are loaded into on startup
    private Texture mRequestingTexture;

    // the parent gameobject of the referenced ImageTargetTemplate - reused for all target search results
    private GameObject mParentOfImageTargetTemplate;

    public static long tickCounter;

    public static string textureTest;

    public static bool dataLoaded;

    public static bool questionTrigger;

    public static bool questionAdded;

    public static bool wrongImageScanned;

    public static string inputQuestion;

    public static string inputSolution;

    public static string alternativeAnswer1;

    public static string alternativeAnswer2;

    public static string alternativeAnswer3;

    public static string dialogButton;

    public static string tp_name;
    public static string tp_info;
    public static string tp_hintImg;
    public static string tp_id;
    public static long toastTimestamp = 0;
	
	
	float w = 0.3f; // proportional width (0..1)
    float h = 0.05f; // proportional height (0..1)
	Rect TextRect = new Rect();
	Rect Text2Rect = new Rect();
	Rect Button1Rect = new Rect();
	Rect QuestionButtonRect = new Rect();
	Rect ContinueButtonRect = new Rect();
	Rect TestToMainMenuButtonRect = new Rect();

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region EXPOSED_PUBLIC_VARIABLES

    /// <summary>
    /// can be set in the Unity inspector to reference a ImageTargetBehaviour that is used for augmentations of new cloud reco results.
    /// </summary>
    public ImageTargetBehaviour ImageTargetTemplate;
	
	public static string[] info;
	
	public static string getTargetFromMetadataUrl = "http://129.241.103.145/getTargetFromMetadata.php";
	
	public static string postQuestionUrl = "http://129.241.103.145/postQuestion.php";
	
	public static string imgHostName = "http://129.241.103.145/hintImg/";
	
	public static ArrayList arElementsForTPArray = new ArrayList();

    #endregion



    #region ICloudRecoEventHandler_IMPLEMENTATION

    /// <summary>
    /// called when TargetFinder has been initialized successfully
    /// </summary>
    public void OnInitialized()
    {
        // get a reference to the Image Tracker, remember it
        mImageTracker = (ImageTracker)TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER);
        mContentManager = (ContentManager)FindObjectOfType(typeof(ContentManager));
    }

    /// <summary>
    /// visualize initialization errors
    /// </summary>
    public void OnInitError(TargetFinder.InitState initError)
    {
        switch (initError)
        {
            case TargetFinder.InitState.INIT_ERROR_NO_NETWORK_CONNECTION:
                ErrorMsg.New("Network Unavailable", "Failed to initialize CloudReco because the device has no network connection.", LoadAboutScene);
                break;
            case TargetFinder.InitState.INIT_ERROR_SERVICE_NOT_AVAILABLE:
                ErrorMsg.New("Service Unavailable", "Failed to initialize CloudReco because the service is not available.", LoadAboutScene);
                break;
        }
    }

    /// <summary>
    /// visualize update errors
    /// </summary>
    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        switch (updateError)
        {
            case TargetFinder.UpdateState.UPDATE_ERROR_AUTHORIZATION_FAILED:
                ErrorMsg.New("Authorization Error","The cloud recognition service access keys are incorrect or have expired.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_BAD_FRAME_QUALITY:
                ErrorMsg.New("Poor Camera Image","The camera does not have enough detail, please try again later");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_NO_NETWORK_CONNECTION:
                ErrorMsg.New("Network Unavailable","Please check your internet connection and try again.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_PROJECT_SUSPENDED:
                ErrorMsg.New("Authorization Error","The cloud recognition service has been suspended.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_REQUEST_TIMEOUT:
                ErrorMsg.New("Request Timeout","The network request has timed out, please check your internet connection and try again.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_SERVICE_NOT_AVAILABLE:
                ErrorMsg.New("Service Unavailable","The service is unavailable, please try again later.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_TIMESTAMP_OUT_OF_RANGE:
                ErrorMsg.New("Clock Sync Error","Please update the date and time and try again.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_UPDATE_SDK:
                ErrorMsg.New("Unsupported Version","The application is using an unsupported version of Vuforia.");
                break;
        }
    }

    /// <summary>
    /// when we start scanning, unregister Trackable from the ImageTargetTemplate, then delete all trackables
    /// </summary>
    public void OnStateChanged(bool scanning)
    {
        if (scanning)
        {
            // clear all known trackables
            mImageTracker.TargetFinder.ClearTrackables(false);

            // hide the ImageTargetTemplate
            mContentManager.HideObject();
        }
    }

    /// <summary>
    /// Handles new search results
    /// </summary>
    /// <param name="targetSearchResult"></param>
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        // This code demonstrates how to reuse an ImageTargetBehaviour for new search results and modifying it according to the metadata
        // Depending on your application, it can make more sense to duplicate the ImageTargetBehaviour using Instantiate(), 
        // or to create a new ImageTargetBehaviour for each new result

        // Vuforia will return a new object with the right script automatically if you use
        // TargetFinder.EnableTracking(TargetSearchResult result, string gameObjectName)
        
        //Check if the metadata isn't null
		
		
        if(targetSearchResult.MetaData == null)
        {
            return;
        }
		
		if(targetSearchResult.MetaData != null)
		{
			string id  = targetSearchResult.MetaData;
			//getInfo(id); //info =
			StartCoroutine(getInfo(id));
			string URL = "lol";
			
			StartCoroutine(setTexture(URL));
		}
		
		


        // enable the new result with the same ImageTargetBehaviour:
        ImageTargetBehaviour imageTargetBehaviour = mImageTracker.TargetFinder.EnableTracking(targetSearchResult, mParentOfImageTargetTemplate);

        if (imageTargetBehaviour != null)
        {
            // stop the target finder
            mCloudRecoBehaviour.CloudRecoEnabled = false;
            
            // Calls the TargetCreated Method of the SceneManager object to start loading
            // the BookData from the JSON
            mContentManager.TargetCreated(targetSearchResult.MetaData);
        }
    }
	
	private IEnumerator setTexture (string URL)
	{
		while(!dataLoaded) 
		{
			yield return new WaitForSeconds(0.1f);	
		}
		WWW www = new WWW (imgHostName+textureTest);
			yield return www;
		
		GameObject.Find("flate").renderer.material.mainTexture = www.texture;
		
		
		

	}
	
	private IEnumerator getInfo(string id) 
	{//string[]
		
		WWWForm form = new WWWForm();
		form.AddField("tp_id", id); 
		WWW getTargetFromMetadata = new WWW(getTargetFromMetadataUrl, form);
		yield return getTargetFromMetadata;
		if ((!string.IsNullOrEmpty(getTargetFromMetadata.error)))
        {
            Debug.Log(getTargetFromMetadata.text);
            print("Error downloading target data: " + getTargetFromMetadata.error);
        }
		else{

			var returnedString = getTargetFromMetadata.data;
			string[] values =returnedString.Split('&');
			var tpData = values[0]; //Data strictly related to the given target picture, such as name and info
			string[] tpDataSplit = tpData.Split('|');
			tp_name = tpDataSplit[0];
			tp_info = tpDataSplit[1];
			tp_hintImg = tpDataSplit[2];
			tp_id = tpDataSplit[3];
			textureTest = tp_hintImg;
			Debug.Log ("tp_name: "+tp_name+" tp_info: "+tp_info);
				for(int i=1;i<values.Length-1;i++)
				{
					ArrayList arElementValueArray = new ArrayList();
					var arElementData = values[i];
					string[] arElementValues = arElementData.Split('|');
						string ar_element_id = arElementValues[0];
						string are_name = arElementValues[1];
						string url = arElementValues[2];
				Debug.Log ("Value: "+i+" ar_element_id: "+ar_element_id+" are_name: "+are_name+" url: "+url);
						arElementValueArray.Add(ar_element_id); arElementValueArray.Add(are_name); arElementValueArray.Add(url);
						arElementsForTPArray.Add(arElementValueArray);
						Debug.Log(arElementsForTPArray.Count);
			}
				getTargetFromMetadata.Dispose();
        }
		
		dataLoaded = true;
		
	}
	
	public static IEnumerator postQuestion(){
		Debug.Log ("entered postQuestion");
		WWWForm form = new WWWForm();
		

		form.AddField("tp_id", tp_id); 
		string group = PlayerPrefs.GetString("group");
        string code = PlayerPrefs.GetString("code");
		form.AddField("code",code);
		form.AddField("grp_name",group);
		form.AddField("question",inputQuestion);
		form.AddField("solution",inputSolution);
		form.AddField("alternative1",alternativeAnswer1);
		form.AddField("alternative2",alternativeAnswer2);
		form.AddField("alternative3",alternativeAnswer3);
		inputQuestion = "";
		inputSolution = "";
		Debug.Log ("lets do this");
		WWW postQuestion = new WWW(postQuestionUrl, form);
		yield return postQuestion;
		if ((!string.IsNullOrEmpty(postQuestion.error)))
        {
            Debug.Log(postQuestion.text);
            print("Error posting question: " + postQuestion.error);
        }
		else{
			var returnedString = postQuestion.data;
			Debug.Log (postQuestion.text);
		}
	
	}

    #endregion // ICloudRecoEventHandler_IMPLEMENTATION



    #region UNTIY_MONOBEHAVIOUR_METHODS

    /// <summary>
    /// register for events at the CloudRecoBehaviour
    /// </summary>
    void Start()
    {
        tickCounter = 0;
        textureTest = "";
        dataLoaded = false;
        questionTrigger = false;
        questionAdded = false;
        wrongImageScanned = false;
        inputQuestion = "";
        inputSolution = "";
        alternativeAnswer1 = "";
        alternativeAnswer2 = "";
        alternativeAnswer3 = "";
        dialogButton = "Add question";

        // look up the gameobject containing the ImageTargetTemplate:
        mParentOfImageTargetTemplate = ImageTargetTemplate.gameObject;

        // intialize the ErrorMsg class
        ErrorMsg.Init();

        // register this event handler at the cloud reco behaviour
        CloudRecoBehaviour cloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        if (cloudRecoBehaviour)
        {
            cloudRecoBehaviour.RegisterEventHandler(this);
        }

        // remember cloudRecoBehaviour for later
        mCloudRecoBehaviour = cloudRecoBehaviour;

        // load and remember all used textures:
        mRequestingTexture = Resources.Load("UserInterface/TextureRequesting") as Texture;
		
		//********TEST*************
		
		
  		TextRect.x = (Screen.width*(1-w))/2;
  		TextRect.y = (Screen.height*(1-h))/2;
  		TextRect.width = Screen.width*w;
  		TextRect.height = Screen.height*(h/2);
		
		
  		Text2Rect.x = (Screen.width*(1-w))/2;
  		Text2Rect.y = TextRect.y + 100;
  		Text2Rect.width = Screen.width*w;
  		Text2Rect.height = Screen.height*(h/2);
		
		
  		Button1Rect.x = (Screen.width*(1-w))/2;
  		Button1Rect.y = TextRect.y + 200;
  		Button1Rect.width = Screen.width*w;
  		Button1Rect.height = Screen.height*h;
		
		
		QuestionButtonRect.width = Screen.width*w;
  		QuestionButtonRect.height = Screen.height*h;
		QuestionButtonRect.x = Screen.width/2 -QuestionButtonRect.width/2;
		QuestionButtonRect.y = Screen.height - 200;
		
		ContinueButtonRect.width = Screen.width*w;
		ContinueButtonRect.height = Screen.height*h;
		ContinueButtonRect.x = Screen.width/2 - QuestionButtonRect.width;
		ContinueButtonRect.y = Screen.height - 200;
		
		TestToMainMenuButtonRect.width = Screen.width*w;
		TestToMainMenuButtonRect.height = Screen.height*h+ ContinueButtonRect.height;
		TestToMainMenuButtonRect.x = Screen.width/2 - QuestionButtonRect.width;
		TestToMainMenuButtonRect.y = Screen.height - 200;
		
    }

    /// <summary>
    /// draw the sample GUI and error messages
    /// </summary>
    void OnGUI()
    {		
		//*************************
		
		
		
        if (mCloudRecoBehaviour.CloudRecoInitialized && mImageTracker.TargetFinder.IsRequesting())
        {
            // draw the requesting texture
            DrawTexture(mRequestingTexture);
        }
		
			GameObject infoText = GameObject.Find("InfoText");
			TextMesh t = (TextMesh)infoText.GetComponent(typeof(TextMesh));
			tp_info = tp_info.Replace("\\n", "\n");
			t.text = tp_info;
		

        // draw error messages in case there were any
        ErrorMsg.Draw();
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS



    #region PRIVATE_METHODS

    /// <summary>
    /// draws a textures using UnityGUI
    /// </summary>
    private void DrawTexture(Texture texture)
    {
        // scale texture up by device dependent factor
        int smallerScreenDimension = Screen.width < Screen.height ? Screen.width : Screen.height;
        float deviceDependentScale = smallerScreenDimension / 480f;

        // draw texture at center with given verticalPosition and scale:
        float width = texture.width * deviceDependentScale;
        float height = texture.height * deviceDependentScale;
        float left = (Screen.width * 0.5f) - (width * 0.5f);
        float top = (Screen.height * 0.9f) - (height * 0.5f);
        GUI.DrawTexture(new Rect(left, top, width, height), texture);
    }

    /// <summary>
    /// Loads the about scene.
    /// </summary>
    private void LoadAboutScene()
    {
        Application.LoadLevel("Vuforia-2-AboutScreen");
    }

    #endregion // PRIVATE_METHODS
}