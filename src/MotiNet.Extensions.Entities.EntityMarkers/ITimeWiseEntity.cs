using System;

namespace MotiNet.Entities
{
    public interface ITimeWiseEntity
    {
        DateTime DataCreateDate { get; set; }

        DateTime DataLastModifyDate { get; set; }
    }
}
