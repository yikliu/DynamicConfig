namespace DynamicConfig.ConfigTray.ViewModel
{
    public class IntKey : KeyType
    {
        public IntKey(int k)
        {
            _key = k;
        }

        public override bool Editable
        {
            get { return false; }
        }

        public override KeyType Clone()
        {
            return new IntKey(_key);
        }

        public override string DisplayString()
        {
            return "[" + _key.ToString() + "]";
        }
    }

    public abstract class KeyType
    {
        protected dynamic _key;

        public abstract bool Editable { get; }

        public abstract KeyType Clone();

        public abstract string DisplayString();

        public dynamic RawValue()
        {
            return _key;
        }

        public override string ToString()
        {
            return _key.ToString();
        }
    }

    public class StringKey : KeyType
    {
        public StringKey(string k)
        {
            _key = k;
        }

        public override bool Editable
        {
            get { return true; }
        }

        public override KeyType Clone()
        {
            return new StringKey(_key + "_copy");
        }

        public override string DisplayString()
        {
            return "\'" + _key.ToString() + "\'";
        }
    }
}