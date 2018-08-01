using System.Collections.Generic;
using EditModule.Commands;
using EditModule.Features;
using EditModule.Features.SpellCheck;
using EditModule.Features.SyntaxHighlighting;
using EditModule.Models;
using EditModule.Views;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using TextEditorOptions = EditModule.Features.TextEditorOptions;

namespace EditModule
{
    public class EditModule : IModule
    {
        public IRegionManager RegionManager { get; }

        public EditModule(IRegionManager regionManager)
        {
            RegionManager = regionManager;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IBlockBackgroundRenderer, BlockBackgroundRenderer>();
            containerRegistry.Register<ISpellCheckBackgroundRenderer, SpellCheckBackgroundRenderer>();
            containerRegistry.Register<ITextEditorComponent, TextEditor>();

            containerRegistry.Register<IEditFeature, DragAndDropSupport>(nameof(DragAndDropSupport));
            containerRegistry.Register<IEditFeature, EditorContextMenu>(nameof(EditorContextMenu));
            containerRegistry.Register<IEditFeature, FileNameChangedEventHandler>(nameof(FileNameChangedEventHandler));
            containerRegistry.Register<IEditFeature, GetFocusOnFlyoutClosed>(nameof(GetFocusOnFlyoutClosed));
            containerRegistry.Register<IEditFeature, PasteEnhancements>(nameof(PasteEnhancements));
            containerRegistry.Register<IEditFeature, TextEditorOptions>(nameof(TextEditorOptions));
            containerRegistry.Register<IEditFeature, SpellCheck>(nameof(SpellCheck));
            containerRegistry.Register<IEditFeature, SyntaxHighlighting>(nameof(SyntaxHighlighting));
            containerRegistry.Register<IEditFeature, SynchronizedScroll>(nameof(SynchronizedScroll));
            containerRegistry.Register<IEditFeature, TextUpdatedEventHandler>(nameof(TextUpdatedEventHandler));
            containerRegistry.Register<IEnumerable<IEditFeature>, IEditFeature[]>();

            containerRegistry.Register<IEditCommandHandler, NewCommandHandler>(nameof(NewCommandHandler));
            containerRegistry.Register<IEditCommandHandler, OpenCommandHandler>(nameof(OpenCommandHandler));
            containerRegistry.Register<IEditCommandHandler, SaveCommandHandler>(nameof(SaveCommandHandler));
            containerRegistry.Register<IEditCommandHandler, SaveAsCommandHandler>(nameof(SaveAsCommandHandler));

            containerRegistry.Register<IEditCommandHandler, ConvertSelectionToListCommandHandler>(nameof(ConvertSelectionToListCommandHandler));
            containerRegistry.Register<IEditCommandHandler, CorrectSpellingErrorCommandHandler>(nameof(CorrectSpellingErrorCommandHandler));
            containerRegistry.Register<IEditCommandHandler, FindDialogCommandHandler>(nameof(FindDialogCommandHandler));
            containerRegistry.Register<IEditCommandHandler, FormatTextCommandHandler>(nameof(FormatTextCommandHandler));
            containerRegistry.Register<IEditCommandHandler, IgnoreSpellingErrorCommandHandler>(nameof(IgnoreSpellingErrorCommandHandler));
            containerRegistry.Register<IEditCommandHandler, InsertBlockQuoteCommandHandler>(nameof(InsertBlockQuoteCommandHandler));
            containerRegistry.Register<IEditCommandHandler, RedoEditCommandHander>(nameof(RedoEditCommandHander));
            containerRegistry.Register<IEditCommandHandler, ReplaceDialogCommandHandler>(nameof(ReplaceDialogCommandHandler));
            containerRegistry.Register<IEditCommandHandler, SnippetCommand>(nameof(SnippetCommand));
            containerRegistry.Register<IEditCommandHandler, ToggleCodeCommandHandler>(nameof(ToggleCodeCommandHandler));
            containerRegistry.Register<IEditCommandHandler, ToggleBoldCommandHandler>(nameof(ToggleBoldCommandHandler));
            containerRegistry.Register<IEditCommandHandler, ToggleItalicCommandHandler>(nameof(ToggleItalicCommandHandler));
            containerRegistry.Register<IEditCommandHandler, ToggleWordWrapCommandHandler>(nameof(ToggleWordWrapCommandHandler));
            containerRegistry.Register<IEditCommandHandler, UndoEditCommandHander>(nameof(UndoEditCommandHander));
            containerRegistry.Register<IEnumerable<IEditCommandHandler>, IEditCommandHandler[]>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            RegionManager.RegisterViewWithRegion(Constants.EditRegion, typeof(EditControl));
        }
    }
}
