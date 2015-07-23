using System.Collections.ObjectModel;

namespace DynamicConfig.ConfigTray.ViewModel
{
    internal class RootModel
    {
        private ObservableCollection<ConfigNode> _firstGeneration;

        private ConfigNode _rootNode;

        public RootModel(ConfigNode root)
        {
            _rootNode = root;
            _firstGeneration = new ObservableCollection<ConfigNode>(
                new[]
                {
                    _rootNode
                });
        }

        public ConfigNode RootViewModel
        {
            get { return _rootNode; }
        }

        public ObservableCollection<ConfigNode> FirstGeneration
        {
            get { return _firstGeneration; }
        }
    }
}