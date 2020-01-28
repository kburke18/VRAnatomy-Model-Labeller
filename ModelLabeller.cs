using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // always need UnityEditor for creating Unity tools

public class ModelLabeller : ScriptableWizard
{
    public string[] labelNames;
    public GameObject FBXObjectWithPoints;
    private Material mat;
    private GameObject labelPrefab;
    private List<Label> labels;

    [MenuItem("Organ Tools/Label Model...")]
    static void AddLabel()
    {
        ScriptableWizard.DisplayWizard<ModelLabeller>("Label Model...", "Make Labels"); 
    }

    /// <summary>
    /// The UnityEditor function that runs when the createButton (in this instance "Make Labels") is pressed
    /// </summary>
    void OnWizardCreate()
    {
        mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Line.mat", typeof(Material));
        labelPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Label.prefab", typeof(GameObject));
        labels = new List<Label>();

        if (Selection.activeTransform != null)
        {
            GameObject selectedOrgan = Selection.activeTransform.gameObject;
            for (int i = 0; i < labelNames.Length; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));

                GameObject newLabel = Instantiate(labelPrefab, selectedOrgan.transform.position, Quaternion.identity);
                newLabel.gameObject.name = "Label " + labelNames[i];
                newLabel.transform.SetParent(selectedOrgan.transform);
                newLabel.transform.position = randomPos;

                Label label = newLabel.GetComponent<Label>();
                labels.Add(label);
                label.label = newLabel.GetComponent<TMPro.TextMeshPro>();
                newLabel.GetComponent<Label>().mat = mat;
                label.label.text = labelNames[i];
            }
            // Find the FBX child
            GameObject model;
            foreach (Transform child in selectedOrgan.transform)
            {
                if (child.gameObject.name.Contains("FBX"))
                {
                    model = child.gameObject;
                    for (int i = 0; i < labels.Count; i++)
                    {
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.transform.SetParent(model.transform);
                        sphere.transform.position = Vector3.zero;
                        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        sphere.gameObject.name = "Point " + labels[i].name;
                        labels[i].point = sphere.transform;
                    }
                }
                break;
            }  
        }
    }

    void OnWizardUpdate()
    {
        helpString = "Creates text labels and point of interest on FBX model. Must position points of interest manually.";
    }
}
