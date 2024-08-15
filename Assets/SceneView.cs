using System.Text;
using TMPro;
using UnityEngine;

public class SceneView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI directionText = null;

    [SerializeField]
    private TextMeshProUGUI distanceText = null;

    public void SetDirectionText(float angle)
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

        directionText.text = builder.ToString();
    }

    public void SetDistanceText(float distance, float limit)
    {
        distanceText.text = $"distance of target: {distance} <= {limit}: " + (distance <= limit ? "inside" : "outside");
    }
}
