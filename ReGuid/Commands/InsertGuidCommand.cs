using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReGuid
{
    [Command(PackageIds.InsertGuidCommand)]

    /// <summary>
    /// This is the command handler for the <see cref="InsertGuidCommand"/> command defined
    /// in VSCommandTable.vsct. It controls what happens when the command is executed.
    /// </summary>
    internal sealed class InsertGuidCommand : BaseCommand<InsertGuidCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();

            int insertionOffset = 0;
            IEnumerable<SnapshotSpan> selectionSpans = await ReGuidPackage.GetCurrentSelectionsAsync(docView);
            foreach (SnapshotSpan selectionSpan in selectionSpans)
            {
                string replacementText = ReGuidPackage.GetNewGuid();

                Span replacementSpan = new Span(
                    selectionSpan.Start + insertionOffset,
                    selectionSpan.Length);

                docView.TextBuffer.Replace(replacementSpan, replacementText);

                insertionOffset += replacementText.Length - selectionSpan.Length;
            }
        }
    }
}
