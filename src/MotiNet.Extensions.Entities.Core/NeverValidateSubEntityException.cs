using System;

namespace MotiNet.Entities
{
    public class NeverValidateSubEntityException<TSubEntity, TManager> : InvalidOperationException
        where TSubEntity : class
        where TManager : class
    {
        public NeverValidateSubEntityException()
            // TODO:: Localization
            : base(string.Format("{0} should never validates {1}.", typeof(TManager).Name, typeof(TSubEntity).Name)) { }
    }
}
