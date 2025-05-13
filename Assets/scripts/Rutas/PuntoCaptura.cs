using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuntoCaptura : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        LibelulaCapturable libelula = other.GetComponent<LibelulaCapturable>();
        if (libelula != null)
        {
            
            libelula.SetEnPuntoDeCaptura(true);
            Debug.Log("Entro a Captura");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        LibelulaCapturable libelula = other.GetComponent<LibelulaCapturable>();
        if (libelula != null)
        {
            libelula.SetEnPuntoDeCaptura(false);
            Debug.Log("salio de caputura");
        }
    }
}
