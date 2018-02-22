﻿using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IEntityCodeGenerator<in TEntity>
        where TEntity : class
    {
        Task<string> GenerateCodeAsync(object manager, TEntity entity, CancellationToken cancellationToken);
    }
}
