
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using DynamicConfig.ConfigTray.Crypt;
using DynamicConfig.ConfigTray.Properties;
using DynamicConfig.ConfigTray.ViewModel.ConfigChangedEvent;

namespace DynamicConfig.ConfigTray.ViewModel
{
    /// <summary>
    /// abstract class representing a node in JSON hierarchy, instances of its subclass are
    /// viewmodel for treeview items
    /// </summary>
    public abstract class ConfigNode : DynamicObject, INotifyPropertyChanged
    {
        /// <summary>
        /// store the currently assigned handler of ConfigChangedEvent. When new children nodes are
        /// added, this saved handler can be assigned to them.
        /// </summary>
        protected EventHandler<ConfigChangedEventArgs> _currentChangedHandler;

        /// <summary>
        /// For treeviewitem expanded property
        /// </summary>
        protected bool _isExpanded;

        /// <summary>
        /// For treeviewitem selected property
        /// </summary>
        protected bool _isSelected;

        /// <summary>
        /// _affectedNodeKey of the node, assigned by parent
        /// </summary>
        protected KeyType _key;

        /// <summary>
        /// reference to parent node, null for root node
        /// </summary>
        protected ConfigNode _parent;

        /// <summary>
        /// The _value changed handlers
        /// </summary>
        private EventHandler<ConfigChangedEventArgs> _valueChangedHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigNode" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="parent">The parent.</param>
        protected ConfigNode(KeyType key, ConfigNode parent)
        {
            _isSelected = false;
            _isExpanded = true;
            _key = key;
            _parent = parent;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when changes are made
        /// </summary>
        internal event EventHandler<ConfigChangedEventArgs> ValueChangedEventHandlers
        {
            add
            {
                //avoid multiple assignment
                if (_valueChangedHandlers == null || !_valueChangedHandlers.GetInvocationList().Contains(value))
                    _valueChangedHandlers += value;
            }
            remove
            {
                if (_valueChangedHandlers != null)
                    _valueChangedHandlers -= value;
            }
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public abstract string StringContent { get; set; }

        /// <summary>
        /// Collection of children of this node
        /// </summary>
        public virtual ObservableCollection<ConfigNode> Children
        {
            get { return null; }
        }

        /// <summary>
        /// flag indicating whether content can be edited
        /// </summary>
        /// <value><c>true</c> if node is Leaf; otherwise, <c>false</c>.</value>
        public abstract bool ContentEditable { get; }

        /// <summary>
        /// Gets the key in display format, with "" or "[]" added.
        /// </summary>
        /// <value>The display key.</value>
        public string DisplayKey
        {
            get { return _key.DisplayString(); }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public abstract string DisplayName { get; }

        /// <summary>
        /// The key in string
        /// </summary>
        public string KeyLabel
        {
            get { return _key.ToString(); }
        }

        /// <summary>
        ///
        /// </summary>
        public abstract string TypeLabel { get; }

        /// <summary>
        /// Gets or sets the cipher string.
        /// </summary>
        /// <value>The cipher string.</value>
        public virtual EncryptionMethod EncryptionMethodUsed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is encryption enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is encryption enabled; otherwise, <c>false</c>.</value>
        public virtual bool IsEncrypted
        {
            get
            {
                return false;
            }
            set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value><c>true</c> if this instance is expanded; otherwise, <c>false</c>.</value>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this treeviewitem is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public KeyType Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets the content of the node.
        /// </summary>
        /// <value>The content of the node.</value>
        public abstract string NodeDescription { get; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public ConfigNode Parent
        {
            get { return _parent; }
        }

        //return key with original type (int or string)
        /// <summary>
        /// Gets or sets the key, in its raw type (int or string)
        /// </summary>
        /// <value>The raw key.</value>
        public dynamic RawKey
        {
            get { return _key.RawValue(); }
            set
            {
                if (value.ToString().Equals(_key.ToString()))
                {
                    return;
                }
                _key = value;
                OnPropertyChanged("DisplayKey"); //update the treeview
            }
        }

        /// <summary>
        /// whether an "IsEncrypted" box is shown
        /// </summary>
        public virtual string ShowEncryptCheckBox
        {
            get { return "Collapsed"; }
            set { throw new InvalidOperationException("Invalid Operation"); }
        }

        /// <summary>
        /// whether the encryption method panel is shown
        /// </summary>
        /// <value>The show encryption panel.</value>
        public virtual string ShowEncryptionPanel
        {
            get { return "Collapsed"; }
        }

        /// <summary>
        /// Gets the current ConofigChanged handler.
        /// </summary>
        /// <value>The current changed handler.</value>
        internal EventHandler<ConfigChangedEventArgs> CurrentChangedHandler
        {
            get { return _currentChangedHandler; }
        }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>The type of the node.</value>
        internal abstract ConfigNodeType NodeType { get; }

        /// <summary>
        /// Determines whether this instance is root.
        /// </summary>
        /// <returns><c>true</c> if this instance is root; otherwise, <c>false</c>.</returns>
        public bool IsRoot()
        {
            return null == _parent;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>ConfigNode.</returns>
        internal abstract ConfigNode Clone();

        /// <summary>
        /// Converts to configuration object.
        /// </summary>
        /// <returns>dynamic.</returns>
        internal abstract dynamic ConvertToExpando();

        /// <summary>
        /// Creates the node.
        /// </summary>
        internal abstract void CreateNode();

        /// <summary>
        /// Deletes the node.
        /// </summary>
        internal void DeleteNode()
        {
            if (IsRoot()) return;
            ((AbsCollectionConfigNode)_parent).DeleteChild(this); //let parent delete this child
        }

        /// <summary>
        /// save the property change handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        internal abstract void SavePropertyChangeHandler(EventHandler<ConfigChangedEventArgs> handler);

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(/*[CallerMemberName]*/ string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Propagates the change.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ConfigChangedEventArgs" /> instance containing the event data.
        /// </param>
        protected void PropagateChange(ConfigChangedEventArgs e)
        {
            if (_valueChangedHandlers != null)
            {
                _valueChangedHandlers(this, e);
            }
        }
    }
}