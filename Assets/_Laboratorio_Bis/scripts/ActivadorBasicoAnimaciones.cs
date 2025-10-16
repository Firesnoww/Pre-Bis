using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivadorBasicoAnimaciones : MonoBehaviour
{
    
        public Animator animator; // arrastra aquí el Animator en el inspector
        private bool isOn = false;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Toggle()
    {
            isOn = !isOn;
            animator.SetBool("On", isOn);
    }
    public void Cerrar() 
    {
        animator.SetBool("On", false);
    }
   
}
