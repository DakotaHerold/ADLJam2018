using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityObject = UnityEngine.Object;
using SerializableCollectionHelpers;
using System.Reflection;

[Serializable, DebuggerDisplay("Count = {Count}")]
public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
	[SerializeField, HideInInspector] int[] _Buckets;
	[SerializeField, HideInInspector] int[] _HashCodes;
	[SerializeField, HideInInspector] int[] _Next;
	[SerializeField, HideInInspector] int _Count;
	[SerializeField, HideInInspector] int _Version;
	[SerializeField, HideInInspector] int _FreeList;
	[SerializeField, HideInInspector] int _FreeCount;
	[SerializeField, HideInInspector] TKey[] _Keys;
	[SerializeField, HideInInspector] TValue[] _Values;
	[SerializeField, HideInInspector] bool _LimitSize;
	[SerializeField, HideInInspector] int _MaxSize;

	public int Max { get { return _MaxSize; } }
	public bool IsSizeLimited { get { return _LimitSize; } }

	/// <summary>
	/// Set to 0 or don't call this function if you want to disable limited Size for the Dictionary
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
	/// Enables Limited Size for Dictionary
	/// </summary>
	/// <param name="max"></param>
	public void EnableLimit(int max) {
		LimitSize(max);
	}

	/// <summary>
	/// Disables Limited Size of Dictionary
	/// </summary>
	public void DisableLimit() {
		LimitSize(0);
	}

	readonly IEqualityComparer<TKey> _Comparer;

	// Mainly for debugging purposes - to get the key-value pairs display
	public Dictionary<TKey, TValue> AsDictionary
	{
		get { return new Dictionary<TKey, TValue>(this); }
	}

	public int Count
	{
		get { return _Count - _FreeCount; }
	}

	public TValue this[TKey key, TValue defaultValue]
	{
		get
		{
			int index = FindIndex(key);
			if (index >= 0)
				return _Values[index];
			return defaultValue;
		}
	}

	public TValue this[TKey key]
	{
		get
		{
			int index = FindIndex(key);
			if (index >= 0)
				return _Values[index];
			throw new KeyNotFoundException(key.ToString());
		}

		set { Insert(key, value, false); }
	}

	public SerializableDictionary()
		: this(0, null)
	{
	}

	public SerializableDictionary(int capacity)
		: this(capacity, null)
	{
	}

	public SerializableDictionary(IEqualityComparer<TKey> comparer)
		: this(0, comparer)
	{
	}

	public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException("capacity");

		Initialize(capacity);

		_Comparer = (comparer ?? EqualityComparer<TKey>.Default);
	}

	public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
		: this(dictionary, null)
	{
	}

	public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
		: this((dictionary != null) ? dictionary.Count : 0, comparer)
	{
		if (dictionary == null)
			throw new ArgumentNullException("dictionary");

		foreach (KeyValuePair<TKey, TValue> current in dictionary)
			Add(current.Key, current.Value);
	}

	public bool ContainsValue(TValue value)
	{
		if (value == null)
		{
			for (int i = 0; i < _Count; i++)
			{
				if (_HashCodes[i] >= 0 && _Values[i] == null)
					return true;
			}
		}
		else
		{
			var defaultComparer = EqualityComparer<TValue>.Default;
			for (int i = 0; i < _Count; i++)
			{
				if (_HashCodes[i] >= 0 && defaultComparer.Equals(_Values[i], value))
					return true;
			}
		}
		return false;
	}

	public bool ContainsKey(TKey key)
	{
		return FindIndex(key) >= 0;
	}

	public void Clear()
	{
		if (_Count <= 0)
			return;

		for (int i = 0; i < _Buckets.Length; i++)
			_Buckets[i] = -1;

		Array.Clear(_Keys, 0, _Count);
		Array.Clear(_Values, 0, _Count);
		Array.Clear(_HashCodes, 0, _Count);
		Array.Clear(_Next, 0, _Count);

		_FreeList = -1;
		_Count = 0;
		_FreeCount = 0;
		_Version++;
	}

	public void Add(TKey key, TValue value)
	{
		if (Keys.Count > _MaxSize && _LimitSize) {
			UnityEngine.Debug.Log("WARNING: Dictionary has a maximum size! ATTEMPTED TO EXCEED THIS MAXIMUM!");
			return;
		}
		else
			Insert(key, value, true);
	}

	private void Resize(int newSize, bool forceNewHashCodes)
	{
		int[] bucketsCopy = new int[newSize];
		for (int i = 0; i < bucketsCopy.Length; i++)
			bucketsCopy[i] = -1;

		var keysCopy = new TKey[newSize];
		var valuesCopy = new TValue[newSize];
		var hashCodesCopy = new int[newSize];
		var nextCopy = new int[newSize];

		Array.Copy(_Values, 0, valuesCopy, 0, _Count);
		Array.Copy(_Keys, 0, keysCopy, 0, _Count);
		Array.Copy(_HashCodes, 0, hashCodesCopy, 0, _Count);
		Array.Copy(_Next, 0, nextCopy, 0, _Count);

		if (forceNewHashCodes)
		{
			for (int i = 0; i < _Count; i++)
			{
				if (hashCodesCopy[i] != -1)
					hashCodesCopy[i] = (_Comparer.GetHashCode(keysCopy[i]) & 2147483647);
			}
		}

		for (int i = 0; i < _Count; i++)
		{
			int index = hashCodesCopy[i] % newSize;
			nextCopy[i] = bucketsCopy[index];
			bucketsCopy[index] = i;
		}

		_Buckets = bucketsCopy;
		_Keys = keysCopy;
		_Values = valuesCopy;
		_HashCodes = hashCodesCopy;
		_Next = nextCopy;
	}

	private void Resize()
	{
		Resize(PrimeHelper.ExpandPrime(_Count), false);
	}

	public bool Remove(TKey key)
	{
		if (key == null)
			throw new ArgumentNullException("key");

		int hash = _Comparer.GetHashCode(key) & 2147483647;
		int index = hash % _Buckets.Length;
		int num = -1;
		for (int i = _Buckets[index]; i >= 0; i = _Next[i])
		{
			if (_HashCodes[i] == hash && _Comparer.Equals(_Keys[i], key))
			{
				if (num < 0)
					_Buckets[index] = _Next[i];
				else
					_Next[num] = _Next[i];

				_HashCodes[i] = -1;
				_Next[i] = _FreeList;
				_Keys[i] = default(TKey);
				_Values[i] = default(TValue);
				_FreeList = i;
				_FreeCount++;
				_Version++;
				return true;
			}
			num = i;
		}
		return false;
	}

	private void Insert(TKey key, TValue value, bool add)
	{
		if (key == null)
			throw new ArgumentNullException("key");
		if (Keys.Count > _MaxSize && _LimitSize) {
			UnityEngine.Debug.Log("WARNING: Dictionary has a maximum size! ATTEMPTED TO EXCEED THIS MAXIMUM!");
			return;
		}
		if (_Buckets == null)
			Initialize(0);

		int hash = _Comparer.GetHashCode(key) & 2147483647;
		int index = hash % _Buckets.Length;
		int num1 = 0;
		for (int i = _Buckets[index]; i >= 0; i = _Next[i])
		{
			if (_HashCodes[i] == hash && _Comparer.Equals(_Keys[i], key))
			{
				if (add)
					throw new ArgumentException("Key already exists: " + key);

				_Values[i] = value;
				_Version++;
				return;
			}
			num1++;
		}
		int num2;
		if (_FreeCount > 0)
		{
			num2 = _FreeList;
			_FreeList = _Next[num2];
			_FreeCount--;
		}
		else
		{
			if (_Count == _Keys.Length)
			{
				Resize();
				index = hash % _Buckets.Length;
			}
			num2 = _Count;
			_Count++;
		}
		_HashCodes[num2] = hash;
		_Next[num2] = _Buckets[index];
		_Keys[num2] = key;
		_Values[num2] = value;
		_Buckets[index] = num2;
		_Version++;

		//if (num3 > 100 && HashHelpers.IsWellKnownEqualityComparer(comparer))
		//{
		//    comparer = (IEqualityComparer<TKey>)HashHelpers.GetRandomizedEqualityComparer(comparer);
		//    Resize(entries.Length, true);
		//}
	}

	private void Initialize(int capacity)
	{
		int prime = PrimeHelper.GetPrime(capacity);

		_Buckets = new int[prime];
		for (int i = 0; i < _Buckets.Length; i++)
			_Buckets[i] = -1;

		_Keys = new TKey[prime];
		_Values = new TValue[prime];
		_HashCodes = new int[prime];
		_Next = new int[prime];

		_FreeList = -1;
	}

	private int FindIndex(TKey key)
	{
		if (key == null)
			throw new ArgumentNullException("key");

		if (_Buckets != null)
		{
			int hash = _Comparer.GetHashCode(key) & 2147483647;
			for (int i = _Buckets[hash % _Buckets.Length]; i >= 0; i = _Next[i])
			{
				if (_HashCodes[i] == hash && _Comparer.Equals(_Keys[i], key))
					return i;
			}
		}
		return -1;
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		int index = FindIndex(key);
		if (index >= 0)
		{
			value = _Values[index];
			return true;
		}
		value = default(TValue);
		return false;
	}
    
	public ICollection<TKey> Keys
	{
		get { return _Keys.Take(Count).ToArray(); }
	}

	public ICollection<TValue> Values
	{
		get { return _Values.Take(Count).ToArray(); }
	}

	public void Add(KeyValuePair<TKey, TValue> item)
	{
		Add(item.Key, item.Value);
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		int index = FindIndex(item.Key);
		return index >= 0 &&
			EqualityComparer<TValue>.Default.Equals(_Values[index], item.Value);
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
	{
		if (array == null)
			throw new ArgumentNullException("array");

		if (index < 0 || index > array.Length)
			throw new ArgumentOutOfRangeException(string.Format("index = {0} array.Length = {1}", index, array.Length));

		if (array.Length - index < Count)
			throw new ArgumentException(string.Format("The number of elements in the dictionary ({0}) is greater than the available space from index to the end of the destination array {1}.", Count, array.Length));

		for (int i = 0; i < _Count; i++)
		{
			if (_HashCodes[i] >= 0)
				array[index++] = new KeyValuePair<TKey, TValue>(_Keys[i], _Values[i]);
		}
	}

	public bool IsReadOnly
	{
		get { return false; }
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		return Remove(item.Key);
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return GetEnumerator();
	}

	public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
	{
		private readonly SerializableDictionary<TKey, TValue> _Dictionary;
		private int _Version;
		private int _Index;
		private KeyValuePair<TKey, TValue> _Current;

		public KeyValuePair<TKey, TValue> Current
		{
			get { return _Current; }
		}

		internal Enumerator(SerializableDictionary<TKey, TValue> dictionary)
		{
			_Dictionary = dictionary;
			_Version = dictionary._Version;
			_Current = default(KeyValuePair<TKey, TValue>);
			_Index = 0;
		}

		public bool MoveNext()
		{
			if (_Version != _Dictionary._Version)
				throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}", _Version, _Dictionary._Version));

			while (_Index < _Dictionary._Count)
			{
				if (_Dictionary._HashCodes[_Index] >= 0)
				{
					_Current = new KeyValuePair<TKey, TValue>(_Dictionary._Keys[_Index], _Dictionary._Values[_Index]);
					_Index++;
					return true;
				}
				_Index++;
			}

			_Index = _Dictionary._Count + 1;
			_Current = default(KeyValuePair<TKey, TValue>);
			return false;
		}

		void IEnumerator.Reset()
		{
			if (_Version != _Dictionary._Version)
				throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}", _Version, _Dictionary._Version));

			_Index = 0;
			_Current = default(KeyValuePair<TKey, TValue>);
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		public void Dispose()
		{
		}
	}
}
	
