using System;
using System.ComponentModel;
using System.Dynamic;
using DynamicConfig.ConfigTray.Crypt;
using DynamicConfig.ConfigTray.JSONConfig;
using DynamicConfig.ConfigTray.Util;
using DynamicConfig.ConfigTray.ViewModel.ConfigChangedEvent;
using DynamicConfig.ConfigTray.ViewModel.ConfigLeafNodeImp;

namespace DynamicConfig.ConfigTray.ViewModel
{
    /// <summary>
    /// Representing a leaf node
    /// </summary>
    public class ConfigLeafNode : ConfigNode
    {
        private ILeafNodeState _state;

        internal ConfigLeafNode(dynamic v, KeyType key, ConfigNode parent)
            : base(key, parent)
        {
            _state = new PlainLeafNodeState(v);
        }

        internal ConfigLeafNode(dynamic v, KeyType key, ConfigNode parent, EncryptionMethod em) 
            :base (key, parent)
        {
            _state = em == EncryptionMethod.DPAPI_MACHINE ? 
                (ILeafNodeState) new DPAPIMachineState(v, true) : new DPAPIUserState(v, true);
            
        }

        internal ILeafNodeState State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// Content of this leaf node in string, if it's encrypted, encrypted text is returned.
        /// </summary>
        public override string StringContent
        {
            get
            {
                return _state.StringContent;
            }
            set
            {
                _state.StringContent = value;
                
                OnPropertyChanged("NodeDescription");
                OnPropertyChanged("RawValue");

                ConfigChangedEventArgs evt = new EditChangeEventArgs(KeyLabel, StringContent);
                PropagateChange(evt);
            }
        }

        internal void ReplaceEncryptedContent(string plain)
        {
            if (IsEncrypted)
            {
                _state.DynamicValue = plain;
                _state.IsCommitted = false;
                _state.Commit();
            }
        }

        /// <summary>
        /// Data type of this node
        /// </summary>
        public Type DataType
        {
            get { return _state.DataType; }
        }

        /// <summary>
        /// Label about the node on tree view
        /// </summary>
        public override string DisplayName
        {
            get { return KeyLabel + ":" + StringContent; }
        }

        /// <summary>
        /// Encryption method applied
        /// </summary>
        public override EncryptionMethod EncryptionMethodUsed
        {
            get { return _state.EncryptionMethodAssigned; }
            set
            {
                
                var leaf_state = value == EncryptionMethod.DPAPI_MACHINE
                    ? LeafState.DPAPIMACHINEEncrypted
                    : LeafState.DPAPIUSerEncrypted;
                
                _state.GoToState(this, leaf_state);
                
                OnPropertyChanged("StringContent");
                OnPropertyChanged("ContentEditable");
            }
        }

        /// <summary>
        /// Whether encryption is enabled for this node.
        /// </summary>
        public override bool IsEncrypted
        {
            get { return _state.IsEncrypted(); }
            set
            {
                if (_state.IsEncrypted() != value)
                {
                    if (value)
                        _state.GoToState(this, LeafState.DPAPIMACHINEEncrypted);
                    else
                    {
                        _state.GoToState(this, LeafState.Plain);
                    }

                    //this value has effect on visibility and enability of some UI elements
                    OnPropertyChanged("ShowEncryptionPanel");
                    OnPropertyChanged("ContentEditable");
                    OnPropertyChanged("StringContent");
                    OnPropertyChanged("NodeDescription");
                }
            }
        }

        /// <summary>
        /// Description of the node, returns StringContent for leaf node
        /// </summary>
        public override string NodeDescription
        {
            get { return StringContent; }
        }

        /// <summary>
        /// Whether or not the encryption checkbox is shown when this node is selelcted
        /// Only string can be encrypted.
        /// </summary>
        public override string ShowEncryptCheckBox
        {
            get
            {
                if (DataType == typeof(String))
                    return "Visible";
                return "Collapsed";
            }
        }

        /// <summary>
        /// Whether the encryption selection panel is shown
        /// </summary>
        public override string ShowEncryptionPanel
        {
            get { return IsEncrypted ? "Visible" : "Collapsed"; }
        }

        /// <summary>
        /// Label indicating the data type of node
        /// </summary>
        public override string TypeLabel
        {
            get { return DataType.Name; }
        }

        /// <summary>
        /// Whether content of this node can be edited
        /// </summary>
        public override bool ContentEditable
        {
            get { return _state.ContentEditable; }
        }

        public void Edit()
        {

            _state.NextState();
            
            OnPropertyChanged("StringContent");
            OnPropertyChanged("RawValue");
            OnPropertyChanged("NodeDescription");
            OnPropertyChanged("ContentEditable");

            ConfigChangedEventArgs evt = new EditChangeEventArgs(KeyLabel, StringContent);
            PropagateChange(evt);
        }

        internal override ConfigNodeType NodeType
        {
            get { return ConfigNodeType.LEAF; }
        }

        internal dynamic RawValue
        {
            get { return _state.DynamicValue; }
            set
            {
                _state.DynamicValue = value;

                OnPropertyChanged("RawValue");
                OnPropertyChanged("StringContent");
                OnPropertyChanged("NodeDescription");
                ConfigChangedEventArgs evt = new EditChangeEventArgs(KeyLabel, StringContent);
                PropagateChange(evt);
            }
        }

