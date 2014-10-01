using UnityEngine;
using System.Collections;

public class debugLabel : MonoBehaviour {

	UILabel label;
	// Use this for initialization
	void Start () {
		label = GetComponent<UILabel> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Camera.main.GetComponent<AudioSource>().isPlaying){
			label.text = ""+Camera.main.GetComponent<AudioSource>().time.ToString("f2");
		}else{
			label.text = "stop";
		}
	}
}
