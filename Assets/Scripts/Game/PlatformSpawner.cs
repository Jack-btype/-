using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformGroupType
{
    Grass,
    Winter,
}

public class PlatformSpawner : MonoBehaviour {
    public Vector3 startSpawnPos;
    /// <summary>
    /// 里程碑数
    /// 当分数到达里程碑后里程碑树翻倍（10 -> 20 -> 40 -> ...）
    /// 当分数到达里程碑后，平台掉落速度加快。
    /// </summary>
    public int mileStoneCount = 10;
    /// <summary>
    /// 下落时间
    /// </summary>
    public float fallTime;
    /// <summary>
    /// 最小下落时间，当分数到达很高之后可能掉落时间会很短，玩家无法操作
    /// 需要一个最小的掉落时间，来让玩家有获得高分的可能
    /// </summary>
    public float minFallTime;
    /// <summary>
    /// 系数，指掉落时间每次翻倍后乘的一个系数，例如每次到达里程碑之后速度乘以0.8
    /// </summary>
    public float coefficient;
    /// <summary>
    /// 生成平台数量
    /// </summary>
    private int spawnPlatformCount;
    private ManagerVars vars;
    /// <summary>
    /// 平台的生成位置
    /// </summary>
    private Vector3 platformSpawnPosition;
    /// <summary>
    /// 是否朝左边生成，反之朝右
    /// </summary>
    private bool isLeftSpawn = false;

    /// <summary>
    /// 选中的平台图
    /// </summary>
    private Sprite selectPlatformSprite;

