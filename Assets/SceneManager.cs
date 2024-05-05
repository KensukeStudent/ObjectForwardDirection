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
    private GameObject obj = null;

    [SerializeField]
    private GameObject target = null;

    [SerializeField]
    private SearchType searchType;

    // Update is called once per frame
    void Update()
    {
        SetMousePosition();
        CalcTargetDirection();
        SearchNormal();
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
        obj.transform.position = screenToWorldPointPosition;
    }

    private void CalcTargetDirection()
    {
        var targetPos = target.transform.position;
        var objPos = obj.transform.position;

        targetPos.y = 0;
        objPos.y = 0;

        Vector3 direction = targetPos - objPos;
        angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg + 180;
    }

    private void SwitchSearchMode()
    {
        switch (searchType)
        {
            case SearchType.Normal:
                SearchNormal();
                break;
            case SearchType.Forward:
                SeachForward();
                break;
        }
    }

    private void SearchNormal()
    {
        if (angle >= 45 && angle < 135)
        {
            Debug.Log("正面");
        }
        else if (angle >= 135 && angle < 225)
        {
            Debug.Log("左");
        }
        else if (angle >= 225 && angle < 315)
        {
            Debug.Log("後ろ");
        }
        else
        {
            Debug.Log("右");
        }
    }

    private void SeachForward()
    {

    }
}