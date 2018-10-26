using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using SerializableCollectionHelpers;
using UnityObject = UnityEngine.Object;
using System.Collections;
using UnityEngine.SceneManagement;

[Serializable, DebuggerDisplay("Count = {Count}")]
public class SerializableQueue<T> : IEnumerable<T> {
    [SerializeField, HideInInspector] int _Count;
    [SerializeField, HideInInspector] int _Version;
    [SerializeField, HideInInspector] bool _LimitSize;
    [SerializeField, HideInInspector] int _MaxSize;
    [SerializeField, HideInInspector] T[] _Values;

    public int Max { get { return _MaxSize; } }
    public bool IsSizeLimited { get { return _LimitSize; } }

    public int Count {
        get { return _Count; }
    }

    /// <summary>
    /// Set to 0 or don't call this function if you want to disable limited Size for the Queue
    /// </summary>
    /// <param name="max"></param>
    private void LimitSize(int max) {
        _MaxSize = max;
        if (max <= 0)
            _LimitSize = false;
        else
            _LimitSize = true;
    }

    /// <summary>
    /// Enables Limited Size for Queue
    /// </summary>
    /// <param name="max"></param>
    public void EnableLimit(int max) {
        LimitSize(max);
    }

    /// <summary>
    /// Disables Limited Size of Queue
    /// </summary>
    public void DisableLimit() {
        LimitSize(0);
    }

    readonly IEqualityComparer<T> _Comparer;

    //// Mainly for debugging purposes - to get the key-value pairs display
    //public Queue<T> AsQueue
    //{
    //	get { return new Queue<T>(this); }
    //}

    public T this[int _index, T defaultValue] {
        get {
            if (_index >= 0)
                return _Values[_index];
            return defaultValue;
        }
    }

    public T this[int _index] {
        get {
            if (_index >= 0)
                return _Values[_index];
            throw new KeyNotFoundException(_index.ToString());
        }

        set { _Values[_index]=(value); }
    }

    public SerializableQueue()
        : this(0, null) {
    }

    public SerializableQueue(int capacity)
        : this(capacity, null) {
    }

    public SerializableQueue(IEqualityComparer<T> comparer)
        : this(0, comparer) {
    }

    public SerializableQueue(int capacity, IEqualityComparer<T> comparer) {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException("capacity");

        Initialize(capacity);

        _Comparer = (comparer ?? EqualityComparer<T>.Default);
    }

    public SerializableQueue(IEnumerable<T> enumerable)
        : this(new SerializableQueue<T>(enumerable), null) {
    }

    public SerializableQueue(IEnumerable<T> enumerable, IEqualityComparer<T> comparer)
        : this((enumerable.IsAny()) ? enumerable.Count() : 0, comparer) {
        if (enumerable == null)
            throw new ArgumentNullException("queue");

        foreach (T value in enumerable)
            Enqueue(value);
    }
    
    public void Clear() {
        if (_Count <= 0)
            return;

        Array.Clear(_Values, 0, _Count);

        _Count = 0;
        _Version++;
    }

    private void Initialize(int capacity) {
        int prime = PrimeHelper.GetPrime(capacity);
        _Values = new T[prime];
    }
    
    //TODO: Remove At index, return true if succeeds, false if fails
    public void RemoveAt(int _index) {
        if (_index >= _Count) {
            throw new IndexOutOfRangeException();
        }
        else {
            T[] newValues = new T[_Values.Length - 1];
            CopyTo(newValues, 0, _index);
            _Values = newValues;
            _Count--;
        }
    }

    public void CopyTo(T[] array, int index, int exclude) {
        if (array == null)
			throw new ArgumentNullException("array");

		if (index < 0 || index > array.Length)
			throw new ArgumentOutOfRangeException(string.Format("index = {0} array.Length = {1}", index, array.Length));

		if (array.Length - index < Count - 1)
			throw new ArgumentException(string.Format("The number of elements in the queue ({0}) is greater than the available space from index to the end of the destination array {1}.", Count, array.Length));

        for (int i = 0; i < _Count; i++)
		{
            if (exclude == i)
                continue;
			array[index++] = _Values[i];
		}
    }

    public void CopyTo(T[] array, int index) {
        if (array == null)
			throw new ArgumentNullException("array");

		if (index < 0 || index > array.Length)
			throw new ArgumentOutOfRangeException(string.Format("index = {0} array.Length = {1}", index, array.Length));

		if (array.Length - index < Count)
			throw new ArgumentException(string.Format("The number of elements in the dictionary ({0}) is greater than the available space from index to the end of the destination array {1}.", Count, array.Length));

        for (int i = 0; i < _Count; i++)
		{
			array[index++] = _Values[i];
		}
    }

