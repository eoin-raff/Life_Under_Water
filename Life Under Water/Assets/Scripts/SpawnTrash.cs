﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnTrash : MonoBehaviour
{
    // TODO: Make trash not spawn on top of each other
    // TODO: Instantiate with animations

    public GameObject Trash;
    public Texture[] trashTextures;
    public int maxTrash;

    GameObject currentTrash;
    float width, height, x, y, wait;
    int trashCount;
    private bool coroutineStarted = false;

    [Range(0, 100)]
    public int edge;

    [Range(0.5f, 10f)]
    public float lowerSpawnSpan, upperSpawnSpan;

    // Start is called before the first frame update
    void Start()
    {
        trashCount = 0;

        // Get dimensions of the canvas
        width = gameObject.GetComponent<RectTransform>().rect.width;
        height = gameObject.GetComponent<RectTransform>().rect.height;

    }

    private void Update()
    {
        if (GameManager.Instance.SceneIsInteractable() && !coroutineStarted)
        {
            coroutineStarted = true;
            StartCoroutine(ProduceTrash());
        }
    }

    IEnumerator ProduceTrash()
    {
        float[,] trashLocations = new float[maxTrash, 2];

        while (trashCount <= maxTrash)
        {
            // Coordinates for where to spawn the next trash
            x = Random.Range(edge, width - edge);
            y = Random.Range(edge, height - edge);

            trashLocations[trashCount, 0] = x;
            trashLocations[trashCount, 1] = y;

            // ---- Instantiate the trash
            Trash.GetComponent<RawImage>().texture = trashTextures[Random.Range(0, trashTextures.Length - 1)]; // Give it a random texture
            Trash.GetComponent<RawImage>().color = new Color(255, 255, 255, 255); // Start with alpha 0
            Instantiate(Trash, new Vector3(x, y, 0), Quaternion.Euler(new Vector3(0,0,Random.Range(0,360))), gameObject.transform);

            // Increase counter
            trashCount++;

            wait = Random.Range(lowerSpawnSpan, upperSpawnSpan);
            yield return new WaitForSeconds(wait);
        }
    }
}