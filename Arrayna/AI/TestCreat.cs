using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestCreat : MonoBehaviour
{
    public bool Regen;

    public int row;
    public int col;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    int[,] mapArray;

    public GameObject black;
    public GameObject hEdge, vEdgeL, vEdgeR, vEdgeB, vEdge, wall;
    public GameObject cubes;
    public GameObject box;
    public GameObject key;
    public GameObject dianti;
    public GameObject Player;

    public static int level=0;

    void Awake()
    {
        cubes = new GameObject();
        useRandomSeed = true;
        Regen = true;
    }


    public void Update()
    {
        if (Regen)
        {
            level++;
            InitMapArray();
            Regen = false;
        }
    }


    void InitMapArray()
    {
        Destroy(cubes);
        cubes = new GameObject();
        cubes.transform.SetParent(transform);

        mapArray = new int[row, col];
        RandomFilMap();

        for (int x = 0; x < 5; x++)
        {
            SmoothMapArray();
        }

        ProcessMap();
        CreateMap(mapArray);
        CreateWalls(mapArray);
    }


    void RandomFilMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (i == 0 || i == row-1 || j == 0 || j == col-1)
                {
                    mapArray[i, j] = 1;
                }
                else
                {
                    mapArray[i, j] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }


    void SmoothMapArray()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                int neighbourWallTiles = CheckNeighborWalls(i, j);
                if (neighbourWallTiles > 4)
                {
                    mapArray[i, j] = 1;
                }
                else if (neighbourWallTiles < 4)
                {
                    mapArray[i, j] = 0;
                }
            }
        }
    }


    void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);

        int wallThresholdSize = 20;

        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    mapArray[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);

        int roomThresholdSize = 20;

        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    mapArray[tile.tileX, tile.tileY] = 1;
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, mapArray));
            }
        }

        survivingRooms.Sort();
        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessiblerromMainRoom = true;

        ConnectClosestRooms(survivingRooms);
    }


    void ConnectClosestRooms(List<Room> allRooms,bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach(Room room in allRooms)
            {
                if (room.isAccessiblerromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach(Room roomB in roomListB)
            {
                if (roomA==roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }

                for(int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms<bestDistance||!possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if (possibleConnectionFound&&!forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA,bestRoomB,bestTileA,bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(allRooms, true);
        }
    }

    void CreatePassage(Room roomA,Room roomB,Coord tileA,Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);

        List<Coord> line = GetLine(tileA, tileB);
        foreach(Coord c in line)
        {
            DrawCircle(c, 5);
        }
    }

    void DrawCircle(Coord c,int r)
    {
        for(int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.tileX + x;
                    int drawY = c.tileY + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        mapArray[drawX, drawY] = 0;
                    }
                }
            }
        }
    }

    List<Coord> GetLine(Coord from,Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;

        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest<shortest)
        {
            inverted = true;
            longest= Mathf.Abs(dy);
            shortest= Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i=0;i<longest;i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }
        return line;
    }

    Vector2 CoordToWorldPoint(Coord tile)
    {
        return new Vector2(-row / 2 + .5f + tile.tileX,  -col / 2 + .5f + tile.tileY);
    }

    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[row, col];

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (mapFlags[i, j] == 0 && mapArray[i, j] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(i, j);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }
        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[row, col];
        int tileType = mapArray[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int i = tile.tileX - 1; i <= tile.tileX + 1; i++)
            {
                for (int j = tile.tileY - 1; j <= tile.tileY + 1; j++)
                {
                    if (IsInMapRange(i, j) && (j == tile.tileY || i == tile.tileX))
                    {
                        if (mapFlags[i, j] == 0 && mapArray[i, j] == tileType)
                        {
                            mapFlags[i, j] = 1;
                            queue.Enqueue(new Coord(i, j));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    bool IsInMapRange(int i, int j)
    {
        return i >= 0 && i < row && j >= 0 && j < col;
    }


    /// <summary>
    /// 寻找一定范围内的黑块数量
    /// </summary>
    /// <param name="array"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    int CheckNeighborWalls(int gx, int gy)
    {
        int count = 0;

        for (int ni = gx - 1; ni <= gx + 1; ni++)
        {
            for (int nj = gy - 1; nj <= gy + 1; nj++)
            {
                if (IsInMapRange(ni, nj))
                {
                    if (ni != gx || nj != gy)
                    {
                        count += mapArray[ni, nj];
                    }
                }
                else
                {
                    count++;
                }
            }
        }
        return count;
    }


    void CreateMap(int[,] array)
    {
        if (mapArray != null)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Vector2 pos = new Vector2(-row/2+i+.5f,-col/2+j+.5f);

                    if (!(array[i, j] == 0))
                    {
                        GameObject go = Instantiate(black, pos, Quaternion.identity) as GameObject;
                        go.transform.SetParent(cubes.transform);
                    }
                }
            }
        }
    }


    /// <summary>
    /// 造墙
    /// </summary>
    /// <param name="array"></param>
    void CreateWalls(int[,] array)
    {
        GameObject go = null;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Vector3 pos = new Vector3(-row / 2 + i + .5f, -col / 2 + j + .5f, -2);

                // 如果是白的，略过
                if (array[i, j] == 0) continue;

                // 如果上方是白块
                if (j + 1 < col && array[i, j + 1] == 0)
                {
                    // 如果左上方是黑块
                    if (i + 1 < row && !(array[i + 1, j + 1] == 0))
                    {
                        go = Instantiate(vEdgeR, pos, Quaternion.Euler(0, 0, 180)) as GameObject;
                        // 如果右上方是黑块
                    }
                    else if (i > 0 && !(array[i - 1, j + 1] == 0))
                    {
                        go = Instantiate(vEdgeL, pos, Quaternion.Euler(0, 0, 180)) as GameObject;
                        // 如果上方左右都没有黑块
                    }
                    else
                    {
                        go = Instantiate(vEdge, pos, Quaternion.Euler(0, 0, 180)) as GameObject;
                    }
                    go.transform.SetParent(cubes.transform);
                }

                // 如果下方是白块
                if (j > 0 && array[i, j - 1] == 0)
                {
                    // 如果左下方是黑块
                    if (i + 1 < row && !(array[i + 1, j - 1] == 0))
                    {
                        go = Instantiate(vEdgeL, pos, Quaternion.identity) as GameObject;
                        // 如果右下方是黑块
                    }
                    else if (i > 0 && !(array[i - 1, j - 1] == 0))
                    {
                        go = Instantiate(vEdgeR, pos, Quaternion.identity) as GameObject;
                        // 如果下方左右都没有黑块
                    }
                    else
                    {
                        go = Instantiate(vEdge, pos, Quaternion.identity) as GameObject;
                    }
                    go.transform.SetParent(cubes.transform);

                    // 造墙壁
                    go = Instantiate(wall, new Vector3(pos.x, pos.y, pos.z + .5f), Quaternion.identity) as GameObject;
                    go.transform.SetParent(cubes.transform);
                }

                // 如果左方是白块
                if (i + 1 < row && array[i + 1, j] == 0)
                {
                    go = Instantiate(hEdge, pos, Quaternion.identity) as GameObject;
                    go.transform.SetParent(cubes.transform);
                }

                // 如果右方是白块
                if (i > 0 && array[i - 1, j] == 0)
                {
                    go = Instantiate(hEdge, pos, Quaternion.Euler(0, 0, 180)) as GameObject;
                    go.transform.SetParent(cubes.transform);
                }
            }
        }
        CreateBox();
    }


    /// <summary>
    /// 造宝箱
    /// </summary>
    void CreateBox()
    { 
		//随机位置
		var random = new System.Random();
		int ran = random.Next(1, 4);
		int ra, co;

            //随机宝箱
            while (ran > 0)
            {
                ra = random.Next(0, row - 1);
                co = random.Next(0, row - 1);
                if (CheckNeighborWalls(ra, co) == 0)
                {
                    ra -= row / 2;
                    co -= col / 2;
                    Instantiate(box, new Vector3(ra, co,-4), Quaternion.identity);
                    ran--;
                }
            }

            //随机电梯
            while (true)
            {
                ra = random.Next(0, row - 1);
                co = random.Next(0, row - 1);
                if (CheckNeighborWalls(ra, co) == 0)
                {
                    ra -= row / 2;
                    co -= col / 2;
                    Instantiate(dianti, new Vector3(ra, co, -4), Quaternion.identity);
                    break;
                }
            }

            //随机钥匙
            while (true)
            {
                ra = random.Next(0, row - 1);
                co = random.Next(0, row - 1);
                if (CheckNeighborWalls(ra, co) == 0)
                {
                    ra = -(row / 2 - ra);
                    co = -(col / 2 - co);
                    Instantiate(key, new Vector3(ra, co, -4), Quaternion.identity);
                    break;
                }
            }

            //随机玩家生成
            while (true)
            {
                ra = random.Next(0, row - 1);
                co = random.Next(0, row - 1);
                if (CheckNeighborWalls(ra, co) == 0)
                {
                    ra = -(row / 2 - ra);
                    co = -(col / 2 - co);
                    Instantiate(Player, new Vector3(ra, co, -4), Quaternion.identity);
                    break;
                }
            }
    }


    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int i, int j)
        {
            tileX = i;
            tileY = j;
        }
    }


    class Room:IComparable<Room>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;
        public bool isAccessiblerromMainRoom;
        public bool isMainRoom;

        public Room()
        {

        }

        public Room(List<Coord> roomTiles, int[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            if (map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        public void SetAccessibleFromMainRoom()
        {
            if (!isAccessiblerromMainRoom)
            {
                isAccessiblerromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA,Room roomB)
        {
            if (roomA.isAccessiblerromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.isAccessiblerromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }
        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }
        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }
}
