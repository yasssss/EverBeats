using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour {
	public bool isRepeat = false;
	public int FrameRate = 30;
	public int FrameNum = 40;
	public int WholeSize = 2048;
	public int PieceSize = 256;

	
	private float piece;
	private int rowNum;
	private float interval;

	private float generatedTime;

	// Use this for initialization
	void Start () {
		piece = ((float)PieceSize)/((float)WholeSize);
		rowNum = WholeSize / PieceSize;
		interval = 1.0f / ((float)FrameRate);
		generatedTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		int frame = Mathf.FloorToInt ((Time.time - generatedTime) / interval);
		if (frame >= FrameNum){
			if (isRepeat){
				generatedTime = Time.time;
			}else{
				Destroy(gameObject);
			}
			return;
		}
		int x = frame % rowNum;
		int y = frame / rowNum;

		gameObject.renderer.material.SetTextureOffset ( "_MainTex" , new Vector2( piece * x , 1.0f -  piece *(y + 1)  ));
		gameObject.renderer.material.SetTextureScale ( "_MainTex" , new Vector2( piece , piece ));
	}
}