    public T Dequeue() {
        T item = _Values[0];
        RemoveAt(0);
        return item;
    }

    public IEnumerator<T> GetEnumerator() {
        return ((IEnumerable<T>)_Values).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable<T>)_Values).GetEnumerator();
    }

    //private int FindIndex(TKey key)
    //{
    //	if (key == null)
    //		throw new ArgumentNullException("key");

    //	if (_Buckets != null)
    //	{
    //		int hash = _Comparer.GetHashCode(key) & 2147483647;
    //		for (int i = _Buckets[hash % _Buckets.Length]; i >= 0; i = _Next[i])
    //		{
    //			if (_HashCodes[i] == hash && _Comparer.Equals(_Keys[i], key))
    //				return i;
    //		}
    //	}
    //	return -1;
    //}

    //public bool TryGetValue(out T value)
    //{
    //	int index = FindIndex();
    //	if (index >= 0)
    //	{
    //		value = _Values[index];
    //		return true;
    //	}
    //	value = default(TValue);
    //	return false;
    //}

    public IEnumerable<T> Values
	{
		get { return Values.Take(_Count).ToArray(); }
	}

	//public void CopyTo(T[] array, int index)
	//{
	//	if (array == null)
	//		throw new ArgumentNullException("array");

	//	if (index < 0 || index > array.Length)
	//		throw new ArgumentOutOfRangeException(string.Format("index = {0} array.Length = {1}", index, array.Length));

	//	if (array.Length - index < Count)
	//		throw new ArgumentException(string.Format("The number of elements in the Queue ({0}) is greater than the available space from index to the end of the destination array {1}.", Count, array.Length));

	//	for (int i = 0; i < _Count; i++)
	//	{
	//		array[index++] = this[i];
	//	}
	//}

	public bool IsReadOnly
	{
		get { return false; }
	}

    public void Enqueue(T value) // Similar to SDict Insert
	{
		if (_Values.Length > _MaxSize && _LimitSize) {
			UnityEngine.Debug.Log("WARNING: Dictionary has a maximum size! ATTEMPTED TO EXCEED THIS MAXIMUM!");
			return;
		}

		if (_Count == _Values.Length)
		{
            Resize();
		}
		
		_Values[_Count] = value;
		_Count++;
		_Version++;
    }

    private void Resize()
	{
		Resize(PrimeHelper.ExpandPrime(_Count));
	}

    private void Resize(int newSize)
	{
        var valuesCopy = new T[newSize];
		Array.Copy(_Values, 0, valuesCopy, 0, _Count);
		_Values = valuesCopy;
	}
}

