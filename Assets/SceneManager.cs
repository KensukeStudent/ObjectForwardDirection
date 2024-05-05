using UnityEngine;

public enum SearchType
{
    Normal,
    Forward
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

    [SerializeField]
    private SearchType searchType;

    // Update is called once per frame
    void Update()
    {
        SetMousePosition();
        CalcTargetDirection();

        if (angle >= -45 && angle < 45)
        {
            Debug.Log("正面");
        }
        else if (angle >= -135 && angle < -45)
        {
            Debug.Log("左");
        }
        else if (angle >= 45 && angle < 135)
        {
            Debug.Log("右");
        }
        else
        {
            Debug.Log("後ろ");
        }
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
}