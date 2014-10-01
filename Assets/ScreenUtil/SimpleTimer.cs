using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleTimer : MonoBehaviour {
	public delegate void OnTimeIsComing();
	public delegate bool OnRoutineIsComing();
	private static List<SimpleTimer> timers;
	//timer
	public OnTimeIsComing method;
	public float time;

	//routine
	public OnRoutineIsComing r_method;
	public float customDeltaTime;

	// Use this for initialization
	void Start () {
	
	}
	
	//Timer
	void Update () {
		if ( time < Time.realtimeSinceStartup ){
			method();
			Destroy(gameObject);
			timers.Remove(this);
		}
	}
	static public void setTimer(float duration , OnTimeIsComing time){
		GameObject g = new GameObject ();
		SimpleTimer t = g.AddComponent<SimpleTimer> ();
		g.name = "timer";
		t.time = Time.realtimeSinceStartup + duration;
		t.method = time;
		if (timers == null){
			timers = new List<SimpleTimer>();
		}
		timers.Add (t);
	}
	static public void dismissTimer(OnTimeIsComing time){
		if (timers == null){
			return;
		}
		foreach( SimpleTimer t in timers){
			if (t.method == time){
				try{
					Destroy(t.gameObject);
				}catch(System.Exception){}
			}
		}
	}
}
