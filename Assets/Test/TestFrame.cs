using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFrame : MonoBehaviour
{
    private float testTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        StaticTest.IsStop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!StaticTest.IsStop)
        {
            testTime += Time.deltaTime;
            StaticTest.j++;
        }

        if (testTime >= 1)
        {
            StaticTest.IsStop = true;
            StaticTest.Log();
        }

    }
}
