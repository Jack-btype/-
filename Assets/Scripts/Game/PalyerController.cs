using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
public class PalyerController : MonoBehaviour {

    public Transform rayDown;
    public Transform rayLeft;
    public Transform rayRight;
    public LayerMask platformLayer;
    public LayerMask obstacleLayer;

    /// <summary>
    /// 是否向左移动，反之向右
    /// </summary>
    private bool isMoveLeft;
    /// <summary>
    /// 是否正在跳跃
    /// </summary>
    private bool isJumping = false;
    /// <summary>
    /// 玩家点击是否点击移动了
    /// 平台根据这个判断开始下落
    /// </summary>
    private bool isMove = false;

    private Vector3 nextPlatformLeft;
    private Vector3 nextPlatformRight;
    private ManagerVars vars;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D my_Body;

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        my_Body = transform.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //Debug.DrawRay(rayDown.position, Vector2.down * 1, Color.red);
        //Debug.DrawRay(rayLeft.position, Vector2.left * 0.15f, Color.red);
        //Debug.DrawRay(rayRight.position, Vector2.right * 0.15f, Color.red);

        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (GameManager.Instance.IsGameStarted == false || GameManager.Instance.IsGameOver == true || GameManager.Instance.IsGamePause == true)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && isJumping == false)
        {
            if (isMove == false)
            {
                EventCenter.Broadcast(EventType.PlayerMove);
                isMove = true;
            }
            EventCenter.Broadcast(EventType.DecidePath);
            isJumping = true;
            Vector3 mousePos = Input.mousePosition;
            // 点击的是左边屏幕
            if (mousePos.x <= Screen.width/2)
            {
                isMoveLeft = true;
            }
            // 点击的右边屏幕
            else if (mousePos.x > Screen.width/2)
            {
                isMoveLeft = false;
            }
            Jump();
        }
        // 游戏结束
        if (my_Body.velocity.y < 0 && IsRayPlatform() == false && GameManager.Instance.IsGameOver == false)
        {
            spriteRenderer.sortingLayerName = "Default";
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.IsGameOver = true;
            
            StartCoroutine(DealyShowGameOverPanel());

        }
        if (isJumping && IsRayObstacle() && GameManager.Instance.IsGameOver == false)
        {
            GameObject go = ObjectPool.Instance.GetDeathEffect();
            go.transform.position = transform.position;
            go.SetActive(true);
            GameManager.Instance.IsGameOver = true;
            spriteRenderer.enabled = false;

            StartCoroutine(DealyShowGameOverPanel());
        }
        if (transform.position.y-Camera.main.transform.position.y < -6)
        {
            GameManager.Instance.IsGameOver = true;
            // 调用结束面板
            StartCoroutine(DealyShowGameOverPanel());
        }
    }
    IEnumerator DealyShowGameOverPanel()
    {
        yield return new WaitForSeconds(1.5f);
        EventCenter.Broadcast(EventType.ShowGameOverPanel);
    }
    /// <summary>
    /// 是否检测到障碍物
    /// </summary>
    /// <returns></returns>
    private bool IsRayObstacle()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(rayLeft.position, Vector2.left, 0.15f, obstacleLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rayRight.position, Vector2.right, 0.15f, obstacleLayer);

        if (leftHit.collider != null)
        {
            if (leftHit.collider.tag == "Obstacle")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (rightHit.collider != null)
        {
            if (rightHit.collider.tag == "Obstacle")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    private GameObject lastHitGo = null;
    /// <summary>
    /// 是否检测到平台
    /// </summary>
    /// <returns></returns>
    private bool IsRayPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 1f, platformLayer);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Platform")
            {
                // 如果每次都检测是否碰撞到平台，会重复检测同一个平台，导致分数不断增加
                // 所以需要一个记录上一个碰撞到的平台引用，如果平台不相同，则广播事件，增加分数，然后更新最后碰到的平台。
                if (lastHitGo != hit.collider.gameObject)
                {
                    if (lastHitGo == null)
                    {
                        lastHitGo = hit.collider.gameObject;
                        return true;
                    }
                    EventCenter.Broadcast(EventType.AddScore);
                    lastHitGo = hit.collider.gameObject;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    private void Jump()
    {
        if (isJumping)
        {
            //SpriteRenderer sp = this.GetComponent<SpriteRenderer>();
            if (isMoveLeft)
            {
                spriteRenderer.flipX = true;
                //transform.localPosition = new Vector3(-1, 1, 1);
                transform.DOMoveX(nextPlatformLeft.x, 0.2f);
                transform.DOMoveY(nextPlatformLeft.y + 0.8f, 0.15f);
            }
            else
            {
                spriteRenderer.flipX = false;
                //transform.localPosition = new Vector3(1, 1, 1);
                transform.DOMoveX(nextPlatformRight.x, 0.2f);
                transform.DOMoveY(nextPlatformRight.y + 0.8f, 0.15f);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Platform")
        {
            isJumping = false;
            Vector3 currentPlatformPos = collider.gameObject.transform.position;
            nextPlatformLeft = new Vector3(currentPlatformPos.x - vars.nextXPos,
                currentPlatformPos.y + vars.nextYPos, 0);
            nextPlatformRight = new Vector3(currentPlatformPos.x + vars.nextXPos,
                currentPlatformPos.y + vars.nextYPos, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Pickup")
        {
            EventCenter.Broadcast(EventType.AddDiamond);
            //吃到钻石了
            collision.gameObject.SetActive(false);
        }
    }
}
