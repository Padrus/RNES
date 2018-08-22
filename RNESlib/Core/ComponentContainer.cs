using Unity;

namespace RNESlib.Core
{
    public class ComponentContainer
    {
        private readonly UnityContainer _unityContainer;

        public ComponentContainer(UnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public void RegisterComponent<InterfaceType, ClassType>() where ClassType : InterfaceType
        {
            _unityContainer.RegisterType<InterfaceType, ClassType>();
        }

        public InterfaceType ResolveComponent<InterfaceType>()
        {
            return _unityContainer.Resolve<InterfaceType>();
        }

        public void RegisterComponentinstance<InterfaceType>(InterfaceType component)
        {
            _unityContainer.RegisterInstance<InterfaceType>(component);
        }
    }
}
