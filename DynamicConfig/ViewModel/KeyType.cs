namespace DynamicConfig.ConfigTray.ViewModel
{
    /// <summary>
    /// Int _affectedNodeKey (for array nodes).
    /// </summary>
    public class IntKey : KeyType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntKey"/> class.
        /// </summary>
        /// <param name="k">The index.</param>
        public IntKey(int k)
        {
            _key = k;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="KeyType" /> is editable.
        /// </summary>
        /// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
        public override bool Editable
        {
            get { return false; }
        }

        /// <summary>
        /// Clones this key.
        /// </summary>
        /// <returns>KeyType.</returns>
        public override KeyType Clone()
        {
            return new IntKey(_key);
        }

        /// <summary>
        /// Displays the key in string.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string DisplayString()
        {
            return "[" + _key.ToString() + "]";
        }
    }

    /// <summary>
    /// Abstract Class for key types.
    /// </summary>
    public abstract class KeyType
    {
        /// <summary>
        /// The key in dynamic object
        /// </summary>
        protected dynamic _key;

        /// <summary>
        /// Gets a value indicating whether this <see cref="KeyType"/> is editable.
        /// </summary>
        /// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
        public abstract bool Editable { get; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>KeyType.</returns>
        public abstract KeyType Clone();

        /// <summary>
        /// Displays the string.
        /// </summary>
        /// <returns>System.String.</returns>
        public abstract string DisplayString();

        /// <summary>
        /// return the key in its raw value
        /// </summary>
        /// <returns>dynamic.</returns>
        public dynamic RawValue()
        {
            return _key;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this key.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this key.</returns>
        public override string ToString()
        {
            return _key.ToString();
        }
    }

    /// <summary>
    /// String _affectedNodeKey.
    /// </summary>
    public class StringKey : KeyType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringKey"/> class.
        /// </summary>
        /// <param name="k">The k.</param>
        public StringKey(string k)
        {
            _key = k;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="StringKey"/> is editable.
        /// </summary>
        /// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
        public override bool Editable
        {
            get { return true; }
        }

        /// <summary>
        /// Clones this _affectedNodeKey.
        /// </summary>
        /// <returns>KeyType.</returns>
        public override KeyType Clone()
        {
            return new StringKey(_key + "_copy");
        }

        /// <summary>
        /// Displays the _affectedNodeKey in string .
        /// </summary>
        /// <returns>System.String.</returns>
        public override string DisplayString()
        {
            return "\'" + _key.ToString() + "\'";
        }
    }
}