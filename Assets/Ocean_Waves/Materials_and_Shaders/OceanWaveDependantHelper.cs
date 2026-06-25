using UnityEngine;

public class OceanWaveDependantHelper : MonoBehaviour
{
    private void Start()
    {
        if(!gameObject.CompareTag("OceanDependant"))
        {
            gameObject.tag = "OceanDependant";
        }
    }
    public void UpdateWaveTiles(Vector4 _wave_base,Vector4 _wave_variance)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Renderer>().material.SetVector("_Wave_1_Properties", _wave_base);
            child.gameObject.GetComponent<Renderer>().material.SetVector("_Wave_2_Properties", _wave_variance);
            child.gameObject.GetComponent<Renderer>().material.SetVector("_WaveObject_Scale", transform.lossyScale);
        }
    }
}
