global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.Text;
using ReGuid.Commands;
using ReGuid.Options;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ReGuid
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.ReGuidString)]
    [ProvideOptionPage(typeof(OptionsProvider.GeneralOptionsPage), "ReGuid", "General", 0, 0, true)]
    [ProvideProfile(typeof(OptionsProvider.GeneralOptionsPage), "ReGuid", "General", 0, 0, true)]
    public sealed class ReGuidPackage : ToolkitPackage
    {
        // This method is called by Visual Studio to initialize the extension.
        // Event listeners should be added here. Commands, tool windows, settings,
        // and other things should be registered here.
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            GeneralOptionsModel.Saved += ApplyOptions;

            await this.RegisterCommandsAsync();

            ApplyOptions(await GeneralOptionsModel.GetLiveInstanceAsync());
        }

        private void ApplyOptions(GeneralOptionsModel generalOptions)
        {
            InsertGuidCommand.SetCreateNewGuidStringMethod(generalOptions.InsertionFormat, generalOptions.InsertionCase);
            ReplaceGuidsCommand.SetCreateNewGuidStringMethod(generalOptions);
            ReplaceGuidsCommand.SetGuidMatchExpression(generalOptions);
        }

        internal static IEnumerable<SnapshotSpan> GetCurrentSelections(DocumentView docView)
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

        internal static string CapitalizeFormatXGuidString(string uncapitalizedGuidString)
        {
            StringBuilder output = new StringBuilder(uncapitalizedGuidString);

            for (int i = 0; i < output.Length; ++i)
            {
                if (char.IsLetter(output[i]) && output[i] != 'x')
                {
                    output[i] = char.ToUpper(output[i]);
                }
            }

            return output.ToString();
        }
    }
}