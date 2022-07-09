using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticTest
{
    public static bool IsStop = true;
    public static int i = 0;
    public static int j = 0;

    public static void Log()
    {
        Debug.LogError($"i渲染帧: {i}     j逻辑帧:{j}");
    }
}
