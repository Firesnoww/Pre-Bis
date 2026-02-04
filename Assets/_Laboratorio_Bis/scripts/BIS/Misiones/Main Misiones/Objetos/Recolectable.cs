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
        if (meshRenderer == null)
        {
            Debug.LogError($"[Recolectable] {gameObject.name} no tiene MeshRenderer asignado.");
            return;
        }

        var mats = meshRenderer.materials;

        // Buscamos automáticamente el material que tenga el shader del fresnel
        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i].shader.name.Contains("Fresnel") ||
                mats[i].name.Contains("Fresnel") ||
                mats[i].name.Contains("Recoleccion"))
            {
                fresnelMaterial = mats[i];
                break;
            }
        }

        if (fresnelMaterial == null)
        {
            Debug.LogWarning($"[Recolectable] {gameObject.name} no tiene material Fresnel. Asignando material por índice {indexMaterialFresnel} (si existe).");

            if (mats.Length > indexMaterialFresnel)
                fresnelMaterial = mats[indexMaterialFresnel];
        }

        if (fresnelMaterial != null)
        {
            fresnelMaterial.SetFloat("_Escala_Fresnel", fresnelMin);
            fresnelMaterial.SetFloat("_Opacidad", opacidadMin);
        }
        else
        {
            Debug.LogError($"[Recolectable] No se pudo asignar material Fresnel en {gameObject.name}. Fresnel desactivado.");
        }
    }


    private void Update()
    {
        // 1. Si no hay fresnel, no hacemos efectos visuales pero seguimos dejando recolección funcionando
        bool tieneFresnel = fresnelMaterial != null;

        // 2. Si no es fase de recolección
        if (!FaseActualEsRecoleccion())
        {
            if (tieneFresnel)
            {
                fresnelMaterial.SetFloat("_Escala_Fresnel", Mathf.Lerp(
                    fresnelMaterial.GetFloat("_Escala_Fresnel"), fresnelMin, Time.deltaTime * velocidadTransicion));

                fresnelMaterial.SetFloat("_Opacidad", Mathf.Lerp(
                    fresnelMaterial.GetFloat("_Opacidad"), opacidadMin, Time.deltaTime * velocidadTransicion));
            }

            return;
        }

        // 3. Si el objetivo YA está completo
        if (GestorMisiones.instancia.ObjetivoDeRecoleccionYaCompleto(objetoAsociado))
        {
            if (tieneFresnel)
            {
                fresnelMaterial.SetFloat("_Escala_Fresnel", Mathf.Lerp(fresnelMaterial.GetFloat("_Escala_Fresnel"), fresnelMin, Time.deltaTime * velocidadTransicion));
                fresnelMaterial.SetFloat("_Opacidad", Mathf.Lerp(fresnelMaterial.GetFloat("_Opacidad"), opacidadMin, Time.deltaTime * velocidadTransicion));
            }
            return;
        }

        // 4. Si NO es parte de la misión activa
        if (!EsParteDeLaMisionActual())
        {
            if (tieneFresnel)
            {
                fresnelMaterial.SetFloat("_Escala_Fresnel", Mathf.Lerp(fresnelMaterial.GetFloat("_Escala_Fresnel"), fresnelMin, Time.deltaTime * velocidadTransicion));
                fresnelMaterial.SetFloat("_Opacidad", Mathf.Lerp(fresnelMaterial.GetFloat("_Opacidad"), opacidadMin, Time.deltaTime * velocidadTransicion));
            }
            return;
        }

        // 5. Si SÍ es parte de la misión
        if (tieneFresnel)
        {
            float objetivoFresnel = jugadorCerca ? fresnelMax : fresnelMin;
            float objetivoOpacidad = jugadorCerca ? opacidadMax : opacidadMin;

            fresnelMaterial.SetFloat("_Escala_Fresnel", Mathf.Lerp(fresnelMaterial.GetFloat("_Escala_Fresnel"), objetivoFresnel, Time.deltaTime * velocidadTransicion));
            fresnelMaterial.SetFloat("_Opacidad", Mathf.Lerp(fresnelMaterial.GetFloat("_Opacidad"), objetivoOpacidad, Time.deltaTime * velocidadTransicion));
        }

        // 6. Recolección
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
