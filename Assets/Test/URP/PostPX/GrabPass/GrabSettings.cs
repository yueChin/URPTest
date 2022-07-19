using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class GrabSettings 
{
    public RenderPassEvent Event = RenderPassEvent.AfterRenderingTransparents;
}
