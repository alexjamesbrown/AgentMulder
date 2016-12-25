using System;

namespace AgentMulder.ReSharper.Tests
{
    /// <summary>
    /// Creates an isolated AppDomain with the specified entry point type.
    /// </summary>
    /// <typeparam name="TEntryPoint">The type of the entry point to load into the isolated AppDomain.</typeparam>
    public sealed class IsolationChamber<TEntryPoint> : IDisposable where TEntryPoint : MarshalByRefObject
    {
        private AppDomain domain;

        public IsolationChamber()
        {
            domain = AppDomain.CreateDomain("Isolated:" + Guid.NewGuid(), null,
                AppDomain.CurrentDomain.SetupInformation);

            var type = typeof(TEntryPoint);

            EntryPoint = (TEntryPoint)domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
        }

        /// <summary>
        /// Gets the AppDomain entry point instance.
        /// </summary>
        public TEntryPoint EntryPoint { get; }

        public void Dispose()
        {
            if (domain == null)
            {
                return;
            }

            AppDomain.Unload(domain);
            domain = null;
        }
    }
}