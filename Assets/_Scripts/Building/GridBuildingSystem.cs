using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem current;

    public GridLayout gridLayout;
    public Tilemap MainTilemap;
    public Tilemap TempTilemap;
    public Tilemap Indicator;

    private static Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();

    public Building temp;
    public Vector3 prevPos;
    public BoundsInt prevArea;

    #region Unity Methods
        
    private void Awake()
    {
        current = this;

        string tilePath = @"Tiles\";
        tileBases.Add(TileType.Empty, null);
        tileBases.Add(TileType.White, Resources.Load<TileBase>(tilePath + "white"));
        tileBases.Add(TileType.Green, Resources.Load<TileBase>(tilePath + "green"));
        tileBases.Add(TileType.Red, Resources.Load<TileBase>(tilePath + "red"));
    }
    private void Update()
    {
        if (!temp)
        {
            return;
        }
        if(Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject(0))
            {
                if (!temp.Placed)
                {
                    Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3Int cellPos = gridLayout.LocalToCell(touchPos);
                    if(prevPos != cellPos)
                    {
                        temp.transform.localPosition = gridLayout.CellToLocalInterpolated(cellPos + new Vector3(0.5f, 0.5f, 0f));
                        prevPos = cellPos;
                        FollowBuilding();
                    }
                }
            }
        }
    }

    public void Plato()
    {
        if (temp.CanBePlaced())
        {
            if (temp.isNew)
            {
                temp.isNew = false;
                CurrencyChangeGameEvent info = new CurrencyChangeGameEvent(-temp.item.Price, temp.item.Currency);
                EventManager.Instance.QueueEvent(info);
                EventManager.Instance.QueueEvent(new BuildingGameEvent(temp.item.Name));
            }
            temp.Place();
        }
    }
    public void Cancel()
    {
        ClearArea();
        Destroy(temp.gameObject);
        ShopManager.current.buildingPanel.SetActive(false);
        EventManager.Instance.RemoveListener<EnoughCurrencyGameEvent>(OnEnoughCurrency);
        EventManager.Instance.RemoveListener<NotEnoughCurrencyGameEvent>(OnNotEnoughCurrency);
        //GameManager.current.GetCoins(temp.item.Price);
    }
    #endregion

    #region Tile Managment

    private static void FileTiles(TileBase[] arr, TileType type)
    {
        for(int i = 0; i < arr.Length; i++)
        {
            arr[i] = tileBases[type];
        }
    }
    private static void SetTilesBlock(BoundsInt area, TileType type, Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FileTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }
    private static TileBase[] GetTileBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;
        foreach(var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }
        return array;
    }
    #endregion
    
    #region Building Placement
    public void InitializeWithBuilding(Building building)
    {
        //RaRaRaRaRa
        temp = Instantiate(building, Vector3.zero, Quaternion.identity).GetComponent<Building>();
        temp.GetComponent<Building>().Initialize(building.item);
        FollowBuilding();

        EventManager.Instance.AddListenerOnce<EnoughCurrencyGameEvent>(OnEnoughCurrency);
        EventManager.Instance.AddListenerOnce<NotEnoughCurrencyGameEvent>(OnNotEnoughCurrency);
    }
    private void OnEnoughCurrency(EnoughCurrencyGameEvent info)
    {
        EventManager.Instance.RemoveListener<NotEnoughCurrencyGameEvent>(OnNotEnoughCurrency);
    }
    private void OnNotEnoughCurrency(NotEnoughCurrencyGameEvent info)
    {
        EventManager.Instance.RemoveListener<EnoughCurrencyGameEvent>(OnEnoughCurrency);
    }
    public GameObject InitializeWithObject(GameObject building, Vector3 pos)
    {
        //instantiate an object
        GameObject obj = Instantiate(building, pos, Quaternion.identity);
        //return object instantiated
        return obj;
    }
    private void ClearArea()
    {
        TileBase[] toClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FileTiles(toClear, TileType.Empty);
        TempTilemap.SetTilesBlock(prevArea, toClear);
    }
    public void ClearAreaOnMove()
    {
        TileBase[] toClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FileTiles(toClear, TileType.White);
        MainTilemap.SetTilesBlock(prevArea, toClear);

        TileBase[] toClear2 = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FileTiles(toClear2, TileType.Empty);
        Indicator.SetTilesBlock(prevArea, toClear2);
        FollowBuilding();
    }
    private void FollowBuilding()
    {
        ClearArea();

        temp.area.position = gridLayout.WorldToCell(temp.gameObject.transform.position);

        BoundsInt buildingArea = temp.area;

        TileBase[] baseArray = GetTileBlock(buildingArea, MainTilemap);
        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];
        for(int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == tileBases[TileType.White])
            {
                tileArray[i] = tileBases[TileType.Green];
            }
            else
            {
                FileTiles(tileArray, TileType.Red);
                break;
            }
        }

        TempTilemap.SetTilesBlock(buildingArea, tileArray);
        prevArea = buildingArea;
    }
    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTileBlock(area, MainTilemap);
        foreach(var b in baseArray)
        {
            if(b != tileBases[TileType.White])
            {
                Debug.Log("Cannot Place here");
                return false;
            }
        }
        return true;
    }
    public void LoadPlacer(BoundsInt area)
    {
        if (CanTakeArea(area)) TakeArea(area);
        else Debug.Log("Cannot load here");
    }
    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType.Empty, TempTilemap);
        SetTilesBlock(area, TileType.Green, MainTilemap);
        SetTilesBlock(area, TileType.Green, Indicator);
    }
    #endregion
    public void LockTerritory(BoundsInt area)
    {
        //fill the area with white tile to indicate the area is unavailable
        SetTilesBlock(area, TileType.White, MainTilemap);
    }

    public void UnlockTerritory(BoundsInt area)
    {
        TileBase[] toClear = new TileBase[area.size.x * area.size.y * area.size.z];
        FileTiles(toClear, TileType.White);
        MainTilemap.SetTilesBlock(area, toClear);

        TileBase[] toClear2 = new TileBase[area.size.x * area.size.y * area.size.z];
        FileTiles(toClear2, TileType.Empty);
        Indicator.SetTilesBlock(area, toClear2);
    }
}
public enum TileType
{
    Empty,
    White,
    Green,
    Red
}
