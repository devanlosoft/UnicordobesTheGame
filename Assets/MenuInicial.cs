using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    // Start is called before the first frame update
    public void Jugar()
    {
        SceneManager.LoadScene(2);
    }

    public void Salir()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
