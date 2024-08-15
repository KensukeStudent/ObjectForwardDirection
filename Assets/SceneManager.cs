// --------------------------------------------------------------------
// reference: https://tsubakit1.hateblo.jp/entry/2018/02/05/235634
// --------------------------------------------------------------------

using UnityEngine;

public enum SearchType
{
    Normal,
    Forward,
    Input
}

public enum CameraType
{
    /// <summary>
    /// 真上視点
    /// </summary>
    DirectlyAbove,

    /// <summary>
    /// 見下ろし方視点
    /// </summary>
    Overlooking
}

[System.Serializable]
public class Settings
{
    [Header("Normal,Forward=targetマウス移動\nInput=targetをスティック入力移動"), SerializeField]
    public SearchType searchType;

    [Header("selfを回転させる"), SerializeField]
    public bool isRotate = false;

    [Header("入力をカメラy軸回転を基準とするか"), SerializeField]
    public bool inputForCamera = false;

    [Header("DirectlyAbove=真上視点/nOverlooking=見下ろし方視点"), SerializeField]
    public CameraType cameraType = CameraType.DirectlyAbove;

    [Header("索敵距離"), SerializeField]
    public float distance;

    [Header("高さ"), SerializeField]
    public float hight;
}

[RequireComponent(typeof(SceneView))]
public class SceneManager : MonoBehaviour
{
    // 位置座標
    private Vector3 position;

    // スクリーン座標をワールド座標に変換した位置座標
    private Vector3 screenToWorldPointPosition;

    /// <summary>
    /// objからみたtargetの角度
    /// </summary>
    private float angle;

    [SerializeField]
    private GameObject target = null;

    [SerializeField]
    private GameObject self = null;

    private SceneView view = null;

    // Settings -------------------------------------------------------

    [SerializeField]
    private Settings settings = null;

    private SearchType searchType => settings.searchType;

    private bool isRotate => settings.isRotate;

    private bool inputForCamera => settings.inputForCamera;

    private CameraType cameraType => settings.cameraType;

    private void Awake()
    {
        TryGetComponent(out view);
    }

    private void OnValidate()
    {
        switch (cameraType)
        {
            case CameraType.DirectlyAbove:
                Camera.main.transform.position = new Vector3(0, 15, 0);
                Camera.main.transform.localRotation = Quaternion.Euler(90, 0, 0);
                break;
            case CameraType.Overlooking:
                Camera.main.transform.position = new Vector3(0, 7.7f, -12.9f);
                Camera.main.transform.localRotation = Quaternion.Euler(20, 0, 0);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (searchType)
        {
            case SearchType.Normal:
            case SearchType.Forward:
                SetMousePosition();
                break;

            case SearchType.Input:
                Stick();
                break;
        }

        CalcTargetDirection();
        RotateSelf();
        view.SetDirectionText(angle);
        view.SetDistanceText(Vector3.Distance(target.transform.position, self.transform.position), settings.distance);
    }

    private void SetMousePosition()
    {
        // Vector3でマウス位置座標を取得する
        position = Input.mousePosition;
        // Z軸修正
        position.z = 10;
        // マウス位置座標をスクリーン座標からワールド座標に変換する
        screenToWorldPointPosition = Camera.main.ScreenToWorldPoint(position);
        screenToWorldPointPosition.y = settings.hight;
        // ワールド座標に変換されたマウス座標を代入
        target.transform.position = screenToWorldPointPosition;
    }

    private void CalcTargetDirection()
    {
        var diff = target.transform.position - self.transform.position;
        var forward = searchType == SearchType.Normal ? Vector3.forward : self.transform.forward;

        // 外積からベクトルの左右を求める
        var axis = Vector3.Cross(forward, diff);
        // マイナスであれば左、プラスであれば右
        angle = Vector3.Angle(forward, diff) * (axis.y < 0 ? -1 : 1);
    }

    private void RotateSelf()
    {
        if (isRotate)
        {
            self.transform.Rotate(0, 30 * Time.deltaTime, 0);
        }
    }

    private void Stick()
    {
        // ※ Vertical Stick-LはinputManagerでinvert設定にしているため
        // スティックを前方に倒せば、プラスの値
        // スティックを後方に倒せば、マイナスの値になります
        float horizontal = Input.GetAxis("Horizontal Stick-L");
        float vertical = -Input.GetAxis("Vertical Stick-L");
        float radians = Mathf.Atan2(vertical, horizontal);

        // ターゲットをself周辺に移動させる
        var selfPos = self.transform.position;
        if (inputForCamera)
        {
            // カメラの回転角度に入力方向を合わせる
            float angleForCamera = radians * Mathf.Rad2Deg - Camera.main.transform.localEulerAngles.y;
            float radiansForCamera = angleForCamera * Mathf.Deg2Rad;
            target.transform.position = selfPos + new Vector3(Mathf.Cos(radiansForCamera), 0, Mathf.Sin(radiansForCamera)) * 5;
        }
        else
        {
            target.transform.position = selfPos + new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * 5;
        }
    }
}