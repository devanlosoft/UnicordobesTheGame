using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablero : MonoBehaviour
{

    private void OnCollisionEnter2D(Collider2D other)
    {
        if (other.GetComponent<Collider>().CompareTag("Player"))
        {
            Debug.Log("Colision con el jugador");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
