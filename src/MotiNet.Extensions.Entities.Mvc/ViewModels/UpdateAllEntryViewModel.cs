using System;

namespace MotiNet.Entities.Mvc.ViewModels
{
    public class UpdateAllEntryViewModel<TKey, TEntityViewModel>
        where TKey : IEquatable<TKey>
        where TEntityViewModel : class
    {
        public TKey Id { get; set; }

        public TEntityViewModel ViewModel { get; set; }

        public GenericResult Result { get; set; }
    }
}
