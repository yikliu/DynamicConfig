using System;

namespace DynamicConfig.ConfigTray.ViewModel.ConfigChangedEvent
{
    /// <summary>
    /// Abstract class for changed event arguments
    /// </summary>
    public abstract class ConfigChangedEventArgs : EventArgs
    {
        /// <summary>
        ///
        /// </summary>
        protected string _affectedNodeKey;

        /// <summary>
        /// Description of Config changed event
        /// </summary>
        /// <returns></returns>
        public abstract string Description();
    }

    /// <summary>
    /// New child added
    /// </summary>
    public class AddChangeEventArgs : ConfigChangedEventArgs
    {
        private readonly string _addNewChildKey;

        /// <summary>
        /// A new child has been added to a container node
        /// </summary>
        /// <param name="k"></param>
        /// <param name="addNewChildKey"></param>
        public AddChangeEventArgs(string k, string addNewChildKey)
        {
            _affectedNodeKey = k;
            _addNewChildKey = addNewChildKey;
        }

        /// <summary>
        /// Add child event description
        /// </summary>
        /// <returns></returns>
        public override string Description()
        {
            return _affectedNodeKey + " added new child with key: " + _addNewChildKey;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class ReplaceChangeEventArgs : ConfigChangedEventArgs
    {
        private readonly string _replacedChildKey;

        /// <summary>
        /// Child of a container node is replaced
        /// </summary>
        /// <param name="k"></param>
        /// <param name="replacedChildKey"></param>
        public ReplaceChangeEventArgs(string k, string replacedChildKey)
        {
            _affectedNodeKey = k;
            _replacedChildKey = replacedChildKey;
        }

        /// <summary>
        /// Description of replacement event
        /// </summary>
        /// <returns></returns>
        public override string Description()
        {
            return _affectedNodeKey + " child of key: " + _replacedChildKey + " has a new node";
        }
    }

    /// <summary>
    /// Delete node event
    /// </summary>
    public class DeleteChangeEventArgs : ConfigChangedEventArgs
    {
        private readonly string _deletedChildKey;

        /// <summary>
        /// A child has been removed from a container node
        /// </summary>
        /// <param name="k"></param>
        /// <param name="deletedChild"></param>
        public DeleteChangeEventArgs(string k, string deletedChild)
        {
            _affectedNodeKey = k;
            _deletedChildKey = deletedChild;
        }

        /// <summary>
        /// Description of the deletion event
        /// </summary>
        /// <returns></returns>
        public override string Description()
        {
            return _affectedNodeKey + " deleted a child with key: " + _deletedChildKey;
        }
    }

    /// <summary>
    /// Edit Change Event
    /// </summary>
    public class EditChangeEventArgs : ConfigChangedEventArgs
    {
        private readonly string newValue;

        /// <summary>
        /// The value of a leaf node is changed.
        /// </summary>
        /// <param name="k"></param>
        /// <param name="v"></param>
        public EditChangeEventArgs(string k, string v)
        {
            _affectedNodeKey = k;
            newValue = v;
        }

        /// <summary>
        /// Description of Edit event
        /// </summary>
        /// <returns></returns>
        public override string Description()
        {
            return _affectedNodeKey + " has new value " + newValue;
        }
    }
}