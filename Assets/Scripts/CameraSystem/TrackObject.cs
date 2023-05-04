using UnityEngine;

public class TrackObject : MonoBehaviour
{
    private Vector2 _posOnScreen;
    private Camera _camera;
    
    public void SetCamera(Camera camera) =>
        _camera = camera;

    public Vector2 PosOnScreen => _posOnScreen;

    private void Awake()
    {
        CameraController.RegisterTrackObject(this);
    }

    void Update()
    {
        _posOnScreen = _camera.WorldToScreenPoint(transform.position);
    }
}