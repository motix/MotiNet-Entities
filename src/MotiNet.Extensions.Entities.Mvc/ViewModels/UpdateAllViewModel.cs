using System;
using System.Collections.Generic;

namespace MotiNet.Entities.Mvc.ViewModels
{
    public class UpdateAllViewModel<TKey, TEntityViewModel>
        where TKey : IEquatable<TKey>
        where TEntityViewModel : class
    {
        public ICollection<UpdateAllEntryViewModel<TKey, TEntityViewModel>> All { get; set; }
     
        public bool RemoveExtra { get; set; }

        public object RemoveExtraParameters { get; set; }
    }
}
