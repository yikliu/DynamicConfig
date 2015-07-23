using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using DynamicConfig.ConfigTray.JSONConfig;
using DynamicConfig.ConfigTray.Util;
using DynamicConfig.ConfigTray.ViewModel.ConfigChangedEvent;

namespace DynamicConfig.ConfigTray.ViewModel
{
    public class ConfigObjectNode : AbsCollectionConfigNode
    {
        private readonly Dictionary<string, ConfigNode> _children;

        internal ConfigObjectNode(KeyType key, ConfigNode parent)
            : base(key, parent)
        {
            _children = new Dictionary<string, ConfigNode>();
        }

        internal ConfigObjectNode(dynamic n, KeyType key, ConfigNode parent)
            : base(key, parent)
        {
            _children = new Dictionary<string, ConfigNode>();

            var expando = n as IDictionary<string, object>;
            foreach (var kvp in expando)
            {
                ConfigNode valueNode;
                var newKey = new StringKey(kvp.Key);

                if (kvp.Value is ExpandoObject)
                {
                    valueNode = ConfigNodeFactory.CreateConfigObjectNode(kvp.Value, newKey, this);
                }
                else if (kvp.Value is IList<object>)
                {
                    valueNode = ConfigNodeFactory.CreateConfigListNode(kvp.Value, newKey, this);
                }
                else
                {
                    valueNode = ConfigNodeFactory.CreateConfigLeafNode(kvp.Value, newKey, this);
                }

                _children.Add(kvp.Key, valueNode);
            }
        }

        public override ObservableCollection<ConfigNode> Children 
            => new ObservableCollection<ConfigNode>(_children.Values);

        public override string NodeDescription => "<Object>";

        internal override ConfigNodeType NodeType => ConfigNodeType.OBJECT;

        //It's ok to access Object[index], but returns NullExceptionPreventer
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = new NullExceptionPreventer();
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_children.ContainsKey(binder.Name))
            {
                result = _children[binder.Name];
            }
            else
            {
                result = new NullExceptionPreventer();
            }
            return true;
        }

        //can't set index to object, throw NotSupportedException
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            throw new NotSupportedException("Cannot set [index] on an object node");
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (_children.ContainsKey(binder.Name)) //replacing existing leaf node
            {
                ConfigNode thisNode = _children[binder.Name];
                if (value is ConfigNode)
                {
                    var strKey = new StringKey(binder.Name);
                    ReplaceChild((ConfigNode)value, strKey);
                }
                else if (PrimitiveTypeHelper.IsPrimitiveOrWrapper(value.GetType()))
                {
                    if (thisNode.NodeType == ConfigNodeType.LEAF)
                    {
                        var n = (ConfigLeafNode) thisNode;
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
                        throw new NotSupportedException("Cannot set values to object or array node, " + _key + ": " + binder.Name);
                    }
                }
                else
                {
                    throw new ArgumentException("Cannot recognize the type of new value: " + value.GetType().Name);
                }
            }
            else //set new properties
            {
                if (value is ConfigNode)
                {
                    AppendChild((ConfigNode)value);
                }
                else if (PrimitiveTypeHelper.IsPrimitiveOrWrapper(value.GetType()))
                {
                    //var s = String.Format("{0}", value);
                    var n = ConfigNodeFactory.CreateConfigLeafNode(value, new StringKey(binder.Name), this);
                    AppendChild(n);
                }
                else
                {
                    throw new ArgumentException("Cannot recognize the type of new value: " + value.GetType().Name);
                }
            }
            return true;
        }

        internal ConfigNode GetNodeByKey(string key)
        {
            return _children.ContainsKey(key) ? _children[key] : null;
        }

        internal override void AppendChild(ConfigNode n)
        {
            n.IsSelected = true;
            _children.Add(n.Key.ToString(), n);
            n.SavePropertyChangeHandler(_currentChangedHandler); //wire handlers to new nodes
            OnPropertyChanged("Children");
            PropagateChange(new AddChangeEventArgs(KeyLabel, n.KeyLabel));
        }

        internal override void ReplaceChild(ConfigNode n, KeyType key)
        {
            if (!(key is StringKey))
            {
                throw new ArgumentException("Replacing node on Object asks for a string key.");
            }
            string k = key.RawValue();
            if (_children.ContainsKey(k))
            {
                if (_children[k].NodeType == ConfigNodeType.LEAF && n.NodeType == ConfigNodeType.LEAF)
                {
                    var node = _children[k] as ConfigLeafNode;
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
                    _children[k] = n;
                    n.IsSelected = true;
                    n.SavePropertyChangeHandler(_currentChangedHandler); //wire handlers to new nodes
                    OnPropertyChanged("Children");
                    PropagateChange(new ReplaceChangeEventArgs(KeyLabel, n.KeyLabel));
                }
            }
            else
            {
                throw new KeyNotFoundException("No child with key " + key + " is found in " + KeyLabel);
            }
        }

        internal override ConfigNode Clone()
        {
            var conObj = ConvertToExpando();
            return ConfigNodeFactory.CreateConfigObjectNode(conObj, Key, _parent);
        }

        internal override dynamic ConvertToExpando()
        {
            var o = new ExpandoObject();
            var dic = o as IDictionary<string, object>;
            foreach (var item in _children)
            {
                object co = item.Value.ConvertToExpando();
                dic[item.Key] = co;
            }
            return o;
        }

        internal override void CreateNode()
        {
            var n = ConfigNodeFactory.CreateConfigLeafNode("Empty", new StringKey("newKey"), this);
            AppendChild(n);
        }

        internal override void DeleteChild(ConfigNode node)
        {
            _children.Remove(node.Key.ToString());
            IsSelected = true; //select parent after deletion
            PropagateChange(new DeleteChangeEventArgs(KeyLabel, node.KeyLabel));
        }

        internal override void SavePropertyChangeHandler(EventHandler<ConfigChangedEventArgs> handler)
        {
            _currentChangedHandler = handler;
            ValueChangedEventHandlers += handler;
            foreach (var item in _children)
            {
                item.Value.SavePropertyChangeHandler(handler);
            }
        }
    }
}