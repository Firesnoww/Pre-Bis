using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAnimacion : MonoBehaviour
{
    public Animator animator; // Asigna el Animator desde el Inspector
    private bool estadoActual = false; // Estado interno del bool

    public void CambiarEstado()
    {
        estadoActual = !estadoActual; // Alternar el valor
        animator.SetBool("Entro", estadoActual); // Enviar el bool al Animator
    }
}
