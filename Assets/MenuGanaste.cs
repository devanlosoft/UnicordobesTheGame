using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGanaste : MonoBehaviour
{

    // Start is called before the first frame update
    public void Jugar()
    {

        SceneManager.LoadScene(1);
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