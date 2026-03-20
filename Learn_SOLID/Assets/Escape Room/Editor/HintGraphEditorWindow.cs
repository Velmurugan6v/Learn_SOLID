#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ─────────────────────────────────────────────────────────────
//  HintGraphEditorWindow.cs  —  with runtime visual feedback
//
//  Three live states while game is playing:
//
//  COMPLETED node  → green header tint + faint green fill
//  ACTIVE node     → pulsing amber/gold outline (glow ring)
//  ACTIVE wire     → animated marching-ants dashed bezier
//
//  How it works:
//  - HintGraphRunner.NotifyEditor() is called every time a node
//    changes state (active or completed).
//  - The window holds the active node ID and reads isCompleted
//    directly from the node ScriptableObjects (they are the same
//    in-memory objects the runner modifies at runtime).
//  - EditorApplication.update drives Repaint() so the glow pulse
//    animates smoothly without blocking the main thread.
// ─────────────────────────────────────────────────────────────

public class HintGraphEditorWindow : EditorWindow
{
    // ── Layout ──
    private const float NODE_WIDTH    = 200f;
    private const float NODE_HEADER_H = 28f;
    private const float PORT_H        = 22f;
    private const float PORT_RADIUS   = 7f;
    private const float TOOLBAR_H     = 22f;
    private const float GLOW_RADIUS   = 6f;   // extra px around active node

    // ── Base colors ──
    private static readonly Color C_BG        = new(0.13f, 0.13f, 0.16f);
    private static readonly Color C_GRID_MIN  = new(1, 1, 1, 0.04f);
    private static readonly Color C_GRID_MAJ  = new(1, 1, 1, 0.09f);
    private static readonly Color C_NODE_BG   = new(0.18f, 0.18f, 0.22f);
    private static readonly Color C_NODE_SEL  = new(0.28f, 0.55f, 0.95f, 0.4f);
    private static readonly Color C_WIRE      = new(0.55f, 0.85f, 0.55f);
    private static readonly Color C_WIRE_DRAG = new(0.9f, 0.75f, 0.3f);
    private static readonly Color C_PORT_IN   = new(0.3f, 0.8f, 0.5f);
    private static readonly Color C_PORT_OUT  = new(0.9f, 0.6f, 0.2f);

    // ── Runtime state colors ──
    private static readonly Color C_COMPLETED_HEADER = new(0.15f, 0.55f, 0.25f);   // green header
    private static readonly Color C_COMPLETED_FILL   = new(0.12f, 0.28f, 0.16f);   // dark green body
    private static readonly Color C_COMPLETED_WIRE   = new(0.2f,  0.75f, 0.35f);   // bright green wire
    private static readonly Color C_ACTIVE_WIRE      = new(1.0f,  0.78f, 0.2f);    // amber wire (animated)

    // ── Node header colors by type ──
    private static readonly Dictionary<Type, Color> NODE_COLORS = new()
    {
        { typeof(StartNode),          new Color(0.2f,  0.6f,  0.3f)  },
        { typeof(EndNode),            new Color(0.6f,  0.2f,  0.2f)  },
        { typeof(BgHintNode),         new Color(0.2f,  0.4f,  0.7f)  },
        { typeof(CloseShotHintNode),  new Color(0.3f,  0.3f,  0.7f)  },
        { typeof(InventoryHintNode),  new Color(0.6f,  0.45f, 0.1f)  },
        { typeof(InnerPanelHintNode), new Color(0.5f,  0.25f, 0.6f)  },
        { typeof(ConditionNode),      new Color(0.55f, 0.45f, 0.08f) },
    };

    // ── Editor state ──
    private HintGraph    _graph;
    private Vector2      _scrollOffset;
    private float        _zoom = 1f;
    private HintNodeBase _selectedNode;
    private HintNodeBase _draggingNode;
    private Vector2      _dragOffset;
    private HintNodeBase _connectFromNode;
    private string       _connectFromPort;
    private Vector2      _connectMousePos;
    private Vector2      _pendingCreatePos;

