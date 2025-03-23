using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机控制器：实现相机的移动、旋转和缩放功能
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("相机灵敏度设置")]
    [Range(0f, 180f)]
    public float cameraSensitivity = 90;  // 相机旋转灵敏度

    [Header("移动速度设置")]
    [Range(0f, 10f)]
    public float climbSpeed = 4;          // 上下移动速度
    [Range(0f, 20f)]
    public float normalMoveSpeed = 8;    // 标准移动速度
    [Range(0.1f, 1f)]
    public float slowMoveFactor = 0.25f;  // 慢速移动系数
    [Range(1f, 5f)]
    public float fastMoveFactor = 3;      // 快速移动系数

    private float rotationX = 0.0f;       // 水平旋转角度
    private float rotationY = 0.0f;       // 垂直旋转角度

    private bool isOrtho = false;         // 是否为正交相机
    private Camera cam;                   // 相机组件引用

    /// <summary>
    /// 初始化相机参数
    /// </summary>
    void Start()
    {
        rotationX = transform.eulerAngles.y;
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            isOrtho = cam.orthographic;
        }
    }

    /// <summary>
    /// 每帧更新相机控制
    /// </summary>
    void Update()
    {
        // 缓存deltaTime以提高性能
        var deltaTime = Time.deltaTime;

        // 处理相机旋转
        rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);  // 限制垂直旋转角度

        // 计算并应用相机旋转
        var tempRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        tempRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, tempRotation, deltaTime * 6.0f);

        // 处理相机移动
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            // 按住Shift键快速移动
            transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * deltaTime;
            transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            // 按住Ctrl键慢速移动
            transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * deltaTime;
            transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * deltaTime;
        }
        else
        {
            if (isOrtho)
            {
                // 正交相机模式下，前后移动改变正交大小（即缩放）
                cam.orthographicSize *= (1.0f - Input.GetAxis("Vertical") * deltaTime);
            }
            else
            {
                // 透视相机模式下，正常前后移动
                transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * deltaTime;
            }
            // 左右移动
            transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * deltaTime;
        }

        // 处理上下移动（Q键下降，E键上升）
        if (Input.GetKey(KeyCode.Q)) { transform.position -= transform.up * climbSpeed * deltaTime; }
        if (Input.GetKey(KeyCode.E)) { transform.position += transform.up * climbSpeed * deltaTime; }
    }
}
