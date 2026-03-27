using UnityEngine;

public class PlayerFormView : MonoBehaviour
{
    [Header("Form Objects")]
    [SerializeField] private GameObject humanObject;
    [SerializeField] private GameObject carObject;
    [SerializeField] private GameObject planeObject;
    [SerializeField] private GameObject boatObject;

    [Header("Facing")]
    [SerializeField] private bool spritesFaceLeftByDefault = true;

    private Vector3 humanScale = Vector3.one;
    private Vector3 carScale = Vector3.one;
    private Vector3 planeScale = Vector3.one;
    private Vector3 boatScale = Vector3.one;

    private void Awake()
    {
        CacheBaseScales();
    }

    public void ShowForm(PlayerFormType formType)
    {
        SetFormActive(humanObject, formType == PlayerFormType.Human);
        SetFormActive(carObject, formType == PlayerFormType.Car);
        SetFormActive(planeObject, formType == PlayerFormType.Plane);
        SetFormActive(boatObject, formType == PlayerFormType.Boat);
    }

    public void SetFacing(bool movingLeft)
    {
        bool faceLeftVisually = spritesFaceLeftByDefault ? movingLeft : !movingLeft;

        ApplyScale(humanObject, humanScale, faceLeftVisually);
        ApplyScale(carObject, carScale, faceLeftVisually);
        ApplyScale(planeObject, planeScale, faceLeftVisually);
        ApplyScale(boatObject, boatScale, faceLeftVisually);
    }

    private void CacheBaseScales()
    {
        humanScale = humanObject != null ? humanObject.transform.localScale : Vector3.one;
        carScale = carObject != null ? carObject.transform.localScale : Vector3.one;
        planeScale = planeObject != null ? planeObject.transform.localScale : Vector3.one;
        boatScale = boatObject != null ? boatObject.transform.localScale : Vector3.one;
    }

    private static void SetFormActive(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }

    private static void ApplyScale(GameObject target, Vector3 baseScale, bool faceLeft)
    {
        if (target == null)
        {
            return;
        }

        Vector3 nextScale = baseScale;
        nextScale.x = Mathf.Abs(baseScale.x) * (faceLeft ? 1f : -1f);
        target.transform.localScale = nextScale;
    }
}
