using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ETileType
{
    E_ROOM,
    E_BRIDGE
}

public abstract class TileFactory
{
    public Vector2Int tileIndex;
    public FactoryData factoryData;

    public Vector2Int parent;

    public TileFactory(Vector2Int index, FactoryData workingData)
    {
        tileIndex = index;
        factoryData = workingData;
    }

    public abstract TileFactory Create();

    //프로세스 확장 갯수 결정
    protected abstract int DecideProcessDirectionCount(int workableDirectionCount);

    //제작가능한 방향 리스트 추출
    protected virtual List<Vector2Int> GetWorkableDirection()
    {
        Vector2Int[] directions = new Vector2Int[4]
        {
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1)
        };

        List<Vector2Int> workableDirections = new List<Vector2Int>();

        for(int i = 0; i < 4; ++i)
        {
            Vector2Int currentIndex = tileIndex + directions[i];
            if (!factoryData._allTileFactories.ContainsKey(currentIndex) &&
                factoryData._workingFactories.Find(tile => tile.tileIndex == currentIndex) == null)
            {
                workableDirections.Add(directions[i]);
            }
        }

        return workableDirections;
    }

    protected static void ShuffleList<T>(ref List<T> list, int shuffleCount)
    {
        for(int i = 0; i < shuffleCount; ++i)
        {
            int prev = Random.Range(0, list.Count), next = Random.Range(0, list.Count);
            T temp = list[prev];
            list[prev] = list[next];
            list[next] = temp;
        }
    }

    protected abstract ETileType DecideTileType();
}

public class RoomFactory : TileFactory
{
    public RoomFactory(Vector2Int index, FactoryData workingData) : base(index, workingData)
    {
        workingData._currentRoomCount++;
    }

    public override TileFactory Create()
    {
        //제작 대기큐 탈퇴
        factoryData._workingFactories.Remove(this);
        //생성된 타일 풀에 등록
        factoryData._allTileFactories.Add(tileIndex, this);

        //제작가능한 방향 가져오기
        List<Vector2Int> workableDirection = GetWorkableDirection();

        if (workableDirection.Count == 0)
        {
            return this;
        }

        //제작할 방향갯수 가져오기
        int workingDirectionCount = DecideProcessDirectionCount(workableDirection.Count);
        ShuffleList(ref workableDirection, 10);

        //해당제작방향들 작업
        for(int i = 0; i < workingDirectionCount; ++i)
        {
            Vector2Int currentIndex = tileIndex + workableDirection[i];
            //제작할 타일결정
            ETileType tileType = DecideTileType();

            if (tileType == ETileType.E_BRIDGE)
            {
                BridgeFactory nextTile = new BridgeFactory(currentIndex, factoryData);
                nextTile.parent = tileIndex;
                //제작할 브릿지에 자신과 이어지는 통로 추가
                nextTile.entrances.Add(workableDirection[i] * -1);
                factoryData._workingFactories.Add(nextTile);
            }
            else if(tileType == ETileType.E_ROOM)
            {
                RoomFactory nextTile = new RoomFactory(currentIndex, factoryData);
                nextTile.parent = tileIndex;
                factoryData._workingFactories.Add(nextTile);
            }
        }

        return this;
    }

    protected override int DecideProcessDirectionCount(int workableDirectionCount)
    {
        //방개수 미달
        if (factoryData._currentRoomCount < factoryData._roomCountRange.x)
        {
            return Mathf.Min(factoryData._workingFactories.Count == 0 ?
                Random.Range(0, workableDirectionCount) + 1 : Random.Range(0, workableDirectionCount + 1), 3);
        }
        //방개수 초과
        else if (factoryData._currentRoomCount >= factoryData._roomCountRange.y)
        {
            return 0;
        }
        //방개수 범위내
        else
        {
            return Mathf.Min(Random.Range(0, workableDirectionCount + 1), 2);
        }
}

    protected override ETileType DecideTileType()
    {
        return Random.Range(0, 10) > 7 ? ETileType.E_ROOM : ETileType.E_BRIDGE;
    }
}

public class BridgeFactory : TileFactory
{
    public List<Vector2Int> entrances;

