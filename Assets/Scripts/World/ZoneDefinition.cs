using UnityEngine.Serialization;
using UnityEngine;
using Nenn.InspectorEnhancements.Runtime.Attributes;

[DisallowMultipleComponent]
public class ZoneDefinition : MonoBehaviour
{
    [Header("Semantic")]
    [FormerlySerializedAs("zoneType")]
    [LabelText("环境类型")]
    [SerializeField] private EnvironmentType environmentType = EnvironmentType.None;
    [FormerlySerializedAs("ruleTags")]
    [LabelText("额外规则标签")]
    [SerializeField] private RuleTag additionalRuleTags = RuleTag.None;

    public EnvironmentType EnvironmentType => environmentType;
    public RuleTag RuleTags => WorldSemanticUtility.GetDefaultRuleTags(environmentType) | additionalRuleTags;

    private void Reset()
    {
        Collider2D zoneCollider = GetComponent<Collider2D>();
        if (zoneCollider != null)
        {
            zoneCollider.isTrigger = true;
        }
    }
}
