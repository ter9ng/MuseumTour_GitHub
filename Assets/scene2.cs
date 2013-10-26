using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class scene2 : MonoBehaviour {

    string getTargetsUrl = "http://129.241.103.145/getTargets.php";
	string getQuestionsUrl = "http://129.241.103.145/getQuestions.php";
	static ArrayList imgTargetArray = new ArrayList();
	static ArrayList questionsArray = new ArrayList();
	static string printList = "";
	string buttontext = "Scan picturez";
	string øyvind = "feit";
	
	
	Texture2D cur_image_loaded = null;
	public string imgHostName = "http://129.241.103.145/hintImg/";
	public string imgName = ""; //sett denne når en får et bilde en skal finne...
	
    void OnGUI()
    {
        string group = PlayerPrefs.GetString("group");
		

        float w = 0.3f; // proportional width (0..1)
        float h = 0.05f; // proportional height (0..1)


        var TextRect = new Rect();
  		TextRect.x = (Screen.width*(1-w))/2;
  		TextRect.y = (Screen.height*(1-h))/2;
  		TextRect.width = Screen.width*w;
  		TextRect.height = Screen.height*(h/2);
		
		var Label1Rect = new Rect();
        Label1Rect.x = (Screen.width * (1 - w)) / 2;
        Label1Rect.y = (Screen.height * (1 - h)) / 2;
        Label1Rect.width = Screen.width * w;
        Label1Rect.height = Screen.height * (h / 2)*100;


        var Label2Rect = new Rect();
        Label2Rect.x = (Screen.width * (1 - w)) / 2;
        Label2Rect.y = Label1Rect.y - 100;
        Label2Rect.width = Screen.width * w;
        Label2Rect.height = Screen.height * (h / 2);
		
		var Button1Rect = new Rect();
  		Button1Rect.x = (Screen.width*(1-w))/2;
  		Button1Rect.y = TextRect.y + 200;
  		Button1Rect.width = Screen.width*w;
  		Button1Rect.height = Screen.height*h;

        // Make a background box
		GUI.Box(new Rect(10,10,Screen.width-20, Screen.height-20), "Find the picture!");
		GUI.skin.box.fontSize = Screen.height / 60;
		
		GUI.Label(Label1Rect, printList);
		GUI.skin.label.fontSize = Screen.height/60;
		
		if(GUI.Button(Button1Rect, buttontext))
		{
			printList = "";
			buttontext = "Loading....";
			Application.LoadLevel(2);
			
		}
		
		if(cur_image_loaded != null)
		{//draw a "hint image" if one is loaded
		GUI.DrawTexture(new Rect(Screen.width/4, 50, Screen.width/2, 256f), cur_image_loaded, ScaleMode.ScaleToFit, true);
		}
		
		
    }

    void Awake()
    {

    }

    IEnumerator Start()
    {
		
		WWWForm form = new WWWForm();
		string code = PlayerPrefs.GetString("code");
		form.AddField("code", code); //bytt senere til id..?
		WWW getTargets = new WWW(getTargetsUrl, form);
		yield return getTargets;
		if ((!string.IsNullOrEmpty(getTargets.error)))
        {
            Debug.Log(getTargets.text);
            print("Error downloading targets list: " + getTargets.error);
        }
		else{
		var returnedString = getTargets.data;
		string[] values =returnedString.Split('&');
			for(int i=0;i<values.Length-1;i++)
			{
				ArrayList imgTargetInnerArray = new ArrayList();
				var value = values[i];
				string[] innerValues = value.Split('|');
					string tp_id = innerValues[0];
					string tp_name = innerValues[1];
					string tp_info = innerValues[2];
					string tp_hintImg = innerValues[3];
					string route_number = innerValues[4];
					imgTargetInnerArray.Add(tp_id); imgTargetInnerArray.Add(tp_name); imgTargetInnerArray.Add(tp_info); imgTargetInnerArray.Add (tp_hintImg); imgTargetInnerArray.Add(route_number);
					imgTargetArray.Add(imgTargetInnerArray);
					Debug.Log(imgTargetArray.Count);
			}
			getTargets.Dispose();
			buildGUIList();
        }
		Debug.Log ("ready to enter..");
		StartCoroutine(getQuestionsForTargetPicture(1));
		
		
		
	}
	
	//Function for testing, returns all imageTargets linked to the session.
	void buildList()	
	{
		printList+="Registered targets: \n";
		for(int i=0;i<imgTargetArray.Count; i++)
		{
			ArrayList tempImgTargetInnerArray = (ArrayList)imgTargetArray[i];
			foreach(string imgTargetField in tempImgTargetInnerArray)
			{
				printList+=imgTargetField;
			}
			printList+="\n";
			 
		}
	}
	
	//Get name and hintImg for a Target Image
	void buildGUIList()	
	{
		printList="Find a picture called ";
		bool route_hasNext = false;
		for(int j = 0;j<imgTargetArray.Count; j++)
		{
			ArrayList tempImgTargetInnerArray = (ArrayList)imgTargetArray[j];
			string temp_route_number = (string)tempImgTargetInnerArray[4];
			string current_img_toString = PlayerPrefs.GetInt("current_img").ToString();
			if(temp_route_number == current_img_toString)
			{
				string picTitle = (string)tempImgTargetInnerArray[1];
				printList+=picTitle;
				imgName = (string)tempImgTargetInnerArray[3];
				PlayerPrefs.SetString("current_image_name", imgName);
				StartCoroutine(load_image_preview(imgHostName+imgName));
				route_hasNext = true;
				break;
			}
		}
			if(route_hasNext == false){
			PlayerPrefs.SetString("phase","phase2");
			PlayerPrefs.SetInt("current_img", 1);
			Application.LoadLevel(0);
			}
	}
	
	private IEnumerator load_image_preview(string _path)
	{
	    WWW www = new WWW(_path);
	    yield return www;
	    Texture2D texTmp = new Texture2D(5, 5, TextureFormat.RGB24, false);
	 
	    www.LoadImageIntoTexture(texTmp);
	    cur_image_loaded = new Texture2D(5, 5, TextureFormat.RGB24, false);
	    cur_image_loaded = texTmp;
	}
	
	/**
	 * Get all questions related to a given Target Picture
	 * @param tp_id - identifier of the Target Picture
	**/
	IEnumerator getQuestionsForTargetPicture(int tp_id){
		Debug.Log ("entered getQuestionsForTargetPicture");
		WWWForm form = new WWWForm();
		form.AddField("tp_id", tp_id);
		WWW getQuestions = new WWW(getQuestionsUrl, form);
		yield return getQuestions;
		if ((!string.IsNullOrEmpty(getQuestions.error)))
        {
            print("Error downloading questions list: " + getQuestions.error);
        }
		else{
			var returnedString = getQuestions.data;
			Debug.Log ("returned: "+returnedString);
			string[] values =returnedString.Split('&');
			for(int i=0;i<values.Length-1;i++)
			{
				ArrayList questionsInnerArray = new ArrayList();
				var value = values[i];
				string[] innerValues = value.Split('|');
				
					string q_id = innerValues[0];
					string question = innerValues[1];
					string solution = innerValues[2];
					string sessions_session_id= innerValues[3];
					string groups_grp_id = innerValues[4];
					string target_pictures_tp_id = innerValues[5];
					string grp_name = innerValues[6];
					questionsInnerArray.Add(q_id); questionsInnerArray.Add(question);questionsInnerArray.Add(solution);
					questionsInnerArray.Add(sessions_session_id); questionsInnerArray.Add(groups_grp_id); questionsInnerArray.Add(target_pictures_tp_id);
					string localGroup = PlayerPrefs.GetString("group");//Skip questions that the local group created
					if(!grp_name.Equals(localGroup)){
						questionsArray.Add(questionsInnerArray);
					}
					
			}
		}
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Application.platform == RuntimePlatform.Android)

        {
            if (Input.GetKey(KeyCode.Escape))

            {
				printList = "";
				Application.LoadLevel(0);
            }
		}
	
	}

}
