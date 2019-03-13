using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour {

    public SpriteRenderer[] spriteRenderers;
    public GameObject obstacles;
    private bool startTimer;
    private float fallTime;
    private Rigidbody2D my_Body;
    private void Awake()
    {
        my_Body = GetComponent<Rigidbody2D>();
    }
    public void Init(Sprite sprite, float fallTime, int obstacleDir)
    {
        my_Body.bodyType = RigidbodyType2D.Static;
        this.fallTime = fallTime;
        startTimer = true;
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = sprite;
        }

        if (obstacleDir == 0) // 朝右边
        {
            if (obstacles != null)
            {
                obstacles.transform.localPosition = new Vector3(-obstacles.transform.localPosition.x,
                    obstacles.transform.localPosition.y,
                    0);
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameStarted == false || GameManager.Instance.IsPlayerMove == false)
        {
            return;
        }
        if (startTimer)
        {
            fallTime = fallTime - Time.deltaTime;
            if (fallTime < 0) // 倒计时结束
            {
                // 掉落
                startTimer = false;
                if (my_Body.bodyType != RigidbodyType2D.Dynamic)
                {
                    my_Body.bodyType = RigidbodyType2D.Dynamic;
                    StartCoroutine(DealyHide());
                }
            }
        }
        if (transform.position.y - Camera.main.transform.position.y < -6f)
        {
            StartCoroutine(DealyHide());
        }
    }
    private IEnumerator DealyHide()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);

    }    
}
