using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bubble : MonoBehaviour
{
	public GameObject occlusionObject;
	

	[Header("Effect Files")]
	public GameObject instantiateEffect;
	public AudioClip instantiateSound;
	public GameObject popEffect;
	public AudioClip popSound;
	public GameObject rightEffect;
	public AudioClip rightSound;
	public GameObject wrongEffect;
	public AudioClip wrongSound;
	public AudioClip ambientSoundWaiting;
	public AudioClip ambientSoundPopped;

	[Header("Audio Sources")]
	public AudioSource eventAudioSource;
	public AudioSource ambientAudioSource;

	[Header("Math Problem")]
	public TextMeshProUGUI problemText;
	public TextMeshProUGUI feedBackText;

	ProblemManager problemManager;

	private void Awake()
	{
		//relationships
		problemManager = FindObjectOfType<ProblemManager>();

		//setup audio sources
		eventAudioSource = gameObject.AddComponent<AudioSource>();
		eventAudioSource.playOnAwake = false;
		eventAudioSource.loop = false;
		eventAudioSource.spatialBlend = 1;

		ambientAudioSource = gameObject.AddComponent<AudioSource>();
		ambientAudioSource.playOnAwake = false;
		ambientAudioSource.loop = true;
		ambientAudioSource.spatialBlend = 1;
		
	}
	// Start is called before the first frame update
	void Start()
    {
		feedBackText.text = "";

		ambientAudioSource.clip = ambientSoundWaiting;
		ambientAudioSource.Play();

		//instantiation effects
		PlayEffects(instantiateEffect, instantiateSound);

	}

	private void OnMouseDown()
	{
		Pop();
	}

	void PlayEffects(GameObject effect, AudioClip soundEffect)
	{
		GameObject effectObject = GameObject.Instantiate(effect, transform);
		effectObject.transform.localPosition = Vector3.zero;

		eventAudioSource.clip = soundEffect;
		eventAudioSource.Play();

	}

	private void OnTriggerEnter(Collider other)
	{
		Pop();
	}
	public void Pop()
	{
		occlusionObject.SetActive(false);
		problemText.gameObject.SetActive(true);
		GetComponent<Collider>().enabled = false;
		PlayEffects(popEffect, popSound);

		WASD wasd;
		if (wasd = GetComponent<WASD>())
		{
			wasd.enabled = false;
		}
		Camera camera;
		if (camera = GetComponent<Camera>())
		{
			camera.enabled = false;
		}

	}
	public void AnswerRight()
	{
		PlayEffects(rightEffect, rightSound);
		feedBackText.text = "Good Job!";
	}
	public void TryAgain()
	{
		PlayEffects(wrongEffect, wrongSound);
		feedBackText.text = "Oops!";

	}
	public void MoveOn()
	{
		GameObject.Destroy(gameObject);
	}

}