#if UNITY_EDITOR
public abstract class SerializableDictionaryDrawer<TK, TV> : PropertyDrawer
{
	private SerializableDictionary<TK, TV> _Dictionary;
    private Dictionary<TK, TV> tempDict;
	private bool _Foldout;
	private const float kButtonWidth = 18f;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		CheckInitialize(property, label);
		if (_Foldout)
			return (_Dictionary.Count + 1) * 17f;
		return 17f;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		CheckInitialize(property, label);

		position.height = 17f;

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
		}

		buttonRect.x -= kButtonWidth;

		if (GUI.Button(buttonRect, new GUIContent("X", "Clear dictionary"), EditorStyles.miniButtonRight)) {
			ClearDictionary();
		}
		 
		if (!_Foldout)
			return;

        foreach (var item in _Dictionary) {
            var key = item.Key;
            var value = item.Value;

            position.y += 17f;

            var keyRect = position;
            keyRect.width /= 2;
            keyRect.width -= 4;
            EditorGUI.BeginChangeCheck();
            var newKey = (TK)DoField(keyRect, typeof(TK), key);
            if (EditorGUI.EndChangeCheck()) {
                if(tempDict.ContainsKey(newKey)){
                    tempDict.Remove(newKey);
                }
                tempDict.Add(newKey, value);
                try {
                    if(_Dictionary.ContainsKey(key))
                        _Dictionary.Remove(key);
                    _Dictionary.Add(newKey, value);
                }
                catch (Exception e) {
                    UnityEngine.Debug.Log(e.Message);
                }
                break;
            }

            var valueRect = position;
            valueRect.x = position.width / 2 + 15;
            valueRect.width = keyRect.width - kButtonWidth;
            EditorGUI.BeginChangeCheck();
            value = DoField(valueRect, typeof(TV), value);
            if (EditorGUI.EndChangeCheck()) {
                _Dictionary[key] = value;
                break;
            }

            var removeRect = valueRect;
            removeRect.x = valueRect.xMax + 2;
            removeRect.width = kButtonWidth;

            if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight))
            {
                RemoveItem(key);
                break;
            }
        }
	}

	private void RemoveItem(TK key)
	{
		_Dictionary.Remove(key);
	}

	private void CheckInitialize(SerializedProperty property, GUIContent label)
	{
		if (_Dictionary == null)
		{
			var target = property.serializedObject.targetObject;
			_Dictionary = fieldInfo.GetValue(target) as SerializableDictionary<TK, TV>;
            tempDict = new Dictionary<TK, TV>();
			if (_Dictionary == null)
			{
				_Dictionary = new SerializableDictionary<TK, TV>();
				fieldInfo.SetValue(target, _Dictionary);
			}

			_Foldout = EditorPrefs.GetBool(label.text);
		}
	}

	private static readonly Dictionary<Type, Func<Rect, object, object>> _Fields =
		new Dictionary<Type,Func<Rect,object,object>>()
	{
        { typeof(int), (rect, value) => EditorGUI.DelayedIntField(rect, (int)value) },
        { typeof(float), (rect, value) => EditorGUI.DelayedFloatField(rect, (float)value) },
        { typeof(double), (rect, value) => EditorGUI.DelayedDoubleField(rect, (double)value) },
		{ typeof(string), (rect, value) => EditorGUI.DelayedTextField(rect, (string)value) },
		{ typeof(bool), (rect, value) => EditorGUI.Toggle(rect, (bool)value) },
		{ typeof(Vector2), (rect, value) => EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value) },
		{ typeof(Vector3), (rect, value) => EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value) },
		{ typeof(Bounds), (rect, value) => EditorGUI.BoundsField(rect, (Bounds)value) },
		{ typeof(Rect), (rect, value) => EditorGUI.RectField(rect, (Rect)value) },
	};

	public static T DoField<T>(Rect rect, Type type, T value)
	{
		Func<Rect, object, object> field;
		if (_Fields.TryGetValue(type, out field))
			return (T)field(rect, value);

		if (type.IsEnum)
			return (T)(object)EditorGUI.EnumPopup(rect, (Enum)(object)value);

		if (typeof(UnityObject).IsAssignableFrom(type))
			return (T)(object)EditorGUI.ObjectField(rect, (UnityObject)(object)value, type, true);

        if (typeof(Color).IsAssignableFrom(type))
            return (T)(object)EditorGUI.ColorField(rect, (Color)(object)value);

        if ((typeof(T) == typeof(string)))
            return (T)Convert.ChangeType(EditorGUI.DelayedTextField(rect, (string)(object)(value)), typeof(T));
        /*if(IsDictType(type)){
            IDictionary ick = (IDictionary)value;
            Type dictType = value.GetType();
            Type[] dictArgs = { ick.GetType().GetGenericArguments()[0], ick.GetType().GetGenericArguments()[1] };
            IDictionary temp = (IDictionary)Activator.CreateInstance(dictType);

            Vector2 position = rect.position;
            Vector2 fieldSize = new Vector2(100f, 15f);
            foreach (DictionaryEntry x in ick){ // TODO: Invoke the DoField     
                var key = x.Key;
                var val = x.Value;

                Rect keyRect = new Rect(position, fieldSize);
                Rect valRect = new Rect(position+rightWardShift, fieldSize);
                MethodInfo method = dictType.GetMethod("DoField", dictArgs);
                //method.Invoke();
                    
                temp[key] = val;
            }
            return (T)ick;
        }*/
        //if(IsCollectionType(type)) //TODO: Edit values
        //{
        //    ICollection ick = ((ICollection)value);
        //    Type t = ick.GetType();
        //    return (T) DoCollectionField(rect,typeof(T),(ICollection)value);
        //}
        /*
        if(IsEnumerableType(type)){ //TODO: Edit values
            
        //}*/
        //if(value is ScriptableObject){ //TODO: Edit values
        //    return Editor.CreateEditor(value,type);
        //}
		UnityEngine.Debug.Log("Type is not supported: " + type);
		return value;
	}
    /*private static K DoCollectionField<K>(Rect position, Type type, K value) where K:ICollection{
        Type listType = value.GetType();
        Type[] typeArgs = { value.GetType().GetGenericArguments()[0] };
        if (value == null)
            value = (K)Activator.CreateInstance(type);
        
        IList ic = value.Cast<K>().ToList();
        IList tempCollection = ((K)Activator.CreateInstance(listType)).Cast<K>().ToList();

        position.height = 17f;
        var buttonRect = position;
        buttonRect.x = position.width - kButtonWidth + position.x;
        buttonRect.width = kButtonWidth + 2;
        if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButton))
        {
            ConstructorInfo constructor = typeArgs[0].GetConstructor(Type.EmptyTypes);
            ic.Add(constructor.Invoke(new object[] { }));
        }

        buttonRect.x -= kButtonWidth;

        if (GUI.Button(buttonRect, new GUIContent("X", "Clear dictionary"), EditorStyles.miniButtonRight))
        {
            ic.Clear();
        }

        for (int i = 0; i < ic.Count; i++)
        {
            position.y += 17f;

            var keyRect = position;
            keyRect.width /= 2;
            keyRect.width -= 4;
            EditorGUILayout.LabelField(""+i);

            var valueRect = position;
            valueRect.x = position.width / 2 + 15;
            valueRect.width = keyRect.width - kButtonWidth;
            EditorGUI.BeginChangeCheck();
            value = DoField(valueRect, value.GetType(), value);
            if (EditorGUI.EndChangeCheck())
            {
                tempCollection.Add(value);
                continue;
            }

            var removeRect = valueRect;
            removeRect.x = valueRect.xMax + 2;
            removeRect.width = kButtonWidth;

            if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight))
            {
                continue;
            }
            tempCollection.Add(value);
        }

        ic.Clear();
        for (int j = 0; j < tempCollection.Count; j++){
            ic.Add(tempCollection.Cast<IList>().ToArray()[j]);
        }
        tempCollection.Clear();
        return (K)ic;
    }*/

	private void ClearDictionary()
	{
		_Dictionary.Clear();
	}

    private void AddNewItem(){
		if (_Dictionary.Count == _Dictionary.Max && _Dictionary.IsSizeLimited) {
			UnityEngine.Debug.Log("TOO MANY ITEMS!");
			return;
		}
		TK key;
		if (typeof(TK)==typeof(string)){
			int i = 0;
			while (_Dictionary.Keys.Contains((TK)Convert.ChangeType(i, typeof(TK))))
				i++;
			key = (TK)Convert.ChangeType(i, typeof(TK));
		}
        else if (typeof(TK) == typeof(int))
        {
            int i = 0;
            while (_Dictionary.Keys.Contains((TK)(object)i))
                i++;
            key = (TK)(object)i;
        }
        else if (typeof(TK) == typeof(float))
        {
            float i = 0.0f;
            while (_Dictionary.Keys.Contains((TK)(object)i))
                i++;
            key = (TK)(object)i;
        }
        else if (typeof(TK) == typeof(double))
        {
            double i = 0.0;
            while (_Dictionary.Keys.Contains((TK)(object)i))
                i++;
            key = (TK)(object)i;
        }
		else if (typeof(TK).IsEnum) {
			_Dictionary.EnableLimit(Enum.GetValues(typeof(TK)).Cast<TK>().ToList<TK>().Count);
			List<TK> possible = Enum.GetValues(typeof(TK)).Cast<TK>().Except(_Dictionary.Keys).ToList<TK>();
			if (possible.Count == 0)
				key = _Dictionary.Keys.ElementAt(0);
			else {
				key = possible.ElementAt(0);
			}
		}
        else key = (typeof(TK)==typeof(string))? (TK)Convert.ChangeType("",typeof(TK)) : (IsDictType(typeof(TK))||IsCollectionType(typeof(TK)) || IsEnumerableType(typeof(TK))) ? (TK)Activator.CreateInstance(typeof(TK)) : default(TK);

        var value = (typeof(TV)==typeof(string))? (TV)Convert.ChangeType("",typeof(TV)) :(IsDictType(typeof(TK)) ||IsCollectionType(typeof(TV))||IsEnumerableType(typeof(TV)))? (TV)Activator.CreateInstance(typeof(TV)): default(TV);
		try
		{
			_Dictionary.Add(key, (TV)value);
		}
		catch(Exception e)
		{
			UnityEngine.Debug.Log(e.Message);
		}
	}
    public static bool IsCollectionType(Type type)
    {
        return (type.GetInterface(nameof(ICollection)) != null);
    }
    public static bool IsDictType(Type type)
    {
        return (type.GetInterface(nameof(IDictionary)) != null);
    }
    public static bool IsEnumerableType(Type type)
    {
        return (type.GetInterface(nameof(IEnumerable)) != null);
    }
}
#endif


// GUIDE TO CREATING PROPERTY DRAWERS IN INSPECTOR
// [Serializable] public class Type: <key_type (non-generic), 
//																				  value_type (non-generic)> {}
// NOTE: DON'T FORGET TO FIX WITH COMPILE CHECKS BEFORE BUILDING
// [UnityEditor.CustomPropertyDrawer(typeof(Type))]
// public class TypeDrawer : Drawer<key_type, value_type> { }
// DONE!

// IF you want to use this in a custom inspector without the base inspector drawing, use this:
// SerializedProperty p = serializedObject.FindProperty("VariableName");
// EditorGUI.BeginChangeCheck();
// EditorGUILayout.PropertyField(p);
// if (EditorGUI.EndChangeCheck())
//     serializedObject.ApplyModifiedProperties();