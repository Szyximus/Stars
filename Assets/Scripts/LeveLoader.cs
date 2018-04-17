using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LeveLoader : MonoBehaviour {

    public GameObject slider;

	public void LoadLevel (string scene)
    {
        if (slider != null)
            slider.SetActive(true);
        StartCoroutine(LoadAsynchronously( scene));
    }

    IEnumerator LoadAsynchronously(string scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (slider != null) slider.GetComponent<Slider>().value = progress;
            yield return null;
        }
    }
}
