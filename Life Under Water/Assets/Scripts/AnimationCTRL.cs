﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AnimationCTRL : Singleton<AnimationCTRL>
{
    public PlayableDirector playableDirector;
    public bool AnimationIsPlaying { get; private set; }

    [SerializeField] private double interactionStartTime;

    private void Start()
    {
        PlayAnimation();
    }
    

    private void Update()
    {
        if (playableDirector == null)
        {
            playableDirector = GameObject.FindGameObjectWithTag("Timeline").GetComponent<PlayableDirector>();
        }
        if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            PlayAnimation();
        }
        //        AnimationIsPlaying = playableDirector.time <= 0;
        AnimationIsPlaying = playableDirector.time >= interactionStartTime;
    }

    public void PlayAnimation () 
    {
        playableDirector.Play();
    }

    internal void PlayReaction()
    {
        throw new NotImplementedException();
    }
}
