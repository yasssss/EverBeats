using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicData {
	public List<NoteData> notes;


	static public MusicData testnotes(){
		return new MusicData( ((TextAsset)Resources.Load("note_sample")).text );
	}

	
	public MusicData (){
		notes = new List<NoteData>();
	}
	public MusicData (string data){
		string[] arr = ScreenUtil.sepalateByEnter (data);
		notes = new List<NoteData> ();
		foreach (string s in arr ){
			notes.Add (new NoteData(s) );
		}
	}
	public string ToString(){
		ArrayList arr = new ArrayList();
		foreach ( MusicData.NoteData n in notes ){
			arr.Add( n.ToString() );
		}
		return ScreenUtil.Join( (string[])arr.ToArray( typeof(string) ) , "\n" );
	}

	public class NoteData {
		public float time;
		public float offset;

		/* modifiable */
		public GameObject gameObject;
		public bool isDead;

		public NoteData (){
			isDead = false;
		}
		public NoteData (string data){
			string[] arr = data.Split(new string[]{ "," } , System.StringSplitOptions.None);
			time = float.Parse(arr[0]);
			offset = float.Parse(arr[1]);
			isDead = false;
		}
		public string ToString(){
			return time + "," + offset;
		}
	}
}
