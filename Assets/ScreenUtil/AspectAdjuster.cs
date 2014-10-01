using UnityEngine;
using System.Collections;

public class AspectAdjuster : MonoBehaviour {
	Camera camera;

	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		adjustScreenSize (16.0f / 9.0f);
	}
	
	public void adjustScreenSize( float aspect ){
		float nowAsp = Screen.width / Screen.height;
		float toAsp = aspect;
		if (nowAsp == toAsp) {
			return;
		}
		float rec = 0;
		float off = 0; 
		if (nowAsp < toAsp) {
			rec = (Screen.width / toAsp )/ Screen.height;
			off = (1 - rec) / 2;
			camera.rect = new Rect ( 0 , off , 1, rec );
		} else {
			rec = (Screen.height * toAsp )/ Screen.width;
			off = (1 - rec) / 2;
			camera.rect = new Rect ( off , 1 , rec, 1 );
		}
	}
}
