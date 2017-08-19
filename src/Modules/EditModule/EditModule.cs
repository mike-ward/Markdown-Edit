using System.Collections.Generic;
using EditModule.Commands;
using EditModule.Features;
using EditModule.Features.SyntaxHighlighting;
using EditModule.Models;
using EditModule.Views;
using ICSharpCode.AvalonEdit;
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
            Container.RegisterType<ITextEditorComponent, TextEditor>();

            Container.RegisterType<IEditFeature, TextEditorOptions>(nameof(TextEditorOptions));
            Container.RegisterType<IEditFeature, SyntaxHighlighting>(nameof(SyntaxHighlighting));
            Container.RegisterType<IEditFeature, SynchronizedScroll>(nameof(SynchronizedScroll));
            Container.RegisterType<IEnumerable<IEditFeature>, IEditFeature[]>();

            Container.RegisterType<IEditCommandHandler, NewCommandHandler>(nameof(NewCommandHandler));
            Container.RegisterType<IEditCommandHandler, OpenCommandHandler>(nameof(OpenCommandHandler));
            Container.RegisterType<IEditCommandHandler, SaveCommandHandler>(nameof(SaveCommandHandler));
            Container.RegisterType<IEditCommandHandler, SaveAsCommandHandler>(nameof(SaveAsCommandHandler));

            Container.RegisterType<IEditCommandHandler, InsertBlockQuoteCommandHandler>(nameof(InsertBlockQuoteCommandHandler));
            Container.RegisterType<IEditCommandHandler, ToggleCodeCommandHandler>(nameof(ToggleCodeCommandHandler));
            Container.RegisterType<IEditCommandHandler, ToggleBoldCommandHandler>(nameof(ToggleBoldCommandHandler));
            Container.RegisterType<IEditCommandHandler, ToggleItalicCommandHandler>(nameof(ToggleItalicCommandHandler));
            Container.RegisterType<IEnumerable<IEditCommandHandler>, IEditCommandHandler[]>();

            RegionManager.RegisterViewWithRegion(Constants.EditRegion, typeof(EditControl));
        }
    }
}
