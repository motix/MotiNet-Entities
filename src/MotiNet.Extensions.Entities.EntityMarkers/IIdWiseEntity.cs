using System;

namespace MotiNet.Entities
{
    public interface IIdWiseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}
