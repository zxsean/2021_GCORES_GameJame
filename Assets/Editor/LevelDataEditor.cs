using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private LevelData levelData;
    private Transform groundRoot;

    private void OnEnable()
    {
        levelData = target as LevelData;
        groundRoot = levelData.transform.Find("Ground");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AutoGeneration"))
        {
            // 自动生成地面
            // 清空之前的
            for (var i = groundRoot.childCount - 1; i >= 0; --i)
            {
                DestroyImmediate(groundRoot.GetChild(i).gameObject);
            }
            var ground = levelData.ground;
            var centerX = levelData.cols / 2;
            var centerY = levelData.rows / 2;
            var offsetX = levelData.cols % 2 == 0 ? 0.44f : 0;
            var offsetY = levelData.rows % 2 == 0 ? -0.34f : 0;
            for (var i = 0; i < levelData.rows; ++i)
            {
                for (var j = 0; j < levelData.cols; ++j)
                {
                    var posX = (j - centerX) * 0.88f + offsetX;
                    var posY = (centerY - i) * 0.68f + offsetY;
                    var ins = Instantiate(ground);
                    ins.GetComponent<Renderer>().sortingOrder = i;
                    var trans = ins.transform;
                    trans.SetParent(groundRoot, false);
                    var pos = trans.localPosition;
                    pos.x = posX;
                    pos.y = posY;
                    trans.localPosition = pos;
                }
            }
            
            var first = groundRoot.GetChild(0);
            var last = groundRoot.GetChild(groundRoot.childCount - 1);
            var firstBounds = first.GetComponent<Renderer>().bounds;
            var lastBounds = last.GetComponent<Renderer>().bounds;
            levelData.width = lastBounds.max.x - firstBounds.min.x;
            levelData.height = firstBounds.max.y - lastBounds.min.y;
        }
    }
}