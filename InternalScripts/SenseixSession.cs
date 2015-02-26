using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace Senseix 
{
	class SenseixSession : MonoBehaviour
	{
		private const int ACCESS_TOKEN_LENGTH = 64;

		private static bool inSession = false;
		private static volatile string accessToken = "";
		private static bool isSignedIn = false;
		private static string authToken; 
		private static IList<Message.Leaderboard.PlayerData> currentLeaderboard;
		private static Message.Player.PlayerListResponse currentPlayerList;
		private static Message.Player.Player currentPlayer;

		private static SenseixSession singletonInstance = null;

		private static SenseixSession GetSingletonInstance()
		{
			if (singletonInstance == null)
			{
				singletonInstance = FindObjectOfType<SenseixSession>();
			}
			return singletonInstance;
		}

		static public ArrayList GetCurrentPlayerList()
		{
			ArrayList returnList = new ArrayList ();
			if (currentPlayerList == null)
			{
				Debug.Log("No current Player list.  Maybe not connected.");
				return returnList;
			}

			for (int i = 0; i < currentPlayerList.player.Count; i++)
			{
				returnList.Add(currentPlayerList.player[i]);
			}
			return returnList;
		}

		static public void SetCurrentPlayerList(Message.Player.PlayerListResponse newPlayerList)
		{
			currentPlayerList = newPlayerList;
		}

		static private void SetCurrentPlayer(Message.Player.Player newPlayer)
		{
			currentPlayer = newPlayer;
		}

		static public IEnumerator SelectPlayer(Senseix.Message.Player.Player selectedPlayer)
		{
			SetCurrentPlayer (selectedPlayer);
			yield return GetSingletonInstance().StartCoroutine(RegisterPlayer (selectedPlayer));
			//ProblemKeeper.CreateSeedFileIfNeeded ();
		}

		static public Message.Player.Player GetCurrentPlayer()
		{
			return currentPlayer;
		}

		static public bool GetSessionState()
		{
			//Debug.Log ("returning inSession: " + inSession);
			return inSession;
		}
		static public void SetSessionState(bool state)
		{
			//UnityEngine.Debug.Log ("Session state set to: " + state);
			inSession = state;
		}

		static public void SetAndSaveAuthToken(string newAuthToken) 
		{
			authToken = newAuthToken;
			//Debug.Log ("auth token: " + authToken);
		}
		static private int CheckAccessToken()
		{
			if (accessToken.Length != ACCESS_TOKEN_LENGTH)
				return -1;
			return 0;
		}
		static public string GetAccessToken()
		{
			return accessToken;
		}
		static public string GetDeviceID()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}
		static public string GetAuthToken()
		{
			if (authToken != null)
				return authToken;
			//else
			//	Debug.LogWarning("Something got the auth token, but there was no auth token available." +
			//		"  It is possible that the register device message failed and/or we are sending a request" +
			//		" without ever having registered. (Normal in the case of the registration message.)");
			return "you don't need to see my identification";
		}
		static public string GetCurrentPlayerID()
		{
			if (currentPlayer == null)
				return "no current player";
			return currentPlayer.player_id;
		}
		static public void SetToPlayerWithID(string newPlayerID)
		{
			foreach(Message.Player.Player Player in GetCurrentPlayerList())
			{
				if (Player.player_id == newPlayerID)
				{
					SetCurrentPlayer(Player);
				}
			}
		}


		public static IEnumerator InitializeSenseix (string newAccessToken) 
		{ 
			//Debug.Log ("initializing");

			SetSessionState (true);

			accessToken = newAccessToken; 
			if (CheckAccessToken() == -1) 
			{
				throw new Exception("The Thinksy Token you have provided is not of a valid length, please" +
					"register at https://developer.thinksylearn.com/ to create a valid key.  Then, fill " +
					"in the Game Access Token field of the ThinksyPlugin script on the Thinksy Prefab.");
			}

			//Creates a temporary account based on device id
			//returns an auth token. This is Syncronous.
			//Debug.Log("registering device");
			yield return GetSingletonInstance().StartCoroutine(RegisterDevice ());

			//Debug.Log ("listing players");
		  	yield return GetSingletonInstance().StartCoroutine(ListPlayers ());

			//Debug.Log("register all players");
			yield return GetSingletonInstance().StartCoroutine(RegisterAllPlayers ());

			//Debug.Log("submit cache");
			SenseixSession.CheckProblemPostCacheSubmission();
			//SenseixPlugin.ShowEmergencyWindow ("testing");

			yield return GetSingletonInstance().StartCoroutine(Message.Request.UpdatePlayerScore (GetCurrentPlayerID(), 0));
			yield return GetSingletonInstance().StartCoroutine(Message.Request.GetPlayerRank (GetCurrentPlayerID ()));

			yield return Message.Request.GetSingletonInstance().StartCoroutine(
				Message.Request.GetProblems (SenseixSession.GetCurrentPlayerID(), ProblemKeeper.PROBLEMS_PER_PULL));

			ThinksyPlugin.GetMostRecentProblem();
		}

		static public IEnumerator ListPlayers()
		{
			yield return GetSingletonInstance().StartCoroutine(Message.Request.ListPlayers ());
		}

		//this assumes that there is at least one Player always.
		static public IEnumerator RegisterAllPlayers()
		{
			ArrayList Players = GetCurrentPlayerList ();
			foreach (Message.Player.Player Player in Players)
			{
				yield return GetSingletonInstance().StartCoroutine(RegisterPlayer(Player));
			}
			if (Players.Count > 0) SetCurrentPlayer (Players [0] as Message.Player.Player);
			else
				UnityEngine.Debug.Log("There are no players.  Maybe never connected.");
		}

		static public void PullLeaderboard(uint pageNumber, uint pageSize)
		{
			Message.Request.LeaderboardPage (pageNumber, Senseix.Message.Leaderboard.SortBy.NONE, pageSize);
		}

		static public void SetLeaderboardPlayers(IList<Message.Leaderboard.PlayerData> PlayerList)
		{
			foreach (Message.Leaderboard.PlayerData Player in PlayerList)
			{
				Debug.Log("Player " + Player.name + " has score " + Player.score);
			}
			currentLeaderboard = PlayerList;
		}

		static public IList<Message.Leaderboard.PlayerData> GetCurrentLeaderboard()
		{
			if (currentLeaderboard == null)
			{
				return new Message.Leaderboard.PlayerData[0];
			}
			return currentLeaderboard;
		}

		static public IEnumerator RegisterDevice()
		{
			//Debug.Log ("register device session");
			yield return GetSingletonInstance().StartCoroutine(Message.Request.RegisterDevice(SystemInfo.deviceName));
		}
		
		static public IEnumerator VerifyGame(string verificationCode)
		{
			yield return GetSingletonInstance().StartCoroutine(Message.Request.VerifyGame (verificationCode));
		}

		static private IEnumerator RegisterPlayer(Message.Player.Player Player)
		{
			yield return GetSingletonInstance().StartCoroutine(Message.Request.RegisterPlayer (Player.player_id));
		}


		static public Senseix.Message.Problem.ProblemData PullProblem()
		{
			return ProblemKeeper.GetProblem ();
		}

		static public bool CheckAnswer(Message.Problem.ProblemData Problem, Answer answer)
		{
			return ProblemKeeper.CheckAnswer(Problem, answer);
		}

		static public bool SubmitAnswer(Message.Problem.ProblemData Problem, Answer answer, bool correct)
		{
			return ProblemKeeper.SubmitAnswer(Problem, answer, correct);
		}
	

		public static IEnumerator UpdateCurrentPlayerScore(UInt32 score)
		{
			yield return GetSingletonInstance().StartCoroutine(Message.Request.UpdatePlayerScore (GetCurrentPlayerID(), score));
		}

		public static void SetSignedIn(bool newIsSignedIn)
		{
			isSignedIn = newIsSignedIn;
		}

		public static bool IsSignedIn()
		{
			return isSignedIn;
		}

		public static void PushProblems(Queue Problems)
		{
			GetSingletonInstance().StartCoroutine(
				Message.Request.PostProblems(GetCurrentPlayerID(), Problems));
		}

		public static void GetEncouragements()
		{
			GetSingletonInstance().StartCoroutine(Message.Request.GetEncouragements (GetCurrentPlayerID ()));
		}

		public static void GetProblems(uint numberOfProblems)
		{
			Message.Request.GetSingletonInstance().StartCoroutine(
				Message.Request.GetProblems (SenseixSession.GetCurrentPlayerID(), numberOfProblems));
		}

		//public static byte[] DecodeServerBytes(Google.ProtocolBuffers.ByteString serverBytes)
		//{
		//	string base64string = serverBytes.ToStringUtf8 ();
		//	byte[] decodedBytes = System.Convert.FromBase64String (base64string);
		//	return decodedBytes;
		//}

		static public void CheckProblemPostCacheSubmission()
		{
			//Debug.Log ("Should cache: " + ShouldCacheProblemPosts ());
			if (!ShouldCacheProblemPosts ())
				GetSingletonInstance().StartCoroutine(Message.Request.SubmitProblemPostCache ());
		}

		static public bool ShouldCacheProblemPosts()
		{
			return !SenseixSession.GetSessionState ();
		}

		static public IEnumerator SubmitBugReport(string additionalMessage)
		{
			string debugText = Logger.GetCurrentLog ();
			string message = additionalMessage + Environment.NewLine + " --- " 
				+ Environment.NewLine + debugText + Environment.NewLine;

			yield return GetSingletonInstance().StartCoroutine(Message.Request.BugReport (GetDeviceID(), message));
		}
	}
}
