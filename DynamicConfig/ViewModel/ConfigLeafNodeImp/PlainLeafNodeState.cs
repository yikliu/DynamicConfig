using System;
using DynamicConfig.ConfigTray.Crypt;
using DynamicConfig.ConfigTray.Util;

namespace DynamicConfig.ConfigTray.ViewModel.ConfigLeafNodeImp
{
    class PlainLeafNodeState : ILeafNodeState
    {
        private dynamic _dynamicValue;
        private readonly Type _thisType;

        public PlainLeafNodeState(dynamic v)
        {
            _dynamicValue = v;
            _thisType = v.GetType();
        }
        
        public dynamic ConvertToExpando()
        {
            return _dynamicValue;
        }

        public string StringContent
        {
            get
            {
                return String.Format("{0}", _dynamicValue); 
            }
            set
            {
                var suc = OmniStringConverter.ConvertStringToPrimitive(value, _thisType, out _dynamicValue);
                if (!suc)
                {
                    throw new InvalidCastException(String.Format("Cannot convert string {0} to type {1}", value, _thisType.Name));
                }
            }
        }

        public bool ContentEditable
        {
            get { return true; }
        }

        public Type DataType { get { return _thisType; } }

        public dynamic DynamicValue
        {
            get { return _dynamicValue; }
            set
            {
                if (PrimitiveTypeHelper.IsCongruence(_thisType, value.GetType()))
                {
                    _dynamicValue = value;
                }
                else
                {
                    throw new ArgumentException("Type Mismatch: current type: " + _thisType.Name + ", new type: " +
                                                value.GetType());
                }
            }
        }

        public EncryptionMethod EncryptionMethodAssigned
        {
            get { return EncryptionMethod.NONE;} 
            set {throw new Exception( "Can't set encryption method on plain");}
        }

        public void GoToState(ConfigLeafNode theLeafNode, LeafState nextState)
        {
            if (nextState == LeafState.Plain) return;
            
            EncryptedLeafNodeState newState = null;
            
            if (nextState == LeafState.DPAPIMACHINEEncrypted)
            {
                newState = new DPAPIMachineState(StringContent);
            }

            if (nextState == LeafState.DPAPIUSerEncrypted)
            {
                newState = new DPAPIUserState(StringContent);
            }
            theLeafNode.State = newState;
        }

        public LeafState CurrentState => LeafState.Plain;

        public bool IsEncrypted()
        {
            return false;
        }

        public void Commit()
        {
            //Do nothing
        }

        public bool IsCommitted
        {
            get { return false; }
            set { }
        }

        public void NextState()
        {
            throw new NotImplementedException();
        }
    }
}
