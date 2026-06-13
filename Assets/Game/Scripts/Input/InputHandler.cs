using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class ActionBinding
{
    public string actionName;
    public Key key;
}

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private List<ActionBinding> _bindings = new List<ActionBinding>
    {
        new ActionBinding { actionName = "MoveLeft",    key = Key.LeftArrow  },
        new ActionBinding { actionName = "MoveRight",   key = Key.RightArrow },
        new ActionBinding { actionName = "RotateRight", key = Key.UpArrow    },
        new ActionBinding { actionName = "RotateLeft",  key = Key.Z          },
        new ActionBinding { actionName = "SoftDrop",    key = Key.DownArrow  },
        new ActionBinding { actionName = "Confirm",     key = Key.DownArrow  },
        new ActionBinding { actionName = "Delete",      key = Key.UpArrow    },
        new ActionBinding { actionName = "NavLeft",     key = Key.LeftArrow  },
        new ActionBinding { actionName = "NavRight",    key = Key.RightArrow },
        new ActionBinding { actionName = "NavUp",       key = Key.UpArrow    },
        new ActionBinding { actionName = "NavDown",     key = Key.DownArrow  },
    };

    public event Action<string> OnActionDown;
    public event Action<string> OnActionHeld;
    public event Action<string> OnActionUp;

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        foreach (var binding in _bindings)
        {
            var keyControl = keyboard[binding.key];
            if (keyControl.wasPressedThisFrame)
                OnActionDown?.Invoke(binding.actionName);
            if (keyControl.isPressed)
                OnActionHeld?.Invoke(binding.actionName);
            if (keyControl.wasReleasedThisFrame)
                OnActionUp?.Invoke(binding.actionName);
        }
    }
}
