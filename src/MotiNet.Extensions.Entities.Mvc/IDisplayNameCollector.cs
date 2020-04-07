using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq.Expressions;

namespace MotiNet.Entities.Mvc
{
    public interface IDisplayNameCollector<TModel>
    {
        IDisplayNameCollector<TModel> Collect<TResult>(Expression<Func<TModel, TResult>> expression, IHtmlHelper<TModel> htmlHelper);

        IHtmlContent ToJson();
    }
}
