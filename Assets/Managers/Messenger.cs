using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class Messenger
{
    private static Messenger _defaultInstance = new Messenger();

    private readonly Dictionary<Type, List<WeakActionAndToken>> _recipientsOfSubclassesAction =
        new Dictionary<Type, List<WeakActionAndToken>>();

    private readonly Dictionary<Type, List<WeakActionAndToken>> _recipientsStrictAction =
        new Dictionary<Type, List<WeakActionAndToken>>();

    private readonly object _registerLock = new object();

    private bool _isCleanupRegistered;

    static Messenger()
    {
    }

    private Messenger()
    {
    }

    public static Messenger Default
    {
        get { return _defaultInstance; }
    }

    public virtual void Register<TMessage>(object recipient, Action<TMessage> action)
    {
        Register(recipient, null, false, action);
    }

    public virtual void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action)
    {
        Register(recipient, null, receiveDerivedMessagesToo, action);
    }

    public virtual void Register<TMessage>(object recipient, object token, Action<TMessage> action)
    {
        Register(recipient, token, false, action);
    }

    public virtual void Register<TMessage>(
        object recipient,
        object token,
        bool receiveDerivedMessagesToo,
        Action<TMessage> action)
    {
        lock (_registerLock)
        {
            Type messageType = typeof (TMessage);

            Dictionary<Type, List<WeakActionAndToken>> recipients = receiveDerivedMessagesToo
                                                                        ? _recipientsOfSubclassesAction
                                                                        : _recipientsStrictAction;

            lock (recipients)
            {
                List<WeakActionAndToken> list;

                if (!recipients.TryGetValue(messageType, out list))
                {
                    list = recipients[messageType] = new List<WeakActionAndToken>();
                }

                var weakAction = new WeakAction<TMessage>(recipient, action);

                var item = new WeakActionAndToken
                               {
                                   Action = weakAction,
                                   Token = token
                               };

                list.Add(item);
            }
        }

        //RequestCleanup();
    }

    public virtual void Send<TMessage>(TMessage message)
    {
        SendToTargetOrType(message, null, null);
    }

    public virtual void Send<TMessage, TTarget>(TMessage message)
    {
        SendToTargetOrType(message, typeof (TTarget), null);
    }

    public virtual void Send<TMessage>(TMessage message, object token)
    {
        SendToTargetOrType(message, null, token);
    }

    public virtual void Unregister(object recipient)
    {
        UnregisterFromLists(recipient, _recipientsOfSubclassesAction);
        UnregisterFromLists(recipient, _recipientsStrictAction);
    }

    public virtual void Unregister<TMessage>(object recipient)
    {
        Unregister<TMessage>(recipient, null, null);
    }

    public virtual void Unregister<TMessage>(object recipient, Action<TMessage> action)
    {
        Unregister(recipient, null, action);
    }

    public virtual void Unregister<TMessage>(object recipient, object token, Action<TMessage> action)
    {
        UnregisterFromLists(recipient, token, action, _recipientsStrictAction);
        UnregisterFromLists(recipient, token, action, _recipientsOfSubclassesAction);
        //RequestCleanup();
    }

    public static void OverrideDefault(Messenger newMessenger)
    {
        _defaultInstance = newMessenger;
    }

    public static void Reset()
    {
        _defaultInstance = null;
    }

    public void ResetAll()
    {
        Reset();
    }

    private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>> lists)
    {
        if (lists == null)
        {
            return;
        }

        lock (lists)
        {
            var listsToRemove = new List<Type>();
            foreach (var list in lists)
            {
                var recipientsToRemove = new List<WeakActionAndToken>();
                foreach (WeakActionAndToken item in list.Value)
                {
                    if (item.Action == null
                        || !item.Action.IsAlive)
                    {
                        recipientsToRemove.Add(item);
                    }
                }

                foreach (WeakActionAndToken recipient in recipientsToRemove)
                {
                    list.Value.Remove(recipient);
                }

                if (list.Value.Count == 0)
                {
                    listsToRemove.Add(list.Key);
                }
            }

            foreach (Type key in listsToRemove)
            {
                lists.Remove(key);
            }
        }
    }

    private static void SendToList<TMessage>(
        TMessage message,
        IList<WeakActionAndToken> list,
        Type messageTargetType,
        object token)
    {
        if (list != null)
        {
            // Clone to protect from people registering in a "receive message" method
            // Correction Messaging BL0004.007
            List<WeakActionAndToken> listClone = list.Take(list.Count()).ToList();

            foreach (WeakActionAndToken item in listClone)
            {
                var executeAction = item.Action as IExecuteWithObject;

                if (executeAction != null
                    && item.Action.IsAlive
                    && item.Action.Target != null
                    && (messageTargetType == null
                        || item.Action.Target.GetType() == messageTargetType
                        || messageTargetType.IsInstanceOfType(item.Action.Target))
                    && ((item.Token == null && token == null)
                        || item.Token != null && item.Token.Equals(token)))
                {
                    executeAction.ExecuteWithObject(message);
                }
            }
        }
    }

    private static void UnregisterFromLists(object recipient, Dictionary<Type, List<WeakActionAndToken>> lists)
    {
        if (recipient == null
            || lists == null
            || lists.Count == 0)
        {
            return;
        }

        lock (lists)
        {
            foreach (Type messageType in lists.Keys)
            {
                foreach (WeakActionAndToken item in lists[messageType])
                {
                    var weakAction = (IExecuteWithObject) item.Action;

                    if (weakAction != null
                        && recipient == weakAction.Target)
                    {
                        weakAction.MarkForDeletion();
                    }
                }
            }
        }
    }

    private static void UnregisterFromLists<TMessage>(
        object recipient,
        object token,
        Action<TMessage> action,
        Dictionary<Type, List<WeakActionAndToken>> lists)
    {
        Type messageType = typeof (TMessage);

        if (recipient == null
            || lists == null
            || lists.Count == 0
            || !lists.ContainsKey(messageType))
        {
            return;
        }

        lock (lists)
        {
            foreach (WeakActionAndToken item in lists[messageType])
            {
                WeakAction weakActionCasted = item.Action;

                if (weakActionCasted != null
                    && recipient == weakActionCasted.Target
                    && (action == null
                        || action.Method.Name == weakActionCasted.MethodName)
                    && (token == null
                        || token.Equals(item.Token)))
                {
                    item.Action.MarkForDeletion();
                }
            }
        }
    }

    //public void RequestCleanup()
    //{
    //    if (!_isCleanupRegistered)
    //    {
    //        ThreadPool.QueueUserWorkItem(o => Cleanup());
    //        _isCleanupRegistered = true;
    //    }
    //}

    public void Cleanup()
    {
        CleanupList(_recipientsOfSubclassesAction);
        CleanupList(_recipientsStrictAction);
        _isCleanupRegistered = false;
    }

    private void SendToTargetOrType<TMessage>(TMessage message, Type messageTargetType, object token)
    {
        Type messageType = typeof (TMessage);

        if (_recipientsOfSubclassesAction != null)
        {
            // Clone to protect from people registering in a "receive message" method
            // Correction Messaging BL0008.002
            List<Type> listClone =
                _recipientsOfSubclassesAction.Keys.Take(_recipientsOfSubclassesAction.Count()).ToList();

            foreach (Type type in listClone)
            {
                List<WeakActionAndToken> list = null;

                if (messageType == type
                    || messageType.IsSubclassOf(type)
                    || type.IsAssignableFrom(messageType))
                {
                    lock (_recipientsOfSubclassesAction)
                    {
                        list =
                            _recipientsOfSubclassesAction[type].Take(_recipientsOfSubclassesAction[type].Count())
                                                               .ToList();
                    }
                }

                SendToList(message, list, messageTargetType, token);
            }
        }

        if (_recipientsStrictAction != null)
        {
            lock (_recipientsStrictAction)
            {
                if (_recipientsStrictAction.ContainsKey(messageType))
                {
                    List<WeakActionAndToken> list = _recipientsStrictAction[messageType]
                        .Take(_recipientsStrictAction[messageType].Count())
                        .ToList();

                    SendToList(message, list, messageTargetType, token);
                }
            }
        }

        //RequestCleanup();
    }

    private struct WeakActionAndToken
    {
        public WeakAction Action;

        public object Token;
    }
}

