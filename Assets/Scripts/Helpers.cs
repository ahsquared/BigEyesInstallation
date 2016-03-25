using UnityEngine;
using System.Collections;
using System.Linq;

public class Helpers : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Show(GameObject go)
    {
        go.GetComponent<Renderer>().enabled = true;
    }
    void Hide(GameObject go)
    {
        go.GetComponent<Renderer>().enabled = false;
    }
    void HideChildren(GameObject go)
    {
        Renderer[] lChildRenderers = go.GetComponentsInChildren<Renderer>();
        foreach (Renderer lRenderer in lChildRenderers)
        {
            lRenderer.enabled = false;
        }
        go.GetComponent<Renderer>().enabled = false;
    }
    void ShowChildren(GameObject go)
    {
        Renderer[] lChildRenderers = go.GetComponentsInChildren<Renderer>();
        foreach (Renderer lRenderer in lChildRenderers)
        {
            lRenderer.enabled = true;
        }
        go.GetComponent<Renderer>().enabled = true;
    }

    public static Transform GetChildTransformByName(Transform objectTransform, string name)
    {
        return objectTransform.Cast<Transform>().FirstOrDefault(t => t.gameObject.name == name);
    }
    public static GameObject GetChildGameObjectByName(GameObject go, string name)
    {
        Transform[] children = go.GetComponentsInChildren<Transform>();
        foreach (Transform t in children)
        {
            if (t.gameObject.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }
}
