using UnityEngine;
using UnityEditor;

//
// MisionesEditor
// Inspector simple y organizado para el ScriptableObject "Misiones".
// Muestra los bloques originales y a�ade:
//  - Objetivos simples (opcionales) por tipo de fase
//  - Objetivos avanzados por ID (opcionales) para Recolecci�n y Captura
//
[CustomEditor(typeof(Misiones))]
public class MisionesEditor : Editor
{
    private bool showDatosGenerales = true;
    private bool showCaptura = true;
    private bool showRecoleccion = true;
    private bool showExploracion = true;
    private bool showMicroscopio = true;
    private bool showResultado = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ========================= Datos de la Misi�n =========================
        showDatosGenerales = EditorGUILayout.Foldout(showDatosGenerales, "Datos de la Misi�n", true);
        if (showDatosGenerales)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InicioMision"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuMision"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("NombreMision"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoMision"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Dificultad"));
        }

        // ========================= Captura de Lib�lulas =========================
        showCaptura = EditorGUILayout.Foldout(showCaptura, "Captura de Lib�lulas", true);
        if (showCaptura)
        {
            var pEsCaptura = serializedObject.FindProperty("EsCaptura");
            EditorGUILayout.PropertyField(pEsCaptura);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuLibelulas"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoCaptura"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LibelulaLow"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LibelulaHigt"), true);

            if (pEsCaptura.boolValue)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Objetivos (elige uno de los dos enfoques)", EditorStyles.boldLabel);

                // Objetivo simple
                EditorGUILayout.PropertyField(serializedObject.FindProperty("CapturasObjetivo"));

                // Avanzado por ID (opcional)
                EditorGUILayout.LabelField("Avanzado por ID (opcional)", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("CapturaIDs"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("CapturaCant"), true);
            }
        }

        // ========================= Recolecci�n =========================
        showRecoleccion = EditorGUILayout.Foldout(showRecoleccion, "Recolecci�n", true);
        if (showRecoleccion)
        {
            var pEsRecoleccion = serializedObject.FindProperty("EsRecoleccion");
            EditorGUILayout.PropertyField(pEsRecoleccion);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuRecoleccion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoRecoleccion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Muestra"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Cantidad"), true);

            if (pEsRecoleccion.boolValue)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Objetivos (elige uno de los dos enfoques)", EditorStyles.boldLabel);

                // Objetivo simple
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RecoleccionesObjetivo"));

                // Avanzado por ID (opcional)
                EditorGUILayout.LabelField("Avanzado por ID (opcional)", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RecoleccionIDs"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RecoleccionCant"), true);
            }
        }

        // ========================= Exploraci�n =========================
        showExploracion = EditorGUILayout.Foldout(showExploracion, "Exploraci�n", true);
        if (showExploracion)
        {
            var pEsExploracion = serializedObject.FindProperty("EsExploracion");
            EditorGUILayout.PropertyField(pEsExploracion);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuExploracion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoExploracion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Lugar"), true);

            if (pEsExploracion.boolValue)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Objetivo simple", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ZonasObjetivo"));
            }
        }

        // ========================= Microscopio =========================
        showMicroscopio = EditorGUILayout.Foldout(showMicroscopio, "Microscopio", true);
        if (showMicroscopio)
        {
            var pEsMicroscopio = serializedObject.FindProperty("EsMicroscopio");
            EditorGUILayout.PropertyField(pEsMicroscopio);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuMicroscopio"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoMicroscopio"));

            if (pEsMicroscopio.boolValue)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Objetivo simple", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AnalisisObjetivo"));
            }
        }

        // ========================= Resultado Final =========================
        showResultado = EditorGUILayout.Foldout(showResultado, "Resultado Final", true);
        if (showResultado)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EsResultado"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuFinal"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoFinal"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoDesbloqueada"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
