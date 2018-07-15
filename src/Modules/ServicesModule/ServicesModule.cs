using System.Collections.Generic;
using Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using ServicesModule.Services;

namespace ServicesModule
{
    public class ServicesModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAbstractSyntaxTree, AbstractSyntaxTree>();
            containerRegistry.RegisterSingleton<IColorService, ColorService>();
            containerRegistry.RegisterSingleton<IEditService, EditService>();
            containerRegistry.RegisterSingleton<IDocumentConverters, DocumentConverters>();
            containerRegistry.RegisterSingleton<IFormatMarkdown, FormatMarkdown>();
            containerRegistry.RegisterSingleton<IImageService, ImageService>();
            containerRegistry.RegisterSingleton<IImageUpload, ImageUploadImgur>();
            containerRegistry.RegisterSingleton<INotify, Notify>();
            containerRegistry.RegisterSingleton<IOpenSaveActions, OpenSaveActions>();
            containerRegistry.RegisterSingleton<ISettings, Settings>();
            containerRegistry.RegisterSingleton<ISnippetService, SnippetService>();
            containerRegistry.RegisterSingleton<ISpellCheckService, SpellCheckService>();
            containerRegistry.RegisterSingleton<ISpellCheckUserDictionaryService, SpellCheckUserDictionaryService>();
            containerRegistry.RegisterSingleton<IStrings, Strings>();
            containerRegistry.RegisterSingleton<ITemplateLoader, TemplateLoader>();

            containerRegistry.Register<IMarkdownEngine, CommonMarkEngine>("CommonMark");
            containerRegistry.RegisterSingleton<IEnumerable<IMarkdownEngine>, IMarkdownEngine[]>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}