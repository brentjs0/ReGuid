global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ReGuid
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.ReGuidString)]
    public sealed class ReGuidPackage : ToolkitPackage
    {
        // This method is called by Visual Studio to initialize the extension.
        // Event listeners should be added here. Commands, tool windows, settings,
        // and other things should be registered here.
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
        }

        public static async Task<IEnumerable<SnapshotSpan>> GetCurrentSelectionsAsync(DocumentView docView)
        {
            if (docView?.TextView?.Selection?.SelectedSpans == null)
            {
                return new SnapshotSpan[0];
            }

            return docView.TextView
                .Selection?
                .SelectedSpans?
                .OrderBy(x => x.Start.Position);
        }

        public static string GetNewGuid()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }
    }
}