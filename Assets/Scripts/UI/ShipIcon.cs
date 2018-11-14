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

using UnityEngine;
using UnityEngine.UI;

//The in-game icons of ships and planets
public class ShipIcon : MonoBehaviour
{
    private GameController gameController;
    private SpriteRenderer sprite;
    private SpriteRenderer shadow;

    public void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        sprite = GetComponent<SpriteRenderer>();

    }

    void LateUpdate()
    {
        var target = Camera.main.transform.position;
        target.x = transform.position.x;
        transform.LookAt(2 * transform.position - target);
        var distanceToCamera = Vector3.Distance(transform.position, target) / 60f;
        transform.localScale = new Vector3(distanceToCamera, distanceToCamera, distanceToCamera);
        //if (GetComponentInParent<Ownable>().GetOwner() == gameController.GetCurrentPlayer())
        //{
        //    sprite.color = Color.white;


        //} else sprite.color = new Color(0.66f, 0, 0);

        if (GetComponentInParent<Ownable>().GetOwner() == null)
        {
            sprite.color = new Color(0.50f, 0.50f, 0.50f);

        }

        else sprite.color = GetComponentInParent<Ownable>().GetOwner().color;


    }
}
