using UnityEngine;
using System.Collections;

public class ChoiceManager : MonoBehaviour {

	public static MusicData music = null;

	// Use this for initialization
	void Start () {
		if (music == null){
			music = new MusicData();
		}
		GameObject.Find ("textbox").GetComponent<UIInput>().value = music.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play(){
		music = new MusicData (GameObject.Find ("textbox").GetComponent<UIInput>().value);

		GameManager.returnScene = "Choice";
		NoteManager.isEditMode = false;
		NoteManager.music = music;
		Application.LoadLevel ("Game");
	}
	
	public void Make(){
		music = new MusicData ();
		
		GameManager.returnScene = "Choice";
		NoteManager.isEditMode = true;
		NoteManager.music = music;
		Application.LoadLevel ("Game");
	}
}
