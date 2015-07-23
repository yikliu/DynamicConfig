using System;
using System.Dynamic;

namespace DynamicConfig.ConfigTray.JSONConfig
{
    /// <summary>
    /// This is a fork from
    /// https://github.com/Dynalon/JsonConfig/blob/master/JsonConfig/ConfigObjects.cs
    ///
    /// Null exception preventer. This allows for hassle-free usage of configuration values that are not
    /// defined in the config file. I.e. we can do Config.Scope.This.Field.Does.Not.Exist.Ever, and it will
    /// not throw an NullPointer exception, but return te NullExceptionPreventer object instead.
    ///
    /// The NullExceptionPreventer can be cast to everything, and will then return default/empty value of
    /// that datatype.
    /// </summary>
    public class NullExceptionPreventer : DynamicObject
    {
        /// <summary>
        /// prevent null from NullPreventor[1], returns another NullExceptionPreventer
        /// </summary>
        /// <param name="index"></param>
        public NullExceptionPreventer this[int index]
        {
            get
            {
                return new NullExceptionPreventer();
            }

            set { } //does nothing
        }

        /// <summary>
        /// prevent null from NullPreventor["foo"], returns another NullExceptionPreventer
        /// </summary>
        /// <param name="key"></param>
        public NullExceptionPreventer this[string key]
        {
            get
            {
                return new NullExceptionPreventer();
            }
            set { }
        }

        /// <summary>
        /// Implicitly convert to string when using string(nep)
        /// </summary>
        /// <param name="nep"></param>
        /// <returns></returns>
        public static implicit operator string(NullExceptionPreventer nep)
        {
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nep"></param>
        /// <returns></returns>
        public static implicit operator string[](NullExceptionPreventer nep)
        {
            return new string[] { };
        }

        // cast to bool will always be false
        /// <summary>
        ///
        /// </summary>
        /// <param name="nep"></param>
        /// <returns></returns>
        public static implicit operator bool(NullExceptionPreventer nep)
        {
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nep"></param>
        /// <returns></returns>
        public static implicit operator bool[](NullExceptionPreventer nep)
        {
            return new bool[] { };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nep"></param>
        /// <returns></returns>
        public static implicit operator int[](NullExceptionPreventer nep)
        {
            return new int[] { };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nep"></param>
        /// <returns></returns>
        public static implicit operator long[](NullExceptionPreventer nep)
        {
            return new long[] { };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nep"></param>
        /// <returns></returns>
        public static implicit operator int(NullExceptionPreventer nep)
        {
            return 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nep"></param>
        /// <returns></returns>
        public static implicit operator long(NullExceptionPreventer nep)
        {
            return 0;
        }

        // nullable types always return null
        /// <summary>
        ///
        /// </summary>
        /// <param name="nep"></param>
        /// <returns></returns>
        public static implicit operator bool?(NullExceptionPreventer nep)
        {
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nep"></param>
        /// <returns></returns>
        public static implicit operator int?(NullExceptionPreventer nep)
        {
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nep"></param>
        /// <returns></returns>
        public static implicit operator long?(NullExceptionPreventer nep)
        {
            return null;
        }

        /// <summary>
        /// all member access to a NullExceptionPreventer will return a new NullExceptionPreventer
        /// this allows for infinite nesting levels: var s = Obj1.foo.bar.bla.blubb; is perfectly valid
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new NullExceptionPreventer();
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            throw new NotSupportedException("Cannot set value on NullExceptionPreventer Object");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return null;
        }
    }
}