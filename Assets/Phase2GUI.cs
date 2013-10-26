using System;
using UnityEngine;
using System.Collections;

public class Phase2GUI : MonoBehaviour {
	
	#region VARIABLES
	float w = 0.3f; // proportional width (0..1)
    float h = 0.05f; // proportional height (0..1)
	
	Rect TextRect = new Rect();
	Rect Text2Rect = new Rect();
	Rect Button1Rect = new Rect();
	Rect QuestionButtonRect = new Rect();
	Rect ContinueButtonRect = new Rect();
	Rect TestToMainMenuButtonRect = new Rect(); 
	Rect QuestionLabel1Rect = new Rect(); //Question label/textfield position
	Rect QuestionInputRect = new Rect();
	Rect CorrectAnswerLabelRect = new Rect(); //Correct answer label/textfield position
	Rect CorrectAnswerInputRect = new Rect();
	Rect Alternative1Answer1LabelRect = new Rect();
	Rect AlternativeAnswer1InputRect = new Rect();
	Rect AlternativeAnswer2InputRect = new Rect();
	Rect AlternativeAnswer3InputRect = new Rect();
	Rect SubmitQuestionButtonRect = new Rect();
	
	#endregion //VARIABLES
	
	#region UNITY_MONOBEHAVIOUR_METHODS
	void Start () {
		
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
		
  		QuestionLabel1Rect.x = (Screen.width*(1-w))/2;
  		QuestionLabel1Rect.y = Screen.height*h; //(Screen.height*(1-h))/2;
  		QuestionLabel1Rect.width = Screen.width*w;
  		QuestionLabel1Rect.height = Screen.height*(h/2);
		
		QuestionInputRect.x = (Screen.width*(1-w))/2;
  		QuestionInputRect.y = QuestionLabel1Rect.y + 50; //(Screen.height*(1-h))/2;
  		QuestionInputRect.width = Screen.width*w;
  		QuestionInputRect.height = Screen.height*(h/2);
		
  		CorrectAnswerLabelRect.x = (Screen.width*(1-w))/2;
  		CorrectAnswerLabelRect.y = QuestionInputRect.y + 50;
  		CorrectAnswerLabelRect.width = Screen.width*w;
  		CorrectAnswerLabelRect.height = Screen.height*(h/2);
		
		CorrectAnswerInputRect.x = (Screen.width*(1-w))/2;
  		CorrectAnswerInputRect.y = CorrectAnswerLabelRect.y + 50;
  		CorrectAnswerInputRect.width = Screen.width*w;
  		CorrectAnswerInputRect.height = Screen.height*(h/2);
		
		Alternative1Answer1LabelRect.x = (Screen.width*(1-w))/2;
  		Alternative1Answer1LabelRect.y = CorrectAnswerInputRect.y + 50;
  		Alternative1Answer1LabelRect.width = Screen.width*w;
  		Alternative1Answer1LabelRect.height = Screen.height*(h/2);
		
		AlternativeAnswer1InputRect.x = (Screen.width*(1-w))/2;
  		AlternativeAnswer1InputRect.y = Alternative1Answer1LabelRect.y + 50;
  		AlternativeAnswer1InputRect.width = Screen.width*w;
  		AlternativeAnswer1InputRect.height = Screen.height*(h/2);
		
		AlternativeAnswer2InputRect.x = (Screen.width*(1-w))/2;
  		AlternativeAnswer2InputRect.y = AlternativeAnswer1InputRect.y + 50;
  		AlternativeAnswer2InputRect.width = Screen.width*w;
  		AlternativeAnswer2InputRect.height = Screen.height*(h/2);
		
		AlternativeAnswer3InputRect.x = (Screen.width*(1-w))/2;
  		AlternativeAnswer3InputRect.y = AlternativeAnswer2InputRect.y + 50;
  		AlternativeAnswer3InputRect.width = Screen.width*w;
  		AlternativeAnswer3InputRect.height = Screen.height*(h/2);
		
		SubmitQuestionButtonRect.x = (Screen.width*(1-w))/2;
  		SubmitQuestionButtonRect.y = AlternativeAnswer3InputRect.y + 100;
  		SubmitQuestionButtonRect.width = Screen.width*w;
  		SubmitQuestionButtonRect.height = Screen.height*h;
	}
	
	void OnGUI() {
		
		{
			GUI.Label(QuestionLabel1Rect, "Upper TestLabel");
			GUI.Label(CorrectAnswerLabelRect, "Lower TestLabel");
			GUI.skin.label.fontSize = Screen.height / 60;
		
			CloudRecoEventHandler.inputQuestion = GUI.TextField(QuestionInputRect, CloudRecoEventHandler.inputQuestion);
			CloudRecoEventHandler.inputSolution = GUI.TextField(CorrectAnswerInputRect, CloudRecoEventHandler.inputSolution);
			GUI.Label (Alternative1Answer1LabelRect, "Alternative Answers:");
			CloudRecoEventHandler.alternativeAnswer1 = GUI.TextField (AlternativeAnswer1InputRect, CloudRecoEventHandler.alternativeAnswer1);
			CloudRecoEventHandler.alternativeAnswer2 = GUI.TextField (AlternativeAnswer2InputRect, CloudRecoEventHandler.alternativeAnswer2);
			CloudRecoEventHandler.alternativeAnswer3 = GUI.TextField (AlternativeAnswer3InputRect, CloudRecoEventHandler.alternativeAnswer3);
			
			GUI.skin.textField.fontSize = Screen.height / 60;

			if(GUI.Button(SubmitQuestionButtonRect, "Submit" ))
			{ 
				StartCoroutine(CloudRecoEventHandler.postQuestion());
				CloudRecoEventHandler.questionTrigger = false;
				CloudRecoEventHandler.toastTimestamp = Environment.TickCount;
				CloudRecoEventHandler.questionAdded = true;
			}
		}
	}
	
	#endregion//UNITY_MONOBEHAVIOUR_METHODS
}

