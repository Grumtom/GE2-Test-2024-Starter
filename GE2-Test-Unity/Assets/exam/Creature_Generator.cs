using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    [SerializeField] private int finIntervalOffset = 0;
    [SerializeField] private int finLengthMultiplier;

    [Header("References")]
    [SerializeField] private GameObject bodySegmentPrefab;
    [SerializeField] private GameObject finSegmentPrefab;
    [SerializeField] private SpineAnimator SpineAnimator;

    [Header("Private")]
    [SerializeField] private GameObject[] segments;
    [SerializeField] private Vector3[] positions;
    [SerializeField] private Vector3[] sizes;
    
    [SerializeField] private List<int> finLengths;
    [SerializeField] private List<int> finIndexes;
    [SerializeField] private List<Vector3> finPositions;
    [SerializeField] private List<Vector3> finSizes;
    [SerializeField] private List<Quaternion> finRotations;
    [SerializeField] private List<int> finParentIndexes;

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
            float size = baseSize + Mathf.Sin(startAngle + frequency * i * 0.1f)  * multiplier;
            sizes[i] = new Vector3(size, size, size);
            positions[i] = pastPosition - (transform.forward * size);
            pastPosition = positions[i];
        }
    }

    void CalculateFins()
    {
        finLengths = new List<int>();
        finIndexes = new List<int>();
        finPositions = new List<Vector3>();
        finSizes = new List<Vector3>();
        finRotations = new List<Quaternion>();
        finParentIndexes = new List<int>();

        if (finInterval < 1)
        {
            finInterval = 1;
        }

        for (int i = 0; i < length; i++)
        {
            int j = i + finIntervalOffset;
            j -= finInterval;
            while (j > 0 && finInterval > 0) 
            {
                j -= finInterval;
            }

            if (j == 0)
            {
                finLengths.Add(Mathf.CeilToInt(sizes[i].x * finLengthMultiplier));
                finIndexes.Add(i);
            }
        }

        for (int i = 0; i < finIndexes.Count; i++)
        {
            for (int j = 0; j < finLengths[i]; j++)
            {
                if (j == 0)
                {
                    finSizes.Add(sizes[finIndexes[i]] * 0.7f);
                    finPositions.Add(positions[finIndexes[i]] + Vector3.right * finSizes[j].x * 1.4f);
                }
                else
                {
                    finSizes.Add(finSizes[^1] * 0.7f);
                    finPositions.Add(finPositions[^1] + Vector3.right * finSizes[j].x * 1.4f);
                }
                
                finRotations.Add(Quaternion.Euler(0,90,0));
                finParentIndexes.Add(finIndexes[i]);
            }
            
            for (int j = 0; j < finLengths[i]; j++)
            {
                if (j == 0)
                {
                    finSizes.Add(sizes[finIndexes[i]] * 0.7f);
                    finPositions.Add(positions[finIndexes[i]] - Vector3.right * finSizes[j].x * 1.4f);
                }
                else
                {
                    finSizes.Add(finSizes[^1] * 0.7f);
                    finPositions.Add(finPositions[^1] - Vector3.right * finSizes[j].x * 1.4f);
                }
                
                finRotations.Add(Quaternion.Euler(0,-90,0));
                finParentIndexes.Add(finIndexes[i]);
            }
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

        if (generateFins)
        {
            for (int i = 0; i < finPositions.Count; i++)
            {
                GameObject finSegment = Instantiate(finSegmentPrefab, finPositions[i], finRotations[i]);

                finSegment.transform.localScale = finSizes[i];
                
                finSegment.transform.SetParent(segments[finParentIndexes[i]].transform, true);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (calculateInEditor && !Application.isPlaying)
        {
            CalculateBoidShape();
            
            Gizmos.color = Color.magenta;

            for (int i = 0; i < length; i++)
            {
                Gizmos.DrawWireCube(positions[i], sizes[i]);
            }

            if (generateFins)
            {
                CalculateFins();
                
                Gizmos.color = Color.red;
                
                for (int i = 0; i < finPositions.Count; i++)
                {
                    Gizmos.DrawWireCube(finPositions[i], finSizes[i]);
                }
            }

            
        }
    }
}
