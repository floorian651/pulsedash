using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class CreateObject : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objetsAPrefabriquer = new List<GameObject>();

    void Start() {
        Create();
    }

    void Create() {
        float positionOffset = 0;
        for (int i = 0; i < objetsAPrefabriquer.Count; i++) {
            Vector3 taille = objetsAPrefabriquer[i].GetComponent<Renderer>().bounds.size;
            Instantiate(
                objetsAPrefabriquer[i],
                new Vector3(0, taille.y/2, positionOffset),
                Quaternion.identity
            );
            positionOffset += taille.z;
        }
    }

    void Update() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) {
            // Create();
        }
    }
}