    /// <summary>
    /// 组合平台的类型
    /// </summary>
    private PlatformGroupType groupType;
    /// <summary>
    /// 钉子组合平台是否生成在左边，反之右边
    /// </summary>
    private bool spikeSpawnLeft = false;
    /// <summary>
    /// 钉子方向平台的位置
    /// </summary>
    private Vector3 spikeDirPlatformPos;
    /// <summary>
    /// 生成钉子平台之后需要在钉子方向生成的平台数量
    /// </summary>
    private int afterSpawnSpikeSpawnCount;
    /// <summary>
    /// 是否同时生成钉子平台后的平台
    /// </summary>
    private bool isSpawnSpike;
    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventType.DecidePath, DecidePath);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.DecidePath, DecidePath);
    }

    private void Start()
    {
        RandomPlatformTheme();
        platformSpawnPosition = startSpawnPos;
        for (int i = 0; i < 5; i++)
        {
            spawnPlatformCount = 5;
            DecidePath();
        }

        //生成人物
        GameObject go = Instantiate(vars.characterPre);
        go.transform.position = new Vector3(0, -1.8f, 0);
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameStarted == true && GameManager.Instance.IsGameOver == false )
        {
            UpdateFallTime();
        }
    }
    /// <summary>
    /// 更新平台掉落时间
    /// </summary>
    private void UpdateFallTime()
    {
        if (GameManager.Instance.GetGameScore() > mileStoneCount)
        {
            mileStoneCount = mileStoneCount * 2;
            fallTime = fallTime * coefficient;
            if (fallTime < minFallTime)
            {
                fallTime = minFallTime;
            }
        }
    }
    /// <summary>
    /// 随机平台主题
    /// </summary>
    private void RandomPlatformTheme()
    {
        int ran = Random.Range(0, vars.platformThemeSpritList.Count);
        selectPlatformSprite = vars.platformThemeSpritList[ran];

        if (ran == 2)
        {
            groupType = PlatformGroupType.Winter;
        }
        else
        {
            groupType = PlatformGroupType.Grass;
        }
    }

    /// <summary>
    /// 决定路径
    /// </summary>
    private void DecidePath()
    {
        if (isSpawnSpike)
        {
            AfterSpwanSpike();
            return;
        }
        if (spawnPlatformCount > 0)
        {
            spawnPlatformCount--;
            SpawnPlatform();
        }
        else
        {
            // 反转生成方向
            isLeftSpawn = !isLeftSpawn;
            spawnPlatformCount = Random.Range(1, 4);
            SpawnPlatform();
        }
    }

    /// <summary>
    /// 生成平台
    /// </summary>
    private void SpawnPlatform()
    {
        int ranObstacleDir = Random.Range(0, 2);
        // 生成单个平台
        if (spawnPlatformCount >= 1)
        {
            SpawnNormlaPlatform(ranObstacleDir);
        }
        // 生成组合平台
        else if (spawnPlatformCount == 0)
        {
            int ran = Random.Range(0, 3);
            // 生成通用组合平台
            if (ran == 0)
            {
                SpawnCommonPlatformGroup(ranObstacleDir);
            }
            // 生成主题组合平台
            else if (ran == 1)
            {
                switch (groupType)
                {
                    case PlatformGroupType.Grass:
                        SpawnGrassPlatformGroup(ranObstacleDir);
                        break;
                    case PlatformGroupType.Winter:
                        SpawnWinterPlatformGroup(ranObstacleDir);
                        break;
                    default:
                        //SpawnGrassPlatformGroup();
                        break;

                }
            }
            // 生成钉子组合平台
            else
            {
                int value = -1;
                if (isLeftSpawn)
                {
                    spikeSpawnLeft = false;
                    value = 0; // 生成右边方向的钉子
                }
                else
                {
                    spikeSpawnLeft = true;
                    value = 1; // 生成左边方向的钉子
                }
                SpawnSpikePlatform(value);

                isSpawnSpike = true;
                afterSpawnSpikeSpawnCount = 4;
                
                if (spikeSpawnLeft) // 钉子在左边
                {
                    spikeDirPlatformPos = new Vector3(platformSpawnPosition.x - 1.65f, platformSpawnPosition.y + vars.nextYPos, 0);
                }
                else // 钉子在右边
                {
                    spikeDirPlatformPos = new Vector3(platformSpawnPosition.x + 1.65f, platformSpawnPosition.y + vars.nextYPos, 0);
                }
            }
        }
        int ranSpawnDiamond = Random.Range(0, 10);
        if (ranSpawnDiamond == 6 && GameManager.Instance.IsPlayerMove == true)
        {
            GameObject go = ObjectPool.Instance.GetDiamond();
            go.transform.position = new Vector3(platformSpawnPosition.x, platformSpawnPosition.y + 0.5f, 0);
            go.SetActive(true);
        }

        if (isLeftSpawn) // 向左生成
        {
            platformSpawnPosition = new Vector3(platformSpawnPosition.x - vars.nextXPos,
                platformSpawnPosition.y + vars.nextYPos, 0);
        }
        else
        {
            platformSpawnPosition = new Vector3(platformSpawnPosition.x + vars.nextXPos,
                platformSpawnPosition.y + vars.nextYPos, 0);
        }
    }

    /// <summary>
    /// 生成普通平台（单个）
    /// </summary>
    private void SpawnNormlaPlatform(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetNormalPlatform();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成通用组合平台
    /// </summary>
    private void SpawnCommonPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetCommonPlatform();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成草地组合平台
    /// </summary>
    private void SpawnGrassPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetGrassPlatform();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成冬季组合平台
    /// </summary>
    private void SpawnWinterPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetWinterPlatform();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成钉子组合平台
    /// </summary>
    /// <param name="dir">方向</param>
    private void SpawnSpikePlatform(int dir)
    {
        // 生成右边
        GameObject tempSpike = null;
        if (dir == 0)
        {
            spikeSpawnLeft = false;
            tempSpike = ObjectPool.Instance.GetRightSpikePlatform();
        }
        else
        {
            spikeSpawnLeft = true;
            tempSpike = ObjectPool.Instance.GetLeftSpikePlatform();
        }
        tempSpike.transform.position = platformSpawnPosition;
        tempSpike.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, dir);
        tempSpike.SetActive(true);

    }
    /// <summary>
    /// 生成钉子平台之后需要生成的平台
    /// 包括钉子方向，也包括原来的方向
    /// </summary>
    private void AfterSpwanSpike()
    {
        if (afterSpawnSpikeSpawnCount > 0)
        {
            afterSpawnSpikeSpawnCount--;
            for (int i = 0; i < 2; i++)
            {
                GameObject tempNormal = ObjectPool.Instance.GetNormalPlatform();
                if (i == 0) // 生成原来方向的平台
                {
                    tempNormal.transform.position = platformSpawnPosition;
                    // 如果钉子在左边，原来方向的平台生成向右边（+）
                    if (spikeSpawnLeft)
                    {
                        platformSpawnPosition = new Vector3(platformSpawnPosition.x + vars.nextXPos,
                            platformSpawnPosition.y + vars.nextYPos, 0);
                    }
                    // 如果钉子在右边，原来方向的平台生成向左边（-）
                    else
                    {
                        platformSpawnPosition = new Vector3(platformSpawnPosition.x - vars.nextXPos,
                            platformSpawnPosition.y + vars.nextYPos, 0);
                    }
                }
                else // 生成钉子方向的平台
                {
                    tempNormal.transform.position = spikeDirPlatformPos;
                    // 如果钉子在左边，钉子方向的平台生成向左边（-）
                    if (spikeSpawnLeft)
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x - vars.nextXPos,
                            spikeDirPlatformPos.y + vars.nextYPos, 0);
                    }
                    // 如果钉子在右边，钉子方向的平台生成向右边（+）
                    else
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x + vars.nextXPos,
                            spikeDirPlatformPos.y + vars.nextYPos, 0);
                    }
                }

                tempNormal.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, 1);
                tempNormal.SetActive(true);
            }
        }
        else
        {
            isSpawnSpike = false;
            DecidePath();
        }
    }
}
