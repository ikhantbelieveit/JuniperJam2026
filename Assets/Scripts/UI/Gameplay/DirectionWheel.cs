using UnityEngine;
using UnityEngine.EventSystems;

public class DirectionWheel : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] RectTransform _rect;
	[SerializeField] Canvas _canvas;

	private Camera _uiCamera;

	[SerializeField] float _mouseSensitivity = 1;
	[SerializeField] float _slowdownInertia = 1;

	private float _previousAngle;
	private float _spinVelocity;

	private void Awake()
	{
		if(_canvas.renderMode == RenderMode.ScreenSpaceCamera)
		{
			_uiCamera = _canvas.worldCamera;
		}
	}

	private void Update()
	{
		_rect.Rotate(0, 0, _spinVelocity * Time.deltaTime);
		_spinVelocity = Mathf.Lerp(_spinVelocity, 0, _slowdownInertia * Time.deltaTime);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_previousAngle = GetPointerAngleDegrees(eventData.position);
	}

	public void OnDrag(PointerEventData eventData)
	{
		float currentAngle = GetPointerAngleDegrees(eventData.position);
		float deltaAngle = Mathf.DeltaAngle(_previousAngle, currentAngle);
		_rect.Rotate(0f, 0f, deltaAngle);
		_previousAngle = currentAngle;

		_spinVelocity = deltaAngle * _mouseSensitivity;
	}

	private float GetPointerAngleDegrees(Vector2 pointerScreenPos)
	{
		Vector2 wheelCentre = RectTransformUtility.WorldToScreenPoint(_uiCamera, _rect.position);

		Vector2 direction = pointerScreenPos - wheelCentre;

		return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
	}

	public void ResetWheel()
	{
		_rect.SetPositionAndRotation(_rect.position, Quaternion.identity);
		_previousAngle = 0f;
		_spinVelocity = 0f;
	}
}
