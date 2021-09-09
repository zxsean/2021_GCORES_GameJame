using System;
using UnityEngine;

public static class CameraMgr
{
    public static Camera Camera { get; private set; }
    public static Transform CameraTrans { get; private set; }

    private static Transform FollowTarget { get; set; }

    private static Vector3 MoveInitPos { get; set; }
    private static float MoveOffset { get; set; }
    private static Vector3 MoveTarget { get; set; }
    private static Action MoveFinished { get; set; }

    // private static float Radius { get; set; }
    // private static float MoveSpeed { get; set; }
    // private static float OffsetX { get; set; }

    public static void Init()
    {
        Camera = Camera.main;
        CameraTrans = Camera.transform;
        // Radius = 5.0f;
        // MoveSpeed = 50.0f;
    }

    public static void Follow(Transform trans)
    {
        FollowTarget = trans;
        MoveTarget = default;
    }

    public static void UnFollow()
    {
        FollowTarget = null;
    }

    public static void Move(Vector3 pos, Action onFinished = null)
    {
        FollowTarget = null;
        MoveInitPos = CameraTrans.localPosition;
        MoveTarget = pos;
        MoveOffset = 0.0f;
        MoveFinished = onFinished;
    }

    public static void SetSize(float size)
    {
        Camera.orthographicSize = size;
    }

    public static void Update()
    {
        // 移动目标
        if (MoveTarget != default)
        {
            MoveOffset += Time.deltaTime;
            var curPos = Vector3.Lerp(MoveInitPos, MoveTarget, MoveOffset);
            var curCamPos = CameraTrans.localPosition;
            curCamPos.x = curPos.x;
            curCamPos.y = curPos.y;
            CameraTrans.localPosition = curCamPos;
            if (MoveOffset >= 1.0f)
            {
                MoveOffset = 0.0f;
                MoveTarget = default;
                MoveInitPos = default;
                MoveFinished?.Invoke();
            }

            return;
        }

        // 跟随目标
        if (FollowTarget == null) return;

        var pos = FollowTarget.localPosition;
        var camPos = CameraTrans.localPosition;
        camPos.x = pos.x;
        camPos.y = pos.y;
        CameraTrans.localPosition = camPos;

        // if (Input.GetKey(KeyCode.Keypad6))
        // {
        //     OffsetX += MoveSpeed * Time.deltaTime;
        //     camPos.x = Mathf.Lerp(camPos.x, camPos.x + Radius, OffsetX / Radius);
        //     CameraTrans.localPosition = camPos;
        // }
        // else
        // {
        //     OffsetX = 0;
        // }
    }

    public static void Clear()
    {
    }
}