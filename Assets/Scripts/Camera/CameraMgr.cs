using UnityEngine;

public static class CameraMgr
{
    private static Camera Camera;
    private static Transform CameraTrans { get; set; }
    
    private static Transform FollowTarget { get; set; }
    
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
    }

    public static void UnFollow()
    {
        FollowTarget = null;
    }
    
    public static void Update()
    {
        // 跟随目标
        if (FollowTarget == null)
        {
            return;
        }

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