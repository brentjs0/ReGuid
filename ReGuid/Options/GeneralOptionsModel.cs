using System.ComponentModel;

namespace ReGuid.Options
{
    public class GeneralOptionsModel : BaseOptionModel<GeneralOptionsModel>
    {
        [Category("Insert")]
        [DisplayName("Case")]
        [Description("Whether to insert GUIDs in uppercase or lowercase.")]
        [DefaultValue(InsertionCases.Lowercase)]
        [TypeConverter(typeof(EnumConverter))]
        public InsertionCases InsertionCase { get; set; } = InsertionCases.Lowercase;

        [Category("Insert")]
        [DisplayName("Format")]
        [Description("How to format inserted GUIDs. These options correspond to the C# Guid.ToString() format specifiers D, N, B, P, and X.")]
        [DefaultValue(InsertionFormats.D_WithHyphens)]
        [TypeConverter(typeof(EnumConverter))]
        public InsertionFormats InsertionFormat { get; set; } = InsertionFormats.D_WithHyphens;

        [Category("Replace")]
        [DisplayName("Case")]
        [Description("Whether to transform to uppercase, transform to lowercase, or attempt to perserve case when replacing GUIDs.")]
        [DefaultValue(ReplacementCases.Preserve)]
        [TypeConverter(typeof(EnumConverter))]
        public ReplacementCases ReplacementCase { get; set; } = ReplacementCases.Preserve;

        [Category("Replace")]
        [DisplayName("Format")]
        [Description("The format in which to render replaced GUIDs. 'Preserve' will keep the existing format. Other options correspond to the C# Guid.ToString() format specifiers.")]
        [DefaultValue(ReplacementFormats.Preserve)]
        [TypeConverter(typeof(EnumConverter))]
        public ReplacementFormats ReplacementFormat { get; set; } = ReplacementFormats.Preserve;

        [Category("Replace")]
        [DisplayName("Replace Format D GUIDs")]
        [Description("Whether to replace GUIDs that are written in this format: 00000000-0000-0000-0000-000000000000")]
        [DefaultValue(true)]
        public bool ReplaceFormatDGuids { get; set; } = true;

        [Category("Replace")]
        [DisplayName("Replace Format N GUIDs")]
        [Description("Whether to replace GUIDs that are written in this format: 00000000000000000000000000000000")]
        [DefaultValue(true)]
        public bool ReplaceFormatNGuids { get; set; } = true;

        [Category("Replace")]
        [DisplayName("Replace Format B GUIDs")]
        [Description("Whether to replace GUIDs that are written in this format: {00000000-0000-0000-0000-000000000000}")]
        [DefaultValue(true)]
        public bool ReplaceFormatBGuids { get; set; } = true;

        [Category("Replace")]
        [DisplayName("Replace Format P GUIDs")]
        [Description("Whether to replace GUIDs that are written in this format: (00000000-0000-0000-0000-000000000000)")]
        [DefaultValue(true)]
        public bool ReplaceFormatPGuids { get; set; } = true;

        [Category("Replace")]
        [DisplayName("Replace Format X GUIDs")]
        [Description("Whether to replace GUIDs that are written in this format: {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}")]
        [DefaultValue(true)]
        public bool ReplaceFormatXGuids { get; set; } = true;
    }
}
