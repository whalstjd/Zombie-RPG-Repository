using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class CutScene : MonoBehaviour
{
    public GameObject[] cutScenes;
    public Transform startPos;
    public Transform endPos;

    public float lerpTime = 0.5f;
    public float turnTime = 0.5f;
    float currentTime = 0;

    public IEnumerator CutCor()
    {
        yield return new WaitForSeconds(turnTime);  // turnTime(0.5초) 대기
        GameManager.Instance.PlayAudio(GameManager.Instance.cutScene_Clip);
        currentTime = 0;
        while (currentTime < lerpTime)  // lerpTime(0.5초) 동안 이동
        {
            currentTime += Time.deltaTime;
            cutScenes[0].transform.position = Vector3.Lerp(startPos.position, endPos.position, currentTime / lerpTime);
            yield return null;
        }
        cutScenes[0].SetActive(false);

        yield return new WaitForSeconds(turnTime);  // turnTime(0.5초) 대기
        cutScenes[1].SetActive(false);

        // 나머지 컷신들도 같은 방식으로 처리
        for (int i = 2; i < cutScenes.Length; i++)
        {
            yield return new WaitForSeconds(turnTime);
            GameManager.Instance.PlayAudio(GameManager.Instance.cutScene_Clip);
            currentTime = 0;
            while (currentTime < lerpTime)
            {
                currentTime += Time.deltaTime;
                cutScenes[i].transform.position = Vector3.Lerp(startPos.position, endPos.position, currentTime / lerpTime);
                yield return null;
            }
            cutScenes[i].SetActive(false);
        }
        gameObject.SetActive(false);
        GameManager.Instance.BGM_AudioSource.volume = DataManager.Instance.gameData.bgmSound;
        GameManager.Instance.BGM_AudioSource.Play();
    }

    public void Skip()
    {
        gameObject.SetActive(false);
        GameManager.Instance.BGM_AudioSource.volume = DataManager.Instance.gameData.bgmSound;
        GameManager.Instance.BGM_AudioSource.Play();
    }
}