public class WeakAction
{
    private Action _action;

    private Action _staticAction;

    protected WeakAction()
    {
    }

    public WeakAction(Action action)
        : this(action.Target, action)
    {
    }

    public WeakAction(object target, Action action)
    {
        if (action.Method.IsStatic)
        {
            _staticAction = action;

            if (target != null)
            {
                Reference = new WeakReference(target);
            }

            return;
        }

        Method = action.Method;
        ActionReference = new WeakReference(action.Target);
        Reference = new WeakReference(target);
    }

    protected MethodInfo Method { get; set; }

    public virtual string MethodName
    {
        get
        {
            if (_staticAction != null)
            {
                return _staticAction.Method.Name;
            }

            return Method.Name;
        }
    }

    protected WeakReference ActionReference { get; set; }

    protected WeakReference Reference { get; set; }

    public bool IsStatic
    {
        get { return _staticAction != null; }
    }

    public virtual bool IsAlive
    {
        get
        {
            if (_staticAction == null && Reference == null)
            {
                return false;
            }

            if (_staticAction != null)
            {
                return Reference == null || Reference.IsAlive;
            }

            return Reference.IsAlive;
        }
    }

    public object Target
    {
        get
        {
            return Reference == null ? null : Reference.Target;
        }
    }

    protected object ActionTarget
    {
        get
        {
            return ActionReference == null ? null : ActionReference.Target;
        }
    }

