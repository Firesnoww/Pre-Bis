using UnityEngine;
using UnityEditor;

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

        // Datos generales
        showDatosGenerales = EditorGUILayout.Foldout(showDatosGenerales, "Datos de la Misi�n", true);
        if (showDatosGenerales)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InicioMision"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuMision"));      
            EditorGUILayout.PropertyField(serializedObject.FindProperty("NombreMision"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoMision"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Dificultad"));
        }

        // Captura
        showCaptura = EditorGUILayout.Foldout(showCaptura, "Captura de Lib�lulas", true);
        if (showCaptura)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EsCaptura"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuLibelulas"));   
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoCaptura"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LibelulaLow"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LibelulaHigt"), true);
        }

        // Recolecci�n
        showRecoleccion = EditorGUILayout.Foldout(showRecoleccion, "Recolecci�n", true);
        if (showRecoleccion)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EsRecoleccion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuRecoleccion")); 
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoRecoleccion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Muestra"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Cantidad"), true);
        }

        // Exploraci�n
        showExploracion = EditorGUILayout.Foldout(showExploracion, "Exploraci�n", true);
        if (showExploracion)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EsExploracion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuExploracion"));  
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoExploracion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Lugar"), true);
        }

        // Microscopio
        showMicroscopio = EditorGUILayout.Foldout(showMicroscopio, "Microscopio", true);
        if (showMicroscopio)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EsMicroscopio"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuMicroscopio"));  
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoMicroscopio"));
        }

        // Resultado final
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