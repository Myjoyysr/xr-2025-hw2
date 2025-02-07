using UnityEngine;
using UnityEngine.InputSystem;

public class QuitKey : MonoBehaviour
{
    public InputActionReference action;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        action.action.Enable();
        action.action.performed += (ctx) =>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        };
    }
    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
