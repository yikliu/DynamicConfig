using System;

namespace DynamicConfig.ConfigTray.ViewModel
{
    /// <summary>
    ///
    /// </summary>
    public abstract class AbsCollectionConfigNode : ConfigNode
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parent"></param>
        protected AbsCollectionConfigNode(KeyType key, ConfigNode parent)
            : base(key, parent)
        { }

        

        /// <summary>
        /// Label showing the type of node
        /// </summary>
        public override string TypeLabel
        {
            get { return NodeDescription; }
        }

        /// <summary>
        /// Whether or not the content of the node can be edited
        /// </summary>
        public override bool ContentEditable
        {
            get { return false; }
        }

        /// <summary>
        /// The content of node
        /// </summary>
        public override string StringContent
        {
            get { return "This is a container node, only leaf nodes can be edited"; }
            set { throw new NotSupportedException("Cannot set string to a container node."); }
        }

        /// <summary>
        /// The label about the node on tree view
        /// </summary>
        public override string DisplayName
        {
            get { return KeyLabel + " : " + NodeDescription; }
        }

        internal abstract void DeleteChild(ConfigNode configNode);

        internal abstract void AppendChild(ConfigNode n);

        internal abstract void ReplaceChild(ConfigNode n, KeyType key);
    }
}