using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace MotiNet.Entities
{
    public abstract class BuilderBase
    {
        protected BuilderBase(IServiceCollection services)
            => Services = services;

        public IServiceCollection Services { get; private set; }

        public Dictionary<string, Type> DomainSpecificTypes = new Dictionary<string, Type>();
    }
}
