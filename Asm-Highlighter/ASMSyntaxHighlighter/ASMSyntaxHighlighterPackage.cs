global using System;
global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using Task = System.Threading.Tasks.Task;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using AssemblySyntaxHighlighter;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ASMSyntaxHighlighter
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.ASMSyntaxHighlighterString)]
    public sealed class ASMSyntaxHighlighterPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
        }
    }
}

namespace ASMSyntaxHighlighter
{
    internal class AssemblyClassifier : IClassifier
    {
        private readonly IClassificationTypeRegistryService _classificationRegistry;

        public AssemblyClassifier(IClassificationTypeRegistryService registry)
        {
            _classificationRegistry = registry;
        }

        // Implement the remaining IClassifier methods here
    }
}



[Export(typeof(ClassifierProvider))]
[ContentType("code")]
internal class AssemblyClassifierProvider : IClassifierProvider
{
    [Import]
    private readonly IClassificationTypeRegistryService _classificationRegistry = null;

    public IClassifier GetClassifier(ITextBuffer buffer)
    {
        return buffer.Properties.GetOrCreateSingletonProperty(() => new AssemblyClassifier(_classificationRegistry, buffer));
    }
}

[Export(typeof(EditorFormatDefinition))]
[ClassificationType(ClassificationTypeNames = "AssemblyOpcode")]
[Name("AssemblyOpcode")]
internal sealed class AssemblyOpcodeFormat : ClassificationFormatDefinition
{
    public AssemblyOpcodeFormat()
    {
        DisplayName = "Assembly Opcode";
        ForegroundColor = Colors.Blue;
    }
}

[Export(typeof(EditorFormatDefinition))]
[ClassificationType(ClassificationTypeNames = "AssemblyRegister")]
[Name("AssemblyRegister")]
internal sealed class AssemblyRegisterFormat : ClassificationFormatDefinition
{
    public AssemblyRegisterFormat()
    {
        DisplayName = "Assembly Register";
        ForegroundColor = Colors.Red;
    }
}

private readonly IClassificationType _opcodeType;
private readonly IClassificationType _registerType;
private readonly ITextBuffer _buffer;

public AssemblyClassifier(IClassificationTypeRegistryService registry, ITextBuffer buffer)
{
    _classificationRegistry = registry;
    _opcodeType = _classificationRegistry.GetClassificationType("AssemblyOpcode");
    _registerType = _classificationRegistry.GetClassificationType("AssemblyRegister");
    _buffer = buffer;
}


public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
{
    var result = new List<ClassificationSpan>();

    // TODO: Implement the classification logic here

    return result;
}


var text = span.GetText();
var tokens = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);


foreach (var token in tokens)
{
    if (IsOpcode(token))
    {
        var tokenSpan = new SnapshotSpan(span.Snapshot, span.Start.Position + text.IndexOf(token), token.Length);
        var classificationSpan = new ClassificationSpan(tokenSpan, _opcodeType);
        result.Add(classificationSpan);
    }
}



namespace ASMSyntaxHighlighter
{
    internal class AssemblyClassifier : IClassifier
    {
        private readonly IClassificationTypeRegistryService _classificationRegistry;

        public AssemblyClassifier(IClassificationTypeRegistryService registry)
        {
            _classificationRegistry = registry;
        }

        private bool IsOpcode(string token)
        {
            // TODO: Implement the opcode detection logic here
            return false;
        }

        // Implement the remaining IClassifier methods here
    }
}



private bool IsOpcode(string token)
{
    switch (token.ToUpperInvariant())
    {
        case "MOV":
        case "ADD":
        case "SUB":
        case "AND":
        case "OR":
        case "XOR":
            return true;

        default:
            return false;
    }
}


private readonly IClassificationType _registerType;
private readonly IClassificationType _constantType;
private readonly IClassificationType _commentType;



_registerType = registry.GetClassificationType("80x86Register");
_constantType = registry.GetClassificationType("80x86Constant");
_commentType = registry.GetClassificationType("80x86Comment");


foreach (var token in tokens)
{
    if (IsOpcode(token))
    {
        var tokenSpan = new SnapshotSpan(span.Snapshot, span.Start.Position + text.IndexOf(token), token.Length);
        var classificationSpan = new ClassificationSpan(tokenSpan, _opcodeType);
        result.Add(classificationSpan);
    }
    else if (IsRegister(token))
    {
        var tokenSpan = new SnapshotSpan(span.Snapshot, span.Start.Position + text.IndexOf(token), token.Length);
        var classificationSpan = new ClassificationSpan(tokenSpan, _registerType);
        result.Add(classificationSpan);
    }
    else if (IsConstant(token))
    {
        var tokenSpan = new SnapshotSpan(span.Snapshot, span.Start.Position + text.IndexOf(token), token.Length);
        var classificationSpan = new ClassificationSpan(tokenSpan, _constantType);
        result.Add(classificationSpan);
    }
    else if (IsComment(token))
    {
        var tokenSpan = new SnapshotSpan(span.Snapshot, span.Start.Position + text.IndexOf(token), token.Length);
        var classificationSpan = new ClassificationSpan(tokenSpan, _commentType);
        result.Add(classificationSpan);
    }
}



private bool IsRegister(string token)
{
    if (token.Length != 2)
        return false;

    if (token[0] != 'E')
        return false;

    switch (token[1])
    {
        case 'A':
        case 'B':
        case 'C':
        case 'D':
        case 'S':
        case 'P':
        case 'I':
        case 'F':
            return true;

        default:
            return false;
    }
}

