using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PokeSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(GameDataRegistrationCodeFixProvider)), Shared]
public class GameDataRegistrationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ["PSG1001"];

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null)
            return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var fieldDeclaration = root.FindToken(diagnosticSpan.Start)
            .Parent?.AncestorsAndSelf()
            .OfType<FieldDeclarationSyntax>()
            .FirstOrDefault();

        if (fieldDeclaration == null)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Make field static and readonly",
                createChangedDocument: c => MakeFieldStaticAndReadonlyAsync(context.Document, fieldDeclaration, c),
                equivalenceKey: nameof(GameDataRegistrationCodeFixProvider)
            ),
            diagnostic
        );
    }

    private static async Task<Document> MakeFieldStaticAndReadonlyAsync(
        Document document,
        FieldDeclarationSyntax fieldDeclaration,
        CancellationToken cancellationToken
    )
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null)
            return document;

        var modifiers = fieldDeclaration.Modifiers;

        // Add 'static' if not present
        if (!modifiers.Any(SyntaxKind.StaticKeyword))
        {
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
        }

        // Add 'readonly' if not present
        if (!modifiers.Any(SyntaxKind.ReadOnlyKeyword))
        {
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));
        }

        var newFieldDeclaration = fieldDeclaration.WithModifiers(modifiers);
        var newRoot = root.ReplaceNode(fieldDeclaration, newFieldDeclaration);

        return document.WithSyntaxRoot(newRoot);
    }
}
