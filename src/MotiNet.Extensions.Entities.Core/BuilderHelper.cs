using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace MotiNet.Entities
{
    public static class BuilderHelper
    {
        public static void ConstructBuilder(object builder, ConstructorInfo constructor, IServiceCollection services, params Type[] types)
        {
            var parameterNames = constructor.GetParameters().Select(x => x.Name).ToArray();
            if (types.Length != parameterNames.Length - 1)
            {
                throw new ArgumentException(Resources.BuilderConstructionInvalidTypesLength);
            }
            for (var i = 0; i < parameterNames.Length; i++)
            {
                var name = parameterNames[i];
                var property = builder.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    throw new InvalidOperationException(string.Format(Resources.BuilderPropertyNotFound, name, builder.GetType().Name));
                }
                if (i == 0)
                {
                    property.SetValue(builder, services);
                }
                else
                {
                    property.SetValue(builder, types[i - 1]);
                }
            }
        }
    }
}
