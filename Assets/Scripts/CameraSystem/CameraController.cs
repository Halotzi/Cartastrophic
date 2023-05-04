using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private static List<TrackObject> _trackObjects;

    [SerializeField, Range(0, 50)]
    private float _framBuffer = 25;
    [SerializeField] private float _speed = 1000;
    [SerializeField, Range(0, 180)] private float _cameraAngle = 60;

#if UNITY_EDITOR
    [FormerlySerializedAs("_ShowGizmos")]
    [Header("Editor")]
    [SerializeField] private bool _debugMode;
    [SerializeField] private float _gizmosRadius;

    private Color[] _colors = new[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
    };
#endif

    public static void RegisterTrackObject(TrackObject trackObject)
    {
        _trackObjects ??= new List<TrackObject>();

        if (_trackObjects.Contains(trackObject))
        {
            Debug.LogWarning("Track Object already register");
            return;
        }
        _trackObjects.Add(trackObject);
    }

    private void OnValidate()
    {
        if (_camera == null)
            _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        foreach (var trackObject in _trackObjects)
            trackObject.SetCamera(_camera);
    }

    private void Update()=>
        GetCamaraPosition();
    

    private void GetCamaraPosition()
    {
        // float screenLeftX = 0;
        // float screenRightX = 0;
        // float screenTopY = 0;
        // float screenBottomY = 0;
        //
        // List<Vector2> posOnScreen;
        //
        // foreach (var objectPosition in _trackObjects.Select(trackObject => trackObject.PosOnScreen))
        // {
        //     if (objectPosition.x < screenLeftX || screenLeftX == 0)
        //         screenLeftX = objectPosition.x - _framBuffer;
        //
        //     if (objectPosition.x > screenRightX || screenRightX == 0)
        //         screenRightX = objectPosition.x + _framBuffer;
        //
        //     if (objectPosition.y < screenBottomY || screenBottomY == 0)
        //         screenBottomY = objectPosition.y - _framBuffer;
        //
        //     if (objectPosition.y > screenTopY || screenTopY == 0)
        //         screenTopY = objectPosition.y + _framBuffer;
        // }

        float wordLeftX = 0;
        float wordRightX = 0;
        float wordTopZ = 0;
        float wordBottomZ = 0;

        foreach (var objectPosition in _trackObjects.Select(trackObject => trackObject.transform.position))
        {
            if (objectPosition.x < wordLeftX || wordLeftX == 0)
                wordLeftX = objectPosition.x;

            if (objectPosition.x > wordRightX || wordRightX == 0)
                wordRightX = objectPosition.x;

            if (objectPosition.z < wordBottomZ || wordBottomZ == 0)
                wordBottomZ = objectPosition.z;

            if (objectPosition.z > wordTopZ || wordTopZ == 0)
                wordTopZ = objectPosition.z;
        }
        
        wordLeftX   -= _framBuffer;
        wordRightX  += _framBuffer;
        wordTopZ    += _framBuffer;
        wordBottomZ -= _framBuffer;


        Vector3 midPosition;

        float wordLengthZ;
        float wordLengthX;

        if (wordRightX < 0)
            wordLengthX = Mathf.Abs(wordLeftX) - Mathf.Abs(wordRightX);
        else if (wordLeftX < 0)
            wordLengthX = Mathf.Abs(wordLeftX) + wordRightX;
        else
            wordLengthX = wordRightX - wordLeftX;

        if (wordTopZ < 0)
            wordLengthZ = Mathf.Abs(wordBottomZ) - Mathf.Abs(wordTopZ);
        else if (wordBottomZ < 0)
            wordLengthZ = Mathf.Abs(wordBottomZ) + wordTopZ;
        else
            wordLengthZ = wordTopZ - wordBottomZ;
        
        var midX = wordRightX - wordLengthX / 2;
        var midZ = wordTopZ - wordLengthZ / 2;
        midPosition = new Vector3(midX, 0,midZ);
        
        
        float distanceFromGraund = 0;

        if (wordLengthX > distanceFromGraund)
        {
            distanceFromGraund = (wordLengthX / 2) / Mathf.Tan(_camera.fieldOfView * Mathf.Deg2Rad / 2) ;
        }
        else
        {
            //need to work on
            var radAngle = _camera.fieldOfView * Mathf.Deg2Rad;
            var radHFOV = 2 * Mathf.Atan(Mathf.Tan(radAngle / 2) * _camera.aspect);

            distanceFromGraund = distanceFromGraund / 2 * Mathf.Tan(radHFOV / 2);
        }
        
        var camYPos = Mathf.Cos(Mathf.Deg2Rad * _cameraAngle) * distanceFromGraund;
        var camZPos = Mathf.Sin(Mathf.Deg2Rad * _cameraAngle) * distanceFromGraund;
        
        var newCamPosition = new Vector3(midPosition.x, camYPos, midPosition.z - camZPos);
        
        _camera.transform.LookAt(midPosition);
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, newCamPosition, _speed * Time.deltaTime);
        
#if UNITY_EDITOR
        if (_debugMode)
        {
            Debug.Log(newCamPosition);
            Debug.DrawLine(transform.position,midPosition,Color.red);
        }
            
#endif
    }

    private void OnDrawGizmos()
    {
        if (!_debugMode)
            return;

        if (_trackObjects == null || _trackObjects.Count == 0)
            return;

        for (int i = 0; i < _trackObjects.Count; i++)
        {
            Gizmos.color = _colors[i];
            Gizmos.DrawWireSphere(_trackObjects[i].transform.position,_gizmosRadius);
        }
    }
}