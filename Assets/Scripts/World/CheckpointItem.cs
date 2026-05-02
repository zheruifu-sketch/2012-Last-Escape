using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[DisallowMultipleComponent]
public class CheckpointItem : MonoBehaviour
{
    [Header("References")]
    [LabelText("触发器")]
    [SerializeField] private Collider2D triggerCollider;

    [Header("State")]
    [LabelText("检查点距离")]
    [SerializeField] private float checkpointDistance;

    private bool activated;

    public void Initialize(float distanceFromLevelStart)
    {
        checkpointDistance = Mathf.Max(0f, distanceFromLevelStart);
    }

    private void Reset()
    {
        CacheReferences();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    private void Awake()
    {
        CacheReferences();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated || other == null)
        {
            return;
        }

        PlayerFormRoot formRoot = other.GetComponentInParent<PlayerFormRoot>();
        if (formRoot == null)
        {
            return;
        }

        GameSessionController sessionController = FindObjectOfType<GameSessionController>();
        if (sessionController == null)
        {
            return;
        }

        sessionController.ActivateCheckpoint(transform.position);
        activated = true;
        SoundEffectPlayback.Play(SoundEffectId.Pickup);
        Destroy(gameObject);
    }

    private void CacheReferences()
    {
        triggerCollider = triggerCollider != null ? triggerCollider : GetComponent<Collider2D>();
    }
}
