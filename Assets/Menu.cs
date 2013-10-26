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
	
	void OnGUI () {
		
		float w = 0.3f; // proportional width (0..1)
  		float h = 0.05f; // proportional height (0..1)
		
		
		
  		var TextRect = new Rect();
  		TextRect.x = (Screen.width*(1-w))/2;
  		TextRect.y = (Screen.height*(1-h))/2;
  		TextRect.width = Screen.width*w;
  		TextRect.height = Screen.height*(h/2);
		
		var PhaseInfoRect = new Rect();
  		PhaseInfoRect.x = (Screen.width*(1-w))/2;
  		PhaseInfoRect.y = TextRect.y - 150;
  		PhaseInfoRect.width = Screen.width*w;
  		PhaseInfoRect.height = Screen.height*(h/2);
		
		var Label1Rect = new Rect();
  		Label1Rect.x = (Screen.width*(1-w))/2;
  		Label1Rect.y = TextRect.y - 50;
  		Label1Rect.width = Screen.width*w;
  		Label1Rect.height = Screen.height*(h/2);
		
		
		
		var Label2Rect = new Rect();
  		Label2Rect.x = (Screen.width*(1-w))/2;
  		Label2Rect.y = TextRect.y + 50;
  		Label2Rect.width = Screen.width*w;
  		Label2Rect.height = Screen.height*(h/2);
		
		var Text2Rect = new Rect();
  		Text2Rect.x = (Screen.width*(1-w))/2;
  		Text2Rect.y = TextRect.y + 100;
  		Text2Rect.width = Screen.width*w;
  		Text2Rect.height = Screen.height*(h/2);
		
		var Button1Rect = new Rect();
  		Button1Rect.x = (Screen.width*(1-w))/2;
  		Button1Rect.y = TextRect.y + 200;
  		Button1Rect.width = Screen.width*w;
  		Button1Rect.height = Screen.height*h;
		
		var Button2Rect = new Rect();
  		Button2Rect.x = (Screen.width*(1-w))/2;
  		Button2Rect.y = Button1Rect.y + 100;
  		Button2Rect.width = Screen.width*w;
  		Button2Rect.height = Screen.height*h;
		
		// Make a background box
		GUI.Box(new Rect(10,10,Screen.width-20, Screen.height-20), "Choose phase");
		GUI.skin.box.fontSize = Screen.height / 30;
		
		//GUI.Label(PhaseInfoRect, "Ready for " + PlayerPrefs.GetString("phase"));
		
		GUI.Label(Label1Rect, "Enter code");
		GUI.Label(Label2Rect, "Enter group name");
		GUI.skin.label.fontSize = Screen.height / 60;
		
		sessioncode = GUI.TextField(TextRect, sessioncode);
		groupname = GUI.TextField(Text2Rect, groupname);
		GUI.skin.textField.fontSize = Screen.height / 60;
	
		
		if(GUI.Button(Button1Rect, "Phase 1"))
		{			
			if((0 < groupname.Length) && (groupname.Length < 60) && (0 < sessioncode.Length) && (sessioncode.Length < 30))
			{
				PlayerPrefs.SetString("group", groupname);
	            PlayerPrefs.SetString("code", sessioncode);
				PlayerPrefs.SetString("phase", "phase1"); // <-- for testing, vi gjør dette en annen plass etterpå..
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
						PlayerPrefs.SetString("phase", "phase2");
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
		GUI.skin.button.fontSize = Screen.height / 60;
		
		//toast for messages e.g you forgot to input session code
		//long temptest = Environment.TickCount;
		if(showToast == true)
		{
			GUI.skin.box.fontSize = Screen.height / 60;
			GUI.Box(new Rect((Screen.width/3)-(Screen.width/6), Screen.height/4, Screen.width/1.5f, 100), "Error: "+toastMessage);
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

	// Use this for initialization
	void Start () {
		session1Enabled = true;
		session2Enabled = true;
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
