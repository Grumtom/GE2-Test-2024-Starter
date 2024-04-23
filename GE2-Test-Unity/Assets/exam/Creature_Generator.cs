using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature_Generator : MonoBehaviour
{
    [Header("Base Values")]
    [SerializeField] private bool calculateInEditor = true;
    
    [SerializeField] private int length = 5;
    [SerializeField] private float startAngle;
    [SerializeField] private float frequency;
    [SerializeField] private float baseSize = 1;
    [SerializeField] private float multiplier;

    [Header("Extra Spice")]
    [SerializeField] private bool generateFins;
    [SerializeField] private int finInterval;
    [SerializeField] private int finLengthMultiplier;

    [Header("References")]
    [SerializeField] private GameObject bodySegmentPrefab;
    [SerializeField] private GameObject finSegmentPrefab;
    [SerializeField] private SpineAnimator SpineAnimator;

    [Header("Private")]
    [SerializeField] private GameObject[] segments;
    private Vector3[] positions;
    private Vector3[] sizes;

    private int finSegmentCount;
    private List<int> finLengths;
    private List<int> finIndexes;
    private List<Vector3> finPositions;
    private List<float> finSizes;

    private void Awake()
    {
        CalculateBoidShape();
        CreateBoidShape();
        if (generateFins) CalculateFins();
    }

    void CalculateBoidShape()
    {
        positions = new Vector3[length];
        sizes = new Vector3[length];
        
        Vector3 pastPosition = transform.position;

        for (int i = 0; i < length; i++)
        {
            float size = baseSize + Mathf.Sin(startAngle + frequency * i * multiplier);
            sizes[i] = new Vector3(size, size, size);
            positions[i] = pastPosition - (transform.forward * size);
            pastPosition = positions[i];

            
           
        }
    }

    void CalculateFins()
    {
        finSegmentCount = 0;
        finLengths = new List<int>();
        finPositions = new List<Vector3>();
        finSizes = new List<float>();

        for (int i = 0; i < length; i++)
        {
            int j = i;
            while (j > -1)
            {
                j -= finInterval;
            }

            if (j == 0)
            {
                finLengths.Add(Mathf.CeilToInt(sizes[i].x));
                finIndexes.Add(i);
            }
        }

        for (int i = 0; i < finIndexes.Count; i++)
        {
            
        }
        
    }

    void CreateBoidShape()
    {
        segments = new GameObject[length];
        SpineAnimator.bones = new GameObject[length];

        for (int i = 0; i < length; i++)
        {
            GameObject segment = Instantiate(bodySegmentPrefab, positions[i], transform.rotation);
                
            segment.transform.localScale = sizes[i];

            SpineAnimator.bones[i] = segment;

            segments[i] = segment;
        }
    }

    private void OnDrawGizmos()
    {
        if (calculateInEditor && !Application.isPlaying)
        {
            CalculateBoidShape();

            for (int i = 0; i < length; i++)
            {
                Gizmos.DrawCube(positions[i], sizes[i]);
            }
        }
    }
}
