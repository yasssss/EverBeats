using UnityEngine;
using System;
 
// Attach this script to an object that uses a Reflective shader.
// Realtime reflective cubemaps!
[ExecuteInEditMode()]
internal class RenderCubemapReflection : MonoBehaviour
{
  public int cubemapSize = 128;
  public float nearClip = 0.01f;
  public float farClip = 500;
  public bool oneFacePerFrame = false;
  public LayerMask layerMask;
  private Camera cam;
  private RenderTexture rtex;

  private void Start()
  {
    // render all six faces at startup
    UpdateCubemap(63);
  }

  private void LateUpdate()
  {
    if (oneFacePerFrame) {
      int faceToRender = Time.frameCount % 6;
      int faceMask = 1 << faceToRender;
      UpdateCubemap(faceMask);
    }
    else {
      UpdateCubemap(63); // all six faces
    }
  }

  private void UpdateCubemap(int faceMask)
  {
    if (!cam) {
      GameObject go = new GameObject("CubemapCamera", typeof (Camera));
      go.hideFlags = HideFlags.HideAndDontSave;
      go.transform.position = transform.position;
      go.transform.rotation = Quaternion.identity;
      cam = go.camera;
      cam.cullingMask = layerMask;
      cam.nearClipPlane = nearClip;
      cam.farClipPlane = farClip;
      cam.enabled = false;
    }

    if (!rtex) {
      rtex = new RenderTexture(cubemapSize, cubemapSize, 16);
      rtex.isPowerOfTwo = true;
      rtex.isCubemap = true;
      rtex.hideFlags = HideFlags.HideAndDontSave;
      foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
        foreach (Material m in r.sharedMaterials) {
          if (m.HasProperty("_Cube")) {
            m.SetTexture("_Cube", rtex);
          }
        }
      }
    }

    cam.transform.position = transform.position;
    cam.RenderToCubemap(rtex, faceMask);
  }

  //private void OnDisable()
  //{
  //  DestroyImmediate(cam);
  //  DestroyImmediate(rtex);
  //}
}