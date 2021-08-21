using UnityEngine;

public static class CameraMgr
{
    private static Camera Camera;
    private static Transform CameraTrans { get; set; }
    
    private static Transform FollowTarget { get; set; }

    public static void Init()
    {
        Camera = Camera.main;
        CameraTrans = Camera.transform;
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

        var pos = FollowTarget.position;
        var camPos = CameraTrans.position;
        camPos.x = pos.x;
        camPos.y = pos.y;
        CameraTrans.position = camPos;
    }

    public static void Clear()
    {
        
    }
}