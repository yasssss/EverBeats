using UnityEngine;
using System.Collections;

public class FingerManager : MonoBehaviour {
	static bool isEditerMode = false;

	// Use this for initialization
	void Start () {

	}
	void Update(){
		if (Input.touchCount == 0 && Input.GetMouseButtonDown (0)) {
			isEditerMode = true;
		}

		if (NoteManager.isEditMode) {
			/* EditMode */
			if (isEditerMode){
				if (Input.GetMouseButtonDown(0)) {
					makeNote(Input.mousePosition);
				}
			}else{
				for ( int i = 0 ; i<Input.touchCount ; i++){
					Touch t = Input.GetTouch(i);
					if (t.phase == TouchPhase.Began){
						makeNote(t.position);
					}
				}
			}
		} else {
			/* Normal */
			if (isEditerMode){
				if (Input.GetMouseButtonDown(0)) {
					tapped(Input.mousePosition);
				}
			}else{
				for ( int i = 0 ; i<Input.touchCount ; i++){
					Touch t = Input.GetTouch(i);
					if (t.phase == TouchPhase.Began){
						tapped(t.position);
					}
				}
			}
		}
	}

	private void makeNote(Vector2 point){
		NoteManager.manager.addNote ( getOffset(point) );
	}

	private void tapped(Vector2 point){
		NoteManager.manager.pushNote ( getOffset(point) );
		/*
		Ray ray = Camera.main.ScreenPointToRay(point);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit)){
			GameObject obj = hit.collider.gameObject;
			if ( obj.GetComponent<Note>() != null){
				obj.GetComponent<Note>().tapped();
			}
		}*/
	}

	private float getOffset( Vector2 point ){
		float offset = 0.14f;
		float x = point.x / Screen.width;
		x = Mathf.Max (offset, Mathf.Min (1.0f - offset, x));
		return (x - offset) / (1.0f - offset * 2);
	}

}
