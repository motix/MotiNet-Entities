using System;

namespace MotiNet.Entities
{
    public interface ILocalTime
    {
        DateTime Now { get; }

        DateTime Today { get; }
    }
}
