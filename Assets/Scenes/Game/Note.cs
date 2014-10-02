using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {
	public MusicData.NoteData data;
	public Material goneMaterial;
	public Material failedMaterial;
	public GameObject effectPrehab;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void tapped(MusicData.NoteData.NotePhase phase){
		data.phase = phase;
		data.tappedTime = NoteManager.manager.audio.time;
		data.tappedPosition = transform.position;

		Vector3 correctPos = getCorrectPos ();
		Instantiate (effectPrehab , correctPos , Quaternion.Euler(new Vector3( 50.0f ,0 , 0)));
		gameObject.renderer.material = goneMaterial;

		if (ScreenUtil.findObject (transform, "flare") != null){
			Destroy(ScreenUtil.findObject (transform, "flare"));
		}
		iTween.FadeTo(gameObject, iTween.Hash("alpha", 0, "time", 0.5f));
		iTween.ScaleTo(gameObject, iTween.Hash("x", 1.5f, "time", 0.5f));

		if (phase == MusicData.NoteData.NotePhase.Great){
			transform.position = correctPos;
		}
	}
	
	public void failed(){
		data.tappedTime = NoteManager.manager.audio.time;
		data.tappedPosition = getCorrectPos ();
		gameObject.renderer.material = failedMaterial;
		Destroy(ScreenUtil.findObject (transform, "flare"));
		iTween.FadeTo(gameObject, iTween.Hash("alpha", 0, "time", 0.5f));
	}

	public Vector3 getCorrectPos(){
		return new Vector3 (transform.position.x, transform.position.y, NoteManager.manager.left.position.z);
	}
}
