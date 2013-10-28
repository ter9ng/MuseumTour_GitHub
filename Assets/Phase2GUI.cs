using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Phase2GUI : MonoBehaviour {
	
	#region VARIABLES
	float w = 0.3f; // proportional width (0..1)
    float h = 0.05f; // proportional height (0..1)
	public GUISkin customStyle;
	
	public int selectionGrid_selectedQuestion; //used to toggle between questions in the GUI selectionGrid
	public int selectionGrid_selectedAnswer; //used to toggle between answers in the GUI selectionGrid
	
	static ArrayList questionSelectionArrayList = new ArrayList(); //the questions to make available for answering
	static ArrayList questionSelectionIDSArrayList = new ArrayList();
	static ArrayList superList = new ArrayList();
	static ArrayList questionDetailsArray = new ArrayList(); //contains details for a selected question (q_id, answer etc..)
	String[] alternativeSolutionsArray; //contains the solution and alternative answers for a given question 
	private ArrayList selectedQuestion;
	private ArrayList allQuestionsArray; //contains every question for the session
	private ArrayList answeredQuestions = new ArrayList();
	
	private static bool showQuestionList = true;
	private static bool showQuestionDetails = false;
	
	string submitAnswerUrl = "http://129.241.103.145/submitAnswer.php";
	string checkIfAnsweredUrl = "http://129.241.103.145/checkIfAnswered.php";
	string getAnsweredQuestionsUrl = "http://129.241.103.145/getAnsweredQuestions.php";
	string testButtonPrint = "øø";
	bool updateWithChecked = false; 
	
	int questionDisplayStartRange;
	int questionDisplayEndRange;
	
	#endregion //VARIABLES
	
	#region UNITY_MONOBEHAVIOUR_METHODS

	void Start () 
	{	
		questionDisplayStartRange = 0;
		questionDisplayEndRange = 5;
		functionInStart(0, 5);
	}
	
	void functionInStart(int startRange, int endRange) {
		
		questionSelectionArrayList.Clear();
		questionSelectionIDSArrayList.Clear();
		superList.Clear();
		
		//Set up a list of questions to answer.. 
		allQuestionsArray = scene2.questionsArray;
		
		for(int i = startRange;i<endRange;i++)
		{
			if(allQuestionsArray.Count>i)
			{
				ArrayList tempAL = (ArrayList)allQuestionsArray[i];
				string questionAsked = (string)tempAL[1];
				string questionID = (string)tempAL[0];
				questionSelectionArrayList.Add(questionAsked);
				questionSelectionIDSArrayList.Add(questionID);
			}
			
		}
		superList.Add(questionSelectionArrayList);
		superList.Add(questionSelectionIDSArrayList);	
		StartCoroutine(getAnsweredQuestions());
	}

	void OnGUI() {
			
		if(showQuestionList) //step 1: show a list of questions that can be answered in the GUI
		{
			if(updateWithChecked)
			{
				updateAnsweredQuestionsInGUI();
			}
			
			String[] questionSelectionArray = (String[])questionSelectionArrayList.ToArray(typeof(string));
			GUILayout.BeginArea(new Rect( (Screen.width/6), (Screen.height*(1f/6f)), Screen.width*(4f/6f), Screen.height*(2f/3f) ));
			GUILayout.BeginHorizontal(GUILayout.MaxHeight(Screen.height/16f));
			if(GUILayout.Button("LEFT",GUILayout.MaxHeight(Screen.height/16f)))
			{
				if(questionDisplayStartRange >= 5)
				{
					questionDisplayStartRange-=5;
					questionDisplayEndRange-=5;
					functionInStart(questionDisplayStartRange,questionDisplayEndRange);
				}
			}
			if(GUILayout.Button("RIGHT",GUILayout.MaxHeight(Screen.height/16f)))
			{
				if(allQuestionsArray.Count>=questionDisplayEndRange)
				{
					questionDisplayStartRange+=5;
					questionDisplayEndRange+=5;
					functionInStart(questionDisplayStartRange,questionDisplayEndRange);
				}
			}
			GUILayout.EndHorizontal();
			
			
		  	selectionGrid_selectedQuestion = GUILayout.SelectionGrid(selectionGrid_selectedQuestion, questionSelectionArray, 1, GUILayout.MaxWidth(Screen.width*(4f/6f)), GUILayout.MaxHeight(200f));
			
			if(GUILayout.Button ("Answer selected question!", GUILayout.MaxHeight(Screen.height/16f)))
			{
				selectedQuestion = (ArrayList)allQuestionsArray[questionDisplayStartRange+selectionGrid_selectedQuestion];
				getQuestionDetails(selectedQuestion);
			}
			GUILayout.EndArea();	
		}
		
		if(showQuestionDetails){//step 2: show question info with alternative answers in the GUI
		GUILayout.BeginArea(new Rect( (Screen.width/6), (Screen.height*(1f/6f)), Screen.width*(4f/6f), Screen.height*(2f/3f) ));
		GUILayout.Label("Question asked: ");
		string question = (string)selectedQuestion[1];
		GUILayout.Label (question);
		GUILayout.Label ("Select an answer:");
		
		selectionGrid_selectedAnswer = GUILayout.SelectionGrid(selectionGrid_selectedAnswer, alternativeSolutionsArray, 1, GUILayout.MaxWidth(Screen.width*(4f/6f)), GUILayout.MaxHeight(200f));
	
		if(GUILayout.Button("Submit answer!"))
			{
				string questionId = (string)selectedQuestion[0];
				string selectedAlternative = (string)alternativeSolutionsArray[selectionGrid_selectedAnswer];
				StartCoroutine(submitAnswer(questionId, selectedAlternative));
				
				//kan brukes til å fjerne besvarte spørsmål fra GUI, vi vil vel heller vise med farge hvilke som er besvart? .. too hard..
				//questionSelectionArrayList.RemoveAt(selectionGrid_selectedQuestion);
				//allQuestionsArray.RemoveAt(selectionGrid_selectedQuestion);
		
				showQuestionList=true;
				showQuestionDetails=false;
				StartCoroutine(getAnsweredQuestions());
			}
		GUILayout.EndArea();
		}
		

	}
	#endregion//UNITY_MONOBEHAVIOUR_METHODS
	
	//submit an answer to DB via PHP
	IEnumerator submitAnswer(string questionId, string selectedAlternative)
	{
		string group = PlayerPrefs.GetString("group");
        string code = PlayerPrefs.GetString("code");
        WWWForm form = new WWWForm();
		form.AddField("code", code);
        form.AddField("grp_name", group);
		form.AddField("answer", selectedAlternative);
		form.AddField("questions_q_id", questionId);
        WWW download = new WWW(submitAnswerUrl, form);
        yield return download;
        if ((!string.IsNullOrEmpty(download.error)))
        {
            Debug.Log(download.text);
            print("Error submitting answer: " + download.error);
        }
        else
        {
			if(download.text == "error: Failed to submit answer")
			{
				//toastMessage = "Invalid session code!";
			}
			else if(download.text == "Answer submitted"){
				Debug.Log("Posted successfully to database");
			}
            download.Dispose();
		}
	}
	
	void updateAnsweredQuestionsInGUI () {
	
	for(int i = 0; i<questionSelectionArrayList.Count;i++)
	{
		string toCheckFor = (string)((ArrayList)superList[1])[i];
		if(answeredQuestions.Contains((string)toCheckFor))
		{
			string stringToChange = (string)questionSelectionArrayList[i];
			if(!(stringToChange.Contains("(answered)"))){
			questionSelectionArrayList[i] +=" (answered)";
			}
			
		}
	}
	updateWithChecked = false;
	
	
	}
	
	//used to check if a question already is answered by the group.. not currently working :)
	IEnumerator checkIfAnswered(string questionId)
	{
		//tempAnswerBool = false; 
		string group = PlayerPrefs.GetString("group");
		string code = PlayerPrefs.GetString("code");
        WWWForm form = new WWWForm();
		form.AddField("code", code);
        form.AddField("grp_name", group);
		form.AddField("questions_q_id", questionId);
		WWW download = new WWW(checkIfAnsweredUrl, form);
		yield return download;
		if ((!string.IsNullOrEmpty(download.error)))
        {
            Debug.Log(download.text);
            print("Error submitting answer: " + download.error);
        }
		else
		{
			if(download.text == "answered")
			{
				//return true;
				//tempAnswerBool = true; 
			}
			else if(download.text == "not answered")
			{
				//return false;
				//tempAnswerBool = false; 
			}
			download.Dispose();
		}
	}
	
	IEnumerator getAnsweredQuestions()
	{
		string group = PlayerPrefs.GetString("group");
		string code = PlayerPrefs.GetString("code");
        WWWForm form = new WWWForm();
		form.AddField("code", code);
        form.AddField("grp_name", group);
		WWW download = new WWW(getAnsweredQuestionsUrl, form);
		yield return download;
		if(download.text == "Error: Failed to check answered")
		{
			//
		}
		if(download.text == "not answered")
		{
			//return false;
			//tempAnswerBool = false; 
		}
		else
		{
			var returnedString = download.data;
			string[] innerValues = returnedString.Split('|');
			for(int i = 0; i<innerValues.Length-1;i++)
			{	
				answeredQuestions.Add((string)innerValues[i]);
			}
				
			}
			updateWithChecked = true;
			download.Dispose();
		}
	
	
	//prepare question details for display in the GUI
	void getQuestionDetails(ArrayList questionDetails)
	{ 
		string solution = (string)questionDetails[2];
		string alternative1 = (string)questionDetails[7];
		string alternative2 = (string)questionDetails[8];
		string alternative3 = (string)questionDetails[9];
	
		questionDetailsArray.Clear();
		questionDetailsArray.Add(solution); 
		questionDetailsArray.Add(alternative1);
		questionDetailsArray.Add(alternative2);
		questionDetailsArray.Add(alternative3);
	
		alternativeSolutionsArray = (String[])questionDetailsArray.ToArray(typeof(string));
		shuffleArray(alternativeSolutionsArray);
	
		showQuestionList = false;
		showQuestionDetails = true;
	}
	
	
	//randomize the order of alternative solutions to a question
	void shuffleArray(string[] arrayToShuffle)
	{
		for(int i = 0; i<arrayToShuffle.Length; i++)
		{
			string tmp = arrayToShuffle[i];
			int randomForRange = UnityEngine.Random.Range(i, arrayToShuffle.Length);
			arrayToShuffle[i] = arrayToShuffle[randomForRange];
			arrayToShuffle[randomForRange] = tmp;
		}
	}
	
	
}

