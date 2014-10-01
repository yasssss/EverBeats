using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoteManager : MonoBehaviour {
	static private float REMOVE_OFFSET_TIME = 1;
	static private float OK_TIME 		= 0.1f;
	static private float OK_DISTANCE	= 0.2f;


	static public bool isEditMode = false;
	static public MusicData music;
	static public NoteManager manager;
	
	public float Interval = 1;


	public Transform left;
	public Transform right;
	public Transform generate;
	public GameObject notePrehab;
	
	public List< MusicData.NoteData> notes;
	private int index;
	private AudioSource audio;

	// Use this for initialization
	void Start () {
		audio = Camera.main.GetComponent<AudioSource> ();
		if (music == null){
			music = MusicData.testnotes ();
		}
		index = 0;
		notes = new List< MusicData.NoteData > ();
		manager = this;
	}

	
	// Update is called once per frame
	void Update () {
		if (isEditMode) {
		} else {
			if (music.notes.Count > index){
				Generate ();
			}
			Move ();
		}

	}

	private void Generate(){
		MusicData.NoteData next = music.notes [index];
		while ( next.time - Interval < audio.time && music.notes.Count > index){
			GameObject myNote = (GameObject)Instantiate( notePrehab , new Vector3( 
			                                                                      left.position.x + (right.position.x - left.position.x) * next.offset ,
			                                                                      0 ,
			                                                                      generate.position.z ), Quaternion.identity );
			next.gameObject = myNote;
			myNote.GetComponent<Note>().data = next;
			notes.Add( next );
			index++;
			if (music.notes.Count <= index){
				break;
			}
			next = music.notes [index];
		}
	}

	private void Move(){
		foreach ( MusicData.NoteData note in notes ){
			Vector3 pos = note.gameObject.transform.position;
			note.gameObject.transform.position = new Vector3(  pos.x , 0 , (( left.position.z - generate.position.z ) * (1 - (note.time - audio.time)/ Interval )  )  + generate.position.z);
		}
		notes.RemoveAll ( ( MusicData.NoteData note )=>{
			if ( note.time + REMOVE_OFFSET_TIME < audio.time){
				Destroy(note.gameObject);
				return true;
			}
			return false;
		});
	}
	public void pushNote( float offset ){
		// get the note
		foreach ( MusicData.NoteData note in music.notes ){
			if ( Mathf.Abs(note.time - audio.time) < OK_TIME && !note.isDead ){
				if (  Mathf.Abs(note.offset - offset) < OK_DISTANCE ){
					//Hit
					note.gameObject.GetComponent<Note>().tapped();
					break;
				}
			}
		}
	}

	/*
	 * EditMode
	 */
	public void addNote (float offset){
		MusicData.NoteData n = new MusicData.NoteData ();
		n.time   = audio.time;
		n.offset = offset;
		notes.Add (n);
	}
	public MusicData exportMusicData(){
		music.notes = notes;
		return music;
	}
}