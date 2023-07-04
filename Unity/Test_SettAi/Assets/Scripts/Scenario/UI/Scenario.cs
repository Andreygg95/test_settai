using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class Scenario : MonoBehaviour
{
    private Transform Target => target == null ? target = transform : target; 

    [SerializeField]
    private Transform target;

    [SerializeField]
    [ListDrawerSettings(Expanded = true)]
    private List<SequenceData> sequences = new List<SequenceData>();

    private List<Material> materials;
    
    private bool isRunning; 

    private void Awake()
    {
        var renderers = Target.GetComponentsInChildren<Renderer>();

        materials = MaterialManager.CopySharedMaterials(Target);
    }

    private void Start()
    {
        StartSequences();
    }
    
    [Button]
    public void StartSequences()
    {
        if (isRunning) return;

        isRunning = true;
        StartCoroutine(RunSequences());
    }

    private System.Collections.IEnumerator RunSequences()
    {
        foreach (var sequence in sequences)
        {
            yield return ExecuteSequence(sequence);
        }

        isRunning = false;
    }

    private System.Collections.IEnumerator ExecuteSequence(SequenceData sequence)
    {
        float elapsedTime = 0f;

        switch (sequence.sequenceType)
        {
            case SequenceType.Move:
                yield return MoveSequence(sequence, elapsedTime);
                break;

            case SequenceType.Rotate:
                yield return RotateSequence(sequence, elapsedTime);
                break;

            case SequenceType.ChangeColor:
                yield return ChangeColorSequence(sequence, elapsedTime);
                break;
        }
    }

    private System.Collections.IEnumerator MoveSequence(SequenceData sequence, float elapsedTime)
    {
        Vector3 startPosition = target.position;
        Vector3 targetPosition = startPosition + sequence.vectorValue;

        while (elapsedTime < sequence.duration)
        {
            float t = elapsedTime / sequence.duration;
            target.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.position = targetPosition;
    }

    private System.Collections.IEnumerator RotateSequence(SequenceData sequence, float elapsedTime)
    {
        Quaternion startRotation = target.rotation;
        Quaternion targetRotation = Quaternion.Euler(sequence.vectorValue);

        while (elapsedTime < sequence.duration)
        {
            float t = elapsedTime / sequence.duration;
            target.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.rotation = targetRotation;
    }

    private System.Collections.IEnumerator ChangeColorSequence(SequenceData sequence, float elapsedTime)
    {
        List<Color> startColors = new List<Color>();
        
        foreach (Material material in materials)
            startColors.Add(material.color);

        Color targetColor = sequence.colorValue;

        while (elapsedTime < sequence.duration)
        {
            float t = elapsedTime / sequence.duration;

            for (int i = 0; i < materials.Count; i++)
                materials[i].color = Color.Lerp(startColors[i], targetColor, t);

            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        for (int i = 0; i < materials.Count; i++)
            materials[i].color = targetColor;
    }
}


public enum SequenceType
{
    Move,
    Rotate,
    ChangeColor
}

[System.Serializable]
public class SequenceData
{
    public SequenceType sequenceType;
    
    [ShowIf("IsMoveSequence")]
    public Vector3 vectorValue;
    
    [ShowIf("IsColorSequence")]
    public Color colorValue;

    public float duration = 1f;
    
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