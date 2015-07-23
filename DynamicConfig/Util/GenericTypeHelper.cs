using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using DynamicConfig.ConfigTray.ViewModel;

namespace DynamicConfig.ConfigTray.Util
{
    internal static class GenericTypeHelper
    {
        public static bool IsObservableCollection(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(ObservableCollection<>);
        }

        public static Type GetElementType(Type p)
        {
            if (IsObservableCollection(p))
            {
                return p.GetGenericArguments()[0];
            }
            return p;
        }

        public static void HookObserverableCollectionOnChangedHandler(object obsCollection, ConfigNode listNode)
        {
            if (IsObservableCollection(obsCollection.GetType()))
            {
                Type elemType = GetElementType(obsCollection.GetType());
                EventInfo eventInfo =
                    typeof(ObservableCollection<>).MakeGenericType(elemType).GetEvent("CollectionChanged");
                MethodInfo method = typeof(ConfigListNode).GetMethod("OnCollectionChange");
                Delegate d = Delegate.CreateDelegate(eventInfo.EventHandlerType, listNode, method);
                eventInfo.AddEventHandler(obsCollection, d);
            }
        }

        public static IList ConstructObservableCollectionOfType(Type elemType)
        {
            var constructorInfo = typeof(ObservableCollection<>).MakeGenericType(elemType).GetConstructor(Type.EmptyTypes);
            if (constructorInfo != null)
            {
                return (IList)constructorInfo.Invoke(null);
            }
            return null;
        }
    }
}