        /// <summary>
        /// Try to convert the content to bool value,
        /// throws InvalidCastException if fails.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static implicit operator bool(ConfigLeafNode node)
        {
            if (PrimitiveTypeHelper.PrimitiveTypeCatgorizer(node.DataType) == DynamicConfig.ConfigTray.Util.DataType.BOOLEAN)
            {
                return node.RawValue;
            }
            throw new InvalidCastException("Cannot cast type of " + node.DataType + " to bool.");
        }

        /// <summary>
        /// Try to convert the content into double,
        /// throws InvalidCastException if fails
        /// </summary>
        public static implicit operator double(ConfigLeafNode node)
        {
            var converter = TypeDescriptor.GetConverter(node.DataType);
            if (converter.CanConvertTo(typeof(double)))
            {
                return (double)node.RawValue;
            }
            throw new InvalidCastException("Cannot cast type of " + node.DataType + " to double.");
        }

        /// <summary>
        /// Try to convert the content into int,
        /// throws InvalidCastException if fails
        /// </summary>
        public static implicit operator int(ConfigLeafNode node)
        {
            var converter = TypeDescriptor.GetConverter(node.DataType);
            if (converter.CanConvertTo(typeof(int)))
            {
                return (int)node.RawValue;
            }
            throw new InvalidCastException("Cannot cast type of " + node.DataType + " to int.");
        }

        /// <summary>
        /// implicitly convert node content to string
        /// </summary>
        /// <param name="node">leaf node</param>
        /// <returns>node content</returns>
        public static implicit operator string(ConfigLeafNode node)
        {
            if (node.State.CurrentState != LeafState.Plain)
            {
                return ((EncryptedLeafNodeState) node.State).PlainContent;
            }
            return node.StringContent;
        }

        /// <summary>
        /// Returns content in string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return StringContent;
        }

        /// <summary>
        /// can't access [] on leaf node, returns NullExceptionPreventer
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="result">NullExceptionPreventer</param>
        /// <returns>true</returns>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = new NullExceptionPreventer();
            return true;
        }

        /// <summary>
        /// Can't access memeber on leaf node, returns NullExceptionPreventer
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result">NullExceptionPreventer</param>
        /// <returns>true</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new NullExceptionPreventer();
            return true;
        }

        /// <summary>
        /// This is not recommended.
        /// - It converts the leaf node to an array and the original string data will be lost.
        /// - It also disrupt the strong typed class definition. The property mapping to this leaf
        ///   node will become invalid. An implementation is provided in comment, uncomment it if
        ///   this feature is really needed.
        /// </summary>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            throw new NotSupportedException("Can't set index on a leaf node");

            /*
            dynamic node = new ConfigListNode(_key, _parent);

            if (value is string)
            {
                var leaf = new ConfigLeafNode("", new IntKey(0), node); //no matter of the index, start with 0
                node[0] = leaf;
            }
            else
            {
                node[0] = value;
            }
            ConfigNode p = _parent;
            if (p.NodeType == ConfigNodeType.ARRAY)
            {
                ((ConfigListNode)p).ReplaceChild(node, _key.RawValue());
            }
            else if (p.NodeType == ConfigNodeType.OBJECT)
            {
                ((ConfigObjectNode)p).ReplaceChild(node, _key.RawValue());
            }
            return true;
             */
        }

        /// <summary>
        /// This is not recommended.
        /// - It actually converts a leaf node to an object node. The original string data will be
        ///   lost after conversion.
        /// - Moreover, If developers have defined strong typed classes, the property mapping to
        ///   this leaf node will be invalid. An implementation is provided in comment, uncomment it
        ///   if this feature is really needed.
        /// </summary>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            throw new NotSupportedException("Can't set index on a leaf node");

            /*
            dynamic node = new ConfigObjectNode(_key, _parent);
            if (value is string)
            {
                var leaf = new ConfigLeafNode("", new StringKey(binder.Name), node);
                node[binder.Name] = leaf;
            }
            else
            {
                node[binder.Name] = value;
            }
            ConfigNode p = _parent;
            if (p.NodeType == ConfigNodeType.ARRAY)
            {
                ((ConfigListNode) p).ReplaceChild(node, _key.RawValue());
            }else if (p.NodeType == ConfigNodeType.OBJECT)
            {
                ((ConfigObjectNode)p).ReplaceChild(node, _key.RawValue());
            }
            return true;
             */
        }

        internal override ConfigNode Clone()
        {
            dynamic conObj = ConvertToExpando();
            return new ConfigLeafNode(conObj, Key, _parent);
        }

        internal override dynamic ConvertToExpando()
        {
            return _state.ConvertToExpando();
        }

        internal override void CreateNode()
        {
            throw new NotSupportedException("Can't create node on leaf nodes");
        }

        internal override void SavePropertyChangeHandler(EventHandler<ConfigChangedEventArgs> handler)
        {
            _currentChangedHandler = handler;
            ValueChangedEventHandlers += handler;
        }

        
    }
}