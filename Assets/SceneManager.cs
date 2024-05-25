// --------------------------------------------------------------------
// reference: https://tsubakit1.hateblo.jp/entry/2018/02/05/235634
// --------------------------------------------------------------------

using System.Text;
using TMPro;
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

    [Header("Normal,Forward=targetマウス移動\nInput=targetをスティック入力移動"), SerializeField]
    private SearchType searchType;

    [SerializeField]
    private TextMeshProUGUI text = null;

    [Header("selfを回転させる"), SerializeField]
    private bool isRotate = false;

    [Header("入力をカメラy軸回転を基準とするか"), SerializeField]
    private bool inputForCamera = false;

    [Header("DirectlyAbove=真上視点/nOverlooking=見下ろし方視点"), SerializeField]
    private CameraType cameraType = CameraType.DirectlyAbove;

    private void OnValidate()
    {
        switch (cameraType)
        {
            case CameraType.DirectlyAbove:
                Camera.main.transform.position = new Vector3(0, 10, 0);
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
        SetText();
    }

    private void SetMousePosition()
    {
        // Vector3でマウス位置座標を取得する
        position = Input.mousePosition;
        // Z軸修正
        position.z = 10;
        // マウス位置座標をスクリーン座標からワールド座標に変換する
        screenToWorldPointPosition = Camera.main.ScreenToWorldPoint(position);
        screenToWorldPointPosition.y = 0;
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

    private void SetText()
    {
        StringBuilder builder = new StringBuilder();

        if (angle >= -45 && angle < 45)
        {
            builder.Append("Forward");
        }
        else if (angle >= -135 && angle < -45)
        {
            builder.Append("Left");
        }
        else if (angle >= 45 && angle < 135)
        {
            builder.Append("Right");
        }
        else
        {
            builder.Append("Back");
        }

        builder.AppendLine();
        builder.Append($@"Angle: {angle}");
        builder.Append(@"
        -45 - 45: Forward
        -135 - 45: Left
        45 - 135: Right
        else: Back");

        text.text = builder.ToString();
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