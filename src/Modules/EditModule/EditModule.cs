using System.Collections.Generic;
using EditModule.Commands;
using EditModule.Features;
using EditModule.Features.SpellCheck;
using EditModule.Features.SyntaxHighlighting;
using EditModule.Models;
using EditModule.Views;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using Infrastructure;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using TextEditorOptions = EditModule.Features.TextEditorOptions;

namespace EditModule
{
    public class EditModule : IModule
    {
        public IUnityContainer Container { get; }
        public IRegionManager RegionManager { get; }

        public EditModule(IUnityContainer container, IRegionManager regionManager)
        {
            Container = container;
            RegionManager = regionManager;
        }

        public void Initialize()
        {
            Container.RegisterType<IBlockBackgroundRenderer, BlockBackgroundRenderer>();
            Container.RegisterType<ISpellCheckBackgroundRenderer, SpellCheckBackgroundRenderer>();
            Container.RegisterType<ITextEditorComponent, TextEditor>();

            Container.RegisterType<IEditFeature, DragAndDropSupport>(nameof(DragAndDropSupport));
            Container.RegisterType<IEditFeature, EditorContextMenu>(nameof(EditorContextMenu));
            Container.RegisterType<IEditFeature, FileNameChangedEventHandler>(nameof(FileNameChangedEventHandler));
            Container.RegisterType<IEditFeature, PasteEnhancements>(nameof(PasteEnhancements));
            Container.RegisterType<IEditFeature, TextEditorOptions>(nameof(TextEditorOptions));
            Container.RegisterType<IEditFeature, SpellCheck>(nameof(SpellCheck));
            Container.RegisterType<IEditFeature, SyntaxHighlighting>(nameof(SyntaxHighlighting));
            Container.RegisterType<IEditFeature, SynchronizedScroll>(nameof(SynchronizedScroll));
            Container.RegisterType<IEditFeature, TextUpdatedEventHandler>(nameof(TextUpdatedEventHandler));
            Container.RegisterType<IEnumerable<IEditFeature>, IEditFeature[]>();

            Container.RegisterType<IEditCommandHandler, NewCommandHandler>(nameof(NewCommandHandler));
            Container.RegisterType<IEditCommandHandler, OpenCommandHandler>(nameof(OpenCommandHandler));
            Container.RegisterType<IEditCommandHandler, SaveCommandHandler>(nameof(SaveCommandHandler));
            Container.RegisterType<IEditCommandHandler, SaveAsCommandHandler>(nameof(SaveAsCommandHandler));

            Container.RegisterType<IEditCommandHandler, ConvertSelectionToListCommandHandler>(nameof(ConvertSelectionToListCommandHandler));
            Container.RegisterType<IEditCommandHandler, FindDialogCommandHandler>(nameof(FindDialogCommandHandler));
            Container.RegisterType<IEditCommandHandler, FormatTextCommandHandler>(nameof(FormatTextCommandHandler));
            Container.RegisterType<IEditCommandHandler, InsertBlockQuoteCommandHandler>(nameof(InsertBlockQuoteCommandHandler));
            Container.RegisterType<IEditCommandHandler, RedoEditCommandHander>(nameof(RedoEditCommandHander));
            Container.RegisterType<IEditCommandHandler, ReplaceDialogCommandHandler>(nameof(ReplaceDialogCommandHandler));
            Container.RegisterType<IEditCommandHandler, SnippetCommand>(nameof(SnippetCommand));
            Container.RegisterType<IEditCommandHandler, ToggleCodeCommandHandler>(nameof(ToggleCodeCommandHandler));
            Container.RegisterType<IEditCommandHandler, ToggleBoldCommandHandler>(nameof(ToggleBoldCommandHandler));
            Container.RegisterType<IEditCommandHandler, ToggleItalicCommandHandler>(nameof(ToggleItalicCommandHandler));
            Container.RegisterType<IEditCommandHandler, ToggleWordWrapCommandHandler>(nameof(ToggleWordWrapCommandHandler));
            Container.RegisterType<IEditCommandHandler, UndoEditCommandHander>(nameof(UndoEditCommandHander));
            Container.RegisterType<IEnumerable<IEditCommandHandler>, IEditCommandHandler[]>();

            RegionManager.RegisterViewWithRegion(Constants.EditRegion, typeof(EditControl));
        }
    }
}
