using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMono : MonoBehaviour
{
    public float sensitivityMouse;
    public float sensitivityScroll;
    
    private Mouse mouse;

    private void OnEnable()
    {
        mouse = Mouse.current;
    }

    private void Update()
    {
        if(mouse != null && mouse.rightButton.isPressed)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Rotation();
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        Movement();
    }

    private void Rotation()
    {
        if(mouse == null) return;
        
        Vector2 mouseDelta = mouse.delta.ReadValue();
        Vector3 mouseInput = new Vector3(-mouseDelta.y, mouseDelta.x, 0);
        transform.Rotate(mouseInput * sensitivityMouse);
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
    }

    private void Movement()
    {
        if(mouse == null) return;
        
        Vector2 scrollDelta = mouse.scroll.ReadValue();
        
        // Mouvement vertical (avant/arrière) avec la molette
        float vertical = scrollDelta.y;

        Vector3 input = new Vector3(0f, 0f, vertical);
        if(input.magnitude > 0) input.Normalize();
        
        transform.Translate(input * sensitivityScroll * Time.deltaTime);
    }
}