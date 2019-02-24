using Microsoft.Extensions.Options;

namespace MotiNet.Entities
{
    public class DailyIncrementalCodeGenerator<TEntity, TEntityManager> : IncrementalCodeGenerator<TEntity, TEntityManager>
        where TEntity : class
        where TEntityManager : class,
                               ITimeTrackedEntityManager<TEntity>,
                               ICodeBasedEntityManager<TEntity>
    {
        private readonly ILocalTime _localTime;

        public DailyIncrementalCodeGenerator(
            ICodeBasedEntityAccessor<TEntity> entityCodeAccessor,
            ILocalTime localTime,
            IOptions<IncrementalCodeGeneratorOptions<TEntity>> incrementalCodeGeneratorOptions)
        : base(entityCodeAccessor, incrementalCodeGeneratorOptions)
        => _localTime = localTime;

        public override string GenerateCode(object manager, TEntity entity)
        {
            var today = _localTime.Today;
            var year = today.Year.ToString();
            year = year.Substring(year.Length - 2);
            var month = today.Month.ToString("D2");
            var day = today.Day.ToString("D2");
            var prefix = $"{Prefix}{year}{month}{day}-";

            return GenerateCode(manager, entity, prefix, false);
        }
    }
}
