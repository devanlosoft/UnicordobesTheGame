using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class portada : MonoBehaviour
{

    public float wait_time = 6f;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(wait_for_intro());
    }

    IEnumerator wait_for_intro()
    {
        yield return new WaitForSeconds(wait_time);
        SceneManager.LoadScene(1);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
