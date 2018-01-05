using UnityEngine;
using System.Collections;

public class TestCreat : MonoBehaviour
{
    public int row = 30;
    public int col = 35;
    private bool[,] mapArray;
	public GameObject black, white;
	public GameObject hEdge, vEdge, wall;
    GameObject cubes;
    public static int times;
    int a;


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
                count1 = CheckNeighborWalls(array, i, j, 1);
                count2 = CheckNeighborWalls(array, i, j, 2);


                if (count1 >= 5 || count2 <= 2)
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


    int CheckNeighborWalls(bool[,] array, int i, int j, int t)
    {
        int count = 0;


        for (int i2 = i - t; i2 < i + t + 1; i2++)
        {
            for (int j2 = j - t; j2 < j + t + 1; j2++)
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
        if (!array[i, j])
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
		for (int i = 0; i < row; i++) {
			for (int j = 0; j < col; j++) {
				// 如果是白的，略过
				if (array[i, j]) continue;
				if (j+1 < col && array[i, j + 1]) {
					var go = Instantiate(vEdge, new Vector2(i, j), Quaternion.Euler(0, 0, 180)) as GameObject;
					go.transform.SetParent(cubes.transform);
				}

				if (j > 0 && array[i, j - 1]) {
					var go = Instantiate(vEdge, new Vector2(i, j), Quaternion.identity) as GameObject;
					go.transform.SetParent(cubes.transform);
					go = Instantiate(wall, new Vector2(i, j), Quaternion.identity) as GameObject;
					go.transform.SetParent(cubes.transform);
				}

				if (i+1 < row && array[i + 1, j]) {
					var go = Instantiate(hEdge, new Vector2(i, j), Quaternion.identity) as GameObject;
					go.transform.SetParent(cubes.transform);
				}

				if (i > 0 && array[i - 1, j]) {
					var go = Instantiate(hEdge, new Vector2(i, j), Quaternion.Euler(0, 0, 180)) as GameObject;
					go.transform.SetParent(cubes.transform);
				}
			}
		}
	}


	public void Update()
    {
        if (times < 7)
        {
            times++;
            Destroy(cubes);
            cubes = new GameObject();
            mapArray = SmoothMapArray(mapArray);
            CreateMap(mapArray);
        }

		// 造地面完之后
		if (times == 7)
		{
			// 造墙
			CreateWalls(mapArray);
			times++;
		}
    }
}
