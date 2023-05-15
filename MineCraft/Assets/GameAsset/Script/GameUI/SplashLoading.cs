using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashLoading : MonoBehaviour
{
    [SerializeField]
    private Transform loadingIcon;
    [SerializeField] 
    private float loadingIconRotateSpeed = 200f;

    [SerializeField] 
    private float splashDelay = 2f;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    // Update is called once per frame
    void Update()
    {
        RotateLoadingIcon();
    }

    private void RotateLoadingIcon()
    {
        if (loadingIcon)
        { 
            loadingIcon.Rotate(-loadingIconRotateSpeed * Time.deltaTime * Vector3.forward, Space.World);
        }
    }

    private IEnumerator LoadScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneName.GAMEPLAY_SCENE);
        asyncOperation.allowSceneActivation = false;

        yield return new WaitForSeconds(splashDelay);

        while (!asyncOperation.isDone)
        {
            asyncOperation.allowSceneActivation = true;
            yield return null;
        }
    }
}
