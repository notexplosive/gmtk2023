using System.Collections;
using System.Collections.Generic;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;

namespace GMTK23;

public class EntityCollection : IEnumerable<Entity>, IUpdateHook
{
    private readonly List<Entity> _content = new();
    private readonly World _world;

    public EntityCollection(World world)
    {
        _world = world;
    }

    public DeferredActions DeferredActions { get; } = new();

    public IEnumerator<Entity> GetEnumerator()
    {
        return _content.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Entity AddImmediate(Entity entity)
    {
        _content.Add(entity);
        entity.World = _world;
        entity.Awake();
        return entity;
    }

    public Entity RemoveImmediate(Entity entity)
    {
        entity.FlaggedAsDead = true;
        _content.Remove(entity);
        return entity;
    }

    public void Update(float dt)
    {
        foreach (var entity in _content)
        {
            entity.Update(dt);
        }
        
        DeferredActions.RunAllAndClear();
    }
}