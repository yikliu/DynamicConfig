using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using DynamicConfig.ConfigTray.JSONConfig;
using DynamicConfig.ConfigTray.Util;
using DynamicConfig.ConfigTray.ViewModel.ConfigChangedEvent;

namespace DynamicConfig.ConfigTray.ViewModel
{
    /// <summary>
    ///
    /// </summary>
    public class ConfigListNode : AbsCollectionConfigNode, IEnumerable<ConfigNode>
    {
        private readonly ObservableCollection<ConfigNode> _children;

        internal ConfigListNode(dynamic node, KeyType key, ConfigNode parent)
            : base(key, parent)
        {
            _children = new ObservableCollection<ConfigNode>();

            var count = node is Array ? node.Length : node.Count;
            for (var i = 0; i < count; i++)
            {
                ConfigNode elemNode;
                var item = node[i];
                var indexKey = new IntKey(i);

                if (item is ExpandoObject)
                {
                    elemNode = ConfigNodeFactory.CreateConfigObjectNode(item, indexKey, this);
                }
                else if (item is IList<object>)
                {
                    elemNode = ConfigNodeFactory.CreateConfigListNode(item, indexKey, this);
                }
                else
                {
                    elemNode = ConfigNodeFactory.CreateConfigLeafNode(item, indexKey, this);
                }
                _children.Add(elemNode);
            }
        }

        internal ConfigListNode(KeyType key, ConfigNode parent)
            : base(key, parent)
        {
            _children = new ObservableCollection<ConfigNode>();
        }

        /// <summary>
        /// Children of list node
        /// </summary>
        public override ObservableCollection<ConfigNode> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// Node Description for Array
        /// </summary>
        public override string NodeDescription
        {
            get { return "<Array>"; }
        }

        internal override ConfigNodeType NodeType
        {
            get { return ConfigNodeType.ARRAY; }
        }

        /// <summary>
        /// When collection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        

        /// <summary>
        /// Get values in one of the slots in array nodes
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var index = (int)indexes[0];
            if (index >= 0 && index < _children.Count)
            {
                result = _children[index];
            }
            else
            {
                result = new NullExceptionPreventer();
            }
            return true;
        }

        /// <summary>
        /// set value to one of slots in array node.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var index = (int)indexes[0];
            if (index < 0 || index >= _children.Count)
            {
                throw new IndexOutOfRangeException(String.Format("{0} is out of bound [0, {1}]", index, _children.Count));
            }

            if (PrimitiveTypeHelper.IsPrimitiveOrWrapper(value.GetType()))
            {
                var oldNode = _children[index];
                if (oldNode.NodeType == ConfigNodeType.LEAF)
                {
                    var n = (ConfigLeafNode)oldNode;
                    if (n.IsEncrypted)
                    {
                        n.ReplaceEncryptedContent(((string)value));
                    }
                    else
                    {
                        n.RawValue = value;
                    }
                }
                else
                {
                    throw new NotSupportedException("Cannot set simple values to array node: " + _key + ":" + index);
                }
            }
            else
            {
                var node = value as ConfigNode;
                if (node == null)
                {
                    throw new ArgumentException("Only compatible objects can be set.");
                }
                ReplaceChild(node, new IntKey(index));
            }

            return true;
        }

        /// <summary>
        /// Get member from array node will return NullExceptionPreventer
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new NullExceptionPreventer();
            return true;
        }

        /// <summary>
        /// Can't set member
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            throw new NotSupportedException("Can't set <key,value> to array node");
        }

        /// <summary>
        /// Get enumerator for children
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ConfigNode> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal override dynamic ConvertToExpando()
        {
            var array = new object[_children.Count];
            foreach (var item in _children)
            {
                var co = item.ConvertToExpando();
                array[item.RawKey] = co;
            }
            return array;
        }

        internal override void CreateNode()
        {
            var index = _children.Count;
            if (index > 0)
            {
                ConfigNode n = _children[0].Clone();
                n.RawKey = new IntKey(index);
                AppendChild(n);
            }
        }

        internal override void AppendChild(ConfigNode n)
        {
            n.IsSelected = true; //set selection before inserting to children list
            _children.Add(n);
            SortIndex();
            OnPropertyChanged("Children");
            n.SavePropertyChangeHandler(_currentChangedHandler); //wire handlers to new nodes
            PropagateChange(new AddChangeEventArgs(KeyLabel, n.KeyLabel));
        }

        internal override void ReplaceChild(ConfigNode n, KeyType key)
        {
            if (!(key is IntKey))
            {
                throw new ArgumentException("Replacing node on List asks for an IntKey");
            }

            int index = key.RawValue();
            if (index < 0 || index >= _children.Count)
            {
                throw new IndexOutOfRangeException("Can't set to index of " + index);
            }

            var oldNode = _children[index];
            if (oldNode.NodeType == ConfigNodeType.LEAF && n.NodeType == ConfigNodeType.LEAF)
            {
                var node = oldNode as ConfigLeafNode;
                if (node != null)
                {
                    if (node.IsEncrypted)
                    {
                        node.ReplaceEncryptedContent(((ConfigLeafNode)n).RawValue);
                    }
                    else
                    {
                        node.RawValue = ((ConfigLeafNode)n).RawValue;

                    }
                }
            }
            else
            {
                _children[index] = n;
                n.IsSelected = true;
                n.SavePropertyChangeHandler(_currentChangedHandler); //wire handlers to new nodes
                OnPropertyChanged("Children");
                PropagateChange(new ReplaceChangeEventArgs(KeyLabel, n.KeyLabel));
            }
        }

        internal override void DeleteChild(ConfigNode node)
        {
            if (_children.Remove(node))
            {
                SortIndex();
                OnPropertyChanged("Children");
                PropagateChange(new DeleteChangeEventArgs(KeyLabel, node.KeyLabel));
            }
        }

        internal override void SavePropertyChangeHandler(EventHandler<ConfigChangedEventArgs> handler)
        {
            _currentChangedHandler = handler;
            ValueChangedEventHandlers += handler;
            foreach (var item in _children)
            {
                item.SavePropertyChangeHandler(handler);
            }
        }

        internal override ConfigNode Clone()
        {
            dynamic conObj = ConvertToExpando();
            return new ConfigListNode(conObj, Key, _parent);
        }

        private void SortIndex()
        {
            for (var i = 0; i < _children.Count; i++)
            {
                var k = new IntKey(i);
                _children[i].RawKey = k;
            }
        }
    }
}