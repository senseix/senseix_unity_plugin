// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
namespace Senseix.Message {
	static public class Response
	{
		static public bool ParseResponse(Constant.MessageType type, ref ResponseHeader reply)
		{
			//Debug.Log ("parse response status: " + reply.Status);

			if (reply == null) 
			{
				throw new Exception ("Reply was returned as null...this should not be possible");
			}

			if (!reply.IsInitialized) 
			{
				Debug.Log ("Could not find a valid reply Message from server...");
				return false; 
			}

			Debug.Log (reply.Message);
			if (reply.Status == Constant.Status.FAILURE) 
			{
				Debug.Log (reply.Message);
				return false;
			}
	
			switch (type) 
			{
				case Constant.MessageType.RegisterDevice:
					if (reply.HasDeviceRegistration && reply.DeviceRegistration.IsInitialized) 
					{	
						//Debug.Log("save auth token.");
						SenseixSession.SetAndSaveAuthToken(reply.DeviceRegistration.AuthToken);
						SenseixSession.SetSessionState(true);
						//Debug.Log("I come from the City of Compton, and am I a temporary account? " + reply.DeviceRegistration.IsTemporaryAccount);
						SenseixSession.SetSignedIn(!reply.DeviceRegistration.IsTemporaryAccount);
						SenseixSession.CheckProblemPostCacheSubmission();
					} 
					else 
					{
						SenseixSession.SetSessionState(false);
						Debug.Log("Can't find key from result");
						return false;
					}
					break;

				case Constant.MessageType.GameVerification:
					SenseixSession.SetSessionState(true);
					Debug.Log("I got a response from my game verification Message");
					break;
				
				case Constant.MessageType.RegisterParent:
					if (reply.Status == Constant.Status.FAILURE) 
					{
						Debug.Log ("Register parent failed.");
					}
					if(reply.HasParentRegistration && reply.ParentRegistration.IsInitialized) 
					{
						SenseixSession.SetAndSaveAuthToken(reply.ParentRegistration.AuthToken);
					} 
					else 
					{	
						SenseixSession.SetSessionState(false);
						Debug.Log("Can't find key from result");
						return false;
					}
					break;

				case Constant.MessageType.SignInParent:
					if (reply.Status == Constant.Status.FAILURE) 
					{
						throw new Exception ("We encountered a fatal failure on sign in.");
					}
					if(reply.HasParentSignIn && reply.ParentSignIn.IsInitialized) 
					{
						SenseixSession.SetAndSaveAuthToken(reply.ParentSignIn.AuthToken);
					} 
					else 
					{
						SenseixSession.SetSessionState(false);
						Debug.Log("Can't find key from result");
						return false;
					}
					break;

				case Constant.MessageType.SignOutParent:
					SenseixSession.SetSessionState(false);//Duane, this seems..odd
					break;

				case Constant.MessageType.MergeParent:
					if(reply.HasParentMerge && reply.ParentMerge.IsInitialized && reply.ParentMerge.HasAuthToken)
					{
						SenseixSession.SetAndSaveAuthToken(reply.ParentRegistration.AuthToken);
					} 
					else 
					{	
						SenseixSession.SetSessionState(false);
						Debug.Log("Can't find key from result");
						return false;
					}
					break;

				case Constant.MessageType.CreatePlayer:
					SenseixSession.SetSessionState(true);
//					Debug.Log("I got a response from a create Player Message");
					break;

				case Constant.MessageType.ListPlayer:
					SenseixSession.SetSessionState(true);
//					Debug.Log("I got a response from a list Player Message");
					SenseixSession.SetCurrentPlayerList(reply.PlayerList);
					break;

				case Constant.MessageType.RegisterPlayerWithApplication:
					SenseixSession.SetSessionState(true);
//					Debug.Log("I got a response from a register Player Message");
					break;

				case Constant.MessageType.ProblemPost:
					SenseixSession.SetSessionState(true);
					Debug.Log("I got a response from a Problem post Message");
					break;
				
				case Constant.MessageType.ProblemGet:
					Debug.Log("I got a response from a Problem get Message");
					if (reply.ProblemGet.ProblemCount != ProblemKeeper.PROBLEMS_PER_PULL)
						Debug.LogWarning("I asked for " + ProblemKeeper.PROBLEMS_PER_PULL + " problems, but I only got " + reply.ProblemGet.ProblemCount);
					if(reply.HasProblemGet && reply.ProblemGet.IsInitialized) 
					{
						ProblemKeeper.ReplaceSeed(reply);
					}
					else
					{
						throw new Exception("Response to ProblemGet request was empty or uninitialized");
					}
					break;

				case Constant.MessageType.LeaderboardPage:
//					Debug.Log ("I recieved a Leaderboard page response");
					Debug.Log(reply.Page.PlayerList);
					SenseixSession.SetLeaderboardPlayers(reply.Page.PlayerList);
					break;

				case Constant.MessageType.PlayerScore:
					SenseixSession.SetSessionState(true);
//					Debug.Log("I got a response from a Player score Message");
					break;

				case Constant.MessageType.PlayerRank:
					SenseixSession.SetSessionState(true);
//					Debug.Log("I got a response from a Player rank Message");
					break;

				case Senseix.Message.Constant.MessageType.EncouragementGet:
					SenseixSession.SetSessionState(true);
					//Debug.Log("I got a response from an encouragement get Message");
					break;

				default:
					throw new Exception("Response.cs recieved a MessageType that it didn't recognize.");
			}
			return true;
		}
	}
}

