using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ZoomBlurSettings : VolumeComponent,IPostProcessComponent
{

    [Range(0,100f),Tooltip("加强效果")]
    public FloatParameter FocusPower = new FloatParameter(0f);

    [Range(0,10f),Tooltip("越大越好，但是负载增加")]
    public IntParameter FocusDetials = new IntParameter(5);

    [Tooltip("沃湖中心坐标已经在屏幕中心")]
    public Vector2Parameter FocusScreenPosition = new Vector2Parameter(Vector2.zero);

    [Tooltip("参考昆都分辨率")]
    public IntParameter ReferenceResolutionX = new IntParameter(1334);

    public bool IsActive()
    {
        return false;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}
