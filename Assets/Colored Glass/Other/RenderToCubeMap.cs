using UnityEngine;
using System.Collections;

public class RenderToCubeMap : MonoBehaviour {

  public Material mat;
  int cubemapSize = 512;
  private Camera cam;
	// Use this for initialization
	void Start ()
	{
	  cam = GetComponent<Camera>();
    var rtex = new RenderTexture(cubemapSize, cubemapSize, 16);
    rtex.isCubemap = true;
    rtex.hideFlags = HideFlags.HideAndDontSave;
    mat.SetTexture("_Cube", rtex);
    cam.RenderToCubemap(rtex, 0);
	}
	
	// Update is called once per frame
	void Update () {
   
	}
}
