using UnityEngine;
using System.Collections;

public class TestCreat : MonoBehaviour
{
	public bool Regen;
	public int row = 30;
    public int col = 35;
	[Tooltip("密集探测范围，直接影响转生限值的有效性")]
	public int denseRange = 1;
	[Tooltip("疏松探测范围，直接影响转生限值的有效性")]
	public int sparseRange = 2;
	[Tooltip("密集转生限值，密集探测内的黑块大于此数时转生。\n此值越大越容易形成密集的块")]
	public int denseLimit = 5;
	[Tooltip("疏松转生限值，疏松探测内的黑块小于此数时转生。\n此值越小越容易形成零散的块")]
	public int sparseLimit = 2;
	private bool[,] mapArray;
	public GameObject black, white;
	public GameObject hEdge, vEdgeL, vEdgeR, vEdgeB, vEdge, wall;
    GameObject cubes;
    public static int times;


    void Awake()
    {
        cubes = new GameObject();
        mapArray = InitMapArray();
        CreateMap(mapArray);
    }


    bool[,] InitMapArray()
    {
        bool[,] array = new bool[row, col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                array[i, j] = Random.Range(0, 100) < 60;
                if (i == 0 || i == row - 1 || j == 0 || j == col - 1)
                {
                    array[i, j] = false;
                }
            }
        }


        return array;
    }


    bool[,] SmoothMapArray(bool[,] array)
    {
        bool[,] newArray = new bool[row, col];
        int count1, count2;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                count1 = CheckNeighborWalls(array, i, j, denseRange);
                count2 = CheckNeighborWalls(array, i, j, sparseRange);

				// 如果一定范围内的黑块太多或太少，生成黑块（生），否则成为白块（死）
                if (count1 >= denseLimit || count2 <= sparseLimit)
                {
                    newArray[i, j] = false;
                }
                else
                {
                    newArray[i, j] = true;
                }


                if (i == 0 || i == row - 1 || j == 0 || j == col - 1)
                {
                    newArray[i, j] = false;
                }
            }
        }


        return newArray;
    }

	/// <summary>
	/// 寻找一定范围内的黑块数量
	/// </summary>
	/// <param name="array"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="range"></param>
	/// <returns></returns>
    int CheckNeighborWalls(bool[,] array, int x, int y, int range)
    {
        int count = 0;
		
        for (int i2 = x - range; i2 < x + range + 1; i2++)
        {
            for (int j2 = y - range; j2 < y + range + 1; j2++)
            {
                if (i2 > 0 && i2 < row && j2 >= 0 && j2 < col)
                {
                    if (!array[i2, j2])
                    {
                        count++;
                    }
                }
            }
        }
        if (!array[x, y])
            count--;
        return count;
    }


    void CreateMap(bool[,] array)
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (!array[i, j])
                {
                    GameObject go = Instantiate(black, new Vector2(i, j), Quaternion.identity) as GameObject;
                    go.transform.SetParent(cubes.transform);
                }
                else
                {
                    GameObject go = Instantiate(white, new Vector2(i, j), Quaternion.identity) as GameObject;
                    go.transform.SetParent(cubes.transform);
                }
            }
        }
    }

	/// <summary>
	/// 造墙
	/// </summary>
	/// <param name="array"></param>
	void CreateWalls(bool[,] array)
	{
		GameObject go = null;

		for (int i = 0; i < row; i++) {
			for (int j = 0; j < col; j++) {
				// 如果是白的，略过
				if (array[i, j]) continue;

				// 如果上方是白块
				if (j+1 < col && array[i, j + 1]) {
					// 如果左上方是黑块
					if (i + 1 < row && !array[i + 1, j + 1]) {
						go = Instantiate(vEdgeR, new Vector2(i, j), Quaternion.Euler(0, 0, 180)) as GameObject;
					// 如果右上方是黑块
					} else if (i > 0 && !array[i - 1, j + 1]) {
						go = Instantiate(vEdgeL, new Vector2(i, j), Quaternion.Euler(0, 0, 180)) as GameObject;
					// 如果上方左右都没有黑块
					} else {
						go = Instantiate(vEdge, new Vector2(i, j), Quaternion.Euler(0, 0, 180)) as GameObject;
					}
					go.transform.SetParent(cubes.transform);
				}

				// 如果下方是白块
				if (j > 0 && array[i, j - 1]) {
					// 如果左下方是黑块
					if (i + 1 < row && !array[i + 1, j - 1]) {
						go = Instantiate(vEdgeL, new Vector2(i, j), Quaternion.identity) as GameObject;
					// 如果右下方是黑块
					} else if (i > 0 && !array[i - 1, j - 1]) {
						go = Instantiate(vEdgeR, new Vector2(i, j), Quaternion.identity) as GameObject;
					// 如果下方左右都没有黑块
					} else {
						go = Instantiate(vEdge, new Vector2(i, j), Quaternion.identity) as GameObject;
					}
					go.transform.SetParent(cubes.transform);

					// 造墙壁
					go = Instantiate(wall, new Vector2(i, j), Quaternion.identity) as GameObject;
					go.transform.SetParent(cubes.transform);
				}

				// 如果左方是白块
				if (i+1 < row && array[i + 1, j]) {
					go = Instantiate(hEdge, new Vector2(i, j), Quaternion.identity) as GameObject;
					go.transform.SetParent(cubes.transform);
				}

				// 如果右方是白块
				if (i > 0 && array[i - 1, j]) {
					go = Instantiate(hEdge, new Vector2(i, j), Quaternion.Euler(0, 0, 180)) as GameObject;
					go.transform.SetParent(cubes.transform);
				}
			}
		}
	}


	public void Update()
    {
		if (Regen)
		{
			Regen = false;
			mapArray = InitMapArray();
			times = Random.Range(3, 8);
		}
        if (times > 1)
        {
            times--;
			Destroy(cubes);
			cubes = new GameObject();
			mapArray = SmoothMapArray(mapArray);
			CreateMap(mapArray);
		}

		// 造地面完之后
		if (times == 1)
		{
			// 造墙
			CreateWalls(mapArray);
			times--;
		}
    }
}
