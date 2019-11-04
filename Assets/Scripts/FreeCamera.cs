using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
#if UNITY_EDITOR
    static Texture2D ms_invisibleCursor = null;
#endif
    public bool enableInputCapture = true;
    public bool holdRightMouseCapture = false;//鼠标右键捕捉

    public float lookSpeed = 5f;
    public float moveSpeed = 5f;
    public float sprintSpeed = 50f;


    bool m_inputCaptured;
    float m_yaw;
    float m_pitch;

    void Awake()
    {
        //Behaviour.enabled 行为启用
        //启用行为被更新，禁用行为不更新。在行为检视面板显示为小的复选框。
        enabled = enableInputCapture;
    }

    void OnValidate()
    {
        if (Application.isPlaying)
            enabled = enableInputCapture;
    }
    void CaptureInput()
    {
        //按下后隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked;
#if UNITY_EDITOR
        Cursor.SetCursor(ms_invisibleCursor, Vector2.zero, CursorMode.ForceSoftware);
#else
        Cursor.visible = false;
#endif
        m_inputCaptured = true;

        m_yaw = transform.eulerAngles.y;
        m_pitch = transform.eulerAngles.x;

    }
    void ReleaseInput()
    {
        Cursor.lockState = CursorLockMode.None;
#if UNITY_EDITOR
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
#else
        Cursor.visible = true;
#endif
        m_inputCaptured = false;

    }
    private void OnApplicationFocus(bool focus)
    {
        if (m_inputCaptured && !focus)
            ReleaseInput();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_inputCaptured)
        {
            if (!holdRightMouseCapture && Input.GetMouseButtonDown(0))
                CaptureInput();
            else if (holdRightMouseCapture && Input.GetMouseButtonDown(1))
                CaptureInput();
        }

        if (!m_inputCaptured) return;
        if (m_inputCaptured)
        {
            if (!holdRightMouseCapture && Input.GetKeyDown(KeyCode.Escape))
                ReleaseInput();
            else if (holdRightMouseCapture && Input.GetMouseButtonUp(1))
                ReleaseInput();
        }

        var rotStrafe = Input.GetAxis("Mouse X");//鼠标左右控制偏航角
        var rotFwd = Input.GetAxis("Mouse Y");//上下控制俯仰角

        m_yaw = (m_yaw + lookSpeed * rotStrafe) % 360f;
        m_pitch = (m_pitch - lookSpeed * rotFwd) % 360f;
        if (m_pitch > 90f) m_pitch = 90f;
        if (m_pitch < -90f) m_pitch = -90f;
        transform.rotation = Quaternion.AngleAxis(m_yaw, Vector3.up) * Quaternion.AngleAxis(m_pitch, Vector3.right);

        var speed = Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed);
        var forward = speed * Input.GetAxis("Vertical");
        var right = speed * Input.GetAxis("Horizontal");
        var up = speed * ((Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f));
        transform.position += transform.forward * forward + transform.right * right + transform.up * up;
    }
}