    // ── Runtime feedback state ──
    // Written by HintGraphRunner via HintGraphBridge (Runtime class, no Editor dep).
    // Read here via HintGraphBridge.ActiveNodeId.
    private float _pulseTime  = 0f;   // drives glow animation
    private float _wireOffset = 0f;   // drives marching ants

    // ────────────────────────────────────────
    #region Open / Lifecycle
    // ────────────────────────────────────────

    [MenuItem("Window/Hint Graph Editor")]
    public static void OpenWindow()
    {
        var w = GetWindow<HintGraphEditorWindow>("Hint Graph");
        w.minSize = new Vector2(800, 500);
    }

    public static void OpenGraph(HintGraph g)
    {
        var w = GetWindow<HintGraphEditorWindow>("Hint Graph");
        w.LoadGraph(g);
        w.minSize = new Vector2(800, 500);
    }

    private void OnEnable()
    {
        // Drive repaint from update so glow animates even with no mouse input
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        // Only animate during play mode — no wasted repaints in edit mode
        if (!EditorApplication.isPlaying) return;
        if (_graph == null) return;

        _pulseTime  += 0.016f;                     // ~60 fps tick
        _wireOffset -= 0.8f;                        // marching ants speed
        Repaint();
    }

    #endregion

    // ────────────────────────────────────────
    #region OnGUI
    // ────────────────────────────────────────

    private void OnGUI()
    {
        DrawToolbar();
        if (_graph == null) { DrawEmpty(); return; }

        Rect canvas = new(0, TOOLBAR_H, position.width, position.height - TOOLBAR_H);
        GUI.BeginClip(canvas);

        DrawGrid(canvas);
        DrawConnections();
        DrawDragWire();
        DrawNodes();

        GUI.EndClip();

        HandleEvents(canvas);

        if (GUI.changed) Repaint();
    }

    #endregion

