using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue;
        GUIStyle nodeStyle;
        DialogueNode draggingNode;
        Vector2 dragginOffset;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow(){
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAssetAttribute(1)]
        public static bool OnOpenAsset(int instanceID, int line){
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if(dialogue != null){
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            MakeNodeStyle();
        }

        private void MakeNodeStyle()
        {
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChanged(){
            Debug.Log("On Selection Changed");
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if(newDialogue != null){
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI() {
            if(selectedDialogue == null){
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else{
                ProcessEvents();
                foreach(DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    OnGUINode(node);
                    DrawConnections(node);
                }
            }
        }


        private void ProcessEvents(){
            if(Event.current.type == EventType.MouseDown && draggingNode == null){
                draggingNode = GetNodeAtPoint(Event.current.mousePosition);
                if(draggingNode != null){
                    dragginOffset = draggingNode.rect.position - Event.current.mousePosition;
                }
            }
            else if(Event.current.type == EventType.MouseDrag && draggingNode != null){
                Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
                draggingNode.rect.position = Event.current.mousePosition + dragginOffset;
                GUI.changed = true;
            }
            else if(Event.current.type == EventType.MouseUp && draggingNode != null){
                draggingNode = null;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundValue = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if(node.rect.Contains(point)){
                    foundValue = node;
                }
            }
            return foundValue;
        }

        private void OnGUINode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, nodeStyle);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Node", EditorStyles.whiteLabel);
            string newUniqueId = EditorGUILayout.TextField(node.uniqueID);
            string newText = EditorGUILayout.TextField(node.text);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");
                node.uniqueID = newUniqueId;
                node.text = newText;
            }

            // foreach(DialogueNode childNode in selectedDialogue.GetAllChildren(node)){
            //     EditorGUILayout.LabelField(childNode.text);                
            // }

            GUILayout.EndArea();
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.rect.xMax, node.rect.center.y);
            foreach(DialogueNode childNode in selectedDialogue.GetAllChildren(node)){
                Vector3 endPosition = new Vector2(childNode.rect.xMin, childNode.rect.center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;
                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset, endPosition - controlPointOffset, Color.white, null, 4f);
            }
        }



    }

}

