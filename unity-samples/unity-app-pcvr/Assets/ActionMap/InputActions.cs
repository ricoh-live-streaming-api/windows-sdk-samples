// GENERATED AUTOMATICALLY FROM 'Assets/ActionMap/InputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""6172a9cb-335c-448e-8256-f60dbeee9890"",
            ""actions"": [
                {
                    ""name"": ""ToggleDisplay"",
                    ""type"": ""Button"",
                    ""id"": ""158cef79-acdf-4b42-88eb-e8333789d04c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Menu"",
                    ""type"": ""Button"",
                    ""id"": ""7832c095-1d3e-49c2-ae83-fdf750184cca"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""InitializeRotation"",
                    ""type"": ""Button"",
                    ""id"": ""3ab0729f-0407-470a-9032-da3556e96d31"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""5d961c5f-12f5-4926-8fb9-caa24dee24d3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""698ea077-54d2-406e-82cf-cab1dba72abd"",
                    ""path"": ""<ViveController>{LeftHand}/gripPressed"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""HTC Vive"",
                    ""action"": ""ToggleDisplay"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9447d1fd-15f8-4ca4-a6c6-1e86980c11d2"",
                    ""path"": ""<ViveController>{RightHand}/gripPressed"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""HTC Vive"",
                    ""action"": ""ToggleDisplay"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""c8f1ecb4-7f64-44c2-9f1b-d96d3122615b"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleDisplay"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""f82c8ac5-9c72-428a-b746-1909cdc813cc"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ToggleDisplay"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""096d3401-d2a3-4b77-b9e0-4a0b61e1f59e"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ToggleDisplay"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9a9d5cae-6582-43be-8a45-5fecd9b7419d"",
                    ""path"": ""<ViveController>{LeftHand}/menu"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""HTC Vive"",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4bde61dc-a419-4b0d-8c90-ff070d998224"",
                    ""path"": ""<ViveController>{RightHand}/menu"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""HTC Vive"",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""a004fcc8-9f35-4411-ae24-f87dc8af3712"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""f64a98d2-0b72-423f-8bcb-26c5ef3e6cee"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""f1c6c2c3-d182-4374-9fc1-15cad655fac7"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5e4f4d79-c7f0-4d01-88ab-6a19dd9b3b68"",
                    ""path"": ""<ViveController>{LeftHand}/trackpadClicked"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""HTC Vive"",
                    ""action"": ""InitializeRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""145b5e2d-f5fd-444b-951f-3de3e726494c"",
                    ""path"": ""<ViveController>{RightHand}/trackpadClicked"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""HTC Vive"",
                    ""action"": ""InitializeRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f82e1e2b-05e1-4f87-b9cf-af16400a3ac9"",
                    ""path"": ""<ViveController>{LeftHand}/trackpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""HTC Vive"",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""51b77659-7804-4db8-b3a4-4a655c9aecc2"",
                    ""path"": ""<ViveController>{RightHand}/trackpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""HTC Vive"",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""HTC Vive"",
            ""bindingGroup"": ""HTC Vive"",
            ""devices"": [
                {
                    ""devicePath"": ""<ViveController>{LeftHand}"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<ViveController>{RightHand}"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_ToggleDisplay = m_Player.FindAction("ToggleDisplay", throwIfNotFound: true);
        m_Player_Menu = m_Player.FindAction("Menu", throwIfNotFound: true);
        m_Player_InitializeRotation = m_Player.FindAction("InitializeRotation", throwIfNotFound: true);
        m_Player_Rotate = m_Player.FindAction("Rotate", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_ToggleDisplay;
    private readonly InputAction m_Player_Menu;
    private readonly InputAction m_Player_InitializeRotation;
    private readonly InputAction m_Player_Rotate;
    public struct PlayerActions
    {
        private @InputActions m_Wrapper;
        public PlayerActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @ToggleDisplay => m_Wrapper.m_Player_ToggleDisplay;
        public InputAction @Menu => m_Wrapper.m_Player_Menu;
        public InputAction @InitializeRotation => m_Wrapper.m_Player_InitializeRotation;
        public InputAction @Rotate => m_Wrapper.m_Player_Rotate;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @ToggleDisplay.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnToggleDisplay;
                @ToggleDisplay.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnToggleDisplay;
                @ToggleDisplay.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnToggleDisplay;
                @Menu.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMenu;
                @Menu.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMenu;
                @Menu.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMenu;
                @InitializeRotation.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInitializeRotation;
                @InitializeRotation.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInitializeRotation;
                @InitializeRotation.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInitializeRotation;
                @Rotate.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRotate;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ToggleDisplay.started += instance.OnToggleDisplay;
                @ToggleDisplay.performed += instance.OnToggleDisplay;
                @ToggleDisplay.canceled += instance.OnToggleDisplay;
                @Menu.started += instance.OnMenu;
                @Menu.performed += instance.OnMenu;
                @Menu.canceled += instance.OnMenu;
                @InitializeRotation.started += instance.OnInitializeRotation;
                @InitializeRotation.performed += instance.OnInitializeRotation;
                @InitializeRotation.canceled += instance.OnInitializeRotation;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    private int m_HTCViveSchemeIndex = -1;
    public InputControlScheme HTCViveScheme
    {
        get
        {
            if (m_HTCViveSchemeIndex == -1) m_HTCViveSchemeIndex = asset.FindControlSchemeIndex("HTC Vive");
            return asset.controlSchemes[m_HTCViveSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnToggleDisplay(InputAction.CallbackContext context);
        void OnMenu(InputAction.CallbackContext context);
        void OnInitializeRotation(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
    }
}
