using Microsoft.VisualStudio.Text;
using ReGuid.Options;
using System.Collections.Generic;

namespace ReGuid.Commands
{
    [Command(PackageIds.InsertGuidCommand)]

    /// <summary>
    /// This is the command handler for the <see cref="InsertGuidCommand"/> command defined
    /// in VSCommandTable.vsct. It controls what happens when the command is executed.
    /// </summary>
    internal sealed class InsertGuidCommand : BaseCommand<InsertGuidCommand>
    {
        private static Func<string> createNewGuidString { get; set; }

        internal static void SetCreateNewGuidStringMethod(InsertionFormats insertionFormat, InsertionCases insertionCase)
        {
            string formatSpecifier = insertionFormat.ToString().Split('_')[0];

            if (insertionCase == InsertionCases.Lowercase)
            {
                createNewGuidString = () => Guid.NewGuid().ToString(formatSpecifier);
            }
            else if (insertionFormat == InsertionFormats.X_WithHexadecimalPrefixesCommasAndNestedCurlyBraces)
            {
                createNewGuidString = () => ReGuidPackage.CapitalizeFormatXGuidString(Guid.NewGuid().ToString(formatSpecifier));
            }
            else
            {
                createNewGuidString = () => Guid.NewGuid().ToString(formatSpecifier).ToUpper();
            }
        }

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();

            int insertionOffset = 0;
            IEnumerable<SnapshotSpan> selectionSpans = ReGuidPackage.GetCurrentSelections(docView);
            foreach (SnapshotSpan selectionSpan in selectionSpans)
            {
                string replacementText = createNewGuidString();

                Span replacementSpan = new Span(
                    selectionSpan.Start + insertionOffset,
                    selectionSpan.Length);

                docView.TextBuffer.Replace(replacementSpan, replacementText);

                insertionOffset += replacementText.Length - selectionSpan.Length;
            }
        }
    }
}
