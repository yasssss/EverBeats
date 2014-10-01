using UnityEngine;
using System.Collections;

public class GUIBehaviour2 : MonoBehaviour
{
  public Material skybox, skyboxNight;
  public Light dirLight;
  public GameObject nightLighting;
  private bool isDay = true;
  private float oldIntensityLight;
	// Use this for initialization
	void Start ()
	{
	  oldIntensityLight = dirLight.intensity;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  private void OnGUI()
  {
    if (GUI.Button(new Rect(10, 10, 150, 25), "Day/Night"))
    {
      if (isDay) {
        RenderSettings.skybox = skyboxNight;
        RenderSettings.ambientLight = new Color(0,0,0,1);
        dirLight.intensity = 0.03f;
        nightLighting.SetActive(true);
      }
      else {
        RenderSettings.skybox = skybox;
        RenderSettings.ambientLight = new Color(50/255f, 50/255f, 50/255f, 1);
        dirLight.intensity = oldIntensityLight;
        nightLighting.SetActive(false);
      }
      isDay = !isDay;
    }
    if (GUI.Button(new Rect(10, 50, 150, 25), "Scene1"))
    {
      Application.LoadLevel("Scene1");
    }
  }
}
