using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioReactor : MonoBehaviour
{
    private bool React;

    private float[] samples = new float[2056];
    private float avgSamples = 0;
    private float rmsValue;
    private float dbValue;
    private float lastValue;

    public AudioSource AudioSource;

    public GameObject Inner;
    public GameObject Outer;
    public SpriteRenderer SpriteRenderer;

    SpriteRenderer InnerSpriteRenderer;

    public float Scale;

    public float InnerScaleMinDuringBreak;
    public float InnerScaleMin;
    public float InnerScaleMax;
    public float OuterScaleMin;
    public float OuterScaleMax;

    private float mappedValueInner;
    private float mappedValueOuter;

    private Tween colorFadeOut;
    private Tween colorFadeIn;
    private Tween innerReturn;
    private Tween outerReturn;
    private Tween innerExtend;
    private Tween outerExtend;


    void Start()
    {
        InnerSpriteRenderer = Inner.GetComponent<SpriteRenderer>();

        InnerScaleMinDuringBreak *= Scale;
        InnerScaleMin *= Scale;
        InnerScaleMax *= Scale;
        OuterScaleMin *= Scale;
        OuterScaleMax *= Scale;
    }

    void Update()
    {
        if (React)
        {
            DaphneReact();
        }
    }

    public void SetReact(bool b)
    {
        React = b;

        if (React)
        {
            Inner.SetActive(true);
            Outer.SetActive(true);
            SpriteRenderer.enabled = false;
        }
        else
        {
            Inner.SetActive(false);
            Outer.SetActive(false);
            SpriteRenderer.enabled = true;
        }
    }

    private void DaphneReact()
    {
        AudioSource.GetOutputData(samples, 0);

        foreach (float sample in samples)
        {
            avgSamples += sample * sample;
        }

        rmsValue = Mathf.Sqrt(avgSamples / 1024); // rms = square root of average
        dbValue = 20 * Mathf.Log10(rmsValue / .01f);

        mappedValueInner = (dbValue / 100) * (InnerScaleMax - InnerScaleMin) + InnerScaleMin;
        mappedValueOuter = (dbValue / 100) * (OuterScaleMax - OuterScaleMin) + OuterScaleMin;

        if (dbValue > .7)
        {
            if (innerReturn != null) innerReturn.Kill();
            if (outerReturn != null) outerReturn.Kill();

            if (colorFadeOut == null || colorFadeOut.IsActive() != true)
            {
                colorFadeIn.Kill();
                colorFadeOut = DOTween.ToAlpha(() => InnerSpriteRenderer.color, x => InnerSpriteRenderer.color = x, .6f, .2f);
            }

            if (innerExtend == null || innerExtend.IsActive() != true)
            {
                innerExtend = Inner.transform.DOScale(mappedValueInner, .2f).SetEase(Ease.OutBack);
            }

            if (outerExtend == null || outerExtend.IsActive() != true)
            {
                outerExtend = Outer.transform.DOScale(mappedValueOuter, .6f).SetEase(Ease.OutQuint);
            }
        }
        else
        {
            innerExtend.Kill();
            outerExtend.Kill();

            if (colorFadeIn == null || colorFadeIn.IsActive() != true)
            {
                colorFadeOut.Kill();
                colorFadeIn = DOTween.ToAlpha(() => InnerSpriteRenderer.color, x => InnerSpriteRenderer.color = x, 1f, 2f);
            }

            if (innerReturn == null || innerReturn.IsActive() != true)
            {
                innerReturn = Inner.transform.DOScale(InnerScaleMinDuringBreak, .3f).SetEase(Ease.OutBack);
            }

            if (outerReturn == null || outerReturn.IsActive() != true)
            {
                outerReturn = Outer.transform.DOScale(OuterScaleMin, .5f).SetEase(Ease.OutQuint);
            }
        }


        avgSamples = 0;
    }
}
