using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowsManager : MonoBehaviour, IWindowEventListener
{
	#region Constants
	private static readonly int OFFSET = 30;
	#endregion

	#region Attributes
	public static WindowsManager Instance { get; private set; }

	[SerializeField]
	private List<WindowTypeElement> _windowPrefabsList = new List<WindowTypeElement>();
	private IDictionary<string, GameObject> _windowPrefabsDict = new Dictionary<string, GameObject>();
	private ISet<Window> _windows = new HashSet<Window>();
	private Vector2 lastSpawnPos = new Vector2(0, Screen.height);
	#endregion

	#region Overridden methods
	// Start is called before the first frame update
	void Start()
	{
		Instance = this;
	}

	// Update is called once per frame
	void Update()
	{
		const int MOUSE_LEFT = 0;
		Vector2 mousePosition = Input.mousePosition;
		if (Input.GetMouseButtonDown(MOUSE_LEFT))
		{
			int maxSIndex = -1;
			Window selected = null;
			foreach (Window win in _windows)
			{
				if (win.Collision.OverlapPoint(mousePosition))
				{
					int sIndex = win.transform.GetSiblingIndex();
					if (sIndex > maxSIndex)
					{
						selected = win;
						maxSIndex = sIndex;
					}
				}
			}
			if (selected != null)
			{
				selected.BeginDrag(mousePosition);
			}
		}
		if (Input.GetMouseButtonUp(MOUSE_LEFT))
		{
			foreach (Window win in _windows)
			{
				win.EndDrag();
			}
		}
	}

	private void OnValidate()
	{
		// Fill the dictionary
		_windowPrefabsDict.Clear();
		foreach (var type in _windowPrefabsList)
		{
			// Doublons are ignored
			if(! _windowPrefabsDict.ContainsKey(type.Name))
				_windowPrefabsDict.Add(type.Name, type.Prefab);
		}
	}
	#endregion

	#region Public methods
	public GameObject CreateWindow(string title, string type)
	{
		GameObject windowObject = Instantiate(_windowPrefabsDict[type]);
		Window window = windowObject.GetComponent<Window>();
		window.Init();
		window.Move(lastSpawnPos);
		lastSpawnPos += new Vector2(OFFSET, -OFFSET);
		if (lastSpawnPos.x > Screen.width - 400 || lastSpawnPos.y < 400)
			lastSpawnPos = new Vector2(0, 0);
		window.SetTitle(title);
		windowObject.transform.SetParent(transform);

		// Handling windows listing
		_windows.Add(window);
		window.AddListener(this);

		return windowObject;
	}

	public void WindowClosed(WindowEventArgs args)
	{
		_windows.Remove(args.Target);
	}
	#endregion
}

[System.Serializable]
public struct WindowTypeElement{
	public string Name;
	public GameObject Prefab;
}