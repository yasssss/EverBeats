using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	static public string returnScene;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void saveAndExit(){
		if (NoteManager.isEditMode){
			ChoiceManager.music = NoteManager.manager.exportMusicData();
		}
		Application.LoadLevel (returnScene);
	}
}
