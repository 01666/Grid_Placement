using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;//放置在地板偏上方

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;

    void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);

        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        cellIndicator.SetActive(false);
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }
    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
            Destroy(previewObject);
    }

    private void PreparePreview(GameObject previewObject)//更换材质
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();//获取Object子物体上的所有渲染器
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;//获取当前Renderer的所有材质
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;//所有材质都变成我们指定材质
            }
            renderer.materials = materials;
        }
    }
    private void PrepareCursor(Vector2Int size)//根据Prefab的尺寸改编Cursor大小
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            //cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }
    public void UpdatePosition(Vector3 position, bool validity)//移动Cursor和Preview 并且控制颜色
    {
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }

        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }
    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }
    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(
            position.x,
            position.y + previewYOffset,
            position.z);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;

        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }
    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;

        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }
}
