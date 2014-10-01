using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ScreenUtil : MonoBehaviour {
	public delegate void onBlackoutFinished();
	public delegate void onFindGameObject(GameObject g);

	private static GameObject resentBlackOut;

	static public int CURVEMODE_EASEIN	=0;
	static public int CURVEMODE_EASEOUT	=1;
	static public int CURVEMODE_NONE	=2;

	static private int MODE_MOVE		=0;
	static private int MODE_FADE		=1;
	static private int MODE_BLACKOUT	=2;
	static private int MODE_SCALE		=3;

	GameObject TargetObj;
	UIAnchor Target;
	Vector2 originOffset;
	Vector2 resetPos;
	Vector2 moveDistance;
	float endTime;
	float interval;
	float startTime;
	bool hideAndResetPos;
	int curveMode;
	float alphaBefore;
	float alphaAfter;
	int mode;
	onBlackoutFinished finishedMethod;
	bool isParent;
	GameObject hideObj;
	Vector2 multiplier;

	public void Start(){

	}

	public void Update(){
		if (TargetObj == null && mode != MODE_BLACKOUT){
			return;
		}

		if (Time.realtimeSinceStartup < startTime){
			return;
		}
		float x = 1.0f - (endTime - Time.realtimeSinceStartup)/interval;
		float easeOut = 3.0f*x*x - 2.0f*x*x*x;
		float easeIn = x*x*x;

		if (mode == MODE_MOVE){
			if (x > 1){
				Destroy(gameObject);
				Target.relativeOffset = originOffset + moveDistance;
				if (hideAndResetPos){
					Target.relativeOffset = resetPos;
					hideObj.SetActive(false);
				}
				return;
			}
			if ( curveMode == CURVEMODE_NONE ){
				Target.relativeOffset = originOffset + x*moveDistance;
			}else if ( curveMode == CURVEMODE_EASEIN ){
				Target.relativeOffset = originOffset + easeIn*moveDistance;
			}else if ( curveMode == CURVEMODE_EASEOUT ){
				Target.relativeOffset = originOffset + easeOut*moveDistance;
			}
		}else if (mode == MODE_FADE){
			TargetObj.GetComponent<UIWidget> () .alpha = (alphaAfter - alphaBefore) * x + alphaBefore;
			if (x > 1){
				if ( TargetObj == resentBlackOut){
					Destroy(resentBlackOut);
					resentBlackOut = null;
				}
				Destroy(gameObject);
			}
		}else if (mode == MODE_BLACKOUT){
			if (x > 1){
				if (finishedMethod != null){
					finishedMethod();
				}
				Destroy(this);
			}
			gameObject.GetComponent<UISprite>() .alpha = x;
		}else if (mode == MODE_SCALE){
			Vector2 m = Vector2.zero;
			if ( curveMode == CURVEMODE_NONE ){
				m = (multiplier - new Vector2(1,1))*x + new Vector2(1,1);
			}else if ( curveMode == CURVEMODE_EASEIN ){
				m = (multiplier - new Vector2(1,1))*easeIn + new Vector2(1,1);
			}else if ( curveMode == CURVEMODE_EASEOUT ){
				m = (multiplier - new Vector2(1,1))*easeOut + new Vector2(1,1);
			}
			TargetObj.GetComponent<UIStretch>().relativeSize = vectorMultiply( m , originOffset);

			if (x > 1){
				Destroy(gameObject);
				return;
			}
		}
	}

	public static void ScaleUI(GameObject target, Vector2 multiplier , float interval , int CurveMode , float delay , bool needsToPreScale ){
		if (needsToPreScale){
			Vector2 pre = target.GetComponent<UIStretch> ().relativeSize;
			target.GetComponent<UIStretch>().relativeSize = new Vector2( pre.x / multiplier.x , pre.y / multiplier.y);
		}
		ScreenUtil s = makeNewScreenUtilObj();
		s.startTime = Time.realtimeSinceStartup + delay;
		s.endTime = s.startTime + interval;
		s.interval = interval;
		s.curveMode = CurveMode;
		s.TargetObj = target;
		s.mode = MODE_SCALE;
		s.originOffset = target.GetComponent<UIStretch>().relativeSize;
		s.multiplier = multiplier;
	}

	public static void moveUI(GameObject target,Vector2 moveDistance , float interval , int CurveMode , bool hideAndResetPos , float delay , bool needsToPremove ){
		foreach (GameObject tar in getChildren (target , new ArrayList() )){
			if (tar.GetComponent<UIAnchor> () == null){
				continue;
			}
			if (interval < 0.01f) {
				tar.GetComponent<UIAnchor> ().relativeOffset += moveDistance;
			} else {
				ScreenUtil s = getScreenUtilFor(tar,MODE_MOVE,delay);
				if (s == null){
					s = makeNewScreenUtilObj();
					if (needsToPremove){
						tar.GetComponent<UIAnchor>().relativeOffset -= moveDistance;
					}
					s.moveDistance = moveDistance;
					s.resetPos = tar.GetComponent<UIAnchor> ().relativeOffset;
				}else{
					Debug.Log("depulicate" + target.name);
					s.moveDistance = s.originOffset + s.moveDistance + moveDistance - s.Target.relativeOffset;
				}
				s.Target = tar.GetComponent<UIAnchor> ();
				s.startTime = Time.realtimeSinceStartup + delay; 
				s.endTime = s.startTime + interval;
				s.interval = interval;
				s.hideAndResetPos = hideAndResetPos;
				s.hideObj = target;
				s.curveMode = CurveMode;
				s.TargetObj = tar;
				s.mode = MODE_MOVE;
				s.originOffset = s.Target.relativeOffset;
			}
		}
	}

	public static void fadeUI(GameObject target, float interval , float delay , float alphaBefore , float alphaAfter ){
		foreach (GameObject tar in getChildren (target , new ArrayList() )) {
			if ( tar.GetComponent<UIWidget> () == null ){
				continue;
			}
			GameObject g = new GameObject ();
			ScreenUtil s = g.AddComponent<ScreenUtil> ();
			s.startTime = Time.realtimeSinceStartup + delay; 
			s.endTime = s.startTime + interval;
			s.interval = interval;
			s.alphaBefore = alphaBefore;
			s.alphaAfter = alphaAfter;
			s.TargetObj = tar;
			s.mode = MODE_FADE;
			tar.GetComponent<UIWidget> ().alpha = alphaBefore;
		}
	}

	public static void blackOut(GameObject blackPrehab , float interval , float delay , onBlackoutFinished method){
		GameObject g = ((GameObject)Instantiate (blackPrehab));
		g.name = "BlackOut";
		resentBlackOut = g;
		if (interval < 0.01f) {
			g.GetComponent<UISprite> ().alpha = 1;
		} else {
			ScreenUtil s = g.AddComponent<ScreenUtil> ();
			s.startTime = Time.realtimeSinceStartup + delay; 
			s.endTime = s.startTime + interval;
			s.interval = interval;
			s.mode = MODE_BLACKOUT;
			s.finishedMethod = method;
			g.GetComponent<UISprite> ().alpha = 0;
		}
	}
	public static void dismissBlackOut(float interval , float delay){
		ScreenUtil.fadeUI ( resentBlackOut,interval,delay,1,0 );
	}
	public static ArrayList getChildren(GameObject g,ArrayList arr){
		if (g == null){
			return arr;
		}
		arr.Add (g);
		for (int i= 0 ; i<g.transform.childCount ; i++ ){
			getChildren( g.transform.GetChild(i).gameObject ,arr);
		}
		return arr;
	}

	public static bool stateEquals(Animator anim,string name){
		AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
		return state.IsName("Base Layer."+name);
	}
	public static float getAsp(){
		return (((float)Screen.width) / ((float)Screen.height) ) / (1136.0f/640f) ;
	}

	public static void sizeAdjust(GameObject g){
		float asp = getAsp() ;
		float x = g.GetComponent<UIStretch> () .relativeSize.x;
		g.GetComponent<UIStretch> () .relativeSize = new Vector2( x , g.GetComponent<UIStretch> () .relativeSize.y * asp);
	}
	
	public static void posAdjust(GameObject g){
		//NOTE set target mode "Top" or "TopLeft"
		float asp = getAsp();
		Vector2 offset = g.GetComponent<UIAnchor> ().relativeOffset;
		g.GetComponent<UIAnchor> ().relativeOffset = new Vector2 (offset.x, offset.y * ScreenUtil.getAsp ());
	}

	public static string Join(string[] array,string sepalator){
		if (array.Length == 0){
			return "";
		}
		if (array.Length == 1){
			return ""+array[0];
		}
		string res = "";
		for (int i = 0 ; i < array.Length -1 ; i++){
			res += array[i] + sepalator;
		}
		return res + array[array.Length -1];
	}
	public static string[] arrayWithRemovingEmpty( string[] strs){
		List<string> res = new List<string> ();
		foreach ( string s in strs){
			if ( !s.Equals("")){
				res.Add(s);
			}
		}
		return res.ToArray();
	}
	public static string[] stringListConvert(ArrayList arr){
		return (string[])arr.ToArray (typeof(string));
	}
	public static List<string> stringListConvert(string[] arr){
		List<string> list = new List<string> ();
		foreach (string s in arr){
			list.Add(s);
		}
		return list;
	}

	static public string generateUniqueId(int i) {
		string return_str = "";
		string[] askey = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
			"k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v",
			"w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H",
			"I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
			"U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5",
			"6", "7", "8", "9" };
		
		for (int j = 0; j < i; j++) {
			return_str += askey[(int) (UnityEngine.Random.value * 10000) % askey.Length];
		}

		return return_str;
	}

	static public void getChildGameObjectRecursive(Transform t , onFindGameObject del){
		del(t.gameObject);
		for (int i = 0 ; i< t.childCount ; i++){
			getChildGameObjectRecursive( t.GetChild(i) , del) ;
		}
	}

	static public string stringFromVector3(Vector3 vec){
		return vec.x + "|" + vec.y + "|" + vec.z ;
	}
	static public string stringFromQuaternion(Quaternion qua){
		return stringFromVector3 (qua.eulerAngles);
	}
	static public Vector3 vector3FromString(string str){
		string[] strs = str.Split ("|".ToCharArray (), System.StringSplitOptions.None);
		return new Vector3 ( float.Parse(strs[0]) ,  float.Parse(strs[1]) , float.Parse(strs[2]) );
	}
	static public Quaternion quaternionFromString(string str){
		return Quaternion.Euler (vector3FromString(str));
	}
	static public Vector2 vectorMultiply(Vector2 vec1 , Vector2 vec2){
		return new Vector2 (vec1.x * vec2.x , vec1.y * vec2.y);
	}
	
	private static ScreenUtil makeNewScreenUtilObj(){
		GameObject g = new GameObject ();
		ScreenUtil s = g.AddComponent<ScreenUtil> ();
		GameObject gc = GameObject.Find ("ScreenUtils");
		if (gc == null){
			gc = new GameObject ();
			gc.name = "ScreenUtils";
		}
		g.transform.parent = gc.transform;
		return s;
	}
	private static ScreenUtil getScreenUtilFor(GameObject target,int mode , float delay){
		GameObject gc = GameObject.Find ("ScreenUtils");
		if (gc == null){
			return null;
		}
		for (int i = 0 ; i< gc.transform.childCount ; i++){
			ScreenUtil s = gc.transform.GetChild(i).GetComponent<ScreenUtil>();
			if ( s != null ){
				if (s.TargetObj == target && s.mode == mode 
				    && Time.realtimeSinceStartup + delay < s.endTime
				    && Time.realtimeSinceStartup + delay > s.startTime){
					return s;
				}
			}
		}
		return null;
	}

	public static void getPicture(){
	}
	
	public static bool isInvalidValue( Quaternion vec){
		return isInvalidValue (vec.x) || isInvalidValue (vec.y) || isInvalidValue (vec.z)|| isInvalidValue (vec.w);
	}
	public static bool isInvalidValue( Vector3 vec){
		return isInvalidValue (vec.x) || isInvalidValue (vec.y) || isInvalidValue (vec.z);
	}
	public static bool isInvalidValue( float value ){
		return float.IsInfinity (value) || float.IsNaN (value);
	}
	public static string loadText(string assetName){
		try{
			return ( ((TextAsset)Instantiate(Resources.Load(assetName))).text);
		}catch{
			Debug.LogError("[ ScreenUtil.loadText() ] "+assetName+" is not found.");
			return "";
		}
	}

	public static string dateConvert(DateTime time){
		return time.ToString("yyyy-MM-dd HH:mm:ss");
	}
	public static DateTime dateConvert(string time){
		return DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss", null);
	}
	public static void setChildColiderEnabled(GameObject parent,bool enabled){
		foreach(GameObject g in ScreenUtil.getChildren(parent,new ArrayList())){
			if (g.GetComponent<BoxCollider>() != null){
				g.GetComponent<BoxCollider>().enabled = enabled;
			}
		}
	}
	public static string toBigNumber(string str) {
		
		string[] num = { "０","１","２","３","４","５","６","７","８","９" }; 
		
		if ( str.Equals("") ) {
			return "";
		}
		
		// get head and tail strings.
		string head = str.Substring(0, 1);
		string tail = str.Substring(1, str.Length-1);
		string res = "";
		
		try {
			res = num[int.Parse(head)];
		} catch (FormatException) {
			res = head;
		}
		
		
		return res + toBigNumber(tail);
		
	}

	public static GameObject[] FindRootObject (){
		return Array.FindAll (GameObject.FindObjectsOfType<GameObject> (), (item) => item.transform.parent == null);
	}

	
	public static void destroyAndReleaseAll (){
		GameObject[] games = GameObject.FindObjectsOfType<GameObject> ();
		for (int i = games.Length -1 ; i != 0 ; i-- ){
			if (games[i].GetComponent<MeshRenderer>() != null
			    ||games[i].GetComponent<SkinnedMeshRenderer>() != null
			    ||games[i].GetComponent<UIWidget>() != null){
				Destroy(games[i]);
			}
		}
		GameObject g = NGUITools.AddChild (GameObject.Find ("UI Root"), Instantiate (Resources.Load ("loadingPrehab")));
		Resources.UnloadUnusedAssets ();
	}

	
	
	public static ArrayList devideDatas(string raw , string marker){
		string[] rows = sepalateByEnter (raw);
		int from = 0;
		ArrayList mutableArray = new ArrayList ();
		for ( int i=0; i<rows.Length ; i++){
			if (rows[i].StartsWith(marker) || i == rows.Length -1){
				string[] arr = arrayFromRange(rows,from,i == rows.Length -1?i:i-1);
				mutableArray.Add ( ScreenUtil.Join(arr,Environment.NewLine) );
				from = i+1;
			}
		}
		return mutableArray;
	}
	public static string[] sepalateByEnter(string raw){
		return raw.Split(new string[]{Environment.NewLine}, StringSplitOptions.None);
	}
	public static string[] arrayFromRange(string[] arr , int from , int to){
		string[] res = new string[ to - from + 1]; 
		for (int i = from ; i<=to ; i++ ){
			res[i-from] = arr[i];
		}
		return res;
	}

	public static Dictionary<string,string> makeDictionary(string[] keys , string[] vals){
		Dictionary<string,string> d = new Dictionary<string,string> ();
		for(int i = 0 ; i< keys.Length ; i++){
			d.Add(keys[i] , vals[i]);
		}
		return d;
	}

}
