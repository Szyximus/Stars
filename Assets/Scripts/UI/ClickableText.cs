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
using UnityEngine.EventSystems;
//used to give a player feedback that the text is clickable
public class ClickableText : MonoBehaviour 
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerUpHandler
{
    private AudioSource sound;
    private Shadow shadow;
    // Use this for initialization
    void Start () {
        sound = gameObject.GetComponent<AudioSource>();
        shadow = gameObject.GetComponent<Shadow>();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Transform>().localScale = new Vector3(1.05f, 1.1f, 1.05f);
        shadow.effectDistance = new Vector2(0,0);
   
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        shadow.effectDistance = new Vector2(3, -3);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        sound.Play();
    }
}
