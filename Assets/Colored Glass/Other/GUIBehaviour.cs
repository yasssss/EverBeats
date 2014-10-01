using UnityEngine;
using System.Collections;

public class GUIBehaviour : MonoBehaviour
{
  public Material skybox, skyboxNight, illumMat1, illumMat2, illumMat3, illumMat4, diffuseMat1, diffuseMat2, diffuseMat3, diffuseMat4;
  public Light dirLight;
  public GameObject akasha1, akasha2, akasha3, akasha4;
  public GameObject nightLighting;
  private bool isDay = true, isIllum;
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
    if (GUI.Button(new Rect(10, 50, 150, 25), "Illumination"))
    {
      if (!isIllum) {
        akasha1.renderer.material = illumMat1;
        akasha2.renderer.material = illumMat2;
        akasha3.renderer.material = illumMat3;
        akasha4.renderer.material = illumMat4;
      }
      else {
        akasha1.renderer.material = diffuseMat1;
        akasha2.renderer.material = diffuseMat2;
        akasha3.renderer.material = diffuseMat3;
        akasha4.renderer.material = diffuseMat4;
      }
      isIllum = !isIllum;
    }
    if (GUI.Button(new Rect(10, 90, 150, 25), "Scene2"))
    {
      Application.LoadLevel("Scene2");
    }
  }
}
