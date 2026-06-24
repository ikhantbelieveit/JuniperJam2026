using UnityEngine;
using JJ26.Input;
using UnityEditor.UI;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class DirectionWheel : MonoBehaviour
{
    float rotation;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, -1 * rotation);
    }

    public void OnButtonClick()
    {
        Vector2 wheelPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 mousePos = Mouse.current.position.ReadValue();

        rotation = Vector2.Angle(wheelPos, mousePos);
        rotation = 0.0f;
    }
}
