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
        showDatosGenerales = EditorGUILayout.Foldout(showDatosGenerales, "Datos de la Misión", true);
        if (showDatosGenerales)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InicioMision"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuMision"));      
            EditorGUILayout.PropertyField(serializedObject.FindProperty("NombreMision"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoMision"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Dificultad"));
        }

        // Captura
        showCaptura = EditorGUILayout.Foldout(showCaptura, "Captura de Libélulas", true);
        if (showCaptura)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EsCaptura"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuLibelulas"));   
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoCaptura"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LibelulaLow"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LibelulaHigt"), true);
        }

        // Recolección
        showRecoleccion = EditorGUILayout.Foldout(showRecoleccion, "Recolección", true);
        if (showRecoleccion)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EsRecoleccion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AuRecoleccion")); 
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfoRecoleccion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Muestra"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Cantidad"), true);
        }

        // Exploración
        showExploracion = EditorGUILayout.Foldout(showExploracion, "Exploración", true);
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