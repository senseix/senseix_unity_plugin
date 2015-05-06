using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ThinksyPlugin : MonoBehaviour
{
	public string gameAccessToken = null; 	
								//this is your developer access token obtained from 
								//the Senseix website.
	public bool testingMode = false;	
								//check this box from the unity GUI to enable offline mode, 
								//useful for testing or offline development
	public bool useLeaderboard = false; 
								//check this box if you plan to use Thinksy leaderboard functionality
								//it will present a leaderboard button in the menu
	public GameObject emergencyWindow = null;	
								//this game object will be activated in the hopefully unlikely
								//scenario of problems in the thinksy plugin
	
	private static ThinksyPlugin singletonInstance;
	private static Problem mostRecentProblem;
	
	private const int reconnectRetryInterval = 3000;
	private const int encouragementGetInterval = 1401;

	static private ThinksyPlugin GetSingletonInstance()
	{
		if (singletonInstance == null)
		{
			throw new Exception("Please drag the Thinksy prefab located in " +
				"thinsy_unity_plugin/prefabs into your object heierarchy");
		}
		return singletonInstance;
	}

	void OnApplicationFocus(bool isFocused)
	{
		if (isFocused) StaticReinitialize ();
	}

	/// <summary>
	/// Shows a window indicating that something horrible has happened.
	/// Use this if something horrible happens.
	/// </summary>
	static public void ShowEmergencyWindow(string additionalMessage)
	{
		GetSingletonInstance().StartCoroutine(Senseix.SenseixSession.SubmitBugReport ("Emergency window being displayed: " + additionalMessage));
		GetSingletonInstance().ShowThisEmergencyWindow (additionalMessage);
	}

	static public bool IsInTestingMode()
	{
		return GetSingletonInstance().testingMode;
	}

	private void ShowThisEmergencyWindow(string additionalMessage)
	{
		emergencyWindow.SetActive (true);
		UnityEngine.UI.Text emergencyText = emergencyWindow.GetComponentInChildren<UnityEngine.UI.Text> ();
		emergencyText.text += " " + additionalMessage;
	}
	
	void Awake()
	{	
		if (singletonInstance != null)
		{
			Debug.LogWarning ("Something is creating a SenseixPlugin, but there is already an " +
			                  "instance in existance.  There should only be one SenseixPlugin component at any " +
			                  "time.  You can access its features \nthrough the class's static methods.   The object this message is coming" +
			                  " from is redundant.  I'm going to delete myself.");
			Destroy(gameObject);
		}

		singletonInstance = this;

		if (testingMode)
		{
			Senseix.ProblemKeeper.DeleteAllSeeds();
		}
		Senseix.ProblemKeeper.CopyFailsafeOver ();

		if (gameAccessToken == null || gameAccessToken == "")
			throw new Exception ("Please enter a game access token.");

		if (!testingMode)
		{
			StartCoroutine(Senseix.SenseixSession.InitializeSenseix (gameAccessToken));
		}
	}

	void Update()
	{
		if (!testingMode && !Senseix.SenseixSession.GetSessionState() && Time.frameCount%reconnectRetryInterval == 0)
		{
			Debug.Log ("Attempting to reconnect...");
			StartCoroutine(Senseix.SenseixSession.InitializeSenseix(gameAccessToken));
		}
		if (Senseix.SenseixSession.GetSessionState() && Time.frameCount%encouragementGetInterval == 0 &&  Time.frameCount != 0)
		{
			Senseix.Logger.BasicLog("Getting encouragements...");
			Senseix.SenseixSession.GetEncouragements();
		}
	}

	public static void StaticReinitialize()
	{
		GetSingletonInstance().Reinitialize ();
	}

	/// <summary>
	/// Resends all the server communication involved in initializing the game.
	/// Primarily a debugging tool.
	/// </summary>
	public void Reinitialize()
	{
		//Debug.Log ("Reinitializing");
		if (!testingMode)
		{
			StartCoroutine(Senseix.SenseixSession.InitializeSenseix (gameAccessToken));
		}
	}
	
	/// <summary>
	/// Updates the high score of the player on the SenseiX server.  This will then
	/// be reflected in the leaderboard in the SenseiX menus.  If this is not a high
	/// score, it will not override previous, higher scores.
	/// </summary>
	public static void UpdateCurrentPlayerScore (UInt32 score)
	{
		if (IsInTestingMode ())
			Debug.LogWarning ("We are currently in offline mode.");
		GetSingletonInstance().StartCoroutine(Senseix.SenseixSession.UpdateCurrentPlayerScore (score));
	}
	
	/// <summary>
	/// Returns the next Problem for the Player as an instance of the Problem class.  If there aren't 
	/// enough Problems left in the queue, an asynchronous task will retrieve more from the Senseix
	/// server.
	/// </summary>
	public static Problem NextProblem()
	{
		if (AllAnswerPartsGiven() && !GetMostRecentProblem().HasBeenSubmitted())
		{
			ThinksyPlugin.GetMostRecentProblem ().SubmitAnswer ();
		}
		Senseix.Message.Problem.ProblemData protobufsProblem = Senseix.SenseixSession.PullProblem ();
		Senseix.Logger.BasicLog ("Next problem!  Problem ID: " + protobufsProblem.uuid + " Category: " + protobufsProblem.category_name);
		//Debug.Log ("Next problem!  Problem ID: " + protobufsProblem.uuid + " Category: " + protobufsProblem.category_name);
		mostRecentProblem = new Problem (protobufsProblem);
		ThinksyQuestionDisplay.DisplayCurrentQuestion ();
		return mostRecentProblem;
	}

	public void NextProblemFromInstance()
	{
		NextProblem ();
	}
	
	/// <summary>
	/// Returns the most recent Problem returned by the NextProblem() function.
	/// </summary>
	public static Problem GetMostRecentProblem()
	{
		if (mostRecentProblem == null)
		{
			//throw new Exception("There are not yet any Problems.  Please use SenseixPlugin.NextProblem()");
			NextProblem();
		}
		return mostRecentProblem;
	}
	
	/// <summary>
	/// Returns given answer of the most recent Problem returned by the
	/// NextProblem() function.  Set given answers with Problem.AddGivenAnswerPart(),
	/// Problem.SetGivenAnswer(), or AddGivenAnswerPartToMostRecentProblem().
	/// </summary>
//	public static Answer GetMostRecentGivenAnswer()
//	{
//		return GetMostRecentProblem ().GetGivenAnswer ();
//	}

	/// <summary>
	/// Clears all the answers given so far, allowing the player to try again.
	/// </summary>
//	public static void ClearMostRecentGivenAnswerParts()
//	{
//		GetMostRecentGivenAnswer().ClearAnswerParts();
//	}
	
	/// <summary>
	/// Gets the question portion of the of the most recent Problem returned by the
	/// NextProblem() function.  The question will also be displayed the the SenseiX
	/// Question Panel (found under the SenseiX Display Canvas).
	/// </summary>
//	public static Question GetMostRecentProblemQuestion()
//	{
//		return GetMostRecentProblem ().GetQuestion();
//	}
	
	/// <summary>
	/// Gets the distractors for the problem most recently returned by NextProblem().
	/// These are wrong answers which can be presented as options to the player.
	/// </summary>
	/// <returns>The most recent problem distractors.</returns>
	/// <param name="howManyDistractors">How many distractors.</param>
//	public static ProblemPart[] GetMostRecentProblemDistractors(int howManyDistractors)
//	{
//		return GetMostRecentProblem ().GetDistractors (howManyDistractors);
//	}

	/// <summary>
	/// The same an GetMostRecentProblemDistractors, but gets only one distractor.
	/// Distractors are wrong answers which can be presented as options to the player.
	/// </summary>
	/// <returns>The most recent problem distractor.</returns>
//	public static ProblemPart GetMostRecentProblemDistractor()
//	{
//		return GetMostRecentProblem ().GetDistractor ();
//	}
	
	/// <summary>
	/// Adds the given answer part to most recent problem.  This will be considered 
	/// part of the player's answer for submissions and checking unless it is removed.
	/// </summary>
	/// <param name="givenAnswerPart">Given answer part.</param>
//	public static void AddGivenAnswerPartToMostRecentProblem(ProblemPart givenAnswerPart)
//	{
//		GetMostRecentProblem ().AddGivenAnswerPart (givenAnswerPart);
//	}
	
	/// <summary>
	/// Checks the Problem's given answer against its correct answer.
	/// This WILL NOT report anything to the SenseiX server, and this therefore DOES NOT
	/// influence your player's progress.  When you have a final answer, submit it with
	/// Problem.SubmitAnswer() or SenseixPlugin.SubmitMostRecentProblemAnswer()
	/// Given and correct answer can be found in the Problem class.
	/// </summary>
//	public static bool CheckAnswer(Problem problem)
//	{
//		return problem.CheckAnswer ();
//	}

	/// <summary>
	/// Gets the correct answer parts for the most recent problem generated by
	/// NextProblem().
	/// </summary>
	/// <returns>The most recent answer parts.</returns>
//	public static ProblemPart[] GetMostRecentCorrectAnswerParts()
//	{
//		return GetMostRecentProblem ().GetCorrectAnswer ().GetAnswerParts ();
//	}
	
	/// <summary>
	/// Based on how many answers have been given so far, gets the next correct answer part.
	/// The same as GetMostRecentProblem ().GetNextCorrectAnswerPart ();
	/// </summary>
	/// <returns>The next correct answer part.</returns>
//	public static ProblemPart GetCurrentCorrectAnswerPart()
//	{
//		return GetMostRecentProblem ().GetCurrentCorrectAnswerPart ();
//	}
	
	/// <summary>
	/// Checks the most recent problem's given answer.
	/// The same as GetMostRecentProblem ().CheckAnswer ().
	/// </summary>
	/// <returns>Whether or not the problem's given answer is correct</returns>
//	public static bool CheckMostRecentProblemAnswer()
//	{
//		return GetMostRecentProblem ().CheckAnswer ();
//	}
	
	/// <summary>
	/// Submits the most recent problem's given answer to the SenseiX server.
	/// This will update your player's progress and metrics.
	/// This is important!  If you don't submit answers, your players will
	/// receive the same problems over and over again, and become bored.
	/// </summary>
	/// <returns>Whether or not the problem's given answer is correct</returns>
//	public static bool SubmitMostRecentProblemAnswer()
//	{
//		return GetMostRecentProblem ().SubmitAnswer ();
//	}
	
	/// <summary>
	/// Returns whether or not the number of answer parts given to the most recent problem
	/// is equal to the correct number of answer parts.
	/// </summary>
	public static bool AllAnswerPartsGiven()
	{
		if (mostRecentProblem == null) return false;
		return GetMostRecentProblem().AnswersGivenSoFar() == GetMostRecentProblem().GetCorrectAnswer().AnswerPartsCount();
	}
	
	/// <summary>
	/// Gets the correct answer to the most recent problem.
	/// The same as GetMostRecentProblem ().GetCorrectAnswer ().
	/// </summary>
	/// <returns>The current correct answer.</returns>
//	public static Answer GetCurrentCorrectAnswer()
//	{
//		return GetMostRecentProblem ().GetCorrectAnswer ();
//	}

	/// <summary>
	/// Sets the current given answer.  
	/// The same as GetMostRecentProblem().SetGivenAnswer(givenAnswer)
	/// </summary>
	/// <param name="givenAnswer">Given answer.</param>
//	public static void SetCurrentGivenAnswer(Answer givenAnswer)
//	{
//		GetMostRecentProblem ().SetGivenAnswer (givenAnswer);
//	}
	
	/// <summary>
	/// Gets the most recent problem HTML.
	/// The same as GetMostRecentProblem ().GetQuestion ().GetHTML ().
	/// </summary>
	/// <returns>The most recent problem HTML.</returns>
//	public static string GetMostRecentProblemHTML()
//	{
//		return GetMostRecentProblem ().GetQuestion ().GetHTML ();
//	}
	
	/// <summary>
	/// Gets the most recent problem's question image.
	/// The same as GetMostRecentProblem ().GetQuestion ().GetImage ().
	/// </summary>
	/// <returns>The most recent problem image.</returns>
//	public static Texture2D GetMostRecentProblemImage()
//	{
//		return GetMostRecentProblem ().GetQuestion ().GetImage ();
//	}

	/// <summary>
	/// Counts the problems answered correctly so far.
	/// </summary>
	/// <returns>The problems answered correctly so far.</returns>
	public static uint CountProblemsAnsweredCorrectlySoFar()
	{
		return Problem.CountProblemsAnsweredCorrectlySoFar();
	}

	public static bool UsesLeaderboard()
	{
		return GetSingletonInstance().useLeaderboard;
	}

	/// <summary>
	/// A category is a group of Thinksy questions which are formatted the same way.
	/// 
	/// This gets the name of the current category.  This string is mostly meaningless,
	/// but can be compared with other strings and looked up on the Thinksy
	/// website for information about the category.
	/// 
	/// This is the same as GetMostRecentProblem ().GetCategoryName ();
	/// </summary>
	/// <returns>The current category name.</returns>
//	public static string GetCurrentCategoryName()
//	{
//		return GetMostRecentProblem ().GetCategoryName ();
//	}

	/// <summary>
	/// A category is a group of Thinksy questions which are formatted the same way.
	/// 
	/// Gets the current category number.  Higher number means more advanced categories.
	/// 
	/// This is the same as GetMostRecentProblem().GetCategoryNumber ();
	/// </summary>
	/// <returns>The current category number.</returns>
//	public static uint GetCurrentCategoryNumber()
//	{
//		return GetMostRecentProblem().GetCategoryNumber ();
//	}

	public static void SetAccessToken(string newAccessToken)
	{
		GetSingletonInstance().gameAccessToken = newAccessToken;
	}
}