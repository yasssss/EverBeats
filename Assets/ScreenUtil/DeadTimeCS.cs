using UnityEngine;
using System.Collections;

public class DeadTimeCS : MonoBehaviour {
	public float deadTime;
	void Start () {
		Destroy (gameObject,deadTime);
	}
}
