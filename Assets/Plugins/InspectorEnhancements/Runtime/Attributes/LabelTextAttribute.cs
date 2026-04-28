using System;
using UnityEngine;
using Nenn.InspectorEnhancements.Runtime.Attributes.Base;

namespace Nenn.InspectorEnhancements.Runtime.Attributes
{
    public class LabelTextAttribute : PropertyAttribute
    {
        public readonly string Label;

        public LabelTextAttribute(string label)
        {
            Label = label;
        }
    }
}

