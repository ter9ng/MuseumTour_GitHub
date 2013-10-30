using UnityEngine;
using System.Collections;
using System;


public class Menu : MonoBehaviour {
	
	string sessioncode = "";
	string groupname = "";
    string url = "http://129.241.103.145/sendMessage.php";
    bool post = false;
	private static long toastTimestamp = 0;
	private static string toastMessage = "";
	private bool showToast = false;
	public bool session1Enabled;
	public bool session2Enabled;
	public GUISkin customskin;
	
	
	void OnGUI () {
		
		float w = 0.3f; // proportional width (0..1)
  		float h = 0.05f; // proportional height (0..1)
		GUI.skin = customskin;
		var border = new RectOffset(0, 0, 0, 0);
		
		GUI.skin.button.fontSize = Screen.height / 30;
		GUI.skin.label.fontSize = Screen.height / 40;
		GUI.skin.textField.fontSize = Screen.height / 40;
		
		
		var Label1Rect = new Rect();
  		Label1Rect.x = (Screen.width*(1-w))/2;
  		Label1Rect.y = (Screen.height / 3);
  		Label1Rect.width = Screen.width*w;
  		Label1Rect.height = Screen.height*(h);
		
  		var TextRect = new Rect();
  		TextRect.width = Screen.width*w;
  		TextRect.height = Screen.height*(h);
		TextRect.x = (Screen.width*(1-w))/2;
  		TextRect.y = Label1Rect.y + 30 + TextRect.height;
		
		var Label2Rect = new Rect();
  		Label2Rect.width = Screen.width*w*2;
  		Label2Rect.height = Screen.height*(h);
		Label2Rect.x = (Screen.width/2) -Label2Rect.width/2;
  		Label2Rect.y = TextRect.y + 30 + Label2Rect.height;
		
		var Text2Rect = new Rect();
  		Text2Rect.width = Screen.width*w;
  		Text2Rect.height = Screen.height*(h);
  		Text2Rect.x = (Screen.width*(1-w))/2;
  		Text2Rect.y = Label2Rect.y + 30 + Text2Rect.height;
		
		var Button1Rect = new Rect();
  		Button1Rect.width = Screen.width*w;
  		Button1Rect.height = Screen.height*h;
  		Button1Rect.x = (Screen.width*(1-w))/2;
  		Button1Rect.y = Text2Rect.y + 50 + Button1Rect.height;
		
		var Button2Rect = new Rect();
  		Button2Rect.width = Screen.width*w;
  		Button2Rect.height = Screen.height*h;
  		Button2Rect.x = (Screen.width*(1-w))/2;
  		Button2Rect.y = Button1Rect.y + 30 + Button2Rect.height;
		
		var PhaseInfoRect = new Rect();
  		PhaseInfoRect.x = (Screen.width*(1-w))/2;
  		PhaseInfoRect.y = TextRect.y - 150;
  		PhaseInfoRect.width = Screen.width*w;
  		PhaseInfoRect.height = Screen.height*(h);
		
		
		
		
		// Make a background box
		GUI.Box(new Rect(10,10,Screen.width-20, Screen.height-20), "");
		GUI.skin.box.fontSize = Screen.height / 30;
		
		//GUI.Label(PhaseInfoRect, "Ready for " + PlayerPrefs.GetString("phase"));
		
		GUI.Label(Label1Rect, "Enter code");
		GUI.Label(Label2Rect, "Enter group name");
		//GUI.skin.label.fontSize = Screen.height / 60;
		
		sessioncode = GUI.TextField(TextRect, sessioncode);
		groupname = GUI.TextField(Text2Rect, groupname);
		GUI.skin.textField.fontSize = Screen.height / 60;
	
		
		if(GUI.Button(Button1Rect, "Phase 1"))
		{			
			if((0 < groupname.Length) && (groupname.Length < 60) && (0 < sessioncode.Length) && (sessioncode.Length < 30))
			{
				PlayerPrefs.SetString("group", groupname);
	            PlayerPrefs.SetString("code", sessioncode);
				PlayerPrefs.SetString("phase", "Phase 1"); // <-- for testing, vi gjør dette en annen plass etterpå..
	            StartCoroutine(Post()); //runs Db posting.... next scene loaded in update after bool post equals true
//	
//	            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//	            AndroidJavaClass jcToast = new AndroidJavaClass("android.widget.Toast");
//	            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
//	            jcToast.CallStatic("makeText", jc, "TESTING", 0);
			}
			else
			{
				if(groupname.Length == 0) toastMessage = "Please enter a name for your group!";
				if(groupname.Length > 60) toastMessage = "Please enter a shorter groupname!";
				if(sessioncode.Length == 0) toastMessage = "Please enter the session code!";
				if(sessioncode.Length > 30) toastMessage = "Wrong session code!";
				showToast = true;
				
				toastTimestamp = Environment.TickCount;
			}
		}
        
		
		if (GUI.Button(Button2Rect, "Phase 2")){
			if(session2Enabled){
					if((0 < groupname.Length) && (groupname.Length < 60) && (0 < sessioncode.Length) && (sessioncode.Length < 30))
					{
						PlayerPrefs.SetString("group", groupname);
	            		PlayerPrefs.SetString("code", sessioncode);
						PlayerPrefs.SetString("phase", "Phase 2");
	            		StartCoroutine(Post()); //runs Db posting.... next scene loaded in update after bool post equals true
					}
					else
					{
					if(groupname.Length == 0) toastMessage = "Please enter a name for your group!";
					if(groupname.Length > 60) toastMessage = "Please enter a shorter groupname!";
					if(sessioncode.Length == 0) toastMessage = "Please enter the session code!";
					if(sessioncode.Length > 30) toastMessage = "Wrong session code!";
					showToast = true;
					
					toastTimestamp = Environment.TickCount;
				}
			}
			else{
				toastMessage = "Session 2 is disabled";
				showToast = true;
				toastTimestamp = Environment.TickCount;
			}
		}
		
		
		//toast for messages e.g you forgot to input session code
		//long temptest = Environment.TickCount;
		if(showToast == true)
		{
			Color col = new Color(1,1,1,1.0f);
			var background = new GUIStyle(GUI.skin.box);
			background.normal.background = MakeTex(2, 2, new Color( 0f, 0f, 0f, 0.7f ));
			background.normal.textColor = col;
			background.fontSize = Screen.height / 40;
			//GUI.skin.box.fontSize = Screen.height / 60;
			GUI.Box(new Rect((Screen.width/3)-(Screen.width/6), Screen.height/8, Screen.width/1.5f, Screen.height/4), "Error: "+toastMessage, background);
		}


		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		//if(GUI.Button(new Rect(20,40,width-10,height/2), "Mona")) {
		//	Application.LoadLevel(1);
		//}

		// Make the second button.
		//if(GUI.Button(new Rect(20,height/2,width-10,height/2), "De to folka")) {
		//	Application.LoadLevel(2);
		//}
	}
	
