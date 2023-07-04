using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class Scenario : MonoBehaviour
{
    private Transform Target => target == null ? target = transform : target;

    [SerializeField] private Transform target;

    [SerializeField] [ListDrawerSettings(Expanded = true)]
    private List<SequenceData> sequences = new();

    [SerializeField] private bool startOnAwake = true;

    private List<Material> materials;

    private bool isRunning;

    private void Awake() => materials = MaterialManager.CopySharedMaterials(Target);

    private void Start()
    {
        if (startOnAwake)
            StartSequences();
    }

    [Button]

    public void StartSequences() => StartCoroutine(StartSequencesAsync());

    private IEnumerator StartSequencesAsync()
    {
        if (isRunning)
            yield break;

        isRunning = true;

        Coroutine previousCoroutine = null;

        foreach (var sequence in sequences)
        {
            yield return previousCoroutine;

            Coroutine coroutine = null;

            switch (sequence.sequenceType)
            {
                case SequenceType.Move:
                    coroutine = StartCoroutine(MoveSequence(sequence));
                    break;

                case SequenceType.Rotate:
                    coroutine = StartCoroutine(RotateSequence(sequence));
                    break;

                case SequenceType.ChangeColor:
                    coroutine = StartCoroutine(ChangeColorSequence(sequence));
                    break;
            }

            if (!sequence.simultaneousWithNext)
                previousCoroutine = coroutine;
        }

        isRunning = false;
    }

    private IEnumerator MoveSequence(SequenceData sequence)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = Target.position;
        Vector3 targetPosition = sequence.vectorValue;

        while (elapsedTime < sequence.duration)
        {
            float t = elapsedTime / sequence.duration;
            Target.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Target.position = targetPosition;
    }

    private IEnumerator RotateSequence(SequenceData sequence)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = Target.rotation;
        Quaternion targetRotation = Quaternion.Euler(sequence.vectorValue);

        while (elapsedTime < sequence.duration)
        {
            float t = elapsedTime / sequence.duration;
            Target.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Target.rotation = targetRotation;
    }

    private IEnumerator ChangeColorSequence(SequenceData sequence)
    {
        List<Color> startColors = new List<Color>();

        foreach (Material material in materials)
            startColors.Add(material.color);

        Color targetColor = sequence.colorValue;
        float elapsedTime = 0f;

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