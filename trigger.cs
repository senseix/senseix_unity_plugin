using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
public class trigger : MonoBehaviour {
    int times = 0;
	private bool mainWindow = true;
	private bool coachLogin = false;
	private bool developerLogin = false;
	private bool coachSignup = false;
	private bool coachSessionBeg = false;
	
	public string emailText = "email";
	public string passwordText = "";
	public string name = "name";
	public string tmp = "";
	public bool local = false;
	public trigger()
	{
		senseix.initSenseix("63c4512541f5d27dd4dd12a8e5b8c0ea7d8c2f6be8e15e7718920b399bc9846f");
		if (senseix.inSession) 
		{
			mainWindow = false;
			coachLogin = false;
			coachSessionBeg = true;
		}
	}
    public void showLoading()
	{
		GUI.Box(new Rect(Screen.width/2-Screen.width/4, Screen.height/2-Screen.height/8, Screen.width/2, Screen.height/4), "Loading");
	}
	void OnGUI() {
		/*
		 * implemented in senseix as private
		if (GUI.Button(new Rect(Screen.width-90, 0,80,30), "Save"))
		{	
			if(senseix.getAuthToken() != null)
			{
				print("Data Saved: " + senseix.getAuthToken());
				PlayerPrefs.SetString("data00",senseix.getAuthToken());
			}
			else
			{
				print("You have not signed in.");

			}

		}
		if (GUI.Button(new Rect(Screen.width-90, 40,80,30), "Load"))
		{	
			if(PlayerPrefs.HasKey("data00"))
			{
				string authToken = PlayerPrefs.GetString("data00","null");
				senseix.setAuthToken(authToken);
				mainWindow = false;
				coachLogin = false;
				coachSessionBeg = true;
				print("Loaded token: " + senseix.getAuthToken());
			}
			else
			{
				print ("No save found, or save data invalid.");
			}
		}
		*/
		if(mainWindow)
		{
        	if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2-15,80,30), "Login"))
			{	
				mainWindow = false;
			}
		}
		if(!mainWindow&&!coachLogin&&!developerLogin&&!coachSignup&&!coachSessionBeg)
		{
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2-15,80,30), "Coach"))
			{	
				coachLogin = true;
			}
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+25,80,30), "Developer"))
			{	
				developerLogin = true;
			}
		}
		if(coachLogin) //coachSignUp
		{
			emailText = GUI.TextField(new Rect(Screen.width/2-75, Screen.height/2-5,80,30), emailText, 25);
			passwordText = GUI.PasswordField(new Rect(Screen.width/2-75, Screen.height/2+30,80,30), passwordText, "*"[0], 25);
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+65,80,30), "Login"))
			{	
				if(senseix.coachLogin(emailText,passwordText) == 0)
				{
					coachLogin = false;
					coachSessionBeg = true;
					emailText="email";
					passwordText="";
				}
				else
				{
					print ("Login failed");
				}
			}
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+100,80,30), "Register"))
			{	
				coachLogin = false;
				coachSignup = true;
			}
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+135,80,30), "Cancel"))
			{	
				coachLogin = false;
				emailText="email";
				passwordText="";
			}
		}
		if(coachSignup)
		{
			emailText = GUI.TextField(new Rect(Screen.width/2-75, Screen.height/2-5,80,30), emailText, 25);
			name = GUI.TextField(new Rect(Screen.width/2-75, Screen.height/2+30,80,30), name, 25);
			passwordText = GUI.PasswordField(new Rect(Screen.width/2-75, Screen.height/2+65,80,30), passwordText, "*"[0], 25);
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+100,80,30), "Summit"))
			{	
				if(senseix.coachSignUp(emailText,name,passwordText) == 0)
				{
					coachSignup = false;
					coachSessionBeg = true;
					emailText="email";
					passwordText="";
				}
				else
					print ("Sign up failed");
			}
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+135,80,30), "Cancel"))
			{	
				coachLogin = true;
				coachSignup = false;
				emailText="email";
				passwordText="";
				name="";
			}
		}
		if(developerLogin)
		{
			emailText = GUI.TextField(new Rect(Screen.width/2-75, Screen.height/2-5,80,30), emailText, 25);
			passwordText = GUI.PasswordField(new Rect(Screen.width/2-75, Screen.height/2+30,80,30), passwordText, "*"[0], 25);
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+100,80,30), "Login"))
			{	
				if(senseix.developerLogin(emailText,passwordText) == 0)
				{
					developerLogin = false;
					emailText="email";
					passwordText="";
				}
				else
				{
					print ("Login failed");
				}
			}
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+65,80,30), "Cancel"))
			{	
				developerLogin = false;
				emailText="email";
				passwordText="";
			}
		}
		if(coachSessionBeg)
		{
			tmp = GUI.TextArea(new Rect(10,10,200,600), tmp, 250);
			//tmp = GUI.TextField(new Rect(Screen.width/2-75, Screen.height/2-40,120,30), tmp, 25);
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2-5,120,30), "create player"))
			{	
				if(senseix.createPlayer (tmp) == 0)
					tmp="";
				else
				{
					print ("Failed to create player");
				}
			}
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+30,120,30), "Player Index"))
			{	
				Queue players = senseix.getPlayer();
				heavyUser player = null;
				StringBuilder result = new StringBuilder();
				if(players != null)
				{
					while(players.Count != 0)
					{
						player = (heavyUser)players.Dequeue();
						if(player != null)
						{
							result.Append("ID: ");
							result.Append(player.id);
							result.Append("  Name: ");
							result.Append(player.name);
							result.Append("\n");
						}
					}
					tmp=result.ToString();
				}
				else
				{
					print ("Failed to get players");
				}
			}
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+65,120,30), "Pull a Problem"))
			{	
				Queue newproblemQ = senseix.pullProblemQ(40,1,"Mathematics",1);
				if(newproblemQ != null)
				{
					problem newproblem = (problem)newproblemQ.Dequeue();
					tmp = newproblem.content;
				}
				else
				{
					print("Failed to pull problems");
				}
			}
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+100,120,30), "Logout"))
			{	
				senseix.coachLogout();
				coachLogin = true;
				coachSessionBeg = false;
				emailText="email";
				passwordText="";
			}
		}
		
    }
	void DoMyWindow(int windowID) {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
	void Update() {
		//HttpWebResponse response = null;
		
		if (Input.GetKey("0"))
		{
			senseix.initSenseix("63c4512541f5d27dd4dd12a8e5b8c0ea7d8c2f6be8e15e7718920b399bc9846f");
		}
		if (Input.GetKey("a"))
		{
			senseix.developerLogin("80644036@qq.com","password.com");
		}
		if (Input.GetKey("b"))
		{
			senseix.developerLogout();
		}
		if (Input.GetKey("c"))
		{
			senseix.coachLogin("80640000@qq.com","password.com");
        }
        if (Input.GetKey("d")){
			senseix.coachLogout();
		}
		if (Input.GetKey("e")){
			senseix.coachSignUp("90909090@qq.com","chris","password.com");
		}
		if (Input.GetKey("f")){
			senseix.getPlayer ();
		}
		if (Input.GetKey("g")){
			senseix.createPlayer ("cheng five");
		}
		if (Input.GetKey("h")){
			senseix.pullProblemQ(40,3,"Mathematics",1);
        }
    }
}
