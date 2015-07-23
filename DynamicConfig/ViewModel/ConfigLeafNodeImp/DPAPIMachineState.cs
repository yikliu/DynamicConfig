using DynamicConfig.ConfigTray.Crypt;

namespace DynamicConfig.ConfigTray.ViewModel.ConfigLeafNodeImp
{
    class DPAPIMachineState : EncryptedLeafNodeState
    {
        public DPAPIMachineState(string text, bool isCommited = false) 
            : base(text, isCommited)
        {
            _encrypter = CipherFactory.CreateCipher(EncryptionMethod.DPAPI_MACHINE);
        }

        public override EncryptionMethod EncryptionMethodAssigned
        {
            get
            {
                return EncryptionMethod.DPAPI_MACHINE;
            }
        }

        public override void GoToState(ConfigLeafNode theLeafNode, LeafState nextState)
        {
            if (nextState == LeafState.DPAPIMACHINEEncrypted) return;
            string text = _isCommitted ? "<Reset Value>" : StringContent;
            switch (nextState)
            {
                case LeafState.DPAPIUSerEncrypted :
                    theLeafNode.State = new DPAPIUserState(text);
                    break;
                case LeafState.Plain:
                    theLeafNode.State = new PlainLeafNodeState(text);
                    break;
            }
        }

        public override LeafState CurrentState
        {
            get { return LeafState.DPAPIMACHINEEncrypted; }
        }

        protected override void CreateEncrypter()
        {
            if (_encrypter == null)
                _encrypter = CipherFactory.CreateCipher(EncryptionMethod.DPAPI_MACHINE);
        }
    }
}
