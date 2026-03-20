// ═══════════════════════════════════════════════════════════════════
//  HintGraphEditor.cs
//  Place in any Editor/ folder.
//  Open via: Window → Hint System → Hint Graph Editor
// ═══════════════════════════════════════════════════════════════════

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HintEditorOne
{



    public class HintGraphEditor : EditorWindow
    {
        // ── Refs ──────────────────────────────────────────────────────
        private HintGraphData _graph;

        // ── Canvas state ──────────────────────────────────────────────
        private Vector2 _panOffset = Vector2.zero;
        private float _zoom = 1f;
        private const float MinZoom = 0.4f;
        private const float MaxZoom = 2f;
        private bool _isPanning = false;
        private Vector2 _lastMousePos;

        // ── Connection dragging ───────────────────────────────────────
        private bool _isDraggingConnection = false;
        private string _connFromNodeId;
        private int _connFromPort;
        private Vector2 _connDragEnd;

        // ── Node dragging ─────────────────────────────────────────────
        private string _draggingNodeId = null;
        private Vector2 _dragOffset;

        // ── Selection ─────────────────────────────────────────────────
        private string _selectedNodeId = null;

        // ── Node sizes ────────────────────────────────────────────────
        private const float NodeWidth = 200f;
        private const float NodeHeight = 80f;
        private const float PortRadius = 7f;
        private const float PortSpacing = 22f;

        // ── Colors ────────────────────────────────────────────────────
        private static readonly Color BgColor = new Color(0.10f, 0.11f, 0.14f);
        private static readonly Color GridMinor = new Color(1, 1, 1, 0.04f);
        private static readonly Color GridMajor = new Color(1, 1, 1, 0.09f);
        private static readonly Color NodeBg = new Color(0.16f, 0.18f, 0.23f);
        private static readonly Color NodeBgSelected = new Color(0.20f, 0.23f, 0.32f);
        private static readonly Color NodeBorderNormal = new Color(0.30f, 0.33f, 0.42f);
        private static readonly Color NodeBorderSelect = new Color(0.88f, 0.72f, 0.29f);
        private static readonly Color PortIn = new Color(0.40f, 0.75f, 0.95f);
        private static readonly Color PortOut = new Color(0.95f, 0.75f, 0.40f);
        private static readonly Color PortYes = new Color(0.30f, 0.90f, 0.50f);
        private static readonly Color PortNo = new Color(0.95f, 0.40f, 0.40f);
        private static readonly Color ConnColor = new Color(0.88f, 0.72f, 0.29f, 0.85f);
        private static readonly Color ConnDrag = new Color(0.88f, 0.72f, 0.29f, 0.45f);

        // Node type header colors
        private static readonly Dictionary<HintNodeType, Color> NodeHeaderColors = new()
        {
            { HintNodeType.Start, new Color(0.20f, 0.65f, 0.35f) },
            { HintNodeType.HintSequence, new Color(0.25f, 0.45f, 0.80f) },
            { HintNodeType.Condition, new Color(0.75f, 0.50f, 0.15f) },
            { HintNodeType.InventoryTrigger, new Color(0.60f, 0.25f, 0.75f) },
            { HintNodeType.End, new Color(0.75f, 0.25f, 0.25f) },
        };

        // ─────────────────────────────────────────────────────────────

        #region Open

        // ─────────────────────────────────────────────────────────────

        [MenuItem("Window/Hint System/Hint Graph Editor")]
        public static void OpenWindow()
        {
            var w = GetWindow<HintGraphEditor>("Hint Graph");
            w.minSize = new Vector2(800, 500);
        }

        public static void OpenWithGraph(HintGraphData graph)
        {
            var w = GetWindow<HintGraphEditor>("Hint Graph");
            w._graph = graph;
            w.minSize = new Vector2(800, 500);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────

        #region OnGUI — main draw loop

        // ─────────────────────────────────────────────────────────────

        private void OnGUI()
        {
            DrawBackground();

            if (_graph == null)
            {
                DrawEmptyState();
                HandleEmptyStateInput();
                return;
            }

            // Transform canvas
            GUI.BeginGroup(new Rect(0, 0, position.width, position.height));
            Matrix4x4 savedMatrix = GUI.matrix;
            GUIUtility.ScaleAroundPivot(Vector2.one * _zoom, position.size * 0.5f);

            DrawGrid();
            DrawConnections();
            DrawDragConnection();
            DrawNodes();

            GUI.matrix = savedMatrix;
            GUI.EndGroup();

            DrawToolbar();
            DrawInspectorPanel();

            HandleInput();
            Repaint();
        }

        #endregion

        // ─────────────────────────────────────────────────────────────

        #region Draw — Background & Grid

        // ─────────────────────────────────────────────────────────────

        private void DrawBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), BgColor);
        }

        private void DrawGrid()
        {
            float minor = 20f;
            float major = 100f;

            DrawGridLines(minor, GridMinor);
            DrawGridLines(major, GridMajor);
        }

        private void DrawGridLines(float spacing, Color color)
        {
            Handles.color = color;

            // FIX 1: Vector2 has no % operator — must do component-wise
            // FIX 2: Raw modulo is negative when panOffset is negative (panning left/up)
            //        Use ((x % s) + s) % s to always get a positive remainder
            float ox = ((_panOffset.x % spacing) + spacing) % spacing;
            float oy = ((_panOffset.y % spacing) + spacing) % spacing;

            int xCount = Mathf.CeilToInt(position.width / spacing) + 1;
            int yCount = Mathf.CeilToInt(position.height / spacing) + 1;

            for (int i = 0; i < xCount; i++)
            {
                float x = ox + i * spacing;
                Handles.DrawLine(new Vector3(x, 0), new Vector3(x, position.height));
            }

            for (int i = 0; i < yCount; i++)
            {
                float y = oy + i * spacing;
                Handles.DrawLine(new Vector3(0, y), new Vector3(position.width, y));
            }
        }

        #endregion

        // ─────────────────────────────────────────────────────────────

        #region Draw — Nodes

        // ─────────────────────────────────────────────────────────────

        private void DrawNodes()
        {
            if (_graph == null) return;
            foreach (var node in _graph.nodes)
                DrawNode(node);
        }

        private void DrawNode(HintNodeData node)
        {
            Rect r = GetNodeScreenRect(node);
            bool selected = node.nodeId == _selectedNodeId;

            // Shadow
            EditorGUI.DrawRect(new Rect(r.x + 3, r.y + 4, r.width, r.height),
                new Color(0, 0, 0, 0.35f));

            // Body
            EditorGUI.DrawRect(r, selected ? NodeBgSelected : NodeBg);

            // Header strip
            Rect header = new Rect(r.x, r.y, r.width, 26f);
            Color headerCol = NodeHeaderColors.TryGetValue(node.nodeType, out Color hc) ? hc : Color.gray;
            EditorGUI.DrawRect(header, headerCol * 0.75f);

            // Border
            Color borderCol = selected ? NodeBorderSelect : NodeBorderNormal;
            DrawBorder(r, borderCol, selected ? 2f : 1f);

            // Header label
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(r.x, r.y + 2, r.width, 22), GetNodeTypeName(node.nodeType), headerStyle);

            // Node label
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 10,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.85f, 0.85f, 0.85f) }
            };
            string displayLabel = string.IsNullOrEmpty(node.label) ? "(unnamed)" : node.label;
            GUI.Label(new Rect(r.x, r.y + 28, r.width, 20), displayLabel, labelStyle);

            // Sub-info
            string info = GetNodeInfo(node);
            if (!string.IsNullOrEmpty(info))
            {
                GUIStyle infoStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = new Color(0.60f, 0.65f, 0.75f) }
                };
                GUI.Label(new Rect(r.x, r.y + 46, r.width, 18), info, infoStyle);
            }

            // Ports
            DrawNodePorts(node, r);
        }

        private void DrawNodePorts(HintNodeData node, Rect r)
        {
            // Input port (top center) — all except Start
            if (node.nodeType != HintNodeType.Start)
            {
                Vector2 inPos = new Vector2(r.x + r.width * 0.5f, r.y);
                DrawPort(inPos, PortIn, "▲");
            }

            // Output ports (bottom)
            List<(int idx, string label, Color col)> outputs = GetOutputPorts(node);
            float totalW = outputs.Count * (PortRadius * 2 + 8f) + (outputs.Count - 1) * 10f;
            float startX = r.x + (r.width - totalW) * 0.5f;

            for (int i = 0; i < outputs.Count; i++)
            {
                var (idx, lbl, col) = outputs[i];
                float px = startX + i * (PortRadius * 2 + 18f) + PortRadius;
                Vector2 pos = new Vector2(px, r.yMax);
                DrawPort(pos, col, lbl);
            }
        }

        private void DrawPort(Vector2 center, Color color, string label)
        {
            Rect portRect = new Rect(center.x - PortRadius, center.y - PortRadius,
                PortRadius * 2, PortRadius * 2);
            EditorGUI.DrawRect(portRect, color);

            // Ring
            Handles.color = Color.white * 0.6f;
            Handles.DrawWireDisc(center, Vector3.forward, PortRadius);

            GUIStyle ps = new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 8,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(center.x - 12, center.y + PortRadius + 1, 24, 12), label, ps);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────

        #region Draw — Connections

        // ─────────────────────────────────────────────────────────────

        private void DrawConnections()
        {
            if (_graph == null) return;
            foreach (var conn in _graph.connections)
            {
                HintNodeData from = _graph.GetNodeById(conn.fromNodeId);
                HintNodeData to = _graph.GetNodeById(conn.toNodeId);
                if (from == null || to == null) continue;

                Vector2 start = GetOutputPortPosition(from, conn.fromPortIndex);
                Vector2 end = GetInputPortPosition(to);

                DrawBezier(start, end, ConnColor);
            }
        }

        private void DrawDragConnection()
        {
            if (!_isDraggingConnection) return;
            HintNodeData from = _graph.GetNodeById(_connFromNodeId);
            if (from == null) return;

            Vector2 start = GetOutputPortPosition(from, _connFromPort);
            DrawBezier(start, _connDragEnd, ConnDrag);
        }

        private void DrawBezier(Vector2 start, Vector2 end, Color color)
        {
            float dy = Mathf.Abs(end.y - start.y);
            Vector2 startTan = start + Vector2.up * Mathf.Max(dy * 0.5f, 40f);
            Vector2 endTan = end - Vector2.up * Mathf.Max(dy * 0.5f, 40f);

            Handles.DrawBezier(start, end, startTan, endTan, color, null, 2.5f);

            // Arrow head
            Vector2 dir = (end - endTan).normalized;
            DrawArrow(end, dir, color);
        }

        private void DrawArrow(Vector2 tip, Vector2 dir, Color color)
        {
            float size = 7f;
            Vector2 right = new Vector2(-dir.y, dir.x);
            Vector2 p1 = tip - dir * size + right * (size * 0.5f);
            Vector2 p2 = tip - dir * size - right * (size * 0.5f);

            Handles.color = color;
            Handles.DrawAAConvexPolygon(tip, p1, p2);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────

        #region Draw — Toolbar & Inspector

        // ─────────────────────────────────────────────────────────────

        private void DrawToolbar()
        {
            GUILayout.BeginArea(new Rect(0, 0, position.width, 34));
            EditorGUI.DrawRect(new Rect(0, 0, position.width, 34), new Color(0.12f, 0.13f, 0.17f));

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);

            // Graph picker
            GUIStyle graphBtnStyle = EditorStyles.toolbarButton;
            _graph = (HintGraphData)EditorGUILayout.ObjectField(
                _graph, typeof(HintGraphData), false, GUILayout.Width(200));

            GUILayout.Space(10);

            if (_graph != null)
            {
                if (GUILayout.Button("+ Start", EditorStyles.toolbarButton)) AddNode(HintNodeType.Start);
                if (GUILayout.Button("+ Hint", EditorStyles.toolbarButton)) AddNode(HintNodeType.HintSequence);
                if (GUILayout.Button("+ Condition", EditorStyles.toolbarButton)) AddNode(HintNodeType.Condition);
                if (GUILayout.Button("+ Inv Trigger", EditorStyles.toolbarButton))
                    AddNode(HintNodeType.InventoryTrigger);
                if (GUILayout.Button("+ End", EditorStyles.toolbarButton)) AddNode(HintNodeType.End);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Clear All", EditorStyles.toolbarButton)) ClearGraph();
                if (GUILayout.Button("New Graph", EditorStyles.toolbarButton)) CreateNewGraph();

                GUILayout.Space(8);
                GUILayout.Label($"Zoom: {_zoom:F1}x", EditorStyles.miniLabel, GUILayout.Width(60));
                if (GUILayout.Button("Reset View", EditorStyles.toolbarButton)) ResetView();
            }

            GUILayout.Space(8);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawInspectorPanel()
        {
            if (_selectedNodeId == null || _graph == null) return;
            HintNodeData node = _graph.GetNodeById(_selectedNodeId);
            if (node == null) return;

            float panelW = 240f;
            float panelH = position.height - 34f;
            Rect panel = new Rect(position.width - panelW, 34f, panelW, panelH);

            EditorGUI.DrawRect(panel, new Color(0.12f, 0.13f, 0.17f, 0.97f));
            DrawBorder(panel, new Color(0.30f, 0.33f, 0.42f), 1f);

            GUILayout.BeginArea(new Rect(panel.x + 10, panel.y + 10, panelW - 20, panelH - 20));

            GUIStyle title = new GUIStyle(EditorStyles.boldLabel)
                { fontSize = 12, normal = { textColor = Color.white } };
            GUILayout.Label("Node Inspector", title);
            GUILayout.Space(6);

            EditorGUI.BeginChangeCheck();

            // Label
            GUILayout.Label("Label", EditorStyles.miniLabel);
            node.label = EditorGUILayout.TextField(node.label);
            GUILayout.Space(4);

            // Type-specific fields
            switch (node.nodeType)
            {
                case HintNodeType.HintSequence:
                    GUILayout.Label("Hint Type", EditorStyles.miniLabel);
                    node.hintType = (HintType)EditorGUILayout.EnumPopup(node.hintType);
                    GUILayout.Space(4);

                    if (node.hintType == HintType.Inventory)
                    {
                        GUILayout.Label("Inventory Item ID", EditorStyles.miniLabel);
                        node.inventoryId = EditorGUILayout.IntField(node.inventoryId);
                    }
                    else if (node.hintType == HintType.InnerPanel)
                    {
                        GUILayout.Label("Inner Panel ID", EditorStyles.miniLabel);
                        node.innerPanelId = EditorGUILayout.IntField(node.innerPanelId);
                    }

                    GUILayout.Space(4);
                    GUILayout.Label("Glow Duration (sec)", EditorStyles.miniLabel);
                    node.glowDuration = EditorGUILayout.FloatField(node.glowDuration);
                    break;

                case HintNodeType.Condition:
                    GUILayout.Label("Condition Type", EditorStyles.miniLabel);
                    node.conditionType = (ConditionType)EditorGUILayout.EnumPopup(node.conditionType);
                    GUILayout.Space(4);

                    if (node.conditionType == ConditionType.CustomFlag)
                    {
                        GUILayout.Label("Flag Key", EditorStyles.miniLabel);
                        node.conditionKey = EditorGUILayout.TextField(node.conditionKey);
                    }
                    else
                    {
                        GUILayout.Label("Target ID", EditorStyles.miniLabel);
                        node.conditionTargetId = EditorGUILayout.IntField(node.conditionTargetId);
                    }

                    break;

                case HintNodeType.InventoryTrigger:
                    GUILayout.Label("Inventory Item ID", EditorStyles.miniLabel);
                    node.triggerInventoryId = EditorGUILayout.IntField(node.triggerInventoryId);
                    GUILayout.Space(4);
                    GUILayout.Label("Trigger On", EditorStyles.miniLabel);
                    node.triggerOnPickup = EditorGUILayout.Toggle("Pickup", node.triggerOnPickup);
                    break;
            }

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(_graph);

            GUILayout.Space(12);

            // Delete button
            GUI.backgroundColor = new Color(0.8f, 0.3f, 0.3f);
            if (GUILayout.Button("Delete Node"))
            {
                DeleteNode(_selectedNodeId);
                _selectedNodeId = null;
            }

            GUI.backgroundColor = Color.white;

            GUILayout.EndArea();
        }

        private void DrawEmptyState()
        {
            GUIStyle s = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 14,
                normal = { textColor = new Color(0.5f, 0.5f, 0.6f) }
            };
            GUI.Label(new Rect(0, position.height * 0.4f, position.width, 30),
                "Select a Hint Graph asset above, or create a new one →", s);

            DrawToolbar();
        }

        #endregion

        // ─────────────────────────────────────────────────────────────

        #region Input Handling

        // ─────────────────────────────────────────────────────────────

        private void HandleInput()
        {
            Event e = Event.current;

            // Inspector panel absorbs mouse when open
            if (_selectedNodeId != null)
            {
                float panelW = 240f;
                Rect panel = new Rect(position.width - panelW, 34f, panelW, position.height - 34f);
                if (panel.Contains(e.mousePosition)) return;
            }

            Vector2 canvasPos = ScreenToCanvas(e.mousePosition);

            switch (e.type)
            {
                case EventType.MouseDown:
                    HandleMouseDown(e, canvasPos);
                    break;

                case EventType.MouseDrag:
                    HandleMouseDrag(e, canvasPos);
                    break;

                case EventType.MouseUp:
                    HandleMouseUp(e, canvasPos);
                    break;

                case EventType.ScrollWheel:
                    HandleScroll(e);
                    break;

                case EventType.KeyDown:
                    HandleKeyDown(e);
                    break;

                case EventType.ContextClick:
                    HandleContextMenu(e, canvasPos);
                    break;
            }
        }

        private void HandleMouseDown(Event e, Vector2 canvasPos)
        {
            if (e.button == 0)
            {
                // Check port click first (start connection drag)
                foreach (var node in _graph.nodes)
                {
                    int port = GetOutputPortHit(node, e.mousePosition);
                    if (port >= 0)
                    {
                        _isDraggingConnection = true;
                        _connFromNodeId = node.nodeId;
                        _connFromPort = port;
                        _connDragEnd = e.mousePosition;
                        e.Use();
                        return;
                    }
                }

                // Check node body click
                HintNodeData hit = GetNodeAtScreenPos(e.mousePosition);
                if (hit != null)
                {
                    _selectedNodeId = hit.nodeId;
                    _draggingNodeId = hit.nodeId;
                    _dragOffset = canvasPos - hit.editorRect.position;
                    e.Use();
                }
                else
                {
                    _selectedNodeId = null;
                }
            }

            if (e.button == 2 || (e.button == 0 && e.alt))
            {
                _isPanning = true;
                _lastMousePos = e.mousePosition;
                e.Use();
            }
        }

        private void HandleMouseDrag(Event e, Vector2 canvasPos)
        {
            if (_isPanning)
            {
                _panOffset += (e.mousePosition - _lastMousePos);
                _lastMousePos = e.mousePosition;
                e.Use();
            }
            else if (_draggingNodeId != null)
            {
                HintNodeData node = _graph.GetNodeById(_draggingNodeId);
                if (node != null)
                {
                    node.editorRect.position = canvasPos - _dragOffset;
                    EditorUtility.SetDirty(_graph);
                }

                e.Use();
            }
            else if (_isDraggingConnection)
            {
                _connDragEnd = e.mousePosition;
                e.Use();
            }
        }

        private void HandleMouseUp(Event e, Vector2 canvasPos)
        {
            _isPanning = false;
            _draggingNodeId = null;

            if (_isDraggingConnection)
            {
                _isDraggingConnection = false;

                // Check if we dropped on an input port of another node
                HintNodeData target = GetNodeAtScreenPos(e.mousePosition);
                if (target != null && target.nodeId != _connFromNodeId)
                {
                    // Remove existing connection from this port
                    _graph.connections.RemoveAll(c =>
                        c.fromNodeId == _connFromNodeId && c.fromPortIndex == _connFromPort);

                    _graph.connections.Add(new HintConnection
                    {
                        fromNodeId = _connFromNodeId,
                        fromPortIndex = _connFromPort,
                        toNodeId = target.nodeId
                    });
                    EditorUtility.SetDirty(_graph);
                }

                e.Use();
            }
        }

        private void HandleScroll(Event e)
        {
            float delta = e.delta.y * 0.05f;
            _zoom = Mathf.Clamp(_zoom - delta, MinZoom, MaxZoom);
            e.Use();
        }

        private void HandleKeyDown(Event e)
        {
            if (e.keyCode == KeyCode.Delete && _selectedNodeId != null)
            {
                DeleteNode(_selectedNodeId);
                _selectedNodeId = null;
                e.Use();
            }

            if (e.keyCode == KeyCode.F)
            {
                ResetView();
                e.Use();
            }
        }

        private void HandleEmptyStateInput()
        {
        }

        private void HandleContextMenu(Event e, Vector2 canvasPos)
        {
            if (_graph == null) return;

            HintNodeData hit = GetNodeAtScreenPos(e.mousePosition);
            GenericMenu menu = new GenericMenu();

            if (hit != null)
            {
                menu.AddItem(new GUIContent("Delete Node"), false, () => DeleteNode(hit.nodeId));
                menu.AddItem(new GUIContent("Disconnect All"), false, () =>
                {
                    _graph.connections.RemoveAll(c =>
                        c.fromNodeId == hit.nodeId || c.toNodeId == hit.nodeId);
                    EditorUtility.SetDirty(_graph);
                });
            }
            else
            {
                menu.AddItem(new GUIContent("Add/Start Node"), false, () => AddNodeAt(HintNodeType.Start, canvasPos));
                menu.AddItem(new GUIContent("Add/Hint Sequence"), false,
                    () => AddNodeAt(HintNodeType.HintSequence, canvasPos));
                menu.AddItem(new GUIContent("Add/Condition"), false,
                    () => AddNodeAt(HintNodeType.Condition, canvasPos));
                menu.AddItem(new GUIContent("Add/Inventory Trigger"), false,
                    () => AddNodeAt(HintNodeType.InventoryTrigger, canvasPos));
                menu.AddItem(new GUIContent("Add/End Node"), false, () => AddNodeAt(HintNodeType.End, canvasPos));
            }

            menu.ShowAsContext();
            e.Use();
        }

        #endregion

        // ─────────────────────────────────────────────────────────────

        #region Graph Operations

        // ─────────────────────────────────────────────────────────────

        private void AddNode(HintNodeType type)
        {
            Vector2 center = new Vector2(position.width * 0.5f, position.height * 0.5f);
            AddNodeAt(type, ScreenToCanvas(center));
        }

        private void AddNodeAt(HintNodeType type, Vector2 canvasPos)
        {
            if (_graph == null) return;

            var node = new HintNodeData
            {
                nodeId = Guid.NewGuid().ToString(),
                nodeType = type,
                label = GetNodeTypeName(type),
                editorRect = new Rect(canvasPos.x, canvasPos.y, NodeWidth, NodeHeight)
            };

            Undo.RecordObject(_graph, "Add Hint Node");
            _graph.nodes.Add(node);
            _selectedNodeId = node.nodeId;
            EditorUtility.SetDirty(_graph);
        }

        private void DeleteNode(string nodeId)
        {
            if (_graph == null) return;
            Undo.RecordObject(_graph, "Delete Hint Node");
            _graph.nodes.RemoveAll(n => n.nodeId == nodeId);
            _graph.connections.RemoveAll(c => c.fromNodeId == nodeId || c.toNodeId == nodeId);
            EditorUtility.SetDirty(_graph);
        }

        private void ClearGraph()
        {
            if (_graph == null) return;
            if (!EditorUtility.DisplayDialog("Clear Graph",
                    "Delete all nodes and connections?", "Yes", "Cancel")) return;

            Undo.RecordObject(_graph, "Clear Hint Graph");
            _graph.nodes.Clear();
            _graph.connections.Clear();
            _selectedNodeId = null;
            EditorUtility.SetDirty(_graph);
        }

        private void CreateNewGraph()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "New Hint Graph", "NewHintGraph", "asset", "Save Hint Graph");
            if (string.IsNullOrEmpty(path)) return;

            var newGraph = CreateInstance<HintGraphData>();
            AssetDatabase.CreateAsset(newGraph, path);
            AssetDatabase.SaveAssets();
            _graph = newGraph;
        }

        #endregion

        // ─────────────────────────────────────────────────────────────

        #region Coordinate & Hit-test Helpers

        // ─────────────────────────────────────────────────────────────

        private Vector2 ScreenToCanvas(Vector2 screenPos)
        {
            Vector2 centered = screenPos - position.size * 0.5f;
            return centered / _zoom - _panOffset / _zoom + position.size * 0.5f;
        }

        private Rect GetNodeScreenRect(HintNodeData node)
        {
            Vector2 pivot = position.size * 0.5f;
            Vector2 offset = _panOffset;
            Vector2 pos = (node.editorRect.position - pivot) * _zoom + pivot + offset;
            return new Rect(pos.x, pos.y + 34f,
                node.editorRect.width * _zoom,
                node.editorRect.height * _zoom);
        }

        private HintNodeData GetNodeAtScreenPos(Vector2 screenPos)
        {
            // Iterate in reverse so top-most node wins
            for (int i = _graph.nodes.Count - 1; i >= 0; i--)
            {
                Rect r = GetNodeScreenRect(_graph.nodes[i]);
                if (r.Contains(screenPos)) return _graph.nodes[i];
            }

            return null;
        }

        private Vector2 GetInputPortPosition(HintNodeData node)
        {
            Rect r = GetNodeScreenRect(node);
            return new Vector2(r.x + r.width * 0.5f, r.y);
        }

        private Vector2 GetOutputPortPosition(HintNodeData node, int portIndex)
        {
            Rect r = GetNodeScreenRect(node);
            var ports = GetOutputPorts(node);
            float totalW = ports.Count * (PortRadius * 2 + 8f) + (ports.Count - 1) * 10f;
            float startX = r.x + (r.width - totalW) * 0.5f;
            float px = startX + portIndex * (PortRadius * 2 + 18f) + PortRadius;
            return new Vector2(px, r.yMax);
        }

        private int GetOutputPortHit(HintNodeData node, Vector2 screenPos)
        {
            var ports = GetOutputPorts(node);
            for (int i = 0; i < ports.Count; i++)
            {
                Vector2 pos = GetOutputPortPosition(node, i);
                if (Vector2.Distance(pos, screenPos) < PortRadius + 4f)
                    return ports[i].idx;
            }

            return -1;
        }

        private List<(int idx, string label, Color col)> GetOutputPorts(HintNodeData node)
        {
            var ports = new List<(int, string, Color)>();
            switch (node.nodeType)
            {
                case HintNodeType.Condition:
                    ports.Add((0, "YES", PortYes));
                    ports.Add((1, "NO", PortNo));
                    break;
                case HintNodeType.End:
                    break;
                default:
                    ports.Add((0, "▼", PortOut));
                    break;
            }

            return ports;
        }

        #endregion

        // ─────────────────────────────────────────────────────────────

        #region Utility

        // ─────────────────────────────────────────────────────────────

        private void DrawBorder(Rect r, Color color, float thickness)
        {
            Handles.color = color;
            Handles.DrawSolidRectangleWithOutline(r, Color.clear, color);
        }

        private void ResetView()
        {
            _panOffset = Vector2.zero;
            _zoom = 1f;
        }

        private string GetNodeTypeName(HintNodeType type) => type switch
        {
            HintNodeType.Start => "START",
            HintNodeType.HintSequence => "HINT SEQUENCE",
            HintNodeType.Condition => "CONDITION",
            HintNodeType.InventoryTrigger => "INVENTORY TRIGGER",
            HintNodeType.End => "END",
            _ => type.ToString()
        };

        private string GetNodeInfo(HintNodeData node) => node.nodeType switch
        {
            HintNodeType.HintSequence => $"{node.hintType}  •  {node.glowDuration:F1}s",
            HintNodeType.Condition => $"{node.conditionType}  •  id:{node.conditionTargetId}",
            HintNodeType.InventoryTrigger => $"Item ID: {node.triggerInventoryId}",
            _ => string.Empty
        };

        #endregion
    }



    [CustomEditor(typeof(HintGraphData))]
    public class HintGraphDataInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            if (GUILayout.Button("Open in Hint Graph Editor", GUILayout.Height(32)))
                HintGraphEditor.OpenWithGraph((HintGraphData)target);
        }
    }
}
#endif