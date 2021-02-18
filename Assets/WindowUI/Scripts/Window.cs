using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Window : MonoBehaviour
{
	#region Constants
	private static readonly int TITLE_BAR_SIZE = 20;
	#endregion

	#region Attributes
	// Parameters
	[SerializeField]
	private bool Draggable = true;
	[SerializeField]
	private bool Closable = true;
	[SerializeField]
	private string Title = "New window";

	// Components
	public BoxCollider2D Collision { get; private set; }
	private RectTransform _area;
	private Text _titleText;
	private Button _crossButton;

	// Util
	private bool _isDragged = false;
	private Vector2 _relativeDraggingPosition = new Vector2(0, 0);
	private ISet<IWindowEventListener> listeners = new HashSet<IWindowEventListener>();
	#endregion

	#region Overridden methods
	void Start()
    {
		//InitComponents();
		//SetTitle(_title);
		//SetClosable(_closable);
		//SetCollisionDimensions();
		//InitCrossButton();
	}

	private void OnValidate()
	{
		InitComponents();
		SetTitle(Title);
		SetClosable(Closable);
		SetCollisionDimensions();
		// Place here all things that must happen when you change
		// a value in the editor
	}

	void Update()
    {
		if (Draggable)
		{
			UpdatePosition();
		}
	}
	#endregion

	#region Public methods
	public void Init()
	{
		InitComponents();
		SetTitle(Title);
		SetClosable(Closable);
		SetCollisionDimensions();
		InitCrossButton();
	}

	/// <summary>
	/// Set the title of the window
	/// </summary>
	/// <param name="title">The new title</param>
	public void SetTitle(string title)
	{
		Title = title;
		GetComponentInChildren<Text>().text = Title;
	}

	/// <summary>
	/// Set whether the window should be closable by using the cross button
	/// </summary>
	/// <param name="closable">new value for argument closable</param>
	public void SetClosable(bool closable)
	{
		Closable = closable;
		_crossButton.interactable = closable;
	}

	/// <summary>
	/// Close the window
	/// </summary>
	public void CloseWindow()
	{
		WindowEventArgs args = new WindowEventArgs();
		args.Target = this;

		foreach (var listener in listeners)
			listener.WindowClosed(args);
		Destroy(gameObject);
	}

	public void Move(Vector2 movement)
	{
		transform.position += new Vector3(movement.x, movement.y, 0);
	}

	public void BeginDrag(Vector2 position)
	{
		_isDragged = true;
		_relativeDraggingPosition = Input.mousePosition;
		transform.SetAsLastSibling();
	}

	public void EndDrag()
	{
		_isDragged = false;
	}

	public void AddListener(IWindowEventListener listener)
	{
		listeners.Add(listener);
	}

	public void RemoveListener(IWindowEventListener listener)
	{
		listeners.Remove(listener);
	}
	#endregion

	#region Private methods
	/// <summary>
	/// Gather all the necessary components from the object
	/// </summary>
	private void InitComponents()
	{
		Collision = GetComponent<BoxCollider2D>();
		_area = GetComponent<RectTransform>();

		// TODO check if always correct
		_titleText = GetComponentInChildren<Text>();
		_crossButton = GetComponentInChildren<Button>();
	}

	/// <summary>
	/// Set the collision box's boundaries to be the same as rect transform
	/// </summary>
	private void SetCollisionDimensions()
	{
		Vector2 dimensions = _area.sizeDelta;
		dimensions.y = TITLE_BAR_SIZE;
		Collision.size = dimensions;
		Collision.offset = new Vector2(dimensions.x, -dimensions.y) / 2;
	}

	/// <summary>
	/// Set the action to be done when cross button is pressed
	/// </summary>
	private void InitCrossButton()
	{
		_crossButton.onClick.AddListener(CloseWindow);
	}

	/// <summary>
	/// Make the window follow the mouse
	/// </summary>
	private void UpdatePosition()
	{
		if (_isDragged)
		{
			Vector2 newPosition = Input.mousePosition;
			Rect screen = new Rect(new Vector2(TITLE_BAR_SIZE, TITLE_BAR_SIZE), new Vector2(Screen.width - (2*TITLE_BAR_SIZE), Screen.height - (2*TITLE_BAR_SIZE)));
			if (screen.Contains(newPosition))
			{
				Vector2 movement = newPosition - _relativeDraggingPosition;
				_relativeDraggingPosition = newPosition;
				Move(movement);
			}
		}
	}
	#endregion
}

public interface IWindowEventListener
{
	void WindowClosed(WindowEventArgs args);
}

public struct WindowEventArgs
{
	public Window Target { get; set; }
}