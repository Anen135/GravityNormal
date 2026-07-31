
using System;
public interface IItem
{
    string Id { get; }
    string DisplayName { get; }
    string Description { get; }
    bool CanUse {get;}
    void Use(object user);
    event Action<IItem, object> OnUsed;
}
