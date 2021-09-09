/// <summary>
///     Entity接口
/// </summary>
public interface IPlayer : ITriggerGrid
{
    bool IsActive { get; }
}

public interface IMonster
{
}

public interface IEntity
{
    int Hp { get; set; }
}