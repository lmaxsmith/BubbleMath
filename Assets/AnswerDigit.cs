using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
	// Start is called before the first frame update
	void Start()
    {
        
    }

	public void ClearField()
	{
		hilight.SetActive(false);
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
