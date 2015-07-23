using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using DynamicConfig.ConfigTray.Crypt;

namespace DynamicConfig.ConfigTray.ViewModel.ConfigLeafNodeImp
{
    abstract class EncryptedLeafNodeState : ILeafNodeState, INotifyPropertyChanged
    {
        protected ICipher _encrypter;
        
        protected string _currentText;
        protected bool _isCommitted;

        public EncryptedLeafNodeState(string text, bool isCommited = false)
        {
            _currentText = text;
            _isCommitted = isCommited;
        }
        
        public dynamic ConvertToExpando()
        {
            var childObj = new ExpandoObject();
            var dic = childObj as IDictionary<string, object>;
            dic["EncryptionMethod"] = EncryptionMethodAssigned.ToString();
            dic["Content"] = StringContent;
            return childObj;
        }

        public string StringContent
        {
            get
            {
                return _currentText;
            }
            set
            {
                _currentText = value;
            }
        }

        public string PlainContent
        {
            get { return _encrypter.DecryptValue(_currentText); }
        }

        public bool ContentEditable
        {
            get { return !_isCommitted; }
        }

        public Type DataType { get { return _currentText.GetType(); }}

        public dynamic DynamicValue
        {
            get { return StringContent; } 
            set { StringContent = value; }
        }

        public abstract EncryptionMethod EncryptionMethodAssigned { get; }

        public abstract void GoToState(ConfigLeafNode theLeafNode, LeafState nextState);
        
        public abstract LeafState CurrentState { get;  }

        public bool IsEncrypted()
        {
            return true;
        }

        protected  abstract void CreateEncrypter();

        public void Commit()
        {
            if (_isCommitted) return;
            CreateEncrypter();
            _currentText = _encrypter.EncryptValue(_currentText);
            _isCommitted = true;
        }

        public bool IsCommitted { get { return _isCommitted;  } set { _isCommitted = value; } }

        public void NextState()
        {
            if (_isCommitted)
            {
                _isCommitted = false;
                _currentText = "";
            }
            else
            {
                Commit();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(/*[CallerMemberName]*/ string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
