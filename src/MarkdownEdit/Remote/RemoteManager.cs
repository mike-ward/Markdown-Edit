using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace MarkdownEdit
{
    public class RemoteManager : IRemoteManager, IDisposable
    {
        private bool _disposed;
        private CompositionContainer _container;

        [Import(typeof(IRemoteProvider))]
        public IRemoteProvider _google;

        public RemoteManager()
        {
            var catalog = new AggregateCatalog();
            var basePath = Utility.AssemblyFolder();

            var googleDrivePath = Path.Combine(basePath, @"Extensions\GoogleDriveRemoteProvider");
            catalog.Catalogs.Add(new DirectoryCatalog(googleDrivePath, "GoogleDriveRemoteProvider.dll"));

            _container = new CompositionContainer(catalog);

            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public IRemoteProvider[] Providers { get; private set; }

        public IEnumerable<IRemoteProvider> LinkedRemoteProviders()
        {
            throw new NotImplementedException();
        }

        public Task LinkRemoteProvider(IRemoteProvider remote)
        {
            throw new NotImplementedException();
        }

        public string[] RemoteProviders()
        {
            return new[] { _google.DisplayName };
        }

        public Task UnlinkRemoteProvider(IRemoteProvider remote)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _container.Dispose();
            }
            _disposed = true;
        }
    }
}
