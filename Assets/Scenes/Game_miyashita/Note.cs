using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {
	public MusicData.NoteData data;
	public Material goneMaterial;
	private GameObject effectPrehab;

	// Use this for initialization
	void Start () {
		effectPrehab = GameObject.Find ("effect");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void tapped(){
		data.isDead = true;
		Destroy (gameObject);
		NoteManager.manager.notes.Remove (data);
		DeadTimeCS d = ((GameObject)Instantiate (effectPrehab , transform.position , Quaternion.identity)).AddComponent<DeadTimeCS>();
		d.deadTime = 0.3f;
		gameObject.renderer.material = goneMaterial;
	}

}
