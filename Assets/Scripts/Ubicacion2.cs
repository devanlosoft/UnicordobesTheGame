using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ubicacion2 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.transform.position = new Vector3(15.19f,13.27f,0.2460514f);
    }
}