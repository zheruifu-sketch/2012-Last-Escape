using System;
using System.Collections.Generic;
using UnityEngine;
using Nenn.InspectorEnhancements.Runtime.Attributes;

[CreateAssetMenu(fileName = "LevelPatternDefinition", menuName = "JumpGame/Level Pattern Definition")]
public class LevelPatternDefinition : ScriptableObject
{
    [Serializable]
    public class EnvironmentGenerationRule
    {
        [LabelText("环境类型")]
        [SerializeField] private EnvironmentType environmentType = EnvironmentType.None;
        [LabelText("权重")]
        [SerializeField] private float weight = 1f;
        [LabelText("最少连续出现次数")]
        [SerializeField] private int minConsecutiveCount = 1;
        [LabelText("最多连续出现次数")]
        [SerializeField] private int maxConsecutiveCount = 2;
        [LabelText("是否可作为随机段首个路段")]
        [SerializeField] private bool canBeFirstRandomSegment = true;
        [LabelText("允许接在这些环境后面")]
        [SerializeField] private List<EnvironmentType> allowedPreviousEnvironments = new List<EnvironmentType>();

        public EnvironmentGenerationRule()
        {
        }

        public EnvironmentGenerationRule(
            EnvironmentType environmentType,
            float weight,
            int minConsecutiveCount,
            int maxConsecutiveCount,
            bool canBeFirstRandomSegment,
            List<EnvironmentType> allowedPreviousEnvironments)
        {
            this.environmentType = environmentType;
            this.weight = weight;
            this.minConsecutiveCount = minConsecutiveCount;
            this.maxConsecutiveCount = maxConsecutiveCount;
            this.canBeFirstRandomSegment = canBeFirstRandomSegment;
            if (allowedPreviousEnvironments != null)
            {
                this.allowedPreviousEnvironments = new List<EnvironmentType>(allowedPreviousEnvironments);
            }
        }

        public EnvironmentType EnvironmentType => environmentType;
        public float Weight => Mathf.Max(0.01f, weight);
        public int MinConsecutiveCount => Mathf.Max(1, minConsecutiveCount);
        public int MaxConsecutiveCount => Mathf.Max(MinConsecutiveCount, maxConsecutiveCount);
        public bool CanBeFirstRandomSegment => canBeFirstRandomSegment;
        public List<EnvironmentType> AllowedPreviousEnvironments => allowedPreviousEnvironments;
    }

    [LabelText("开场公路重复次数")]
    [SerializeField] private int openingRoadRepeatCount = 3;
    [LabelText("环境生成规则")]
    [SerializeField] private List<EnvironmentGenerationRule> environmentRules = new List<EnvironmentGenerationRule>();

    public int OpeningRoadRepeatCount => Mathf.Max(1, openingRoadRepeatCount);
    public List<EnvironmentGenerationRule> EnvironmentRules => environmentRules;
}
