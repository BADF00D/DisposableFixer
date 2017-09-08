using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.MiscTests
{
    [TestFixture]
    public class If_resources_get_loaded 
    {

        [Test, TestCaseSource(nameof(TestCases))]
        public void Ressource_should_be_loaded(string ressource)
        {
            ressource.Should().NotBeNullOrWhiteSpace();
        }


        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                var staticProperties = typeof (Resources)
                    .GetProperties(BindingFlags.Static|BindingFlags.NonPublic)
                    .Where(prop => prop.PropertyType == typeof(string))
                    .ToArray();

                foreach (var staticProperty in staticProperties)
                {
                    var name = staticProperty.Name;
                    var value = staticProperty.GetValue(null);
                    yield return new TestCaseData(value)
                        .SetName(name);
                }    
            }
        }
    }
}