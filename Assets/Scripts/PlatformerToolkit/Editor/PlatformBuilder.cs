using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlatformerToolkit
{
	public class PlatformBuilder
	{
		public void Build(Platform pf)
		{
			var mf = pf.GetComponent<MeshFilter>();
			var mr = pf.GetComponent<MeshRenderer>();
			if (mf == null || mr == null || pf.Material == null)
				return;
			var m = mf.sharedMesh;
			if (m == null) {
				m = new Mesh();
				mf.sharedMesh = m;
			}
			var verts = new List<Vector3>();
			var uv = new List<Vector2>();
			var indices = new List<int>();
			var subEdges = new List<SubEdge>();
			var corners = new List<Corner>();
			GetEdge(pf, subEdges, corners);
			foreach (var se in subEdges)
				BuildSubEdge(pf, se, verts, uv, indices);
			foreach (var c in corners)
				BuildCorner(pf, c);
			m.Clear();
			m.SetVertices(verts);
			m.SetUVs(0, uv);
			m.SetTriangles(indices, 0);
			m.RecalculateNormals();
			m.RecalculateTangents();
			m.RecalculateBounds();
			mr.sharedMaterial = pf.Material.EdgeMaterial;
		}

		private void BuildSubEdge(Platform pf, SubEdge subEdge, List<Vector3> verts, List<Vector2> uv, List<int> indices)
		{
			var totalLen = 0.0f;
			var lens = new List<float>();
			for (var i = 0; i < subEdge.OuterVerts.Count - 1; i++) {
				var iv1 = subEdge.InnerVerts[i];
				var ov1 = subEdge.OuterVerts[i];
				var iv2 = subEdge.InnerVerts[i + 1];
				var ov2 = subEdge.OuterVerts[i + 1];
				var v1 = (iv1 + ov1) * 0.5f;
				var v2 = (iv2 + ov2) * 0.5f;
				var len = (v2 - v1).magnitude;
				lens.Add(len);
				totalLen += len;
			}
			var atlas = pf.Material.EdgeMaterial.mainTexture;
			var atlasRegion = pf.Material.GetEdge(subEdge.Side);
			var uvRect = Rect.MinMaxRect(
				atlasRegion.xMin / atlas.width,
				atlasRegion.yMin / atlas.height,
				atlasRegion.xMax / atlas.width,
				atlasRegion.yMax / atlas.height);
			var numSlices = Mathf.Max(1, Mathf.Round(totalLen * atlasRegion.height / (atlasRegion.width * pf.Thickness)));
			var sliceWidth = totalLen / numSlices;
			var pos = 0.0f;
			var curSeg = 0;
			var iv = subEdge.InnerVerts[0];
			var ov = subEdge.OuterVerts[0];
			for (var i = 0; i < numSlices; i++) {
				var baseVert = verts.Count;
				var sliceStart = i * sliceWidth;
				var sliceEnd = (i + 1) * sliceWidth;
				verts.Add(iv);
				verts.Add(ov);
				uv.Add(new Vector2(uvRect.xMin, uvRect.yMin));
				uv.Add(new Vector2(uvRect.xMin, uvRect.yMax));
				while (curSeg < subEdge.OuterVerts.Count - 2 && sliceEnd > pos + lens[curSeg]) {
					pos = pos + lens[curSeg];
					curSeg++;
					iv = subEdge.InnerVerts[curSeg];
					ov = subEdge.OuterVerts[curSeg];
					verts.Add(iv);
					verts.Add(ov);
					var u = Mathf.LerpUnclamped(uvRect.xMin, uvRect.xMax, (pos - sliceStart) / sliceWidth);
					uv.Add(new Vector2(u, uvRect.yMin));
					uv.Add(new Vector2(u, uvRect.yMax));
				}
				var t = (sliceEnd - pos) / lens[curSeg];
				iv = Vector2.LerpUnclamped(subEdge.InnerVerts[curSeg], subEdge.InnerVerts[curSeg + 1], t);
				ov = Vector2.LerpUnclamped(subEdge.OuterVerts[curSeg], subEdge.OuterVerts[curSeg + 1], t);
				verts.Add(iv);
				verts.Add(ov);
				uv.Add(new Vector2(uvRect.xMax, uvRect.yMin));
				uv.Add(new Vector2(uvRect.xMax, uvRect.yMax));
				for (var j = baseVert; j < verts.Count - 3; j += 2) {
					indices.Add(j);
					indices.Add(j + 1);
					indices.Add(j + 2);
					indices.Add(j + 1);
					indices.Add(j + 3);
					indices.Add(j + 2);
				}
			}
		}

		private void BuildCorner(Platform pf, Corner corner)
		{
		}

		private void GetEdge(Platform pf, List<SubEdge> subEdges, List<Corner> corners)
		{
			if (pf.Path.Count < 2)
				return;
			var subEdge = new SubEdge { Side = GetSide(pf.Path[1].Pos - pf.Path[0].Pos) };
			var numPoints = pf.Closed ? pf.Path.Count + 1 : pf.Path.Count;
			for (var i = 0; i < numPoints; i++) {
				Platform.PathNode cur = null;
				Platform.PathNode prev = null;
				Platform.PathNode next = null;
				if (pf.Closed) {
					cur = pf.Path[i % pf.Path.Count];
					prev = pf.Path[(i - 1 + pf.Path.Count) % pf.Path.Count];
					next = pf.Path[(i + 1 + pf.Path.Count) % pf.Path.Count];
				} else {
					cur = pf.Path[i];
					if (i > 0)
						prev = pf.Path[i - 1];
					if (i < pf.Path.Count - 1)
						next = pf.Path[i + 1];
				}
				if (prev == null || next == null) {
					Vector2 norm;
					if (prev == null)
						norm = Utility.Orthogonal(next.Pos - cur.Pos).normalized;
					else
						norm = Utility.Orthogonal(cur.Pos - prev.Pos).normalized;
					norm *= cur.Scale * pf.Thickness * 0.5f;
					subEdge.OuterVerts.Add(cur.Pos + norm);
					subEdge.InnerVerts.Add(cur.Pos - norm);
				} else {
					var dirIn = (cur.Pos - prev.Pos).normalized;
					var dirOut = (next.Pos - cur.Pos).normalized;
					var miter = Utility.Miter(dirIn, dirOut) * cur.Scale * pf.Thickness * 0.5f;
					var side = GetSide(dirOut);
					if (side == subEdge.Side) {
						subEdge.OuterVerts.Add(cur.Pos + miter);
						subEdge.InnerVerts.Add(cur.Pos - miter);
					} else {
						var subEdgeNext = new SubEdge { Side = side };
						var normalIn = Utility.Orthogonal(dirIn) * cur.Scale * pf.Thickness * 0.5f;
						var normalOut = Utility.Orthogonal(dirOut) * cur.Scale * pf.Thickness * 0.5f;
						var ivCur = subEdge.InnerVerts;
						var ovCur = subEdge.OuterVerts;
						var ivNext = subEdgeNext.InnerVerts;
						var ovNext = subEdgeNext.OuterVerts;
						var concaveCorner = Utility.Cross(dirIn, dirIn + dirOut) > 0.0f;
						if (concaveCorner) {
							ivCur = subEdge.OuterVerts;
							ovCur = subEdge.InnerVerts;
							ivNext = subEdgeNext.OuterVerts;
							ovNext = subEdgeNext.InnerVerts;
							miter = -miter;
							normalIn = -normalIn;
							normalOut = -normalOut;
						}
						var a = cur.Pos - miter;
						var b = a + normalIn * 2;
						var c = cur.Pos + miter;
						var d = a + normalOut * 2;
						var sharpCorner = Vector2.Dot(-dirIn, dirOut) > 0.0f;
						if (sharpCorner && pf.CorrectSharpCorners)
							c = (b + d) * 0.5f + Utility.Orthogonal(b - d) * 0.5f;
						corners.Add(new Corner {
							A = a,
							B = b,
							C = c,
							D = d,
							Concave = concaveCorner,
							SideFrom = subEdge.Side,
							SideTo = side
						});
						ivCur.Add(a);
						ovCur.Add(b);
						ivNext.Add(a);
						ovNext.Add(d);
						subEdges.Add(subEdge);
						subEdge = subEdgeNext;
					}
				}
			}
			if (pf.Closed && subEdges.Count > 0) {
				var firstSubEdge = subEdges[0];
				firstSubEdge.InnerVerts.RemoveAt(0);
				firstSubEdge.OuterVerts.RemoveAt(0);
				firstSubEdge.InnerVerts.InsertRange(0, subEdge.InnerVerts);
				firstSubEdge.OuterVerts.InsertRange(0, subEdge.OuterVerts);
			} else {
				subEdges.Add(subEdge);
			}
		}

		private static Side GetSide(Vector2 dir)
		{
			if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
				return dir.x > 0.0f ? Side.Top : Side.Bottom;
			else
				return dir.y > 0.0f ? Side.Left : Side.Right;
		}

		private class SubEdge
		{
			public List<Vector2> InnerVerts = new List<Vector2>();
			public List<Vector2> OuterVerts = new List<Vector2>();
			public Side Side;
		}

		private class Corner
		{
			public Vector2 A;
			public Vector2 B;
			public Vector2 C;
			public Vector2 D;
			public bool Concave;
			public Side SideFrom;
			public Side SideTo;
		}
	}
}
