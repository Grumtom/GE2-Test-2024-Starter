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
    [SerializeField] private float finLengthMultiplier;

    [Header("References")]
    [SerializeField] private GameObject bodySegmentPrefab;
    [SerializeField] private GameObject finSegmentPrefab;
    [SerializeField] private SpineAnimator SpineAnimator;

    [Header("Private")]
    [SerializeField] private GameObject[] segments;
    [SerializeField] private Vector3[] positions;
    [SerializeField] private float[] sizes;
    [SerializeField] private Vector3[] sizesVectors;

    private void Awake()
    {
        CalculateBoidShape();
        CreateBoidShape();
    }

    void CalculateBoidShape()
    {
        sizes = new float[length];
        positions = new Vector3[length];
        sizesVectors = new Vector3[length];
        
        Vector3 pastPosition = transform.position;

        for (int i = 0; i < length; i++)
        {
            sizes[i] = baseSize + Mathf.Sin(startAngle + frequency * i * multiplier);
            sizesVectors[i] = new Vector3(sizes[i], sizes[i], sizes[i]);
            positions[i] = pastPosition - (transform.forward * sizes[i]);
            pastPosition = positions[i];

            if (generateFins)
            {
                int j = i;

                while (j > -1)
                {
                    j -= finInterval;
                }

                if (j == 0)
                {
                    
                }
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
                
            segment.transform.localScale = sizesVectors[i];

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
                Gizmos.DrawCube(positions[i], sizesVectors[i]);
            }
        }
    }
}