private bool IsConstant(string token)
{
    double value;
    return double.TryParse(token, out value);
}

private bool IsComment(string token)
{
    return token.StartsWith(";");
}




namespace AssemblySyntaxHighlighter
{
    [Guid("10de7169-56a3-4722-81a7-15e13aa899f7")]
    public class AssemblyOptionsPage : DialogPage
    {
        private AssemblyOptions _options = new AssemblyOptions();

        [Category("Syntax Highlighting")]
        [DisplayName("Opcode Color")]
        [Description("The color used to highlight opcodes.")]
        public System.Windows.Media.Color OpcodeColor
        {
            get { return _options.OpcodeColor; }
            set { _options.OpcodeColor = value; }
        }

        [Category("Syntax Highlighting")]
        [DisplayName("Register Color")]
        [Description("The color used to highlight registers.")]
        public System.Windows.Media.Color RegisterColor
        {
            get { return _options.RegisterColor; }
            set { _options.RegisterColor = value; }
        }

        [Category("Syntax Highlighting")]
        [DisplayName("Constant Color")]
        [Description("The color used to highlight constants.")]
        public System.Windows.Media.Color ConstantColor
        {
            get { return _options.ConstantColor; }
            set { _options.ConstantColor = value; }
        }

        [Category("Syntax Highlighting")]
        [DisplayName("Comment Color")]
        [Description("The color used to highlight comments.")]
        public System.Windows.Media.Color CommentColor
        {
            get { return _options.CommentColor; }
            set { _options.CommentColor = value; }
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();
            AssemblyClassifier.Options = _options;
        }
    }

    public class AssemblyOptions
    {
        public System.Windows.Media.Color OpcodeColor { get; set; } = System.Windows.Media.Colors.Blue;
        public System.Windows.Media.Color RegisterColor { get; set; } = System.Windows.Media.Colors.Green;
        public System.Windows.Media.Color ConstantColor { get; set; } = System.Windows.Media.Colors.Red;
        public System.Windows.Media.Color CommentColor { get; set; } = System.Windows.Media.Colors.DarkGreen;
    }
}

public static AssemblyOptions Options { get; set; }

public AssemblyClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry) : base()
{
    _opcodeType = registry.GetClassificationType("80x86Opcode");
    _registerType = registry.GetClassificationType("80x86Register");
    _constantType = registry.GetClassificationType("80x86Constant");
    _commentType = registry.GetClassificationType("80x86Comment");

    if (Options == null)
    {
        Options = new AssemblyOptionsPage().LoadSettings();
    }

    Tokens = new List<string>();
    foreach (var opcode in OpCode.Mnemonics)
    {
        Tokens.Add(opcode.ToUpper());
    }
}

private IClassificationType GetClassificationType(string typeName)
{
    return _classificationRegistry.GetClassificationType(typeName);
}

private static bool IsOpcode(string token)
{
    return OpCode.Mnemonics.Contains(token.ToUpper());
}

private bool IsRegister(string token)
{
    return Register.Names.Contains(token.ToUpper());
}

private void HighlightToken(SnapshotSpan span, IClassificationType type)
{
    var classification = new ClassificationSpan(span, type);
    _classifications.Add(classification);
}

public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
{
    _classifications.Clear();

    var text = span.GetText().ToUpper();
    var start = span.Start.Position;
    var length = span.Length;

    var tokenStart = start;
    var tokenLength = 0;

    for (int i = 0; i < length; i++)
    {
        var c = text[i];

        if (Char.IsWhiteSpace(c) || !Char.IsLetterOrDigit(c))
        {
            if (tokenLength > 0)
            {
                var token = text.Substring(tokenStart, tokenLength);

                if (IsOpcode(token))
                {
                    HighlightToken(new SnapshotSpan(span.Snapshot, new Span(tokenStart, tokenLength)), _opcodeType);
                }
                else if (IsRegister(token))
                {
                    HighlightToken(new SnapshotSpan(span.Snapshot, new Span(tokenStart, tokenLength)), _registerType);
                }
                else if (token.StartsWith(";"))
                {
                    HighlightToken(new SnapshotSpan(span.Snapshot, new Span(tokenStart, tokenLength)), _commentType);
                }
                else
                {
                    HighlightToken(new SnapshotSpan(span.Snapshot, new Span(tokenStart, tokenLength)), _constantType);
                }

                tokenStart = start + i + 1;
                tokenLength = 0;
            }
        }
        else
        {
            tokenLength++;
        }
    }

    return _classifications;
}




protected override void OnActivate(CancelEventArgs e)
{
    LoadSettings();
    base.OnActivate(e);
}

public override void SaveSettingsToStorage()
{
    var settingsStore = (IVsWritableSettingsStore)GetDialogPage(typeof(AssemblyOptionsPage)).GetSettingsStore();

    var key = $"{nameof(AssemblySyntaxHighlighter)}.{nameof(AssemblyOptions)}";

    var value = JsonConvert.SerializeObject(_options);

    settingsStore.SetString(key, value);

    AssemblyClassifier.Options = _options;

    base.SaveSettingsToStorage();
}

public AssemblyOptions LoadSettings()
{
    var settingsStore = (IVsWritableSettingsStore)GetDialogPage(typeof(AssemblyOptionsPage)).GetSettingsStore();

    var key = $"{nameof(AssemblySyntaxHighlighter)}.{nameof(AssemblyOptions)}";

    if (settingsStore.CollectionExists(key))
    {
        var value = settingsStore.GetString(key);

        _options = JsonConvert.DeserializeObject<AssemblyOptions>(value);
    }

    return _options;
}

