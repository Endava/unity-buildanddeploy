using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
#if CUSTOM_BUILD
        gameObject.transform.Rotate(Vector3.down, 0.25f);
#endif
    }
}