    public BridgeFactory(Vector2Int index, FactoryData workingData) : base(index, workingData)
    {
        entrances = new List<Vector2Int>();
    }

    public override TileFactory Create()
    {
        //제작 대기큐 탈퇴
        factoryData._workingFactories.Remove(this);
        //생성된 타일 풀에 등록
        factoryData._allTileFactories.Add(tileIndex, this);
        //제작가능한 방향 가져오기
        List<Vector2Int> workableDirection = GetWorkableDirection();

        if (workableDirection.Count == 0)
        {
            factoryData._allTileFactories.Remove(tileIndex);
            RoomFactory room = new RoomFactory(tileIndex, factoryData);
            factoryData._allTileFactories.Add(tileIndex, room);
            return room;
        }

        //제작할 방향갯수 가져오기
        int workingDirectionCount = DecideProcessDirectionCount(workableDirection.Count);
        ShuffleList(ref workableDirection, 10);

        //해당제작방향들 작업
        for (int i = 0; i < workingDirectionCount; ++i)
        {
            //자신의 타일에 해당 방향으로 통로 추가
            entrances.Add(workableDirection[i]);
            Vector2Int currentIndex = tileIndex + workableDirection[i];
            ETileType tileType = DecideTileType();

            if (tileType == ETileType.E_BRIDGE)
            {
                BridgeFactory nextTile = new BridgeFactory(currentIndex, factoryData);
                nextTile.parent = tileIndex;
                //제작 브릿지에 자신의 방향으로 통로추가
                nextTile.entrances.Add(workableDirection[i] * -1);
                factoryData._workingFactories.Add(nextTile);
            }
            else if (tileType == ETileType.E_ROOM)
            {
                RoomFactory nextTile = new RoomFactory(currentIndex, factoryData);
                nextTile.parent = tileIndex;
                factoryData._workingFactories.Add(nextTile);
            }
        }

        return this;
    }

    protected override int DecideProcessDirectionCount(int workableDirectionCount)
    {
        return 1 + Mathf.Max(0, Random.Range(0, Mathf.Min(factoryData._roomCountRange.y - factoryData._currentRoomCount, workableDirectionCount)));
    }

    protected override ETileType DecideTileType()
    {
        return Random.Range(0, 10) > 4 ? ETileType.E_BRIDGE : ETileType.E_ROOM;
    }

    protected override List<Vector2Int> GetWorkableDirection()
    {
        List<Vector2Int> workableDirection = base.GetWorkableDirection();
        for(int i = 0; i < entrances.Count; ++i)
        {
            workableDirection.Remove(entrances[i]);
        }
        return workableDirection;
    }
}

public class FactoryData
{
    public int _currentRoomCount;
    public Vector2Int _roomCountRange;
    public List<TileFactory> _workingFactories;
    public Dictionary<Vector2Int, TileFactory> _allTileFactories;

    public FactoryData(Vector2Int roomCountRange)
    {
        _currentRoomCount = 0;
        _roomCountRange = roomCountRange;
        _workingFactories = new List<TileFactory>();
        _allTileFactories = new Dictionary<Vector2Int, TileFactory>();
    }
}

public class ModelSet
{
    public GameObject prefab;
    public Quaternion rotate;

    public ModelSet(GameObject o, Quaternion r)
    {
        prefab = o;
        rotate = r;
    }
}

public class MapGenerator : MonoBehaviour
{
    public GameObject[] models;

    FactoryData data;

    Dictionary<List<Vector2Int>, ModelSet> preset;

    public void Preset()
    {
        preset = new Dictionary<List<Vector2Int>, ModelSet>();

        Quaternion degree0 = Quaternion.identity,
            degree90 = Quaternion.Euler(0, 90f, 0),
            degree180 = Quaternion.Euler(0, 180f, 0),
            degree270 = Quaternion.Euler(0, 270f, 0);

        preset.Add(new List<Vector2Int>() { new Vector2Int(-1, 0), new Vector2Int(1, 0) }
        , new ModelSet(models[1], degree0));
        preset.Add(new List<Vector2Int>() { new Vector2Int(0, 1), new Vector2Int(0, -1) }
        , new ModelSet(models[1], degree90));

        preset.Add(new List<Vector2Int>() { new Vector2Int(-1, 0), new Vector2Int(0, 1) }
        , new ModelSet(models[2], degree0));
        preset.Add(new List<Vector2Int>() { new Vector2Int(0, 1), new Vector2Int(1, 0) }
        , new ModelSet(models[2], degree90));
        preset.Add(new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(0, -1) }
        , new ModelSet(models[2], degree180));
        preset.Add(new List<Vector2Int>() { new Vector2Int(0, -1), new Vector2Int(-1, 0) }
        , new ModelSet(models[2], degree270));

