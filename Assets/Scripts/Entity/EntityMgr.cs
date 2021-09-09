using System.Collections.Generic;
using UnityEngine;

public static class EntityMgr
{
    private static readonly List<IEntity> entities = new List<IEntity>();
    public static Player Player { get; set; }

    public static void CreateEntities(Transform assets)
    {
        for (var i = 0; i < assets.childCount; ++i) CreateEntity(assets.GetChild(i));
    }

    public static void CreateEntity(Transform asset)
    {
        var data = asset.GetComponent<GridData>();
        IEntity entity = null;
        if (data is PlayerData)
        {
            entity = new Player(asset.gameObject);
            Player = (Player) entity;
        }
        else if (data is MonsterData)
        {
            entity = new Monster(asset.gameObject);
        }
        else if (data is BulletMonsterData)
        {
            entity = new BulletMonster(asset.gameObject);
        }
        else if (data is BossData)
        {
            entity = new Boss(asset.gameObject);
        }

        entities.Add(entity);
    }

    public static T GetOrCreateEntity<T>() where T : IEntity, new()
    {
        for (var i = 0; i < entities.Count; ++i)
            if (entities[i] is T)
                return (T) entities[i];

        var entity = new T();
        entities.Add(entity);
        return entity;
    }

    public static void DestroyEntity()
    {
    }

    public static void CreateMonsters(MonsterData[] monsterData)
    {
        if (monsterData == null) return;
    }

    public static void CreateMonster(MonsterData monsterData)
    {
    }

    public static void GetAll<T>(out List<T> list)
    {
        list = new List<T>();
        for (var i = 0; i < entities.Count; ++i)
            if (entities[i] is T)
                list.Add((T) entities[i]);
    }

    public static List<IEntity> GetAllEntity()
    {
        return entities;
    }

    public static void Update()
    {
        for (var i = 0; i < entities.Count; ++i)
        {
            var entity = entities[i];
            if (entity is IUpdatable updatable) updatable.Update();
        }

        // 清理掉被标记为删除的Entity
        for (var i = entities.Count - 1; i >= 0; --i)
            if (entities[i] is IUpdatable updatable && updatable.IsDestroy)
                entities.RemoveAt(i);
    }

    public static void Clear()
    {
        entities.Clear();
    }
}