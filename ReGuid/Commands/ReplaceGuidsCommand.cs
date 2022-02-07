using Microsoft.VisualStudio.Text;
using ReGuid.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReGuid.Commands
{
    [Command(PackageIds.ReplaceGuidsCommand)]

    /// <summary>
    /// This is the command handler for the <see cref="ReplaceGuidsCommand"/> command defined
    /// in VSCommandTable.vsct. It controls what happens when the command is executed.
    /// </summary>
    internal sealed class ReplaceGuidsCommand : BaseCommand<ReplaceGuidsCommand>
    {
        private static Func<Group, string> createNewGuidString { get; set; }

        internal static void SetCreateNewGuidStringMethod(GeneralOptionsModel generalOptions)
        {
            ApplyFormat applyFormat = GetApplyFormatMethod(generalOptions);
            ApplyCase applyCase = GetApplyCaseMethod(generalOptions);

            createNewGuidString = (g) => applyCase(applyFormat(Guid.NewGuid(), g.Name), g.Value, g.Name);
        }

        #region ApplyFormat
        private delegate string ApplyFormat(Guid newGuid, string originalFormatSpecifier = null);

        private static ApplyFormat GetApplyFormatMethod(GeneralOptionsModel generalOptions)
        {
            if (generalOptions.ReplacementFormat == ReplacementFormats.Preserve)
            {
                return (n, o) => n.ToString(o);
            }

            string configuredFormatSpecifier = generalOptions.ReplacementFormat.ToString().Split('_')[0];
            return (n, o) => n.ToString(configuredFormatSpecifier);
        }
        #endregion

        #region ApplyCase
        private delegate string ApplyCase(string newGuidString, string originalGuidString = null, string originalFormatSpecifier = null);

        private static ApplyCase GetApplyCaseMethod(GeneralOptionsModel generalOptions)
        {
            if (generalOptions.ReplacementFormat == ReplacementFormats.Preserve)
            {
                switch (generalOptions.ReplacementCase)
                {
                    case ReplacementCases.Preserve:
                        return ApplyPreservedCasePreserveFormat;
                    case ReplacementCases.Uppercase:
                        return ApplyUppercasePreserveFormat;
                    default:
                        return ApplyLowercase;
                }
            }
            else if (generalOptions.ReplacementFormat == ReplacementFormats.X_WithHexadecimalPrefixesCommasAndNestedCurlyBraces)
            {
                switch (generalOptions.ReplacementCase)
                {
                    case ReplacementCases.Preserve:
                        return ApplyPreservedCaseFormatX;
                    case ReplacementCases.Uppercase:
                        return ApplyUppercaseFormatX;
                    default:
                        return ApplyLowercase;
                }
            }
            else
            {
                switch (generalOptions.ReplacementCase)
                {
                    case ReplacementCases.Preserve:
                        return ApplyPreservedCaseNotFormatX;
                    case ReplacementCases.Uppercase:
                        return ApplyUppercaseNotFormatX;
                    default:
                        return ApplyLowercase;
                }
            }
        }
        private static string ApplyLowercase(string newGuidString, string originalGuidString = null, string originalFormatSpecifier = null) => newGuidString;

        private static string ApplyUppercaseNotFormatX(string newGuidString, string originalGuidString = null, string originalFormatSpecifier = null) => newGuidString.ToUpper();

        private static string ApplyUppercaseFormatX(string newGuidString, string originalGuidString = null, string originalFormatSpecifier = null) => ReGuidPackage.CapitalizeFormatXGuidString(newGuidString);

        private static string ApplyUppercasePreserveFormat(string newGuidString, string originalGuidString = null, string originalFormatSpecifier = null)
        {
            return originalFormatSpecifier == "X"
                ? ApplyUppercaseFormatX(newGuidString)
                : ApplyUppercaseNotFormatX(newGuidString);
        }

        private static string ApplyPreservedCaseNotFormatX(string newGuidString, string originalGuidString = null, string originalFormatSpecifier = null)
        {
            return IsUppercase(originalGuidString)
                ? ApplyUppercaseNotFormatX(newGuidString)
                : ApplyLowercase(newGuidString);
        }

        private static string ApplyPreservedCaseFormatX(string newGuidString, string originalGuidString = null, string originalFormatSpecifier = null)
        {
            return IsUppercase(originalGuidString)
                ? ApplyUppercaseFormatX(newGuidString)
                : ApplyLowercase(newGuidString);
        }

        private static string ApplyPreservedCasePreserveFormat(string newGuidString, string originalGuidString = null, string originalFormatSpecifier = null)
        {
            return IsUppercase(originalGuidString)
                ? ApplyUppercasePreserveFormat(newGuidString, originalGuidString, originalFormatSpecifier)
                : ApplyLowercase(newGuidString);
        }

        private static bool IsUppercase(string str) => str.Any(c => char.IsUpper(c));
        #endregion

        private static Regex guidMatchExpression { get; set; }

        private static HashSet<string> guidFormatSpecifiersBeingReplaced;

        internal static void SetGuidMatchExpression(GeneralOptionsModel generalOptions)
        {
            HashSet<string> alternatives = new HashSet<string>();
            guidFormatSpecifiersBeingReplaced = new HashSet<string>();

            if (generalOptions.ReplaceFormatXGuids)
            {
                alternatives.Add(@"(?<X>{0x[a-fA-F\d]{8},0x[a-fA-F\d]{4},0x[a-fA-F\d]{4},{0x[a-fA-F\d]{2},0x[a-fA-F\d]{2},0x[a-fA-F\d]{2},0x[a-fA-F\d]{2},0x[a-fA-F\d]{2},0x[a-fA-F\d]{2},0x[a-fA-F\d]{2},0x[a-fA-F\d]{2}}})");
                guidFormatSpecifiersBeingReplaced.Add("X");
            }

            if (generalOptions.ReplaceFormatBGuids)
            {
                alternatives.Add(@"(?<B>{[a-fA-F\d]{8}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{12}})");
                guidFormatSpecifiersBeingReplaced.Add("B");
            }

            if (generalOptions.ReplaceFormatPGuids)
            {
                alternatives.Add(@"(?<P>\([a-fA-F\d]{8}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{12}\))");
                guidFormatSpecifiersBeingReplaced.Add("P");
            }

            if (generalOptions.ReplaceFormatDGuids)
            {
                alternatives.Add(@"(?<D>(?<![\(\{])[a-fA-F\d]{8}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{12}|[a-fA-F\d]{8}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{12}(?![\)\}]))");
                guidFormatSpecifiersBeingReplaced.Add("D");
            }

            if (generalOptions.ReplaceFormatNGuids)
            {
                alternatives.Add(@"(?<N>[a-fA-F\d]{32})");
                guidFormatSpecifiersBeingReplaced.Add("N");
            }

            string pattern = alternatives.Any()
                ? string.Join("|", alternatives)
                : "(?!)";

            guidMatchExpression = new Regex(pattern, RegexOptions.ExplicitCapture);
        }


        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            if (!guidFormatSpecifiersBeingReplaced.Any())
            {
                return;
            }

            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();

            int insertionOffset = 0;
            IEnumerable<SnapshotSpan> selectionSpans = ReGuidPackage.GetCurrentSelections(docView);
            foreach (SnapshotSpan selectionSpan in selectionSpans)
            {
                if (!selectionSpan.IsEmpty)
                {
                    string selectionText = selectionSpan.GetText();
                    string replacementText = ReplaceGuids(selectionText);

                    if (replacementText != selectionText)
                    {
                        Span replacementSpan = new Span(
                            selectionSpan.Start + insertionOffset,
                            selectionSpan.Length);

                        docView.TextBuffer.Replace(replacementSpan, replacementText);

                        insertionOffset += replacementText.Length - selectionSpan.Length;
                    }
                }
            }
        }

        private static string ReplaceGuids(string input)
        {
            StringBuilder output = new StringBuilder(input);

            foreach (Group group in GetGroupsToReplaceLastToFirst(input))
            {
                output.Remove(group.Index, group.Length);
                output.Insert(group.Index, createNewGuidString(group));
            }

            return output.ToString();
        }

        private static IEnumerable<Group> GetGroupsToReplaceLastToFirst(string input)
        {
            MatchCollection matches = guidMatchExpression.Matches(input);
            for (int i = matches.Count - 1; i >= 0; --i)
            {
                foreach (Group group in matches[i].Groups)
                {
                    if (group.Length > 0 && guidFormatSpecifiersBeingReplaced.Contains(group.Name))
                    {
                        yield return group;
                        break;
                    }
                }
            }
        }
    }
}
