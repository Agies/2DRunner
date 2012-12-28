using Pathfinding;
using UnityEngine;
using System.Collections;

[CustomGraphEditor(typeof(MyGridGraph), "My Grid Graph")]
public class MyGridGraphEditor : GridGraphEditor
{

    public override void OnInspectorGUI(NavGraph target)
    {
        base.OnInspectorGUI(target);
        Separator();

        MyGridGraph graph = target as MyGridGraph;


        GUILayout.BeginHorizontal();
        graph.levelLayerMask = EditorGUILayoutx.LayerMaskField("Level Layers", graph.levelLayerMask);
        GUILayout.EndHorizontal();
    }
}
