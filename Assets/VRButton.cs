using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRButton : MonoBehaviour
{
	Button button;
	BoxCollider boxCollider;

	// Start is called before the first frame update
	void Start()
    {
		button = GetComponent<Button>();

		if (boxCollider = button.gameObject.GetComponent<BoxCollider>())
		{
			//do nothing
		}
		else
		{
			boxCollider = button.gameObject.AddComponent<BoxCollider>();
		}
		boxCollider.isTrigger = true;


	}

	private void OnTriggerEnter(Collider stylusCollider)
	{
		button.onClick.Invoke();
	}


	// Update is called once per frame
	void Update()
    {
        
    }
}