	private Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}

	// Use this for initialization
	void Start () {
		session1Enabled = true;
		session2Enabled = true;
		
		
		
		
		var test = PlayerPrefs.HasKey("phase");
		var enda = PlayerPrefs.GetString("phase");
		
		if(!PlayerPrefs.HasKey("phase"))
		{
			PlayerPrefs.SetString("phase", "Phase 1");
		}
		
//		if(PlayerPrefs.HasKey("phase") == false)
//		{//		<-- denne ville ikke trigge.. 
//			PlayerPrefs.SetString("phase", "phase1");
//		}
//		else if(PlayerPrefs.GetString("phase") == "phase2")
//		{
//			session1Enabled = false;
//			session2Enabled = true;
//		}
//		else if(PlayerPrefs.GetString("phase") == "phase1")
//		{
//			session1Enabled = true;
//			session2Enabled = false;
//		}
//		
//		if(PlayerPrefs.HasKey("current_img") == false)
//		{
//			PlayerPrefs.SetInt("current_img", 1);
//		}
//		
//		
//
//		if(!(PlayerPrefs.HasKey("phase")))
//		{
//			PlayerPrefs.SetString("phase", "Phase 1");
//		}
		//get login details from playerprefs if they have already been given
		//var storedGroupName = PlayerPrefs.GetString("group");
		//var storedSessionCode = PlayerPrefs.GetString("code");
		//if(storedGroupName.Equals(null) && storedGroupName.Equals(""))
		//{
		//	groupname = storedGroupName;
		//}
		//if(storedSessionCode.Equals(null) && storedSessionCode.Equals(""))
		//{
		//	sessioncode = storedSessionCode;
		//}

	}
	
	void OnLevelWasLoaded(int level) {
        if (level == 0){
							var storedGroupName = PlayerPrefs.GetString("group");
							var storedSessionCode = PlayerPrefs.GetString("code");
							if(!storedGroupName.Equals(null) && !storedGroupName.Equals(""))
							{
								groupname = storedGroupName;
							}
							if(!storedSessionCode.Equals(null) && !storedSessionCode.Equals(""))
							{
								sessioncode = storedSessionCode;
							}
            print("Woohoo");
		}
        
    }
	
	// Update is called once per frame
	void Update () {
        if (post)
        {
            Application.LoadLevel(1);
        }
	
	}

    IEnumerator Post()
    {


        
        string group = PlayerPrefs.GetString("group");
        string code = PlayerPrefs.GetString("code");
        WWWForm form = new WWWForm();
        form.AddField("code", code);
        form.AddField("grp_name", group);
        WWW download = new WWW(url, form);
        yield return download;
        Debug.Log(download.text);
        if ((!string.IsNullOrEmpty(download.error)))
        {
            Debug.Log(download.text);
            print("Error downloading: " + download.error);
        }
        else
        {
			//check results for errors: 
			
			if(download.text == "error: no such session"){toastMessage = "Invalid session code!";}
			else if(download.text == "error: data missing"){Debug.Log ("Data missing: "+download.text); toastMessage = "Connection Issues";}
			if(download.text == "error: duplicated group"){
				Debug.Log ("Duplicated group: "+download.text);
				post = true;}
			//no errors found: 
			else if(download.text == "success"){
				Debug.Log("Posted successfully to database");
				post = true;
			}
            download.Dispose();
        }
        //post = true;
    }

}
