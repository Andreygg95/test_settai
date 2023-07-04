using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class SequenceData
{
    public SequenceType sequenceType;
    
    [ShowIf("IsMoveSequence")]
    public Vector3 vectorValue;
    
    [ShowIf("IsColorSequence")]
    public Color colorValue;

    public float duration = 1f;
    
    public bool simultaneousWithNext = false;
    
    private bool IsMoveSequence()
    {
        return sequenceType == SequenceType.Move ||
               sequenceType == SequenceType.Rotate;
    }

    private bool IsColorSequence()
    {
        return sequenceType == SequenceType.ChangeColor;
    }
}