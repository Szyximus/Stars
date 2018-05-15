using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using Assets.Scripts;
using UnityEngine.EventSystems;

public class PlayModeTest
{
    [UnityTest]
    public IEnumerator YearGoForward()
    {
        SetupScence();
        int previous = GameController.GetYear();
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitUntil(() => (Input.GetMouseButtonDown(0)));
            int current = GameController.GetYear();
            if (current < previous || current > previous + 1)
            {
                Assert.Fail();
            }
            previous = current;
        }
        RemoveScence();
        yield break;
    }

    public void RemoveScence()
    {
    }

    public void SetupScence()
    {
        SceneManager.LoadScene("DemoScene");
    }
}
