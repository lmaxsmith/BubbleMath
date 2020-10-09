using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// AnswerDigit sits on the game object of each available digit.
/// When player pushes buttons to answer the problem, the number appears here. 
/// </summary>
public class AnswerDigit : MonoBehaviour
{
	public GameObject hilight;
	TMP_InputField answerField;
	
	int answerdigitInt = 0;

	public int AnswerdigitInt { get => answerdigitInt;}

	private void Awake()
	{
		answerField = GetComponent<TMP_InputField>();
	}

	public void ClearField()
	{
		hilight.SetActive(false);
		answerdigitInt = 0;
		answerField.text = "";
	}

	public void FillField(int number)
	{
		answerdigitInt = number;

		answerField.text = number.ToString();
		hilight.SetActive(false);
	}
	public void HilightField()
	{
		ClearField();
		hilight.SetActive(true);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
