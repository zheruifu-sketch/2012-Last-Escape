using UnityEngine;
using Nenn.InspectorEnhancements.Runtime.Attributes;

public class PlayerGroundSensor : MonoBehaviour
{
    [LabelText("地面检测点")]
    [SerializeField] private Transform groundCheckPoint;
    [LabelText("地面检测层")]
    [SerializeField] private LayerMask groundLayers = ~0;
    [LabelText("检测半径")]
    [SerializeField] private float checkRadius = GameConstants.DefaultGroundCheckRadius;

    public bool IsGrounded { get; private set; }

    private void Reset()
    {
        groundCheckPoint = transform;
    }

    private void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        Vector2 origin = groundCheckPoint != null ? groundCheckPoint.position : transform.position;
        IsGrounded = Physics2D.OverlapCircle(origin, checkRadius, groundLayers) != null;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = groundCheckPoint != null ? groundCheckPoint.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, checkRadius);
    }
}
