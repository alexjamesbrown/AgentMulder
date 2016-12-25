using System;
using AgentMulder.ReSharper.Domain.Containers;
using JetBrains.Annotations;

namespace AgentMulder.ReSharper.Tests
{
    /// <summary>
    ///  Serves as the entriyt point into the AppDomain the test will be run in.
    /// </summary>
    /// <remarks>Marshall by reference object</remarks>
    [UsedImplicitly]
    public class IsolatedTestEntryPoint : MarshalByRefObject
    {
        public void Test<T>(string filename, T info) where T : IContainerInfo, new()
        {
            var test = new TestInvocator<T>();
            test.Test(filename, info);
        }
    }
}