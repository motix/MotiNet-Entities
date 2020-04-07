using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities.Mvc
{
    public class DisplayNameCollector<TModel> : IDisplayNameCollector<TModel>
    {
        private readonly Dictionary<string, string> _displayNames = new Dictionary<string, string>();
        private readonly IJsonHelper _jsonHelper;

        public DisplayNameCollector(IJsonHelper jsonHelper)
            => _jsonHelper = jsonHelper ?? throw new ArgumentNullException(nameof(jsonHelper));

        public IDisplayNameCollector<TModel> Collect<TResult>(Expression<Func<TModel, TResult>> expression, IHtmlHelper<TModel> htmlHelper)
        {
            var key = ((MemberExpression)expression.Body).Member.Name;
            var displayName = htmlHelper.DisplayNameFor(expression);
            _displayNames.Add(key, displayName);

            return this;
        }

        public IHtmlContent ToJson()
        {
            return _jsonHelper.Serialize(_displayNames);
        }
    }
}