    // ────────────────────────────────────────
    #region Toolbar
    // ────────────────────────────────────────

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(TOOLBAR_H));

        if (GUILayout.Button("Load Graph", EditorStyles.toolbarButton, GUILayout.Width(90)))
            PickGraph();

        if (_graph != null)
        {
            GUILayout.Label(_graph.name, EditorStyles.boldLabel, GUILayout.Width(160));
            GUILayout.FlexibleSpace();

            // Runtime status badge
            if (EditorApplication.isPlaying)
            {
                string badge = HintGraphBridge.ActiveNodeId != null ? "● LIVE" : "● Playing";
                Color  prev  = GUI.contentColor;
                GUI.contentColor = HintGraphBridge.ActiveNodeId != null ? new Color(1f, 0.78f, 0.2f) : new Color(0.4f, 1f, 0.5f);
                GUILayout.Label(badge, EditorStyles.boldLabel, GUILayout.Width(80));
                GUI.contentColor = prev;
            }

            if (GUILayout.Button("Center", EditorStyles.toolbarButton, GUILayout.Width(60)))  CenterView();
            if (GUILayout.Button("+ Start", EditorStyles.toolbarButton, GUILayout.Width(60))) CreateNode<StartNode>(new Vector2(80, 80));
            if (GUILayout.Button("+ End",   EditorStyles.toolbarButton, GUILayout.Width(55))) CreateNode<EndNode>(new Vector2(500, 80));
        }

        GUILayout.EndHorizontal();
    }

    #endregion

    // ────────────────────────────────────────
    #region Grid
    // ────────────────────────────────────────

    private void DrawGrid(Rect canvas)
    {
        EditorGUI.DrawRect(new Rect(0, 0, canvas.width, canvas.height), C_BG);
        DrawGridLines(canvas, 20f * _zoom, C_GRID_MIN);
        DrawGridLines(canvas, 100f * _zoom, C_GRID_MAJ);
    }

    private void DrawGridLines(Rect canvas, float sp, Color col)
    {
        Handles.color = col;
        for (float x = _scrollOffset.x % sp; x < canvas.width;  x += sp)
            Handles.DrawLine(new Vector3(x, 0), new Vector3(x, canvas.height));
        for (float y = _scrollOffset.y % sp; y < canvas.height; y += sp)
            Handles.DrawLine(new Vector3(0, y), new Vector3(canvas.width, y));
        Handles.color = Color.white;
    }

    #endregion

    // ────────────────────────────────────────
    #region Nodes
    // ────────────────────────────────────────

    private void DrawNodes()
    {
        foreach (var node in _graph.nodes)
            DrawNode(node);
    }

    private void DrawNode(HintNodeBase node)
    {
        Vector2 pos  = ToCanvas(node.editorPosition);
        float   h    = NodeH(node);
        Rect    rect = new(pos.x, pos.y, NODE_WIDTH * _zoom, h);

        bool isActive    = EditorApplication.isPlaying && node.nodeId == HintGraphBridge.ActiveNodeId;
        bool isCompleted = EditorApplication.isPlaying && node.isCompleted;

        // ── 1. Glow ring for active node ──────────────────
        // EditorGUI.DrawRect is reliable inside GUI.BeginClip.
        // 6 expanding filled rects (large→small, alpha fading out) give
        // a soft bloom, then 4 border strips give a sharp pulsing ring.
        if (isActive)
        {
            float pulse  = (Mathf.Sin(_pulseTime * 3.5f) + 1f) * 0.5f;
            int   layers = 6;
            for (int i = layers; i >= 1; i--)
            {
                float expand = i * 4f * Mathf.Lerp(0.6f, 1.4f, pulse);
                float alpha  = Mathf.Lerp(0.04f, 0.22f, pulse) * (1f - (float)i / (layers + 1));
                Color gc     = new Color(1f, 0.78f, 0.2f, alpha);
                EditorGUI.DrawRect(
                    new Rect(rect.x - expand, rect.y - expand,
                             rect.width + expand * 2f, rect.height + expand * 2f), gc);
            }
            // Sharp border ring — 4 strips around the node
            float ring    = Mathf.Lerp(2f, 3.5f, pulse);
            Color ringCol = new Color(1f, 0.78f, 0.2f, Mathf.Lerp(0.6f, 1f, pulse));
            EditorGUI.DrawRect(new Rect(rect.x - ring, rect.y - ring, rect.width + ring * 2f, ring), ringCol); // top
            EditorGUI.DrawRect(new Rect(rect.x - ring, rect.yMax,     rect.width + ring * 2f, ring), ringCol); // bottom
            EditorGUI.DrawRect(new Rect(rect.x - ring, rect.y,        ring, rect.height),               ringCol); // left
            EditorGUI.DrawRect(new Rect(rect.xMax,     rect.y,        ring, rect.height),               ringCol); // right
        }

        // ── 2. Selection ring ────────────────────────────
        if (node == _selectedNode)
            EditorGUI.DrawRect(new Rect(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4), C_NODE_SEL);

        // ── 3. Body fill ──────────────────────────────────
        Color bodyColor = isCompleted
            ? C_COMPLETED_FILL
            : C_NODE_BG;
        EditorGUI.DrawRect(rect, bodyColor);

        // ── 4. Header fill ────────────────────────────────
        Color baseHeader = NODE_COLORS.TryGetValue(node.GetType(), out var cc) ? cc : new Color(0.3f, 0.3f, 0.4f);
        Color headerColor;

        if (isActive)
        {
            float t = (Mathf.Sin(_pulseTime * 3.5f) + 1f) * 0.5f;
            headerColor = Color.Lerp(baseHeader, new Color(0.9f, 0.65f, 0.1f), t * 0.6f);
        }
        else if (isCompleted)
        {
            headerColor = C_COMPLETED_HEADER;
        }
        else
        {
            headerColor = baseHeader;
        }

        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, NODE_HEADER_H * _zoom), headerColor);

        // ── 5. Title text ─────────────────────────────────
        string titleText = node.GetNodeTitle();
        if (EditorApplication.isPlaying)
        {
            if (isActive)    titleText = "▶ " + titleText;
            else if (isCompleted) titleText = "✓ " + titleText;
        }

        var titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize  = Mathf.RoundToInt(11 * _zoom),
            alignment = TextAnchor.MiddleCenter,
            normal    = { textColor = Color.white }
        };
        GUI.Label(new Rect(rect.x, rect.y, rect.width, NODE_HEADER_H * _zoom), titleText, titleStyle);

        // ── 6. Completed tick overlay on body ─────────────
        if (isCompleted && !isActive)
        {
            var doneStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize  = Mathf.RoundToInt(10 * _zoom),
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = new Color(0.4f, 1f, 0.5f, 0.6f) }
            };
            float bodyH = h - NODE_HEADER_H * _zoom;
            GUI.Label(new Rect(rect.x, rect.y + NODE_HEADER_H * _zoom, rect.width, bodyH), "completed", doneStyle);
        }

        // ── 7. Ports ──────────────────────────────────────
        if (!(node is StartNode))
            DrawPort(new Vector2(rect.x, rect.y + (NODE_HEADER_H * _zoom) / 2f), C_PORT_IN, false, node, "In");

        float py   = rect.y + NODE_HEADER_H * _zoom + 4 * _zoom;
        var   ports = OutPorts(node);
        for (int i = 0; i < ports.Count; i++)
        {
            var labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize  = Mathf.RoundToInt(10 * _zoom),
                alignment = TextAnchor.MiddleRight,
                normal    = { textColor = new Color(0.8f, 0.8f, 0.8f) }
            };
            GUI.Label(new Rect(rect.x + 4 * _zoom, py, rect.width - 14 * _zoom, PORT_H * _zoom), ports[i], labelStyle);
            DrawPort(new Vector2(rect.x + rect.width, py + (PORT_H * _zoom) / 2f), C_PORT_OUT, true, node, ports[i]);
            py += PORT_H * _zoom;
        }

        // ── 8. Border ─────────────────────────────────────
        Color borderColor = isCompleted
            ? new Color(0.3f, 0.8f, 0.4f, 0.5f)
            : new Color(1f, 1f, 1f, 0.15f);
        // Border: 4 thin strips (EditorGUI.DrawRect, always reliable)
        float bw = 1f;
        EditorGUI.DrawRect(new Rect(rect.x,         rect.y,         rect.width, bw),        borderColor); // top
        EditorGUI.DrawRect(new Rect(rect.x,         rect.yMax - bw, rect.width, bw),        borderColor); // bottom
        EditorGUI.DrawRect(new Rect(rect.x,         rect.y,         bw, rect.height),       borderColor); // left
        EditorGUI.DrawRect(new Rect(rect.xMax - bw, rect.y,         bw, rect.height),       borderColor); // right
    }

    private void DrawPort(Vector2 center, Color col, bool isOut, HintNodeBase node, string label)
    {
        float r  = PORT_RADIUS * _zoom;
        Rect  pr = new(center.x - r, center.y - r, r * 2, r * 2);

        Handles.color = col;
        Handles.DrawSolidDisc(center, Vector3.forward, r);
        Handles.color = Color.white;

        if (Event.current.type == EventType.MouseDown && pr.Contains(Event.current.mousePosition))
        {
            if (isOut)
            {
                _connectFromNode = node;
                _connectFromPort = label;
                _connectMousePos = Event.current.mousePosition;
                Event.current.Use();
            }
            else if (_connectFromNode != null && _connectFromNode != node)
            {
                Undo.RecordObject(_graph, "Connect Nodes");
                _graph.Connect(_connectFromNode, _connectFromPort, node);
                _connectFromNode = null;
                Event.current.Use();
            }
        }
    }

    private float NodeH(HintNodeBase n) =>
        (NODE_HEADER_H + OutPorts(n).Count * PORT_H + 8) * _zoom;

    private List<string> OutPorts(HintNodeBase n)
    {
        if (n is ConditionNode) return new List<string> { "True", "False" };
        if (n is EndNode)       return new List<string>();
        return new List<string> { "Next" };
    }

    #endregion

    // ────────────────────────────────────────
    #region Connections
    // ────────────────────────────────────────

    private void DrawConnections()
    {
        foreach (var node in _graph.nodes)
        {
            var ports = OutPorts(node);
            for (int i = 0; i < ports.Count; i++)
            {
                var conn = node.outputs.Find(c => c.portLabel == ports[i]);
                if (conn == null) continue;

                var target = _graph.GetNode(conn.targetNodeId);
                if (target == null) continue;

                bool fromActive    = EditorApplication.isPlaying && node.nodeId == HintGraphBridge.ActiveNodeId;
                bool fromCompleted = EditorApplication.isPlaying && node.isCompleted;
                bool toActive      = EditorApplication.isPlaying && target.nodeId == HintGraphBridge.ActiveNodeId;

                Vector2 from = OutPortPos(node, i);
                Vector2 to   = InPortPos(target);

                if (fromActive || toActive)
                {
                    // Marching-ants animated wire for the active connection
                    DrawAnimatedBezier(from, to, C_ACTIVE_WIRE);
                }
                else if (fromCompleted)
                {
                    // Completed path — bright green, slightly thicker
                    DrawBezier(from, to, C_COMPLETED_WIRE, 3f);
                }
                else
                {
                    // Default wire
                    DrawBezier(from, to, C_WIRE, 2f);
                }
            }
        }
    }

    private void DrawDragWire()
    {
        if (_connectFromNode == null) return;
        int idx = OutPorts(_connectFromNode).IndexOf(_connectFromPort);
        if (idx < 0) return;
        DrawBezier(OutPortPos(_connectFromNode, idx), _connectMousePos, C_WIRE_DRAG, 2f);
        Repaint();
    }

    private void DrawBezier(Vector2 from, Vector2 to, Color col, float width = 2f)
    {
        float dx = Mathf.Abs(to.x - from.x) * 0.5f + 30f;
        Handles.DrawBezier(from, to,
            new Vector3(from.x + dx, from.y),
            new Vector3(to.x  - dx, to.y),
            col, null, width);
    }

    // Animated marching-ants bezier — draws two overlapping beziers:
    // a dim base + a bright dashed animated layer using Handles.DrawDottedLine approximation
    private void DrawAnimatedBezier(Vector2 from, Vector2 to, Color col)
    {
        float dx = Mathf.Abs(to.x - from.x) * 0.5f + 30f;
        Vector3 p0 = from;
        Vector3 p1 = new(from.x + dx, from.y);
        Vector3 p2 = new(to.x   - dx, to.y);
        Vector3 p3 = to;

        // Base wire (dim)
        Handles.DrawBezier(p0, p3, p1, p2, new Color(col.r, col.g, col.b, 0.3f), null, 2f);

        // Animated dashes — sample N points along bezier, draw short segments with offset
        int   segments  = 40;
        float dashLen   = 8f;
        float gapLen    = 6f;
        float period    = dashLen + gapLen;
        float offset    = _wireOffset % period;

        for (int i = 0; i < segments; i++)
        {
            float t0 = (float)i       / segments;
            float t1 = (float)(i + 1) / segments;
            Vector3 pt0 = CubicBezier(p0, p1, p2, p3, t0);
            Vector3 pt1 = CubicBezier(p0, p1, p2, p3, t1);

            // Approximate arc distance along this segment
            float segLen = Vector3.Distance(pt0, pt1) * segments;
            // Cheap dash logic: check if this segment falls in a "dash" phase
            float pos = (i / (float)segments * 300f + offset) % period;
            if (pos < dashLen)
            {
                Handles.color = col;
                Handles.DrawLine(pt0, pt1);
            }
        }

        // Arrowhead at destination
        Vector3 nearEnd  = CubicBezier(p0, p1, p2, p3, 0.92f);
        Vector3 tipDir   = ((Vector3)to - nearEnd).normalized;
        float   arrowSz  = 8f * _zoom;
        Vector3 left     = Quaternion.Euler(0, 0,  140) * tipDir * arrowSz;
        Vector3 right    = Quaternion.Euler(0, 0, -140) * tipDir * arrowSz;
        Handles.color    = col;
        Handles.DrawLine(p3, p3 + left);
        Handles.DrawLine(p3, p3 + right);
        Handles.color    = Color.white;
    }

    private static Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u  = 1f - t;
        float tt = t * t;
        float uu = u * u;
        return uu * u * p0 + 3 * uu * t * p1 + 3 * u * tt * p2 + tt * t * p3;
    }

    private Vector2 OutPortPos(HintNodeBase n, int i)
    {
        var p = ToCanvas(n.editorPosition);
        return new Vector2(p.x + NODE_WIDTH * _zoom, p.y + NODE_HEADER_H * _zoom + (i + 0.5f) * PORT_H * _zoom);
    }

    private Vector2 InPortPos(HintNodeBase n)
    {
        var p = ToCanvas(n.editorPosition);
        return new Vector2(p.x, p.y + (NODE_HEADER_H * _zoom) / 2f);
    }

    #endregion

    // ────────────────────────────────────────
    #region Events
    // ────────────────────────────────────────

    private void HandleEvents(Rect canvas)
    {
        Event   e  = Event.current;
        Vector2 mp = e.mousePosition - new Vector2(0, TOOLBAR_H);

        switch (e.type)
        {
            case EventType.MouseDown:
                if (!canvas.Contains(e.mousePosition)) break;
                if (e.button == 0)
                {
                    var clicked = NodeAt(mp);
                    if (clicked != null)
                    {
                        _selectedNode = clicked;
                        _draggingNode = clicked;
                        _dragOffset   = mp - ToCanvas(clicked.editorPosition);
                        Selection.activeObject = clicked;
                        e.Use();
                    }
                    else { _selectedNode = null; _connectFromNode = null; }
                }
                else if (e.button == 1)
                {
                    _pendingCreatePos = FromCanvas(mp);
                    ShowContextMenu(NodeAt(mp));
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && _draggingNode != null)
                {
                    Undo.RecordObject(_draggingNode, "Move Node");
                    _draggingNode.editorPosition = FromCanvas(mp - _dragOffset);
                    EditorUtility.SetDirty(_draggingNode);
                    e.Use();
                }
                else if (e.button == 2) { _scrollOffset += e.delta; e.Use(); }
                if (_connectFromNode != null) { _connectMousePos = mp; Repaint(); }
                break;

            case EventType.MouseUp:
                _draggingNode = null;
                break;

            case EventType.ScrollWheel:
                if (canvas.Contains(e.mousePosition))
                {
                    _zoom = Mathf.Clamp(_zoom - e.delta.y * 0.02f, 0.3f, 2f);
                    e.Use();
                }
                break;

            case EventType.KeyDown:
                if (e.keyCode == KeyCode.Delete && _selectedNode != null)
                {
                    DeleteNode(_selectedNode);
                    e.Use();
                }
                break;
        }
    }

    private void ShowContextMenu(HintNodeBase clicked)
    {
        var m = new GenericMenu();
        if (clicked != null)
        {
            m.AddItem(new GUIContent("Delete Node"), false, () => DeleteNode(clicked));
            m.AddItem(new GUIContent("Set as Start"), false, () => {
                Undo.RecordObject(_graph, "Set Start");
                _graph.startNodeId = clicked.nodeId;
                EditorUtility.SetDirty(_graph);
            });
            m.AddSeparator("");
        }
        m.AddItem(new GUIContent("Add/Start Node"),       false, () => CreateNode<StartNode>(_pendingCreatePos));
        m.AddItem(new GUIContent("Add/End Node"),         false, () => CreateNode<EndNode>(_pendingCreatePos));
        m.AddItem(new GUIContent("Add/Bg Hint"),          false, () => CreateNode<BgHintNode>(_pendingCreatePos));
        m.AddItem(new GUIContent("Add/CloseShot Hint"),   false, () => CreateNode<CloseShotHintNode>(_pendingCreatePos));
        m.AddItem(new GUIContent("Add/Inventory Hint"),   false, () => CreateNode<InventoryHintNode>(_pendingCreatePos));
        m.AddItem(new GUIContent("Add/InnerPanel Hint"),  false, () => CreateNode<InnerPanelHintNode>(_pendingCreatePos));
        m.AddItem(new GUIContent("Add/Condition Branch"), false, () => CreateNode<ConditionNode>(_pendingCreatePos));
        m.ShowAsContext();
    }

    #endregion

    // ────────────────────────────────────────
    #region Helpers
    // ────────────────────────────────────────

    private void LoadGraph(HintGraph g)
    {
        _graph        = g;
        _scrollOffset = g.editorScrollOffset;
        _zoom         = g.editorZoom > 0 ? g.editorZoom : 1f;
        Repaint();
    }

    private void PickGraph()
    {
        string path = EditorUtility.OpenFilePanel("Select HintGraph", "Assets", "asset");
        if (string.IsNullOrEmpty(path)) return;
        path = "Assets" + path.Substring(Application.dataPath.Length);
        var g = AssetDatabase.LoadAssetAtPath<HintGraph>(path);
        if (g != null) LoadGraph(g);
    }

    private T CreateNode<T>(Vector2 pos) where T : HintNodeBase
    {
        Undo.RecordObject(_graph, "Create Node");
        var node = _graph.AddNode<T>(pos);
        if (node is StartNode) _graph.startNodeId = node.nodeId;
        _selectedNode = node;
        return node;
    }

    private void DeleteNode(HintNodeBase n)
    {
        Undo.RecordObject(_graph, "Delete Node");
        if (_selectedNode == n) _selectedNode = null;
        _graph.RemoveNode(n);
    }

    private void CenterView()
    {
        if (_graph.nodes.Count == 0) return;
        Vector2 c = Vector2.zero;
        foreach (var n in _graph.nodes) c += n.editorPosition;
        c /= _graph.nodes.Count;
        _scrollOffset = new Vector2(position.width / 2f - c.x * _zoom,
                                    position.height / 2f - c.y * _zoom);
    }

    private HintNodeBase NodeAt(Vector2 p)
    {
        for (int i = _graph.nodes.Count - 1; i >= 0; i--)
        {
            var n = _graph.nodes[i];
            if (new Rect(ToCanvas(n.editorPosition), new Vector2(NODE_WIDTH * _zoom, NodeH(n))).Contains(p))
                return n;
        }
        return null;
    }

    private Vector2 ToCanvas(Vector2 p)   => p * _zoom + _scrollOffset;
    private Vector2 FromCanvas(Vector2 p) => (p - _scrollOffset) / _zoom;

    private void DrawEmpty()
    {
        var s = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = 13 };
        GUI.Label(new Rect(0, 0, position.width, position.height),
                  "No Hint Graph loaded.\nUse toolbar → Load Graph, or double-click a HintGraph asset.", s);
    }

    #endregion
}

// ─────────────────────────────────────────────────────────────
//  Inspector button to open the graph editor
// ─────────────────────────────────────────────────────────────
[CustomEditor(typeof(HintGraph))]
public class HintGraphInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open in Hint Graph Editor", GUILayout.Height(32)))
            HintGraphEditorWindow.OpenGraph((HintGraph)target);
        GUILayout.Space(8);
        DrawDefaultInspector();
    }
}
#endif