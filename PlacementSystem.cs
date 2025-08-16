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

    private GridData floorData, furnitureData;//�������ݲ�ͬ �ذ�Dataռ�õ�λ�üҾ�DataҲ�����ã����Ƕ��߶��������Լ��Ѿ�ռ�õ�λ�����ٴη���

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

        #region ��ʱ��ȡ����λ�ò�ת��Ϊ��������
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
    public void StopPlacement()//����ͨ��ί��������
    {
        if (buildingState == null)
            return;

        #region ָʾ������ʾ&&ί�е����
        //ȡ����ʾ
        gridVisualization.SetActive(false);
        buildingState.EndState();

        //���ί��
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        #endregion

        lastDetectedPosition = Vector3Int.zero;

        buildingState = null;//�˳�״̬
    }
    private void PlaceStructure()//ͨ��ί��������
    {
        if(inputManager.IsPointerOverUI()) return; //��������UI�Ͼͷ���

        #region ��ȡ����λ�ò�ת��Ϊ��������
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        #endregion

        buildingState.OnAction(gridPosition);
    }
    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)//�鿴�Ƿ�ɷ���
    //{
    //    //�ж��ǵ�̺�����ǼҾ�����
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
