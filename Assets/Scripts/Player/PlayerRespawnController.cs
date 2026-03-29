using UnityEngine;

[RequireComponent(typeof(PlayerFormRoot))]
public class PlayerRespawnController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerFormRoot formRoot;
    [SerializeField] private PlayerRuleController ruleController;

    [Header("Respawn")]
    [SerializeField] private PlayerFormType respawnForm = PlayerFormType.Human;
    [SerializeField] private float cliffGroundY = GameConstants.DefaultCliffGroundY;
    [SerializeField] private float cliffDeathY = GameConstants.DefaultCliffDeathY;
    [SerializeField] private float waterDeathDelay = GameConstants.DefaultWaterDeathDelay;

    public FailureType LastFailureType { get; private set; } = FailureType.None;

    private Vector3 spawnPosition;
    private float waterDeathTimer;

    private void Reset()
    {
        formRoot = GetComponent<PlayerFormRoot>();
        ruleController = GetComponent<PlayerRuleController>();
    }

    private void Awake()
    {
        if (formRoot == null)
        {
            formRoot = GetComponent<PlayerFormRoot>();
        }

        spawnPosition = transform.position;
    }

    private void Update()
    {
        UpdateWaterFailure();
        UpdateCliffFailure();
        UpdateInvalidBoatFailure();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (formRoot == null || formRoot.CurrentForm != PlayerFormType.Plane)
        {
            return;
        }

        if (collision.collider.CompareTag("Obstacle"))
        {
            Respawn(FailureType.HitObstacle);
            return;
        }

        ZoneDefinition zoneDefinition = collision.collider.GetComponent<ZoneDefinition>();
        if (zoneDefinition != null && zoneDefinition.ZoneType == ZoneType.Obstacle)
        {
            Respawn(FailureType.HitObstacle);
        }
    }

    public void Respawn(FailureType failureType)
    {
        LastFailureType = failureType;
        waterDeathTimer = 0f;

        transform.position = spawnPosition;
        if (formRoot != null && formRoot.PlayerRigidbody != null)
        {
            formRoot.PlayerRigidbody.velocity = Vector2.zero;
            formRoot.PlayerRigidbody.angularVelocity = 0f;
        }

        if (formRoot != null)
        {
            formRoot.SetForm(respawnForm);
        }
    }

    private void UpdateWaterFailure()
    {
        if (ruleController == null || formRoot == null)
        {
            return;
        }

        if (!ruleController.IsInWater())
        {
            waterDeathTimer = 0f;
            return;
        }

        if (formRoot.CurrentForm == PlayerFormType.Boat)
        {
            waterDeathTimer = 0f;
            return;
        }

        waterDeathTimer += Time.deltaTime;
        if (waterDeathTimer >= waterDeathDelay)
        {
            Respawn(FailureType.FellIntoWater);
        }
    }

    private void UpdateCliffFailure()
    {
        if (ruleController == null || formRoot == null)
        {
            return;
        }

        if (formRoot.CurrentForm == PlayerFormType.Plane || !ruleController.IsInCliff())
        {
            return;
        }

        float currentY = transform.position.y;
        if (currentY > cliffGroundY)
        {
            return;
        }

        if (currentY <= cliffDeathY)
        {
            Respawn(FailureType.FellFromCliff);
        }
    }

    private void UpdateInvalidBoatFailure()
    {
        if (ruleController == null || formRoot == null)
        {
            return;
        }

        if (formRoot.CurrentForm == PlayerFormType.Boat && !ruleController.IsBoatSupportedSurface())
        {
            Respawn(FailureType.InvalidForm);
        }
    }
}
