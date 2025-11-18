using System.Collections.Generic;
using UnityEngine;

public class MissionCaptureSpawner : MonoBehaviour
{
    [Header("Refs")]
    public MainMisiones main;   // arrástralo desde la escena

    // Estado interno
    private int _lastFaseIndex = -1;
    private string _lastTipo = "";
    private readonly List<GameObject> _spawned = new List<GameObject>();

    void OnEnable()
    {
        if (main != null)
            main.OnMissionUI += OnMissionUIChanged;
    }

    void OnDisable()
    {
        if (main != null)
            main.OnMissionUI -= OnMissionUIChanged;
    }

    private void OnMissionUIChanged(MainMisiones.MissionSnapshot s)
    {
        if (s == null) return;

        bool faseCambio = (s.faseIndex != _lastFaseIndex) || (s.tipo != _lastTipo);

        // Si cambiamos de fase/tipo y salimos de Captura, limpiar si corresponde
        if (faseCambio && _lastTipo == "Captura")
        {
            // ¿La fase anterior tenía la flag de despawn?
            var mPrev = TryGetMisionByIndex(_lastFaseIndex - 1); // snapshot es 1-based
            if (mPrev != null && mPrev.EsCaptura && mPrev.DespawnAlSalirDeCaptura)
                DespawnAll();
        }

        // Si entramos a Captura y es una fase nueva, spawn si corresponde
        if (s.activa && s.tipo == "Captura" && faseCambio)
        {
            var mCur = TryGetMisionByIndex(s.faseIndex - 1);
            if (mCur != null && mCur.EsCaptura && mCur.SpawnAlIniciarCaptura)
            {
                SpawnForMission(mCur);
            }
        }

        _lastFaseIndex = s.faseIndex;
        _lastTipo = s.tipo;
    }

    private Misiones TryGetMisionByIndex(int zeroBasedIndex)
    {
        if (main == null || main.misiones == null) return null;
        if (zeroBasedIndex < 0 || zeroBasedIndex >= main.misiones.Length) return null;
        return main.misiones[zeroBasedIndex];
    }

    private void SpawnForMission(Misiones m)
    {
        if (m == null) return;

        // 1) Resolver prefab
        GameObject prefab = m.PrefabDragonfly;
        if (prefab == null)
        {
            if (m.LibelulaLow != null && m.LibelulaLow.Length > 0 && m.LibelulaLow[0] != null)
                prefab = m.LibelulaLow[0];
            else if (m.LibelulaHigt != null && m.LibelulaHigt.Length > 0 && m.LibelulaHigt[0] != null)
                prefab = m.LibelulaHigt[0];
        }
        if (prefab == null)
        {
            Debug.LogWarning("[MissionCaptureSpawner] No hay prefab definido (PrefabDragonfly/LibelulaLow/High).");
            return;
        }

        // 2) Resolver punto de spawn
        Transform spawnTransform = ResolveSpawnPoint(m.SpawnPointId);
        if (spawnTransform == null)
        {
            Debug.LogWarning($"[MissionCaptureSpawner] No encontré DragonflySpawnPoint con Id='{m.SpawnPointId}'. Spawneo en (0,0,0).");
        }

        // 3) Instanciar N
        int n = Mathf.Max(1, m.SpawnCantidad);
        for (int i = 0; i < n; i++)
        {
            Vector3 pos = spawnTransform ? spawnTransform.position : Vector3.zero;
            Quaternion rot = spawnTransform ? spawnTransform.rotation : Quaternion.identity;

            // Pequeño jitter opcional (evita solapados si spawneas varios)
            pos += new Vector3(Random.Range(-0.4f, 0.4f), 0f, Random.Range(-0.4f, 0.4f));

            var go = Instantiate(prefab, pos, rot);
            AsegurarComponentesCaptura(go);
            _spawned.Add(go);
        }
    }

    private Transform ResolveSpawnPoint(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            // Puedes decidir: null (0,0,0) o usar tu propio transform
            return null;
        }

        var all = FindObjectsOfType<DragonflySpawnPoint>(true);
        foreach (var sp in all)
        {
            if (sp != null && sp.Id == id) return sp.transform;
        }
        return null;
    }

    private void AsegurarComponentesCaptura(GameObject go)
    {
        // Garantiza que la libélula tenga lo necesario para ser capturable
        if (go.GetComponent<DragonflyCapturable>() == null)
            go.AddComponent<DragonflyCapturable>();

        if (go.GetComponent<NotificadorCapturaEspecie>() == null)
            go.AddComponent<NotificadorCapturaEspecie>();
        // Nota: si tu Notificador necesita setear especieID/Main por prefab,
        // configúralo en el prefab. Aquí solo aseguramos que existan los comp.
    }

    private void DespawnAll()
    {
        for (int i = _spawned.Count - 1; i >= 0; i--)
        {
            if (_spawned[i] != null) Destroy(_spawned[i]);
        }
        _spawned.Clear();
    }
}
