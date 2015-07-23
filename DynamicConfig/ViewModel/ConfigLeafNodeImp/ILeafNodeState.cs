using System;
using DynamicConfig.ConfigTray.Crypt;

namespace DynamicConfig.ConfigTray.ViewModel.ConfigLeafNodeImp
{
    enum LeafState
    {
        Plain,
        DPAPIMACHINEEncrypted,
        DPAPIUSerEncrypted
    }
    
    interface ILeafNodeState
    {
        dynamic ConvertToExpando();

        string StringContent { get; set; }

        bool ContentEditable { get; }

        Type DataType { get; }

        dynamic DynamicValue { get; set; }

        EncryptionMethod EncryptionMethodAssigned { get; }

        void GoToState(ConfigLeafNode theLeafNode, LeafState nextState);

        LeafState CurrentState { get; }

        bool IsEncrypted();

        void Commit();

        bool IsCommitted { get; set; }

        void NextState();
    }


}
