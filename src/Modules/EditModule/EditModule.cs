using ICSharpCode.AvalonEdit;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace EditModule
{
    public class EditModule : IModule
    {
        private readonly IUnityContainer _container;

        public EditModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterType<ITextEditorComponent, TextEditor>();
        }
    }
}
