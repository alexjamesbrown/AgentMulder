using System.Collections;
using System.IO;
using System.Linq;
using AgentMulder.ReSharper.Domain.Containers;
using JetBrains.Annotations;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework.Utils;
using NUnit.Framework;

namespace AgentMulder.ReSharper.Tests
{
    // The ReSharper SDK 2016.3 onward broke the tests - it seems like consequent runs of test cases will screw with the semantic analysis (type resolution in particular)
    // Issue has been reported at https://youtrack.jetbrains.com/issue/RSRP-462277, but no response so far
    // However, running the test cases one per AppDomaion works around the issue
    // This incurs a heavy performance penalty on the tests, but at least they work properly
    // Hopefully, we will be able to revert this once JetBrains resolve the issue on their end

    [TestNetFramework46]
    [TestFileExtension(".cs")]
    public abstract class AgentMulderTestBase<TContainerInfo> : BaseTestWithSingleProject
        where TContainerInfo : IContainerInfo, new()
    {
        protected virtual TContainerInfo ContainerInfo => new TContainerInfo();

// ReSharper disable MemberCanBePrivate.Global
        protected IEnumerable TestCases
// ReSharper restore MemberCanBePrivate.Global
        {
            [UsedImplicitly]
            get
            {
                TestUtil.SetHomeDir(GetType().Assembly);
                var testCasesDirectory = new DirectoryInfo(SolutionItemsBasePath.FullPath);
                return testCasesDirectory.EnumerateFiles("*.cs").Select(info => new TestCaseData(info.Name)).ToList();
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void Test(string fileName)
        {
            // runs the test in a separate AppDomain
            // the using block will dispose of the temporary AppDomain
            using (var i = new IsolationChamber<IsolatedTestEntryPoint>())
            {
                i.EntryPoint.Test(fileName, ContainerInfo);
            }
        }
    }
}