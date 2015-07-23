using DynamicConfig.ConfigTray.Crypt;

namespace DynamicConfig.ConfigTray.ViewModel.ConfigLeafNodeImp
{
    class DPAPIUserState : EncryptedLeafNodeState
    {
        public DPAPIUserState(string text, bool isCommited = false) 
            : base(text, isCommited)
        {
            _encrypter = CipherFactory.CreateCipher(EncryptionMethod.DPAPI_USER);
        }

        public override EncryptionMethod EncryptionMethodAssigned
        {
            get
            {
                return EncryptionMethod.DPAPI_USER;
            }
        }

        public override void GoToState(ConfigLeafNode theLeafNode, LeafState nextState)
        {
            if (nextState == LeafState.DPAPIUSerEncrypted) return;
            string text = _isCommitted ? "<Reset Value>" : StringContent;

            switch (nextState)
            {
                case LeafState.DPAPIMACHINEEncrypted:
                    theLeafNode.State = new DPAPIMachineState(text);
                    break;
                case LeafState.Plain:
                    theLeafNode.State = new PlainLeafNodeState(text);
                    break;
            }
        }

        public override LeafState CurrentState
        {
            get { return LeafState.DPAPIUSerEncrypted; }
        }

        protected override void CreateEncrypter()
        {
            if (_encrypter == null)
                _encrypter = CipherFactory.CreateCipher(EncryptionMethod.DPAPI_USER);
        }
    }
}
