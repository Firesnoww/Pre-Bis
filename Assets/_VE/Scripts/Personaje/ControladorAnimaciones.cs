using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ControladorAnimaciones : MonoBehaviour
{
    Animator animator;
    public Vector3 posicionAnterior;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        posicionAnterior = transform.position;
        StartCoroutine(Actualizador());
    }

    IEnumerator Actualizador()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            animator.SetFloat("Velocidad", (transform.position - posicionAnterior).magnitude * 50);
            posicionAnterior = transform.position;
        }
    }

}
