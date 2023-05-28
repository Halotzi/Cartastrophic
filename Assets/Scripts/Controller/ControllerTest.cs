using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerTest : MonoBehaviour
{
    public float speed = 5;
    private Vector2 movmentInput;

    private void Update()
    {
        transform.Translate(new Vector3(movmentInput.x,0,movmentInput.y)*speed*Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext ctx) => movmentInput = ctx.ReadValue<Vector2>();
}
