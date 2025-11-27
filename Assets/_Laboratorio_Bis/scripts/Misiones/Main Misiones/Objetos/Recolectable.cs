using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Recolectable : MonoBehaviour
{
    [Header("Objeto asociado (ScriptableObject)")]
    public ObjetoRecoleccion objetoAsociado;

    [Header("Material de fresnel (en el segundo slot del MeshRenderer)")]
    public MeshRenderer meshRenderer;
    public int indexMaterialFresnel = 1;

    [Header("Parametros de fresnel")]
    public float fresnelMax = 3.5f;
    public float fresnelMin = 0f;

    public float opacidadMax = 4.3f;
    public float opacidadMin = 0f;

    public float velocidadTransicion = 2f;

    private Material fresnelMaterial;

    private bool jugadorCerca = false;
    private bool interactuable = false;

    private void Start()
    {
        // Obtener el material fresnel desde el renderer
        if (meshRenderer != null)
        {
            fresnelMaterial = meshRenderer.materials[indexMaterialFresnel];

            // Iniciarlo apagado
            fresnelMaterial.SetFloat("_Escala_Fresnel", fresnelMin);
            fresnelMaterial.SetFloat("_Opacidad", opacidadMin);
        }
    }

    private void Update()
    {

        // Si NO es fase de recolección → apagar fresnel y no hacer nada
        if (!FaseActualEsRecoleccion())
        {
            if (fresnelMaterial != null)
            {
                float f = fresnelMaterial.GetFloat("_FresnelScale");
                float o = fresnelMaterial.GetFloat("_Opacity");

                fresnelMaterial.SetFloat("_FresnelScale",
                    Mathf.Lerp(f, fresnelMin, Time.deltaTime * velocidadTransicion));

                fresnelMaterial.SetFloat("_Opacity",
                    Mathf.Lerp(o, opacidadMin, Time.deltaTime * velocidadTransicion));
            }

            return; // <-- PROTECCIÓN CLAVE
        }

        if (GestorMisiones.instancia.ObjetivoDeRecoleccionYaCompleto(objetoAsociado))
        {
            // Apagar fresnel suavemente
            fresnelMaterial.SetFloat("_Escala_Fresnel",
                Mathf.Lerp(fresnelMaterial.GetFloat("_Escala_Fresnel"), fresnelMin, Time.deltaTime * velocidadTransicion));

            fresnelMaterial.SetFloat("_Opacidad",
                Mathf.Lerp(fresnelMaterial.GetFloat("_Opacidad"), opacidadMin, Time.deltaTime * velocidadTransicion));

            return;
        }
        // Animación de acercar/alejar
        if (fresnelMaterial != null)
        {
            // Si no es parte de la misión → fresnel apagado siempre
            if (!EsParteDeLaMisionActual())
            {
                fresnelMaterial.SetFloat("_Escala_Fresnel", Mathf.Lerp(fresnelMaterial.GetFloat("_Escala_Fresnel"), fresnelMin, Time.deltaTime * velocidadTransicion));
                fresnelMaterial.SetFloat("_Opacidad", Mathf.Lerp(fresnelMaterial.GetFloat("_Opacidad"), opacidadMin, Time.deltaTime * velocidadTransicion));
                return;
            }

            float objetivoFresnel = jugadorCerca ? fresnelMax : fresnelMin;
            float objetivoOpacidad = jugadorCerca ? opacidadMax : opacidadMin;

            fresnelMaterial.SetFloat("_Escala_Fresnel", Mathf.Lerp(fresnelMaterial.GetFloat("_Escala_Fresnel"), objetivoFresnel, Time.deltaTime * velocidadTransicion));
            fresnelMaterial.SetFloat("_Opacidad", Mathf.Lerp(fresnelMaterial.GetFloat("_Opacidad"), objetivoOpacidad, Time.deltaTime * velocidadTransicion));
        }

        // Si el jugador está cerca y presiona E → intentar recolectar
        if (jugadorCerca
     && Input.GetKeyDown(KeyCode.E)
     && EsParteDeLaMisionActual()
     && !GestorMisiones.instancia.ObjetivoDeRecoleccionYaCompleto(objetoAsociado))
        {
            IntentarRecolectar();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!EsParteDeLaMisionActual()) return;
        if (GestorMisiones.instancia.ObjetivoDeRecoleccionYaCompleto(objetoAsociado)) return;


        jugadorCerca = true;

        UI_Recoleccion.instancia.MostrarMensaje("Presiona E para recolectar " + objetoAsociado.nombreObjeto);
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorCerca = false;

        // Ocultar UI
        UI_Recoleccion.instancia.OcultarMensaje();
    }

    private void IntentarRecolectar()
    {
        // Validar si estamos en fase de recolección Y si este objeto es parte de los objetivos
        bool aceptado = GestorMisiones.instancia.RecogerObjeto(objetoAsociado);

        if (aceptado)
        {
            UI_Recoleccion.instancia.OcultarMensaje(); // <--- agregamos esto
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Este objeto no es parte de la misión actual.");
        }
    }

    private bool EsParteDeLaMisionActual()
    {
        return GestorMisiones.instancia.ObjetoEsParteDeRecoleccion(objetoAsociado);
    }

    private bool FaseActualEsRecoleccion()
    {
        return GestorMisiones.instancia != null &&
               GestorMisiones.instancia.FaseActualEsRecoleccion();
    }

}
