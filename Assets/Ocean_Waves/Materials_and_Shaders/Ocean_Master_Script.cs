using JJ26.Framework;
using JJ26.Gameplay;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

//This handy script helps turn the shader graph info about the waves into useable functions!
// This makes funky ship movements

public struct _wavefloats
{
    public Vector3 _position;
    public Vector3 _tangeant;
    public Vector3 _normal;
}

public class Ocean_Master_Script : MonoBehaviour
{

    Material m_OceanMaterial;
    WaveSystem m_Wave;
    

    [Header("The base wave of the ocean")]
    public float Base_Wave_Length = 10f;
    public float Base_Wave_Height_Strength = 0.5f;
    public float Base_Wave_X_Direction_Strength = 1f;
    public float Base_Wave_Y_Direction_Strength = 1f;

    [Header("A wave that adds variance to the wave form")]
    public float Variance_Wave_Length = 0.5f;
    public float Variance_Wave_Height_Strength = 0.25f;
    public float Variance_Wave_X_Direction_Strength = 1f;
    public float Variance_Wave_Y_Direction_Strength = 1f;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Wave = FindAnyObjectByType<WaveSystem>();
        //m_OceanMaterial = gameObject.GetComponent<Renderer>().material;

        Vector4 _wave_base = new(Base_Wave_X_Direction_Strength, Base_Wave_Y_Direction_Strength, Base_Wave_Height_Strength, Base_Wave_Length);
        Vector4 _wave_variance = new(Variance_Wave_X_Direction_Strength, Variance_Wave_Y_Direction_Strength, Variance_Wave_Height_Strength, Variance_Wave_Length);
        
        //used to update the shader with the new wave parameters when this was attached to a tile
        //m_OceanMaterial.SetVector("_Wave_1_Properties", _wave_base);
        //m_OceanMaterial.SetVector("_Wave_2_Properties", _wave_variance);
        //m_OceanMaterial.SetVector("_WaveObject_Scale", transform.localScale);

        //update the wave script with the new parameters
        m_Wave.GetComponent<WaveSystem>().WaveA.Parameters = _wave_base;
        m_Wave.GetComponent<WaveSystem>().WaveB.Parameters = _wave_variance;
        m_Wave.GetComponent<WaveSystem>().OCEAN_SCALE_X = transform.localScale.x;
        m_Wave.GetComponent<WaveSystem>().OCEAN_SCALE_Z = transform.localScale.z;

        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Renderer>().material.SetVector("_Wave_1_Properties", _wave_base);
            child.gameObject.GetComponent<Renderer>().material.SetVector("_Wave_2_Properties", _wave_variance);
            child.gameObject.GetComponent<Renderer>().material.SetVector("_WaveObject_Scale", transform.lossyScale);
        }
    }

    //a helper function that gets the height of the wave at a specific x and z coordinate
    public float GetWaveHeight(Vector3 _coordinate)
    {
        //remove the height value so we can get a coordinate of the location.
        Vector3 _position = new(_coordinate.x, 0, _coordinate.z);
        float height_loc = m_Wave.GetWavePosition(_position).y;

        return height_loc;
    }


    // Update is called once per frame
    void Update()
    {
        //print(m_OceanMaterial.GetVector("_Wave_1_Properties"));

    }

}
