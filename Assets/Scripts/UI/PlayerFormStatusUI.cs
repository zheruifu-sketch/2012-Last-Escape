using UnityEngine;

public class PlayerFormStatusUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerFormRoot playerFormRoot;
    [SerializeField] private GameLevelController levelController;

    [Header("State Roots")]
    [SerializeField] private Transform humanRoot;
    [SerializeField] private Transform carRoot;
    [SerializeField] private Transform planeRoot;
    [SerializeField] private Transform boatRoot;

    private GameObject humanActive;
    private GameObject carActive;
    private GameObject planeActive;
    private GameObject boatActive;
    private PlayerFormType lastForm = (PlayerFormType)(-1);

    private void Reset()
    {
        AutoBind();
    }

    private void Awake()
    {
        AutoBind();
        CacheActiveObjects();
        Refresh(true);
    }

    private void LateUpdate()
    {
        Refresh(false);
    }

    private void AutoBind()
    {
        if (playerFormRoot == null)
        {
            playerFormRoot = FindObjectOfType<PlayerFormRoot>();
        }

        if (levelController == null)
        {
            levelController = GameLevelController.GetOrCreateInstance();
        }

        if (humanRoot == null)
        {
            humanRoot = transform.Find("Human");
        }

        if (carRoot == null)
        {
            carRoot = transform.Find("Car");
        }

        if (planeRoot == null)
        {
            planeRoot = transform.Find("Plane");
        }

        if (boatRoot == null)
        {
            boatRoot = transform.Find("Boat");
        }
    }

    private void CacheActiveObjects()
    {
        humanActive = FindActiveChild(humanRoot);
        carActive = FindActiveChild(carRoot);
        planeActive = FindActiveChild(planeRoot);
        boatActive = FindActiveChild(boatRoot);
    }

    private static GameObject FindActiveChild(Transform root)
    {
        if (root == null)
        {
            return null;
        }

        Transform active = root.Find("Active");
        return active != null ? active.gameObject : null;
    }

    private void Refresh(bool force)
    {
        if (playerFormRoot == null)
        {
            return;
        }

        PlayerFormType currentForm = playerFormRoot.CurrentForm;
        if (!force && currentForm == lastForm)
        {
            RefreshRootVisibility();
            return;
        }

        RefreshRootVisibility();
        SetVisible(humanActive, currentForm == PlayerFormType.Human);
        SetVisible(carActive, currentForm == PlayerFormType.Car);
        SetVisible(planeActive, currentForm == PlayerFormType.Plane);
        SetVisible(boatActive, currentForm == PlayerFormType.Boat);
        lastForm = currentForm;
    }

    private static void SetVisible(GameObject target, bool visible)
    {
        if (target == null || target.activeSelf == visible)
        {
            return;
        }

        target.SetActive(visible);
    }

    private void RefreshRootVisibility()
    {
        SetVisible(humanRoot != null ? humanRoot.gameObject : null, IsFormUnlocked(PlayerFormType.Human));
        SetVisible(carRoot != null ? carRoot.gameObject : null, IsFormUnlocked(PlayerFormType.Car));
        SetVisible(planeRoot != null ? planeRoot.gameObject : null, IsFormUnlocked(PlayerFormType.Plane));
        SetVisible(boatRoot != null ? boatRoot.gameObject : null, IsFormUnlocked(PlayerFormType.Boat));
    }

    private bool IsFormUnlocked(PlayerFormType formType)
    {
        if (levelController == null)
        {
            return true;
        }

        return levelController.IsFormUnlocked(formType);
    }
}