    public void Execute()
    {
        if (_staticAction != null)
        {
            _staticAction();
            return;
        }

        object actionTarget = ActionTarget;

        if (!IsAlive) return;
        if (Method != null
            && ActionReference != null
            && actionTarget != null)
        {
            Method.Invoke(ActionTarget, null);
        }
    }

    public void MarkForDeletion()
    {
        Reference = null;
        ActionReference = null;
        Method = null;
        _staticAction = null;
    }
}

public class WeakAction<T> : WeakAction, IExecuteWithObject
{
    private Action<T> _staticAction;

    public override string MethodName
    {
        get
        {
            return _staticAction != null ? _staticAction.Method.Name : Method.Name;
        }
    }

    public override bool IsAlive
    {
        get
        {
            if (_staticAction == null && Reference == null)
            {
                return false;
            }

            if (_staticAction != null)
            {
                return Reference == null || Reference.IsAlive;
            }

            return Reference.IsAlive;
        }
    }

    public WeakAction(Action<T> action) : this(action.Target, action)
    {
    }

    public WeakAction(object target, Action<T> action)
    {
        if (action.Method.IsStatic)
        {
            _staticAction = action;

            if (target != null)
            {
                Reference = new WeakReference(target);
            }

            return;
        }

        Method = action.Method;
        ActionReference = new WeakReference(action.Target);
        Reference = new WeakReference(target);
    }

    public new void Execute()
    {
        Execute(default(T));
    }

    public void Execute(T parameter)
    {
        if (_staticAction != null)
        {
            _staticAction(parameter);
            return;
        }

        if (!IsAlive) return;
        if (Method != null && ActionReference != null)
        {
            Method.Invoke(ActionTarget,new object[] { parameter });
        }
    }

    public void ExecuteWithObject(object parameter)
    {
        var parameterCasted = (T)parameter;
        Execute(parameterCasted);
    }

    public new void MarkForDeletion()
    {
        _staticAction = null;
        base.MarkForDeletion();
    }
}

public interface IExecuteWithObject
{
    object Target
    {
        get;
    }

    void ExecuteWithObject(object parameter);

    void MarkForDeletion();
}