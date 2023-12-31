﻿using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Invector.vShooter
{
    using vCharacterController;
    using vMelee;
    using vItemManager;
    using vCharacterController.vActions;
    public class vCreateShooterCharacterEditor : EditorWindow
    {
        GUISkin skin;
        GameObject charObj;
        Animator charAnimator;
        public RuntimeAnimatorController controller;
        public vThirdPersonCameraListData cameraListData;
        public GameObject hud;
        public GameObject aimCanvas;
        Vector2 rect = new Vector2(500, 630);
        Vector2 scrool;
        Editor humanoidpreview;
        Texture2D m_Logo;

        /// <summary>
        /// 3rdPersonController Menu 
        /// </summary>    
        [MenuItem("Invector/Shooter/Create Shooter Controller", false, 2)]
        public static void CreateNewCharacter()
        {
            GetWindow<vCreateShooterCharacterEditor>();
        }

        bool isHuman, isValidAvatar, charExist;
        void OnEnable()
        {        
            m_Logo = Resources.Load("icon_v2") as Texture2D;
            charObj = Selection.activeGameObject;

            if (charObj)
            {                
                charAnimator = charObj.GetComponent<Animator>();
                humanoidpreview = Editor.CreateEditor(charObj);
            }

            charExist = charAnimator != null;
            isHuman = charExist ? charAnimator.isHuman : false;
            isValidAvatar = charExist ? charAnimator.avatar.isValid : false;
        }

        void OnGUI()
        {
            if (!skin) skin = Resources.Load("vSkin") as GUISkin;
            GUI.skin = skin;

            this.minSize = rect;
            this.titleContent = new GUIContent("Character", null, "Third Person Character Creator");

            GUILayout.BeginVertical("Character Creator Window", "window");
            GUILayout.Label(m_Logo, GUILayout.MaxHeight(25));
            GUILayout.Space(5);

            GUILayout.BeginVertical("box");

            if (!charObj)
                EditorGUILayout.HelpBox("Make sure your FBX model is set as Humanoid!", MessageType.Info);
            else if (!charExist)
                EditorGUILayout.HelpBox("Missing a Animator Component", MessageType.Error);
            else if (!isHuman)
                EditorGUILayout.HelpBox("This is not a Humanoid", MessageType.Error);
            else if (!isValidAvatar)
                EditorGUILayout.HelpBox(charObj.name + " is a invalid Humanoid", MessageType.Info);

            charObj = EditorGUILayout.ObjectField("FBX Model", charObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;

            if (GUI.changed && charObj != null && charObj.GetComponent<vThirdPersonController>() == null)
                humanoidpreview = Editor.CreateEditor(charObj);
            if (charObj != null && charObj.GetComponent<vThirdPersonController>() != null)
            {
                EditorGUILayout.HelpBox("This gameObject already contains the component vThirdPersonController", MessageType.Warning);
            }
            controller = EditorGUILayout.ObjectField("Animator Controller: ", controller, typeof(RuntimeAnimatorController), false) as RuntimeAnimatorController;
            cameraListData = EditorGUILayout.ObjectField("Camera List Data: ", cameraListData, typeof(vThirdPersonCameraListData), false) as vThirdPersonCameraListData;
            hud = EditorGUILayout.ObjectField("Hud Controller: ", hud, typeof(GameObject), false) as GameObject;
            if (hud != null && hud.GetComponent<vHUDController>() == null)
            {
                EditorGUILayout.HelpBox("This object does not contain a vHUDController", MessageType.Warning);
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("Need to know how it works?");
            if (GUILayout.Button("Video Tutorial"))
            {
                Application.OpenURL("https://www.youtube.com/watch?v=KQ5xha36tfE");
            }
            GUILayout.EndHorizontal();

            if (charObj)
                charAnimator = charObj.GetComponent<Animator>();
            charExist = charAnimator != null;
            isHuman = charExist ? charAnimator.isHuman : false;
            isValidAvatar = charExist ? charAnimator.avatar.isValid : false;

            if (CanCreate())
            {
                DrawHumanoidPreview();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (controller != null)
                {
                    if (GUILayout.Button("Create"))
                        Create();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        bool CanCreate()
        {
            return isValidAvatar && isHuman && charObj != null && charObj.GetComponent<vThirdPersonController>() == null;
        }

        /// <summary>
        /// Draw the Preview window
        /// </summary>
        void DrawHumanoidPreview()
        {
            GUILayout.FlexibleSpace();

            if (humanoidpreview != null)
            {
                humanoidpreview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(100, 400), "window");
            }
        }

        private GameObject InstantiateNewCharacter(GameObject selected)
        {
            if (selected == null) return selected;
            if (selected.scene.IsValid()) return selected;

            return PrefabUtility.InstantiatePrefab(selected) as GameObject;

        }
        /// <summary>
        /// Created the Third Person Controller
        /// </summary>
        public virtual void Create()
        {
            // base for the character
            GameObject newCharacter = InstantiateNewCharacter(charObj);

            if (!newCharacter)
                return;

            newCharacter.name = "vShooterController_" + charObj.gameObject.name;
            newCharacter.AddComponent<vThirdPersonController>();
            newCharacter.AddComponent<vShooterMeleeInput>();
            newCharacter.AddComponent<vShooterManager>();
            newCharacter.AddComponent<vAmmoManager>();
            newCharacter.AddComponent<vHeadTrack>();
            newCharacter.AddComponent<vGenericAction>();

            var rigidbody = newCharacter.AddComponent<Rigidbody>();
            var collider = newCharacter.AddComponent<CapsuleCollider>();

            // camera
            GameObject camera = null;
            if (Camera.main == null)
            {
                var cam = new GameObject("vThirdPersonCamera");
                cam.AddComponent<Camera>();              
                cam.AddComponent<AudioListener>();
                camera = cam;
                camera.GetComponent<Camera>().tag = "MainCamera";
                camera.GetComponent<Camera>().nearClipPlane = 0.03f;
            }
            else
            {
                camera = Camera.main.gameObject;
                camera.GetComponent<Camera>().tag = "MainCamera";
                camera.GetComponent<Camera>().nearClipPlane = 0.03f;
                camera.gameObject.name = "vThirdPersonCamera";
            }
            var tpcamera = camera.GetComponent<vCamera.vThirdPersonCamera>();

            if (tpcamera == null)
                tpcamera = camera.AddComponent<vCamera.vThirdPersonCamera>();

            // define the camera cursorObject       
            tpcamera.target = newCharacter.transform;
            if (cameraListData != null)
            {
                tpcamera.CameraStateList = cameraListData;
            }

            GameObject gC = null;
            var gameController = FindObjectOfType<vGameController>();
            if (gameController == null)
            {
                gC = new GameObject("vGameController");
                gC.AddComponent<vGameController>();
            }

            CreateHud();
            if (aimCanvas != null && FindObjectOfType<vAimCanvas>()==null)
                PrefabUtility.InstantiatePrefab(aimCanvas);

            newCharacter.tag = "Player";

            var p_layer = LayerMask.NameToLayer("Player");
            newCharacter.layer = p_layer;

            foreach (Transform t in newCharacter.transform.GetComponentsInChildren<Transform>())
                t.gameObject.layer = p_layer;

            var s_layer = LayerMask.NameToLayer("StopMove");
            newCharacter.GetComponent<vThirdPersonMotor>().stopMoveLayer = LayerMask.GetMask(LayerMask.LayerToName(s_layer));

            // rigidbody
            rigidbody.useGravity = true;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidbody.mass = 50;

            // capsule collider 
            collider.height = ColliderHeight(newCharacter.GetComponent<Animator>());
            collider.center = new Vector3(0, (float)System.Math.Round(collider.height * 0.5f, 2), 0);
            collider.radius = (float)System.Math.Round(collider.height * 0.15f, 2);

            if (controller)
                newCharacter.GetComponent<Animator>().runtimeAnimatorController = controller;
            Selection.activeGameObject = newCharacter;
            UnityEditor.SceneView.lastActiveSceneView.FrameSelected();
            this.Close();

        }

        /// <summary>
        /// Capsule Collider height based on the Character height
        /// </summary>
        /// <param name="animator">animator humanoid</param>
        /// <returns></returns>
        float ColliderHeight(Animator animator)
        {
            var foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            return (float)System.Math.Round(Vector3.Distance(foot.position, hips.position) * 2f, 2);
        }

        /// <summary>
        /// Return Hud Object
        /// </summary>
        /// <returns></returns>
        vHUDController CreateHud()
        {
            var _hud = FindObjectOfType<vHUDController>();
            if (_hud) return _hud;
            if (hud == null) return null;
            var canvas = FindObjectOfType<Canvas>();
            if (FindObjectOfType<EventSystem>() == null)
            {
#if UNITY_5_3_OR_NEWER
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
#else
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule), typeof(TouchInputModule));
#endif
            }
            if (canvas == null || (canvas != null && !canvas.tag.Equals("PlayerUI")))
            {
                var canvasObj = new GameObject("vUI");
                canvasObj.tag = "PlayerUI";
                canvas = canvasObj.AddComponent<Canvas>();
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            if (canvas.GetComponent<UnityEngine.UI.CanvasScaler>() != null)
            {
                canvas.GetComponent<UnityEngine.UI.CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvas.GetComponent<UnityEngine.UI.CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            }
            ////Check HUD
            //RectTransform Hud = canvas.transform.FindChild("HUD") as RectTransform;
            GameObject hudObject = null;

            if (_hud == null)
            {
                hudObject =(GameObject) PrefabUtility.InstantiatePrefab(hud);
                if (hudObject)
                    hudObject.GetComponent<RectTransform>().SetParent(canvas.transform);
                var rect = hudObject.GetComponent<RectTransform>();
                rect.offsetMax = new Vector2(0, 0);

                hudObject.name = "HUD";
            }
            if (hudObject.GetComponent<vHUDController>() == null)
                hudObject.gameObject.AddComponent<vHUDController>();
            //HUD Components       

            return hudObject.GetComponent<vHUDController>();
        }

    }
}