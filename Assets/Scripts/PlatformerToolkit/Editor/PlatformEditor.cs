using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace PlatformerToolkit
{
	[CustomEditor(typeof(Platform))]
	public class PlatformEditor : Editor
	{
		private const int Segment_None = -1;
		private const int Segment_BeforeFirst = -2;
		private const int Segment_AfterLast = -3;

		private PlatformBuilder builder = new PlatformBuilder();
		private Platform platform;
		private Transform transform;
		private bool onSceneGUIFirstCall;
		private int nearestSegment;
		private Vector3 worldMousePos;
		private Vector2 localMousePos;

		public List<Platform.PathNode> Path { get { return platform.Path; } }

		private void OnEnable()
		{
			platform = (Platform)target;
			transform = platform.GetComponent<Transform>();
			nearestSegment = Segment_None;
			onSceneGUIFirstCall = true;
		}

		private void OnSceneGUI()
		{
			var ev = Event.current;
			var insertMode = ev.shift;
			if (ev.type == EventType.MouseMove || onSceneGUIFirstCall) {
				UpdateMouse();
				SceneView.currentDrawingSceneView.Repaint();
			}
			if (ev.type == EventType.Repaint)
				DrawPath();
			if (insertMode)
				HandleInsertMode();
			else if (nearestSegment >= 0)
				HandleSegmentDrag();
			HandlePoints();
			onSceneGUIFirstCall = false;
		}

		private void DrawPath()
		{
			if (Path.Count < 2)
				return;
			var i = 0;
			var j = 1;
			if (platform.Closed) {
				i = Path.Count - 1;
				j = 0;
			}
			var start = transform.TransformPoint(Path[i].Pos);
			var oldColor = Handles.color;
			Handles.color = Color.white;
			while (j < Path.Count) {
				var end = transform.TransformPoint(Path[j].Pos);
				Handles.DrawAAPolyLine(6.0f, start, end);
				start = end;
				j++;
			}
			Handles.color = oldColor;
		}

		private void HandleInsertMode()
		{
			var ev = Event.current;
			var defControl = GUIUtility.GetControlID(FocusType.Passive);
			switch (ev.GetTypeForControl(defControl)) {
				case EventType.MouseDown:
					if (HandleUtility.nearestControl != defControl || ev.button != 0)
						break;
					if (nearestSegment != Segment_None) {
						BeginModify();
						if (nearestSegment == Segment_BeforeFirst)
							Path.Insert(0, new Platform.PathNode {
								Pos = localMousePos
							});
						else if (nearestSegment == Segment_AfterLast)
							Path.Insert(Path.Count, new Platform.PathNode {
								Pos = localMousePos
							});
						else
							Path.Insert(nearestSegment + 1, new Platform.PathNode {
								Pos = localMousePos
							});
						EndModify();
					}
					ev.Use();
					break;
				case EventType.Layout:
					HandleUtility.AddDefaultControl(defControl);
					break;
			}
		}

		private void HandlePoints()
		{
			for (var i = 0; i < Path.Count; i++) {
				var node = Path[i];
				var worldPos = transform.TransformPoint(node.Pos);
				EditorGUI.BeginChangeCheck();
				worldPos = Handles.Slider2D(
					worldPos, transform.forward, transform.right, transform.up,
					HandleUtility.GetHandleSize(worldPos) * 0.05f, Handles.DotHandleCap, 0.0f);
				if (EditorGUI.EndChangeCheck()) {
					BeginModify();
					node.Pos = (Vector2)transform.InverseTransformPoint(worldPos);
					EndModify();
				}
			}
		}

		private void HandleSegmentDrag()
		{
			var startNode = Path[nearestSegment];
			var endNode = Path[(nearestSegment + 1) % Path.Count];
			var center = (startNode.Pos + endNode.Pos) * 0.5f;
			var worldCenter = transform.TransformPoint(center);
			EditorGUI.BeginChangeCheck();
			worldCenter = Handles.Slider2D(
				worldCenter, transform.forward, transform.right, transform.up,
				4.0f, SegmentDraggingCap, 0.0f);
			if (EditorGUI.EndChangeCheck()) {
				var newCenter = (Vector2)transform.InverseTransformPoint(worldCenter);
				var delta = newCenter - center;
				BeginModify();
				startNode.Pos += delta;
				endNode.Pos += delta;
				EndModify();
			}
		}

		private void SegmentDraggingCap(int id, Vector3 pos, Quaternion rotation, float size, EventType eventType)
		{
			var start = Path[nearestSegment];
			var end = Path[(nearestSegment + 1) % Path.Count];
			var worldStart = transform.TransformPoint(start.Pos);
			var worldEnd = transform.TransformPoint(end.Pos);
			if (eventType == EventType.Repaint && HandleUtility.nearestControl == id) {
				var oldColor = Handles.color;
				var oldZTest = Handles.zTest;
				Handles.color = Color.white;
				Handles.zTest = CompareFunction.Always;
				Handles.DrawAAPolyLine(size * 2.0f, worldStart, worldEnd);
				Handles.color = Color.black;
				Handles.DrawAAPolyLine(size, worldStart, worldEnd);
				Handles.color = oldColor;
				Handles.zTest = oldZTest;
			} else if (eventType == EventType.Layout) {
				var d = HandleUtility.DistanceToLine(worldStart, worldEnd);
				HandleUtility.AddControl(id, d);
			}
		}

		private void UpdateMouse()
		{
			nearestSegment = Segment_None;
			var ev = Event.current;
			var plane = new Plane(transform.forward, transform.position);
			var ray = HandleUtility.GUIPointToWorldRay(ev.mousePosition);
			float enter;
			if (!plane.Raycast(ray, out enter))
				return;
			worldMousePos = ray.GetPoint(enter);
			localMousePos = transform.InverseTransformPoint(worldMousePos);
			if (Path.Count < 2) {
				nearestSegment = Segment_AfterLast;
				return;
			}
			Vector2 nearestPt;
			FindNearestPoint(localMousePos, out nearestPt, out nearestSegment);
			var worldNearestPt = transform.TransformPoint(nearestPt);
			var guiNearestPt = HandleUtility.WorldToGUIPoint(worldNearestPt);
			if ((ev.mousePosition - guiNearestPt).magnitude * EditorGUIUtility.pixelsPerPoint > 30.0f) {
				if (platform.Closed) {
					nearestSegment = Segment_None;
				} else {
					var sqrDistToStart = (localMousePos - Path[0].Pos).sqrMagnitude;
					var sqrDistToEnd = (localMousePos - Path[Path.Count - 1].Pos).sqrMagnitude;
					if (sqrDistToStart < sqrDistToEnd)
						nearestSegment = Segment_BeforeFirst;
					else
						nearestSegment = Segment_AfterLast;
				}
			}
		}

		private void FindNearestPoint(Vector2 pt, out Vector2 nearestPt, out int nearestSegment)
		{
			nearestPt = Vector2.zero;
			nearestSegment = 0;
			var minSqrDist = float.PositiveInfinity;
			var i = 0;
			var j = 1;
			if (platform.Closed) {
				i = Path.Count - 1;
				j = 0;
			}
			while (j < Path.Count) {
				var segmentVec = Path[j].Pos - Path[i].Pos;
				var t = Vector2.Dot(pt - Path[i].Pos, segmentVec) / segmentVec.sqrMagnitude;
				t = Mathf.Clamp(t, 0.0f, 1.0f);
				var nearestPtOnSegment = Path[i].Pos + segmentVec * t;
				var sqrDist = (pt - nearestPtOnSegment).sqrMagnitude;
				if (sqrDist < minSqrDist) {
					minSqrDist = sqrDist;
					nearestPt = nearestPtOnSegment;
					nearestSegment = i;
				}
				i = j;
				j++;
			}
		}

		private void BeginModify()
		{
			Undo.RecordObject(platform, "Platform Modification");
		}

		private void EndModify()
		{
			Rebuild();
		}

		private void Rebuild()
		{
			builder.Build(platform);
		}
	}
}