        preset.Add(new List<Vector2Int>() { new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) }
        , new ModelSet(models[3], degree0));
        preset.Add(new List<Vector2Int>() { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1) }
        , new ModelSet(models[3], degree90));
        preset.Add(new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) }
        , new ModelSet(models[3], degree180));
        preset.Add(new List<Vector2Int>() { new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(0, 1) }
        , new ModelSet(models[3], degree270));

        preset.Add(new List<Vector2Int>() { new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1)}
        , new ModelSet(models[4], degree0));
    }

    IEnumerator Init()
    {
        Preset();

        data = new FactoryData(new Vector2Int(30, 70));
        RoomFactory startTile = new RoomFactory(Vector2Int.zero, data);
        data._workingFactories.Add(startTile);
        for(;data._workingFactories.Count != 0;)
        {
            TileFactory currentFactory = data._workingFactories[0].Create();
            CreateModel(currentFactory);
            yield return new WaitForSeconds(0.1f);
        }

        //foreach(var iter in data._allTileFactories)
        //{
        //    GameObject createdModel;
        //    if(iter.Value is RoomFactory)
        //    {
        //        createdModel = Instantiate(models[0]);
        //        createdModel.transform.position = new Vector3(iter.Key.x * 5f, 0f, iter.Key.y * 5f);
        //    }
        //    else if (iter.Value is BridgeFactory)
        //    {
        //        BridgeFactory bridge = iter.Value as BridgeFactory;
        //        List<Vector2Int> keyInstance = preset.Keys.First(item => item.Except(bridge.entrances).Count() == 0);

        //        ModelSet modelset = preset[keyInstance];

        //        createdModel = Instantiate(modelset.prefab);
        //        createdModel.transform.position = new Vector3(iter.Key.x * 5f, 0f, iter.Key.y * 5f);
        //        createdModel.transform.rotation = modelset.rotate;
        //    }
        //}
    }

    void CreateModel(TileFactory tile)
    {
        GameObject createdModel;
        if (tile is RoomFactory)
        {
            createdModel = Instantiate(models[0]);
            createdModel.name = tile.parent.x + "," + tile.parent.y;
            createdModel.transform.position = new Vector3(tile.tileIndex.x * 10f, 0f, tile.tileIndex.y * 10f);
        }
        else if (tile is BridgeFactory)
        {
            BridgeFactory bridge = tile as BridgeFactory;
            if (!data._allTileFactories.ContainsKey(bridge.tileIndex))
                return;
            if(bridge.entrances.Count == 1)
            {
                data._allTileFactories.Remove(bridge.tileIndex);
                return;
            }

            List<Vector2Int> keyInstance = preset.Keys.First(
                item => bridge.entrances.OrderBy(e => e.x * 10000 + e.y).SequenceEqual(item.OrderBy(e => e.x * 10000 + e.y)));

            ModelSet modelset = preset[keyInstance];

            createdModel = Instantiate(modelset.prefab);
            createdModel.name = bridge.parent.x + "," + bridge.parent.y + " " + bridge.entrances.Count + " ";
            for (int i = 0; i < bridge.entrances.Count; ++i)
            {
                createdModel.name += bridge.entrances[i].x + "," + bridge.entrances[i].y + " ";
            }
            createdModel.transform.position = new Vector3(tile.tileIndex.x * 10f, 0f, tile.tileIndex.y * 10f);
            createdModel.transform.rotation = modelset.rotate;
        }
    }

    private void Start()
    {
        StartCoroutine(Init());
    }
}