#if UNITY_EDITOR
public abstract class SerializableQueueDrawer<T> : PropertyDrawer
{
	private SerializableQueue<T> _Queue;
	private bool _Foldout;
	private const float kButtonWidth = 18f;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		CheckInitialize(property, label);
		if (_Foldout)
			return (_Queue.Count + 1) * 17f;
		return 17f;
	}

    private void RemoveAt(int key)
	{
		_Queue.RemoveAt(key);
	}

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
        CheckInitialize(property, label);
		position.height = 17f;
        
        bool change = false;
		var foldoutRect = position;
		foldoutRect.width -= 2 * kButtonWidth;
		EditorGUI.BeginChangeCheck();
		_Foldout = EditorGUI.Foldout(foldoutRect, _Foldout, label, true);
		if (EditorGUI.EndChangeCheck())
			EditorPrefs.SetBool(label.text, _Foldout);

		var buttonRect = position;
		buttonRect.x = position.width - kButtonWidth + position.x;
		buttonRect.width = kButtonWidth + 2;

		if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButton)) {
			AddNewItem();
            EditorUtility.SetDirty((UnityObject)property.serializedObject.targetObject);
		}

		buttonRect.x -= kButtonWidth;

		if (GUI.Button(buttonRect, new GUIContent("X", "Clear queue"), EditorStyles.miniButtonRight)) {
			ClearQueue();
            change = true;
		}

        if (!_Foldout)
			return;
        bool take = true;
        change = false;
        T[] _sQ = new T[_Queue.Count];
        _Queue.CopyTo(_sQ, 0);
        int _qIndexOff = 0;

        for (int _qIndex = 0; _qIndex < _sQ.Length; _qIndex++)
		{
            T item = _sQ[_qIndex];
            take = true;
			var value = item;
			position.y += 17f;

			var indexRect = position;
			indexRect.width /= 2;
			indexRect.width -= 4;
			EditorGUI.LabelField(indexRect, ""+ _qIndex);

            var valueRect = position;
			valueRect.x = position.width / 2 + 15;
			valueRect.width = indexRect.width - kButtonWidth;
			EditorGUI.BeginChangeCheck();
			value = DoField(valueRect, typeof(T), value);
			if (EditorGUI.EndChangeCheck())
			{
				try
				{
                    change = true;
                    _Queue[_qIndex+_qIndexOff]=((T)value);
                    UnityEngine.Debug.Log(_qIndex + " changed to: " + value.ToString() + "\n\n");
				}
				catch(Exception e)
				{
					UnityEngine.Debug.Log(e.Message);
				}
			}
            
			var removeRect = valueRect;
			removeRect.x = valueRect.xMax + 2;
			removeRect.width = kButtonWidth;

			if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight)) {
                _Queue.RemoveAt(_qIndex + _qIndexOff);
                _qIndexOff -= 1;
                UnityEngine.Debug.Log(_qIndex + " deleted\n\n");
                change = true;
			}
		}
        if (change || GUI.changed) { 
            property.serializedObject.UpdateIfRequiredOrScript();
            EditorUtility.SetDirty((UnityObject)property.serializedObject.targetObject);
            if (Application.isEditor) { 
                var scene = SceneManager.GetActiveScene();
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            }
        }
    }
    
	private void CheckInitialize(SerializedProperty property, GUIContent label)
	{
		if (_Queue == null)
		{
			var target = property.serializedObject.targetObject;
			_Queue = fieldInfo.GetValue(target) as SerializableQueue<T>;
			if (_Queue == null)
			{
				_Queue = new SerializableQueue<T>();
				fieldInfo.SetValue(target, _Queue);
			}

			_Foldout = EditorPrefs.GetBool(label.text);
		}
	}

	private static readonly Dictionary<Type, Func<Rect, object, object>> _Fields =
		new Dictionary<Type,Func<Rect,object,object>>()
	{
		{ typeof(int), (rect, value) => EditorGUI.IntField(rect, (int)value) },
		{ typeof(float), (rect, value) => EditorGUI.FloatField(rect, (float)value) },
		{ typeof(string), (rect, value) => EditorGUI.TextField(rect, (string)value) },
		{ typeof(bool), (rect, value) => EditorGUI.Toggle(rect, (bool)value) },
		{ typeof(Vector2), (rect, value) => EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value) },
		{ typeof(Vector3), (rect, value) => EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value) },
		{ typeof(Bounds), (rect, value) => EditorGUI.BoundsField(rect, (Bounds)value) },
		{ typeof(Rect), (rect, value) => EditorGUI.RectField(rect, (Rect)value) },
	};

	private static T DoField<T>(Rect rect, Type type, T value)
	{
		Func<Rect, object, object> field;
		if (_Fields.TryGetValue(type, out field))
			return (T)field(rect, value);
		
        if (type.IsEnum)
			return (T)(object)EditorGUI.EnumPopup(rect, (Enum)(object)value);

        if (typeof(UnityObject).IsAssignableFrom(type)) {
            return (T)(object)EditorGUI.ObjectField(rect, (UnityObject)(object)value, type, true);
        }

		UnityEngine.Debug.Log("Type is not supported: " + type);
		return value;
	}

	private void ClearQueue()
	{
		_Queue.Clear();
	}

    private void AddNewItem()
    {
        if (_Queue.Count == _Queue.Max && _Queue.IsSizeLimited)
        {
            UnityEngine.Debug.Log("TOO MANY ITEMS!");
            return;
        }
        T value = (IsCollectionType(typeof(T)) || IsEnumerableType(typeof(T))) ? (T)Activator.CreateInstance(typeof(T)) : default(T);
        try
        {
            _Queue.Enqueue(value);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
        }
    }
    bool IsCollectionType(Type type)
    {
        return (type.GetInterface(nameof(ICollection)) != null);
    }
    bool IsEnumerableType(Type type)
    {
        return (type.GetInterface(nameof(IEnumerable)) != null);
    }
}
#endif
public static class QueueUtils {
    public static bool IsAny<T>(this IEnumerable<T> data) {
        return data != null && data.Any();
    }
}

// GUIDE TO CREATING PROPERTY DRAWERS IN INSPECTOR
// [Serializable] public class SerializeableQueueType: SerializableQueue<value_type (non-generic)> {}
// 
// [UnityEditor.CustomPropertyDrawer(typeof(SerializeableQueueType))]
// public class SerializeableQueueTypeDrawer : SerializeableQueueDrawer<value_type> { }
// DONE!