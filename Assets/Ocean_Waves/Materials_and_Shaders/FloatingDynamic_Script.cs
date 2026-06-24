using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingDynamic_Script : MonoBehaviour
{

    public List<Transform> m_BuoyancyPoints;
    public Transform m_Testpoint;
    public Transform Object_to_Float;

    Quaternion m_applied_rotation;
    

    Ocean_Helper_Script m_Ocean;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_applied_rotation = Object_to_Float.transform.rotation;
        m_Ocean = FindAnyObjectByType<Ocean_Helper_Script>();

        //foreach (Transform child in Object_to_Float)
        //{
        //    if(child.CompareTag("BuoyancyPoint"))
        //    {
        //        print("adding to list");
        //        m_BuoyancyPoints.Add(child);
        //    }
        //}
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Vector3 _worldspace = transform.TransformPoint(m_Testpoint.transform.position);
        //Vector2 _coordinate = new(_worldspace.x, _worldspace.z);
        //print(_coordinate);

        Vector3 _mean_points = new();

        foreach (Transform point in m_BuoyancyPoints)
        { 
            setBuoyancyPoint(point);
            _mean_points += point.position;
        }

        //calculate the centrepoint using each buoyancy point, this will be used for rotation setting.


        //set the offset of the model we want to animate using the average point between the buoyancy points
        Vector3 _target_point = _mean_points / m_BuoyancyPoints.Count;
        float y_offset = _target_point.y - Object_to_Float.position.y;

        Object_to_Float.position += new Vector3(0f, y_offset, 0f);


        //calculate a side and forward angle to change the object angle
        float Forward_Angle = Vector3.SignedAngle(m_BuoyancyPoints[1].position, m_BuoyancyPoints[0].position,transform.up);
        float Side_Angle = Vector3.SignedAngle(m_BuoyancyPoints[1].position, m_BuoyancyPoints[2].position, transform.up);
        Vector3 Angle_rot = new(Forward_Angle, 0f, Side_Angle);
        print(Angle_rot);
        //Vector3 _original_angle = Object_to_Float.transform.rotation.eulerAngles;
        //print(_original_angle);

        Vector3 Forward = Vector3.Normalize(m_BuoyancyPoints[0].position - m_BuoyancyPoints[1].position);

        Vector3 Wooble = Vector3.Normalize(m_BuoyancyPoints[2].position - m_BuoyancyPoints[1].position);
        Vector3 Up = Vector3.Cross(Forward, Wooble);

        Quaternion rotation = Quaternion.LookRotation(Forward, Up);

        //float anglex = Mathf.Atan2(m_BuoyancyPoints[1].position.x, m_BuoyancyPoints[0].position.x);
        //float angley = Mathf.Atan2(m_BuoyancyPoints[1].position.y, m_BuoyancyPoints[0].position.y);
        //float anglez = Mathf.Atan2(m_BuoyancyPoints[1].position.z, m_BuoyancyPoints[0].position.z);
        //Vector3 Anglenew = new(anglex, angley, anglez);
        //
        //Vector3 _original_angle = Object_to_Float.transform.rotation.eulerAngles;
        //
        //_original_angle += Anglenew;
        Object_to_Float.rotation = rotation * m_applied_rotation;

        //Quaternion forward_Quat = Quaternion.LookRotation(Forward, Up);

        //Quaternion _original_angle = Object_to_Float.transform.rotation;

        //Quaternion new_rot = forward_Quat * _original_angle;
        //Object_to_Float.rotation = new_rot;







        //float y_target = m_Ocean.GetWaveHeight(_target_point);
        //
        //float floatpoint_offset = Object_to_Float.position.y - _target_point.y;
        //y_target = y_target + floatpoint_offset;
        //float target_offset = y_target - Object_to_Float.position.y;


        // - Rotation
        //Vector3 direction = Object_to_Float.position - _target_point;
        //
        //Vector3 _original_angle = Object_to_Float.transform.rotation.eulerAngles;
        //print(_original_angle);

        //_original_angle += angle;

        //Object_to_Float.transform.position += new Vector3(0f, target_offset, 0f);
        //Object_to_Float.rotation = Quaternion.LookRotation(angle);

    }

    private void setBuoyancyPoint(Transform _point)
    {
        float y_target = m_Ocean.GetWaveHeight(_point.position);
        float offset = y_target - _point.position.y;

        _point.transform.position += new Vector3(0f, offset, 0f);
    }

    private void calculate3pointrotation(Transform point1, Transform point2, Transform point3)
    {
        Plane _rotplane = new Plane();
        _rotplane.Set3Points(point1.position, point2.position, point3.position);
        Vector3 _Up = transform.up;
        Vector3 _Forward = transform.forward;
        
    }

}
