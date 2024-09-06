using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pergaminoEliminar : MonoBehaviour
{
    public int codigo;
    // Start is called before the first frame update
    public void Eliminar()
    {
        GameObject objeto = GameObject.FindGameObjectWithTag("per" + codigo);
        if (objeto != null)
            objeto.SetActive(false);

    }

    // Update is called once per frame
    
}
