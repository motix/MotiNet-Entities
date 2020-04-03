using Microsoft.Extensions.Options;

namespace MotiNet.Entities
{
    public class LocalTime<T> : LocalTime, ILocalTime<T>
    {
        public LocalTime(IOptions<LocalTimeOptions<T>> options) : base(options) { }
    }
}
