using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    [RequireComponent(typeof(LineRenderer))]
    public class RutaVisual : MonoBehaviour
    {
        [Header("Control de velocidad automática")]
        public bool modoCapturaActivo = false;
        [Range(0.1f, 1.5f)] public float velocidadMinima = 0.5f;
        [Range(0.1f, 3f)] public float velocidadMaxima = 2f;
        [Range(1, 5)] public int cantidadPuntosDeCaptura = 1;

        public Transform[] puntosRuta;
        [Range(0.1f, 3f)]
        public float[] modificadoresVelocidad;

        [Header("Marcadores visuales de puntos de captura")]
        public GameObject[] marcadoresPool; // Pool de objetos que se activarán 1m encima de los puntos

        private LineRenderer lineRenderer;

        [HideInInspector]
        public List<int> puntosCaptura = new List<int>();

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            InicializarModificadores();
            OcultarMarcadores();
        }

        void Update()
        {
            DibujarRuta();

            if (modoCapturaActivo)
            {
                AplicarPuntosDeCaptura();
                modoCapturaActivo = false; // Solo lo ejecuta una vez por activación
            }
        }

        void DibujarRuta()
        {
            if (puntosRuta.Length < 2) return;

            if (lineRenderer.positionCount != puntosRuta.Length)
            {
                lineRenderer.positionCount = puntosRuta.Length;
            }

            for (int i = 0; i < puntosRuta.Length; i++)
            {
                if (puntosRuta[i] != null)
                    lineRenderer.SetPosition(i, puntosRuta[i].position);
            }
        }

        void InicializarModificadores()
        {
            if (modificadoresVelocidad == null || modificadoresVelocidad.Length != puntosRuta.Length)
            {
                modificadoresVelocidad = new float[puntosRuta.Length];
            }

            for (int i = 0; i < puntosRuta.Length; i++)
            {
                modificadoresVelocidad[i] = 1f; // velocidad normal por defecto
            }
        }

        public void AplicarPuntosDeCaptura()
        {
            if (puntosRuta.Length == 0 || modificadoresVelocidad.Length != puntosRuta.Length)
                InicializarModificadores();

            // Resetear velocidades
            for (int i = 0; i < modificadoresVelocidad.Length; i++)
            {
                modificadoresVelocidad[i] = velocidadMaxima;
            }

        List<int> posiblesIndices = Enumerable.Range(0, puntosRuta.Length).ToList();
        List<int> seleccionados = new List<int>();
        int intentosMaximos = 100;
        int intentos = 0;

        while (seleccionados.Count < cantidadPuntosDeCaptura && intentos < intentosMaximos)
        {
            intentos++;

            int randIndex = Random.Range(0, puntosRuta.Length);

            // Si ya fue seleccionado, ignóralo
            if (seleccionados.Contains(randIndex))
                continue;

            seleccionados.Add(randIndex);
        }


        puntosCaptura = new List<int>(seleccionados);

            // Aplicar velocidad mínima
            foreach (int index in seleccionados)
            {
                modificadoresVelocidad[index] = velocidadMinima;
            }

            MostrarMarcadoresEnPuntos(seleccionados);
            Debug.Log("Puntos de captura aplicados: " + string.Join(", ", seleccionados));
        }

    void MostrarMarcadoresEnPuntos(List<int> indices)
    {
        OcultarMarcadores();

        if (marcadoresPool.Length < indices.Count)
            Debug.LogWarning("Pool de marcadores insuficiente para mostrar todos los puntos de captura.");

        for (int i = 0; i < Mathf.Min(indices.Count, marcadoresPool.Length); i++)
        {
            int puntoIndex = Mathf.Clamp(indices[i] +1, 0, puntosRuta.Length - 1);
            

            if (puntosRuta[puntoIndex] != null)
            {
                GameObject marcador = marcadoresPool[i];
                marcador.transform.position = puntosRuta[puntoIndex].position + Vector3.up;
                marcador.SetActive(true);
            }
        }
    }

    void OcultarMarcadores()
        {
            foreach (GameObject marcador in marcadoresPool)
            {
                if (marcador != null)
                    marcador.SetActive(false);
            }
        }
    }
