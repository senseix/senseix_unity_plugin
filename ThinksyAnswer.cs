using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer
{
	private ArrayList answerParts = new ArrayList();
	
	/// <summary>
	/// Build an answer from a message received from the Thinksy server.  For internal use within the Thinksy plugin.
	/// </summary>
	public Answer(Senseix.Message.Problem.Answer protoAnswer)
	{
		foreach (Senseix.Message.Atom.Atom atom in protoAnswer.answers)
		{
			answerParts.Add(ProblemPart.CreateProblemPart(atom));
		}
	}
	
	/// <summary>
	/// Initializes a new empty instance of the <see cref="Answer"/> class.
	/// </summary>
	public Answer()
	{
		
	}
	
	/// <summary>
	/// Adds a part to this answer.
	/// </summary>
	/// <param name="part">Added Part.</param>
	public void AddAnswerPart(ProblemPart part)
	{
		answerParts.Add(part);
		ThinksyQuestionDisplay.DisplayCurrentQuestion ();
	}
	
	/// <summary>
	/// Clears the answer parts.  You might use this if you want to give your player a second
	/// chance at giving answers.
	/// </summary>
	public void ClearAnswerParts()
	{
		answerParts.Clear ();
	}
	
	/// <summary>
	/// Removes the most recent answer part.  You might use this if you want to give your player
	/// a chance to retry an incorrect answer part, without retrying the whole problem.
	/// </summary>
	public void RemoveMostRecentAnswerPart()
	{
		answerParts.RemoveAt (answerParts.Count - 1);
		ThinksyQuestionDisplay.DisplayCurrentQuestion ();
	}
	
	/// <summary>
	/// Returns a UUID for each answer part.
	/// </summary>
	public string[] GetAnswerIDs()
	{
		string[] answerIDs = new string[answerParts.Count];
		for (int i = 0; i < answerParts.Count; i++)
		{
			answerIDs[i] = (((ProblemPart)answerParts[i]).GetUniqueID());
		}
		return answerIDs;
	}
	
	/// <summary>
	/// Gets the answer parts given so far.
	/// </summary>
	/// <returns>The answer parts associated with this answer.</returns>
	public ProblemPart[] GetAnswerParts()
	{
		return (ProblemPart[])answerParts.ToArray(typeof(ProblemPart));
	}
	
	/// <summary>
	/// Gets the answer part of the given index.  You might use this if you want to review past
	/// or future correct or given answers parts.
	/// You can also iterate through answer parts using foreach.
	/// </summary>
	/// <param name="index">Index.</param>
	public ProblemPart GetAnswerPart(int index)
	{
		return (ProblemPart)answerParts [index];
	}
	
	public System.Collections.IEnumerator GetEnumerator()
	{
		foreach(ProblemPart part in answerParts)
		{
			yield return part;
		}
	}
	
	/// <summary>
	/// Returns the number of answer parts current associated with this answer.
	/// </summary>
	public int AnswerPartsCount()
	{
		return answerParts.Count;
	}
}
