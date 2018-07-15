using System.Collections.Generic;
using Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using ServicesModule.Services;
using Unity;
using Unity.Lifetime;

namespace ServicesModule
{
    public class ServicesModule : IModule
    {
        public IUnityContainer Container { get; }

        public ServicesModule(IUnityContainer container)
        {
            Container = container;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Container.RegisterType<IAbstractSyntaxTree, AbstractSyntaxTree>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IColorService, ColorService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IEditService, EditService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDocumentConverters, DocumentConverters>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IFormatMarkdown, FormatMarkdown>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IImageService, ImageService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IImageUpload, ImageUploadImgur>(new ContainerControlledLifetimeManager());
            Container.RegisterType<INotify, Notify>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IOpenSaveActions, OpenSaveActions>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISettings, Settings>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISnippetService, SnippetService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISpellCheckService, SpellCheckService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISpellCheckUserDictionaryService, SpellCheckUserDictionaryService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IStrings, Strings>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITemplateLoader, TemplateLoader>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IMarkdownEngine, CommonMarkEngine>("CommonMark", new ContainerControlledLifetimeManager());
            Container.RegisterType<IEnumerable<IMarkdownEngine>, IMarkdownEngine[]>(new ContainerControlledLifetimeManager());
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}