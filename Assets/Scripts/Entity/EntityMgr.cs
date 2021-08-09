using System.Collections.Generic;

public static class EntityMgr
{
    public static Player Player { get; set; }
    
    private static List<IEntity> entities = new List<IEntity>();
    
    public static void CreatePlayer(PlayerData playerData)
    {
        Player = new Player(playerData);
        entities.Add(Player);
    }

    public static void CreateMonsters(MonsterData[] monsterData)
    {
        if (monsterData == null) return;
    }

    public static void CreateMonster(MonsterData monsterData)
    {
        
    }
    
    public static List<IEntity> GetAllEntity()
    {
        return entities;
    }

    public static void Update()
    {
        for (var i = 0; i < entities.Count; ++i)
        {
            if (!entities[i].IsDestroy)
            {
                entities[i].Update();
            }
        }
        
        // 清理掉被标记为删除的Entity
        for (var i = entities.Count; i >= 0; --i)
        {
            if (entities[i].IsDestroy)
            {
                entities.RemoveAt(i);
            }
        }
    }
}