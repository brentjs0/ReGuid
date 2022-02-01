using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReGuid
{
    [Command(PackageIds.ReplaceGuidsCommand)]

    /// <summary>
    /// This is the command handler for the <see cref="ReplaceGuidsCommand"/> command defined
    /// in VSCommandTable.vsct. It controls what happens when the command is executed.
    /// </summary>
    internal sealed class ReplaceGuidsCommand : BaseCommand<ReplaceGuidsCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();

            int insertionOffset = 0;
            IEnumerable<SnapshotSpan> selectionSpans = await ReGuidPackage.GetCurrentSelectionsAsync(docView);
            foreach (SnapshotSpan selectionSpan in selectionSpans)
            {
                if (!selectionSpan.IsEmpty)
                {
                    string replacementText = ReplaceGuids(selectionSpan.GetText());

                    Span replacementSpan = new Span(
                        selectionSpan.Start + insertionOffset,
                        selectionSpan.Length);

                    docView.TextBuffer.Replace(replacementSpan, replacementText);

                    insertionOffset += replacementText.Length - selectionSpan.Length;
                }
            }
        }

        private static Regex guidExpression =
            new Regex(@"([a-fA-F\d]{8}-?[a-fA-F\d]{4}-?[a-fA-F\d]{4}-?[a-fA-F\d]{4}-?[a-fA-F\d]{12})", RegexOptions.Multiline);

        private static string ReplaceGuids(string input)
        {
            StringBuilder output = new StringBuilder(input);

            IEnumerable<Match> matches = guidExpression.Matches(input)
                .Cast<Match>()
                .OrderByDescending(x => x.Index);

            foreach (Match match in matches)
            {
                output.Remove(match.Index, match.Length);
                output.Insert(match.Index, ReGuidPackage.GetNewGuid());
            }

            return output.ToString();
        }
    }
}
