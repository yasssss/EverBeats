using UnityEngine;
using System.Collections;

public class TubeDownloader : MonoBehaviour {

	public bool isDebugMode = false;
	public GameObject white_gage;

	// Use this for initialization
	void Start () {
		white_gage = GameObject.Find ("gage_white");
		if (isDebugMode) {
			Camera.main.GetComponent<AudioSource> ().Play ();
		} else {
			StartCoroutine ("Load");
		}
	}
	
	// Update is called once per frame
	void Update () {
		float max = 1.05f;
		float min = 0.03f;
		white_gage.GetComponent<UIAnchor>().relativeOffset = new Vector2(  (max - min )*(NoteManager.manager.audio.time / NoteManager.music.length) + min , white_gage.GetComponent<UIAnchor>().relativeOffset.y);
	}
	
	private IEnumerator Load()
	{
		WWW www = new WWW("http://YouTubeInMP3.com/fetch/?video=http://www.youtube.com/watch?v=FLUC8aINF1c&lol=cmonunity.mp3");
		yield return www; // 一度中断。読み込みが完了したら再開。
		Camera.main.GetComponent<AudioSource> ().clip = (www.audioClip);
		Camera.main.GetComponent<AudioSource> ().Play ();
	}
}
