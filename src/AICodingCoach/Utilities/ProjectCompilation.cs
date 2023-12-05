using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace AICodingCoach.Utilities
{
    /// <summary>
    /// Wraps the work of compiling the code. 
    /// </summary>
    public class ProjectCompilation : INotifyPropertyChanged
    {
        public CancellationTokenSource TokenSource = new CancellationTokenSource();
        private CSharpCompilation _compilation;
        private SyntaxTree? _tree;
        private SourceText? _text;
        private EmitResult? _result;
        private Assembly? _assembly;
        private IReadOnlyList<string> _diagnostics;

        public CSharpCompilation Compilation
        {
            get => _compilation;
            private set
            {
                if (Equals(value, _compilation)) return;
                _compilation = value;
                OnPropertyChanged();
            }
        }

        public SyntaxTree? Tree
        {
            get => _tree;
            private set
            {
                if (Equals(value, _tree)) return;
                _tree = value;
                OnPropertyChanged();
            }
        }

        public SourceText? Text
        {
            get => _text;
            private set
            {
                if (Equals(value, _text)) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public EmitResult? Result
        {
            get => _result;
            private set
            {
                if (Equals(value, _result)) return;
                _result = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Success));
            }
        }

        public string Name { get; }
        public string OutputPath { get; private set; }
        public string Prefix = ""; //=> $"public class {Name} {{" + Environment.NewLine;
        public string Postfix = ""; // Environment.NewLine + "}" + Environment.NewLine;

        public Assembly? Assembly
        {
            get => _assembly;
            private set
            {
                if (Equals(value, _assembly)) return;
                _assembly = value;
                OnPropertyChanged();
            }
        }

        public ProjectCompilation(string name, IReadOnlyList<Assembly> assembliesToRef)
        {
            Name = name;
            var compilationOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);
            var refs = ReferencesFromAssemblies(assembliesToRef).ToImmutableArray();
            Compilation = CSharpCompilation.Create(null, null, refs, compilationOptions);
        }

        public void Recompile(string text)
        {
            text = Prefix + text + Postfix;
            OutputPath = Path.ChangeExtension(Path.GetTempFileName(), "dll");
            Diagnostics = new[] { "Recompiling" };
            TokenSource.Cancel();
            TokenSource = new CancellationTokenSource();
            var oldTree = Tree;
            Result = null;
            Assembly = null;
            Text = SourceText.From(text);
            if (oldTree == null)
            {
                Tree = CSharpSyntaxTree.ParseText(Text);
                Compilation = Compilation.RemoveAllSyntaxTrees().AddSyntaxTrees(Tree);
            }
            else
            {
                Tree = oldTree.WithChangedText(Text);
                Compilation = Compilation.ReplaceSyntaxTree(oldTree, Tree);

            }

            if (TokenSource.IsCancellationRequested)
            {
                Diagnostics = new[] { "Canceled" };
                return;
            }
            Result = Compilation.Emit(OutputPath, null, null, null, null, TokenSource.Token);
            Diagnostics = Result.Diagnostics.Select(d => d.ToString()).ToList();
            if (Result.Success)
            {
                Assembly = Assembly.LoadFile(OutputPath);
                Diagnostics = Diagnostics.Append($"Success").ToList();
            }
            else
            {
                Diagnostics = Diagnostics.Append($"Failed").ToList();
            }
        }

        public bool Success
            => Result?.Success == true;

        public IReadOnlyList<string> Diagnostics
        {
            get => _diagnostics;
            private set
            {
                if (Equals(value, _diagnostics)) return;
                _diagnostics = value;
                OnPropertyChanged();
            }
        }

        public static IEnumerable<string> LoadedAssemblyLocations(AppDomain domain = null)
            => (domain ?? AppDomain.CurrentDomain).GetAssemblies().Where(x => !x.IsDynamic).Select(x => x.Location);

        public static string ToPackageReference(AssemblyIdentity asm)
            => $"<PackageReference Include=\"{asm.Name}\" Version=\"{asm.Version}\" />";

        public static IEnumerable<MetadataReference> ReferencesFromAssemblies(IEnumerable<Assembly> assemblies)
            => ReferencesFromFiles(assemblies.Select(asm => asm.Location));

        public static IEnumerable<MetadataReference> ReferencesFromFiles(IEnumerable<string> files)
            => files.Select(x => MetadataReference.CreateFromFile(x));

        public static IEnumerable<MetadataReference> ReferencesFromLoadedAssemblies()
            => ReferencesFromFiles(LoadedAssemblyLocations());

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}