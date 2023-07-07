using System.Collections;
using System.Collections.Generic;
using ExplogineMonoGame.Data;

namespace GMTK23;

public class EntityCollection : IEnumerable<Entity>
{
    private readonly List<Entity> _content = new();

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
        return entity;
    }

    public Entity RemoveImmediate(Entity entity)
    {
        _content.Remove(entity);
        return entity;
    }
}
