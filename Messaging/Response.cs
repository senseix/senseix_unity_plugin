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
	public class Response
	{
		public Response ()
		{
	   
		}
		public static int ParseResponse(Constant.MessageType type, ref ResponseHeader reply)
		{
			if (reply == null) {
				Debug.Log ("Reply was returned as null...this should not be possible");
				return -1;
			}
			if (!reply.IsInitialized) {
				Debug.Log ("Could not find a valid reply message from server...");
				return -2; 
			}
			if (reply.Status == Constant.Status.FAILURE) 
			{
				Debug.Log (reply.Message);
				return -2;
			}
	
			switch (type) {
	
			case Constant.MessageType.RegisterDevice:
				if (reply.HasDeviceRegistration && reply.DeviceRegistration.IsInitialized) {	
					SenseixController.SetAndSaveAuthToken(reply.DeviceRegistration.AuthToken);
					SenseixController.SetPlayerID(reply.DeviceRegistration.PlayerId);
					SenseixController.SetSessionState(true);
				} else {
					SenseixController.SetSessionState(false);
					Debug.Log("Can't find key from result");
					return -2;
				}
				break;
			case Constant.MessageType.GameVerification:
				//henry wrote this
				SenseixController.SetSessionState(true);
				Debug.Log("I got a response from my game verification message");
				break;
			
			case Constant.MessageType.RegisterParent:
				if (reply.Status == Constant.Status.FAILURE) {
					Debug.Log ("DUANE!!!! MERGE CODE REQUIRED HERE!!!");
				}
				if(reply.HasParentRegistration && reply.ParentRegistration.IsInitialized) {
					SenseixController.SetAndSaveAuthToken(reply.ParentRegistration.AuthToken);
				} else {	
					SenseixController.SetSessionState(false);
					Debug.Log("Can't find key from result");
					return -2;
				}
				break;
			case Constant.MessageType.SignInParent:
				if (reply.Status == Constant.Status.FAILURE) {
					Debug.Log ("DUANE!!!! MERGE CODE REQUIRED HERE!!!");
				}
				if(reply.HasParentSignIn && reply.ParentSignIn.IsInitialized) {
					SenseixController.SetAndSaveAuthToken(reply.ParentSignIn.AuthToken);
				} else {
					SenseixController.SetSessionState(false);
					Debug.Log("Can't find key from result");
					return -2;
				}

				break;
			case Constant.MessageType.SignOutParent:
	//			SenseixController.cleanData ();

				SenseixController.SetSessionState(false);//Duane, this seems..odd
				break;
//			case Constant.MessageType.EditParent:
//				//We will only return a new authtoken if required i.e. password changed, email changed.
//				if(reply.HasParentEdit && reply.ParentEdit.IsInitialized && reply.ParentEdit.HasAuthToken){
//					SenseixController.SetAndSaveAuthToken(reply.ParentRegistration.AuthToken);
//				} else {
//					SenseixController.SetSessionState(false);
//				}
//				break;
			case Constant.MessageType.MergeParent:
				if(reply.HasParentMerge && reply.ParentMerge.IsInitialized && reply.ParentMerge.HasAuthToken){
					SenseixController.SetAndSaveAuthToken(reply.ParentRegistration.AuthToken);
				} else {	
					SenseixController.SetSessionState(false);
					Debug.Log("Can't find key from result");
					return -2;
				}
			break;
				//DUANE! NEED TO FILL THESE IN!
			case Constant.MessageType.CreatePlayer:
			break;
			case Constant.MessageType.ListPlayer:
			break;
			case Constant.MessageType.RegisterPlayerWithApplication:
			break;
			case Constant.MessageType.ProblemPost:
				if (reply.HasProblemPost){
					Debug.Log ("Successfully posted problems to the server.");
				}
				else {
					Debug.Log ("Message sent back for a problem was not as expected");
				}
			break;
			case Constant.MessageType.ProblemGet:
				if(reply.HasProblemGet && reply.ProblemGet.IsInitialized) 
				{
					foreach(Problem.ProblemData entry in reply.ProblemGet.ProblemList) 
					{
						Problem.ProblemData.Builder problem =  entry.ToBuilder();
						ProblemWorker.AddProblemsToProblemQueue(problem);
					}
				}

			break;
			case Constant.MessageType.LeaderboardPage:
				Debug.Log ("I recieved a leaderboard page response");
				Debug.Log(reply.Page.PlayerList);
				SenseixController.SetLeaderboardPlayers(reply.Page.PlayerList);
			break;
			case Constant.MessageType.PlayerScore:
			break;
			case Constant.MessageType.PlayerRank:
			break;

			default:
			return -1;
			break;
		}
			return 0;

	}

  }
}

