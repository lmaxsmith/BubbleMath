using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRUI : MonoBehaviour
{

	Button[] buttons;

    // Start is called before the first frame update
    void Start()
    {
		buttons = GetComponentsInChildren<Button>();
		foreach (var button in buttons)
		{
			button.gameObject.AddComponent<VRButton>();

		}


    }

}
