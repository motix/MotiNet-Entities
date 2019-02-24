using Microsoft.Extensions.Options;
using System;

namespace MotiNet.Entities
{
    public class LocalTime : ILocalTime
    {
        private readonly int _timeZone;

        public LocalTime(IOptions<LocalTimeOptions> options) => _timeZone = options.Value.TimeZone;

        public DateTime Now => DateTime.Now.ToUniversalTime().AddHours(_timeZone);

        public DateTime Today => Now.Date;
    }
}
