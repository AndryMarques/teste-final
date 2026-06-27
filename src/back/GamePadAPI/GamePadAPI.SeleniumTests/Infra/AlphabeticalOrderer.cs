using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GamePadAPI.SeleniumTests.Infra
{
    /// <summary>
    /// Ordena os casos de teste pelo nome do método (CT11, CT12, ...),
    /// garantindo que o vídeo siga a narrativa na ordem correta.
    /// </summary>
    public class AlphabeticalOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
            => testCases.OrderBy(tc => tc.TestMethod.Method.Name);
    }
}
