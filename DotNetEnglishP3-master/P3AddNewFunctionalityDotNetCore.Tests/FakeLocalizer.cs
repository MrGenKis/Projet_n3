using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using System.Collections.Generic;
using System.Globalization;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class FakeLocalizer : IStringLocalizer<ProductService>
    {
        public LocalizedString this[string name]
            => new LocalizedString(name, name);

        public LocalizedString this[string name, params object[] arguments]
            => new LocalizedString(name, name);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return new List<LocalizedString>();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this;
        }
    }
}
