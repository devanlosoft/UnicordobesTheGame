using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ubicacion1 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.transform.position = new Vector3(15.5f,45f,-0.1510144f);
    }
}
