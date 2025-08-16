using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private ObjectsDatabaseSO database;
    [SerializeField]
    private GameObject gridVisualization;

    private GridData floorData, furnitureData;//二者数据不同 地板Data占用的位置家具Data也可以用，但是二者都不能在自己已经占用的位置上再次放置

    [SerializeField]
    private PreviewSystem previewSystem;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    IBuildingState buildingState;
    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
    }
    private void Update()
    {
        if (buildingState == null)
            return;

        #region 随时获取鼠标的位置并转换为网格坐标
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        #endregion

        if(lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }

    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);

        buildingState = new PlacementState(
            ID,
            grid,
            previewSystem,
            database,
            floorData,
            furnitureData,
            objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
    public void StopPlacement()//可以通过委托来调用
    {
        if (buildingState == null)
            return;

        #region 指示器的显示&&委托的添加
        //取消显示
        gridVisualization.SetActive(false);
        buildingState.EndState();

        //清空委托
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        #endregion

        lastDetectedPosition = Vector3Int.zero;

        buildingState = null;//退出状态
    }
    private void PlaceStructure()//通过委托来调用
    {
        if(inputManager.IsPointerOverUI()) return; //如果鼠标在UI上就返回

        #region 获取鼠标的位置并转换为网格坐标
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        #endregion

        buildingState.OnAction(gridPosition);
    }
    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)//查看是否可放置
    //{
    //    //判断是地毯网格还是家具网格
    //    GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;

    //    return selectedData.CanPlaceObejctAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    //}

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, previewSystem, floorData, furnitureData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

}
