using UnityEngine;

public class TimeStopEffect : IEffect, IUpdatable
{
    public bool IsSingle { get; private set; }
    public bool IsDestroy { get; private set; }
    
    public float Radius { get; set; }
    public float Duration { get; set; }
    public float DecayTime { get; set; }
    
    private GameObject gameObject { get; set; }
    private Transform transform { get; set; }
    private Renderer Renderer { get; set; }

    public TimeStopEffect(GameObject asset)
    {
        gameObject = asset;
        transform = asset.transform;
        Renderer = asset.GetComponent<Renderer>();
        IsSingle = true;
    }
    
    public void Update()
    {
        
    }
}