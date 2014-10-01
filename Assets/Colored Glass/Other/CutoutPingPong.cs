using UnityEngine;
using System.Collections;

public class CutoutPingPong : MonoBehaviour
{

  public int timeSec = 3;
  public bool isSmooth = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    if(isSmooth) renderer.material.SetFloat("_Cutout", Mathf.PingPong(Time.time / timeSec, 2 + 1) - 1.5f);
    else renderer.material.SetFloat("_Cutout", Mathf.PingPong(Time.time / timeSec, 2) - 0.5f);
	}
}
