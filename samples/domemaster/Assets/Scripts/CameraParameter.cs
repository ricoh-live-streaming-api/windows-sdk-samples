using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// カメラの移動・回転制御パラメータ管理
/// </summary>
public class CameraParameter : MonoBehaviour
{
    [SerializeField]
    private GameObject mainCamera = null;
    [SerializeField]
    private InputField positionX = null;
    [SerializeField]
    private InputField positionY = null;
    [SerializeField]
    private InputField positionZ = null;
    [SerializeField]
    private InputField rotationX = null;
    [SerializeField]
    private InputField rotationY = null;
    [SerializeField]
    private InputField rotationZ = null;

    private Transform cameraTransform;
    private UserData userData;

    private float PositionX { get => float.Parse(positionX.text); set => positionX.text = value.ToString(); }
    private float PositionY { get => float.Parse(positionY.text); set => positionY.text = value.ToString(); }
    private float PositionZ { get => float.Parse(positionZ.text); set => positionZ.text = value.ToString(); }
    private float RotationX { get => float.Parse(rotationX.text); set => rotationX.text = value.ToString(); }
    private float RotationY { get => float.Parse(rotationY.text); set => rotationY.text = value.ToString(); }
    private float RotationZ { get => float.Parse(rotationZ.text); set => rotationZ.text = value.ToString(); }

    public void Awake()
    {
        cameraTransform = mainCamera.transform;
        Load();
    }

    public void Update()
    {
        if (cameraTransform.hasChanged)
        {
            // カメラ位置を Position 表示に反映
            PositionX = cameraTransform.position.x;
            PositionY = cameraTransform.position.y;
            PositionZ = cameraTransform.position.z;

            // カメラ回転を Rotation 表示に反映
            // (Quaternion → Euler に変換)
            var euler = cameraTransform.rotation.eulerAngles;
            RotationX = euler.x;
            RotationY = euler.y;
            RotationZ = euler.z;

            cameraTransform.hasChanged = false;
        }
    }

    /// <summary>
    /// Save ボタンクリック
    /// </summary>
    public void OnSaveButtonClick()
    {
        Save();
    }

    /// <summary>
    /// Load ボタンクリック
    /// </summary>
    public void OnLoadButtonClick()
    {
        Load();
    }

    /// <summary>
    /// Position X 入力
    /// </summary>
    public void OnPositionXChanged()
    {
        var position = cameraTransform.position;
        position.x = PositionX;
        cameraTransform.position = position;
    }

    /// <summary>
    /// Position Y 入力
    /// </summary>
    public void OnPositionYChanged()
    {
        var position = cameraTransform.position;
        position.y = PositionY;
        cameraTransform.position = position;
    }

    /// <summary>
    /// Position Z 入力
    /// </summary>
    public void OnPositionZChanged()
    {
        var position = cameraTransform.position;
        position.z = PositionZ;
        cameraTransform.position = position;
    }

    /// <summary>
    /// Rotation X 入力
    /// </summary>
    public void OnRotationXChanged()
    {
        var euler = cameraTransform.rotation.eulerAngles;
        euler.x = RotationX;
        cameraTransform.rotation = Quaternion.Euler(euler);
    }

    /// <summary>
    /// Rotation Y 入力
    /// </summary>
    public void OnRotationYChanged()
    {
        var euler = cameraTransform.rotation.eulerAngles;
        euler.y = RotationY;
        cameraTransform.rotation = Quaternion.Euler(euler);
    }

    /// <summary>
    /// Rotation Z 入力
    /// </summary>
    public void OnRotationZChanged()
    {
        var euler = cameraTransform.rotation.eulerAngles;
        euler.z = RotationZ;
        cameraTransform.rotation = Quaternion.Euler(euler);
    }

    /// <summary>
    /// Potion リセット
    /// </summary>
    public void OnPositionResetButtonClick()
    {
        cameraTransform.position = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Rotation リセット
    /// </summary>
    public void OnRotationResetButtonClick()
    {
        cameraTransform.rotation = new Quaternion();
    }

    /// <summary>
    /// Potion/Rotation 保存<br>
    /// Rotation は Quaternion → Euler に変換
    /// </summary>
    private void Save()
    {
        userData.Position = new UserData.Param(cameraTransform.position);
        userData.Rotation = new UserData.Param(cameraTransform.rotation.eulerAngles);
        UserDataSerializer.Save(userData);
    }

    /// <summary>
    /// Potion/Rotation 読み込み
    /// </summary>
    private void Load()
    {
        // Rotation は Euler → Quaternion に変換
        userData = UserDataSerializer.Load(Application.persistentDataPath + "/UserData.json");
        cameraTransform.position = new Vector3(userData.Position.X, userData.Position.Y, userData.Position.Z);
        cameraTransform.rotation = Quaternion.Euler(new Vector3(userData.Rotation.X, userData.Rotation.Y, userData.Rotation.Z));
    }
}
