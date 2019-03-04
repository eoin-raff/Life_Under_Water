﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class MeasureDepth : MonoBehaviour
{
    public MultiSourceManager multiSource;
    public Texture2D depthTexture;

    // Cutoffs
    [Range(0, 1.0f)]
    public float depthSensitivity = 1.0f;

    [Range(-10.0f, 10.0f)]
    public float wallDepth = -10.0f;

    [Header("Top and Bottom")]
    [Range(-1.0f, 1.0f)]
    public float topCutOff = 1.0f;
    [Range(-1.0f, 1.0f)]
    public float bottomCutOff = -1.0f;

    [Header("Left and Right")]
    [Range(-1.0f, 1.0f)]
    public float leftCutOff = -1.0f;
    [Range(-1.0f, 1.0f)]
    public float rightCutOff = 1.0f;

    // Depth Data
    private ushort[] depthData = null;
    private CameraSpacePoint[] cameraSpacePoints = null;
    private ColorSpacePoint[] colorSpacePoints = null;
    private List<ValidPoint> validPoints = null;

    // Kinect
    private KinectSensor sensor = null;
    private CoordinateMapper mapper = null;

    private readonly Vector2Int depthResolution = new Vector2Int(512, 424);

    private void Awake()
    {
        sensor = KinectSensor.GetDefault();
        mapper = sensor.CoordinateMapper;

        int arraySize = depthResolution.x * depthResolution.y;

        cameraSpacePoints = new CameraSpacePoint[arraySize];
        colorSpacePoints = new ColorSpacePoint[arraySize];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            validPoints = DepthToColor();

            depthTexture = CreateTexture(validPoints);
        }
    }

    private List<ValidPoint> DepthToColor()
    {
        List<ValidPoint> validPoints = new List<ValidPoint>();

        // Get Depth Data From Kinect
        depthData = multiSource.GetDepthData();

        //Map Depth Data to Camera & Color Space
        mapper.MapDepthFrameToCameraSpace(depthData, cameraSpacePoints);
        mapper.MapDepthFrameToColorSpace(depthData, colorSpacePoints);

        //Filter
        for (int i = 0; i < depthResolution.x / 8; i++)
        {
            for (int j = 0; j < depthResolution.y / 8; j++)
            {
                //Down Sampling
                int sampleIndex = j * depthResolution.x + i;
                sampleIndex *= 8; 
                
                // Cutoff Tests
                if (cameraSpacePoints[sampleIndex].X < leftCutOff)
                    continue;

                if (cameraSpacePoints[sampleIndex].X > rightCutOff)
                    continue;

                if (cameraSpacePoints[sampleIndex].Y < bottomCutOff)
                    continue;

                if (cameraSpacePoints[sampleIndex].Y > topCutOff)
                    continue;

                // Create Valid Point
                ValidPoint newPoint = new ValidPoint(colorSpacePoints[sampleIndex], cameraSpacePoints[sampleIndex].Z);

                // Depth Test
                if (cameraSpacePoints[sampleIndex].Z >= wallDepth)
                {
                    newPoint.withinWallDepth = true;
                }

                // Add to List
                validPoints.Add(newPoint);
            }
        }

        return validPoints;
    }

    private Texture2D CreateTexture(List<ValidPoint> validPoints)
    {
        Texture2D newTexture = new Texture2D(1920, 1080, TextureFormat.Alpha8, false);

        for (int x = 0; x < 1920; x++)
        {
            for (int y = 0; y < 1080; y++)
            {
                newTexture.SetPixel(x, y, Color.clear);
            }
        }

        foreach(ValidPoint point in validPoints)
        {
            newTexture.SetPixel((int)point.colorSpace.X, (int)point.colorSpace.Y, Color.black);
        }

        newTexture.Apply();

        return newTexture;
    }
}

public class ValidPoint
{
    public ColorSpacePoint colorSpace;
    public float z = 0.0f;

    public bool withinWallDepth = false;

    public ValidPoint(ColorSpacePoint colorSpace, float z)
    {
        this.colorSpace = colorSpace;
        this.z = z;
    }
}