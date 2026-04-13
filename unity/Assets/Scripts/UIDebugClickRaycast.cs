using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDebugClickRaycast : MonoBehaviour
{
    [Header("Debug Options")]
    public bool logOnClick = true;
    public int maxResultsToLog = 8;

    private readonly List<RaycastResult> _results = new List<RaycastResult>();

    private void Update()
    {
        if (!logOnClick)
        {
            return;
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        EventSystem es = EventSystem.current;
        if (es == null)
        {
            Debug.LogWarning("UIDebugClickRaycast: aucun EventSystem actif.");
            return;
        }

        PointerEventData ped = new PointerEventData(es)
        {
            position = Input.mousePosition
        };

        _results.Clear();
        es.RaycastAll(ped, _results);

        if (_results.Count == 0)
        {
            Debug.Log("UIDebugClickRaycast: aucun hit UI au clic.");
            return;
        }

        int count = Mathf.Min(maxResultsToLog, _results.Count);
        for (int i = 0; i < count; i++)
        {
            RaycastResult r = _results[i];
            string raycaster = r.module != null ? r.module.GetType().Name : "null";
            string name = r.gameObject != null ? r.gameObject.name : "null";
            Debug.Log($"UIDebugClickRaycast: hit #{i + 1} => {name} (raycaster: {raycaster})");
        }
    }
}
