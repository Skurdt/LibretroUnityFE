// GENERATED AUTOMATICALLY FROM 'Assets/LibretroControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace SK.Libretro
{
    public class @LibretroControls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @LibretroControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""LibretroControls"",
    ""maps"": [
        {
            ""name"": ""RetroPad 1"",
            ""id"": ""639a1fe9-28e1-41ea-a98f-20b729bf505d"",
            ""actions"": [
                {
                    ""name"": ""Directions"",
                    ""type"": ""Value"",
                    ""id"": ""3d136013-1adf-4a01-ad86-7e5f4e85dd81"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Start"",
                    ""type"": ""Button"",
                    ""id"": ""e878ca3f-db65-4bde-a551-b6835a94fe79"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""4d4bc469-ce5e-4b79-ab42-acdd23d704ae"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""A"",
                    ""type"": ""Button"",
                    ""id"": ""9332aaf6-1881-41c4-931a-e035f8f7770a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""B"",
                    ""type"": ""Button"",
                    ""id"": ""9c24f261-5878-4731-84f6-c78c63030707"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""X"",
                    ""type"": ""Button"",
                    ""id"": ""f36bdbe8-984d-4ee1-828f-09864532efd2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Y"",
                    ""type"": ""Button"",
                    ""id"": ""e53ba5ed-7472-4b78-84c0-1fb5090f8c02"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""L"",
                    ""type"": ""Button"",
                    ""id"": ""43910c93-05e7-444e-a9a2-e9672dc24851"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""R"",
                    ""type"": ""Button"",
                    ""id"": ""e4753e14-08e3-4a66-8300-14400a4ac68f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""L2"",
                    ""type"": ""Button"",
                    ""id"": ""b06fcc7f-56a6-474b-8210-8542c0a84670"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""R2"",
                    ""type"": ""Button"",
                    ""id"": ""7b0cf759-77c7-4989-9771-c92f0d252cbe"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""L3"",
                    ""type"": ""Button"",
                    ""id"": ""818c3450-ed6f-43c4-b028-bd2e462d9531"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""R3"",
                    ""type"": ""Button"",
                    ""id"": ""056ab63b-501f-43c2-b9ef-6f18ba200cbc"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""b2a7ceab-af4e-4bfc-90db-e3360d2fe0b7"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Directions"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""fdc94176-2f02-4c3d-89d1-74e09fa56104"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Directions"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a04efd59-bfcf-4ceb-be36-5a390048db55"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Directions"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f7196911-5a3f-4bc9-919e-93a050149ecf"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Directions"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""7cf4fcff-1d2d-456d-9ece-46014e690c43"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Directions"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""DPad"",
                    ""id"": ""13a1db1a-a08d-47d1-8ef6-1eb4d980b1d5"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Directions"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c4a5c986-e12b-47b4-a5e5-b6bc48b65883"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Directions"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""37e086d4-1d62-4717-a4ad-269b669c4176"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Directions"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2774aadb-9811-4295-8ddc-5ce5b6f7934b"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Directions"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""69a888bc-d73e-4c8f-bf91-6e4641701de0"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Directions"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""LeftStick"",
                    ""id"": ""81f1baff-614e-4fbd-8216-ce80537a3a61"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Directions"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Up"",
                    ""id"": ""8154415e-4e9d-42ca-bd6e-dffa189c2f17"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Directions"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""93c698a3-68a3-4686-8b4d-e8b90d6ba57c"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Directions"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""ba19ae2d-9b41-4d24-b852-3cc44c994b77"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Directions"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""0f3516fc-653c-4258-8348-cae729dd0bec"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Directions"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""371504df-3569-4ff7-825d-dfac2008a4e4"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""da956248-c9a0-4720-8f05-36ee4fe4eab0"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bfb94c62-c926-4803-b582-2f9bbfd6669d"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""edf7d7f0-f327-4522-9a4f-5537f814565c"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""148082d5-648c-47d7-a33d-ced76faa0f00"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""A"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""23b882cc-1d8b-4a82-8f95-ba1cae2d95af"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""A"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6af3ab78-e3ec-487d-b16d-ddc0be9dd370"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""B"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f2674dfa-4693-47c0-923b-0c2f8bbedd20"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""B"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a50fd981-edc8-4d8f-beca-37ed2a81cf5d"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""X"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7142fb69-a121-4623-9586-7bb1820f606b"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""X"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""df703060-b781-4433-ba5a-55de31890683"",
                    ""path"": ""<Keyboard>/o"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Y"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7bd4ecdd-c825-4a51-aa78-870ec3dbbb58"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Y"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""efec07f2-901c-48b6-a9a6-b41598c91060"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""L"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""acb65e16-b5bc-45a5-b577-0d0c52455a5e"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""L"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""adbd6098-65d2-46bd-b877-83868eca0a22"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""R"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8483a88c-bff6-46ae-ac7e-1e05aca3dd87"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""R"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""776d9766-2caa-4086-80c6-e1340fe4c0b8"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""L3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b0af2dd0-742e-49c6-9c07-2aab8db54630"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""L3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""96b4f8a6-043a-45df-90f6-606cf5cfc0de"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""R2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3af253b0-f0e6-473e-829a-c11c4328465a"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""R2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""470d6926-15b7-4938-abfa-1a7fb219f668"",
                    ""path"": ""<Keyboard>/v"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""R3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3c74aaf7-dc59-4593-817e-198c0fd3b499"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""R3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""92ffa048-c985-48db-b6c0-2192d8a8c1f3"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""L2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6c7b15af-10d7-4473-a7d9-57d3ff0972d1"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""L2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard & Mouse"",
            ""bindingGroup"": ""Keyboard & Mouse"",
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
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // RetroPad 1
            m_RetroPad1 = asset.FindActionMap("RetroPad 1", throwIfNotFound: true);
            m_RetroPad1_Directions = m_RetroPad1.FindAction("Directions", throwIfNotFound: true);
            m_RetroPad1_Start = m_RetroPad1.FindAction("Start", throwIfNotFound: true);
            m_RetroPad1_Select = m_RetroPad1.FindAction("Select", throwIfNotFound: true);
            m_RetroPad1_A = m_RetroPad1.FindAction("A", throwIfNotFound: true);
            m_RetroPad1_B = m_RetroPad1.FindAction("B", throwIfNotFound: true);
            m_RetroPad1_X = m_RetroPad1.FindAction("X", throwIfNotFound: true);
            m_RetroPad1_Y = m_RetroPad1.FindAction("Y", throwIfNotFound: true);
            m_RetroPad1_L = m_RetroPad1.FindAction("L", throwIfNotFound: true);
            m_RetroPad1_R = m_RetroPad1.FindAction("R", throwIfNotFound: true);
            m_RetroPad1_L2 = m_RetroPad1.FindAction("L2", throwIfNotFound: true);
            m_RetroPad1_R2 = m_RetroPad1.FindAction("R2", throwIfNotFound: true);
            m_RetroPad1_L3 = m_RetroPad1.FindAction("L3", throwIfNotFound: true);
            m_RetroPad1_R3 = m_RetroPad1.FindAction("R3", throwIfNotFound: true);
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

        // RetroPad 1
        private readonly InputActionMap m_RetroPad1;
        private IRetroPad1Actions m_RetroPad1ActionsCallbackInterface;
        private readonly InputAction m_RetroPad1_Directions;
        private readonly InputAction m_RetroPad1_Start;
        private readonly InputAction m_RetroPad1_Select;
        private readonly InputAction m_RetroPad1_A;
        private readonly InputAction m_RetroPad1_B;
        private readonly InputAction m_RetroPad1_X;
        private readonly InputAction m_RetroPad1_Y;
        private readonly InputAction m_RetroPad1_L;
        private readonly InputAction m_RetroPad1_R;
        private readonly InputAction m_RetroPad1_L2;
        private readonly InputAction m_RetroPad1_R2;
        private readonly InputAction m_RetroPad1_L3;
        private readonly InputAction m_RetroPad1_R3;
        public struct RetroPad1Actions
        {
            private @LibretroControls m_Wrapper;
            public RetroPad1Actions(@LibretroControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Directions => m_Wrapper.m_RetroPad1_Directions;
            public InputAction @Start => m_Wrapper.m_RetroPad1_Start;
            public InputAction @Select => m_Wrapper.m_RetroPad1_Select;
            public InputAction @A => m_Wrapper.m_RetroPad1_A;
            public InputAction @B => m_Wrapper.m_RetroPad1_B;
            public InputAction @X => m_Wrapper.m_RetroPad1_X;
            public InputAction @Y => m_Wrapper.m_RetroPad1_Y;
            public InputAction @L => m_Wrapper.m_RetroPad1_L;
            public InputAction @R => m_Wrapper.m_RetroPad1_R;
            public InputAction @L2 => m_Wrapper.m_RetroPad1_L2;
            public InputAction @R2 => m_Wrapper.m_RetroPad1_R2;
            public InputAction @L3 => m_Wrapper.m_RetroPad1_L3;
            public InputAction @R3 => m_Wrapper.m_RetroPad1_R3;
            public InputActionMap Get() { return m_Wrapper.m_RetroPad1; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(RetroPad1Actions set) { return set.Get(); }
            public void SetCallbacks(IRetroPad1Actions instance)
            {
                if (m_Wrapper.m_RetroPad1ActionsCallbackInterface != null)
                {
                    @Directions.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnDirections;
                    @Directions.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnDirections;
                    @Directions.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnDirections;
                    @Start.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnStart;
                    @Start.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnStart;
                    @Start.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnStart;
                    @Select.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnSelect;
                    @Select.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnSelect;
                    @Select.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnSelect;
                    @A.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnA;
                    @A.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnA;
                    @A.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnA;
                    @B.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnB;
                    @B.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnB;
                    @B.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnB;
                    @X.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnX;
                    @X.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnX;
                    @X.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnX;
                    @Y.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnY;
                    @Y.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnY;
                    @Y.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnY;
                    @L.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnL;
                    @L.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnL;
                    @L.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnL;
                    @R.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnR;
                    @R.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnR;
                    @R.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnR;
                    @L2.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnL2;
                    @L2.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnL2;
                    @L2.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnL2;
                    @R2.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnR2;
                    @R2.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnR2;
                    @R2.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnR2;
                    @L3.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnL3;
                    @L3.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnL3;
                    @L3.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnL3;
                    @R3.started -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnR3;
                    @R3.performed -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnR3;
                    @R3.canceled -= m_Wrapper.m_RetroPad1ActionsCallbackInterface.OnR3;
                }
                m_Wrapper.m_RetroPad1ActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Directions.started += instance.OnDirections;
                    @Directions.performed += instance.OnDirections;
                    @Directions.canceled += instance.OnDirections;
                    @Start.started += instance.OnStart;
                    @Start.performed += instance.OnStart;
                    @Start.canceled += instance.OnStart;
                    @Select.started += instance.OnSelect;
                    @Select.performed += instance.OnSelect;
                    @Select.canceled += instance.OnSelect;
                    @A.started += instance.OnA;
                    @A.performed += instance.OnA;
                    @A.canceled += instance.OnA;
                    @B.started += instance.OnB;
                    @B.performed += instance.OnB;
                    @B.canceled += instance.OnB;
                    @X.started += instance.OnX;
                    @X.performed += instance.OnX;
                    @X.canceled += instance.OnX;
                    @Y.started += instance.OnY;
                    @Y.performed += instance.OnY;
                    @Y.canceled += instance.OnY;
                    @L.started += instance.OnL;
                    @L.performed += instance.OnL;
                    @L.canceled += instance.OnL;
                    @R.started += instance.OnR;
                    @R.performed += instance.OnR;
                    @R.canceled += instance.OnR;
                    @L2.started += instance.OnL2;
                    @L2.performed += instance.OnL2;
                    @L2.canceled += instance.OnL2;
                    @R2.started += instance.OnR2;
                    @R2.performed += instance.OnR2;
                    @R2.canceled += instance.OnR2;
                    @L3.started += instance.OnL3;
                    @L3.performed += instance.OnL3;
                    @L3.canceled += instance.OnL3;
                    @R3.started += instance.OnR3;
                    @R3.performed += instance.OnR3;
                    @R3.canceled += instance.OnR3;
                }
            }
        }
        public RetroPad1Actions @RetroPad1 => new RetroPad1Actions(this);
        private int m_KeyboardMouseSchemeIndex = -1;
        public InputControlScheme KeyboardMouseScheme
        {
            get
            {
                if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard & Mouse");
                return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
            }
        }
        private int m_GamepadSchemeIndex = -1;
        public InputControlScheme GamepadScheme
        {
            get
            {
                if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
                return asset.controlSchemes[m_GamepadSchemeIndex];
            }
        }
        public interface IRetroPad1Actions
        {
            void OnDirections(InputAction.CallbackContext context);
            void OnStart(InputAction.CallbackContext context);
            void OnSelect(InputAction.CallbackContext context);
            void OnA(InputAction.CallbackContext context);
            void OnB(InputAction.CallbackContext context);
            void OnX(InputAction.CallbackContext context);
            void OnY(InputAction.CallbackContext context);
            void OnL(InputAction.CallbackContext context);
            void OnR(InputAction.CallbackContext context);
            void OnL2(InputAction.CallbackContext context);
            void OnR2(InputAction.CallbackContext context);
            void OnL3(InputAction.CallbackContext context);
            void OnR3(InputAction.CallbackContext context);
        }
    }
}
