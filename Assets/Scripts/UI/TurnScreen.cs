/*
    Copyright (c) 2018, Szymon Jakóbczyk, Paweł Płatek, Michał Mielus, Maciej Rajs, Minh Nhật Trịnh, Izabela Musztyfaga
    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation 
          and/or other materials provided with the distribution.
        * Neither the name of the [organization] nor the names of its contributors may be used to endorse or promote products derived from this software 
          without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
    HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON 
    ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
    USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;

/*
 *  It is used for blocking input between turns
 */
public class TurnScreen : MonoBehaviour
{

    Text label;
    RawImage background;
    AudioSource sound;

    public void Start()
    {
        Debug.Log("TurnScreen Start");
        label = gameObject.GetComponentInChildren<Text>();
        background = gameObject.GetComponent<RawImage>();
        sound = gameObject.GetComponent<AudioSource>();
        label.color = new Vector4(255, 255, 255, 0);
        background.color = new Vector4(0, 0, 0, 0);
    }


    public void Play(string value)
    {
        gameObject.SetActive(true);
        label.text = value;
        sound.Play();
        FadeIn(true);
    }

    public void Show(string value)
    {
        gameObject.SetActive(true);
        label.text = value;
        sound.Play();
        FadeIn(false);
    }

    public void Hide()
    {
        sound.Play();
        if(gameObject.activeSelf)
            StartCoroutine(FadeOut());
    }

    void FadeIn(bool autoFadeOut)
    {
        Debug.Log("TurnScreen: fade in");
        gameObject.SetActive(true);
        //float startTime = Time.time;

        //while (Time.time - startTime < 1) // takes exactly 1 s. regardless of framerate
        //{
        //    float passedTime = Time.time - startTime;
        //    label.color = new Vector4(1, 1, 1, passedTime);
        //    background.color = new Vector4(0, 0, 0, passedTime / 1.2f);
        //    yield return null;
        //}
        label.color = new Vector4(1, 1, 1, 1);
        background.color = new Vector4(0, 0, 0, 1);

        if (autoFadeOut)
            StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        Debug.Log("TurnScreen: fade out");
        float startTime = Time.time;

        while (Time.time - startTime < 1) // takes exactly 2 s. regardless of framerate
        {
            float passedTime = Time.time - startTime;
            label.color = new Vector4(1, 1, 1, 1f- passedTime / 2f);
            background.color = new Vector4(0, 0, 0, 1f - passedTime / 2f);
            yield return null;
        }

        label.color = new Vector4(1, 1, 1, 0);
        background.color = new Vector4(0, 0, 0, 0);
        gameObject.SetActive(false);
    }
}
