using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rogero.ReactiveProperty.Tests
{
    public class DecimalTests
    {
        [Fact()]
        [Trait("Category", "Instant")]
        public void CreateDecimalReactiveProperty()
        {
            var property = new ReactiveProperty<decimal>();
            var result = property.Value;
        }
    }
}