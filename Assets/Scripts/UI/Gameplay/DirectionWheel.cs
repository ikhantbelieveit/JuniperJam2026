using UnityEngine;
using JJ26.Input;
using UnityEditor.UI;
using UnityEngine.UIElements;

public class DirectionWheel : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, -1 * InputSystem.WalkValue.x);
    }
}
