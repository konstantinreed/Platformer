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
			var uvs = new List<Vector2>();
			var indices = new List<int>();
			var subEdges = new List<SubEdge>();
			var corners = new List<Corner>();
			GetEdge(pf, subEdges, corners);
			foreach (var se in subEdges)
				BuildSubEdge(pf, se, verts, uvs, indices);
			foreach (var c in corners)
				BuildCorner(pf, c);
			m.Clear();
			m.SetVertices(verts);
			m.SetUVs(0, uvs);
			m.SetTriangles(indices, 0);
			m.RecalculateNormals();
			m.RecalculateTangents();
			m.RecalculateBounds();
			mr.sharedMaterial = pf.Material.EdgeMaterial;
		}

		private void BuildSubEdge(Platform pf, SubEdge subEdge, List<Vector3> verts, List<Vector2> uvs, List<int> indices)
		{
			var dists = new float[subEdge.OuterVerts.Count];
			var texDists = new float[subEdge.OuterVerts.Count];
			dists[0] = texDists[0] = 0.0f;
			for (var i = 1; i < subEdge.OuterVerts.Count; i++) {
				var iv1 = subEdge.InnerVerts[i - 1];
				var iv2 = subEdge.InnerVerts[i];
				var ov1 = subEdge.OuterVerts[i - 1];
				var ov2 = subEdge.OuterVerts[i];
				var scale1 = subEdge.Scales[i - 1];
				var scale2 = subEdge.Scales[i];
				var v1 = (iv1 + ov1) * 0.5f;
				var v2 = (iv2 + ov2) * 0.5f;
				var dist = (v2 - v1).magnitude;
				dists[i] = dists[i - 1] + dist;
				texDists[i] = texDists[i - 1] + dist * SegmentToTexture(1.0f, scale1, scale2);
			}
			var atlas = pf.Material.EdgeMaterial.mainTexture;
			var atlasRegion = pf.Material.GetEdge(subEdge.Side);
			var uvRect = Rect.MinMaxRect(
				atlasRegion.xMin / atlas.width,
				atlasRegion.yMin / atlas.height,
				atlasRegion.xMax / atlas.width,
				atlasRegion.yMax / atlas.height);
			var hSubdivs = Mathf.Max(1, pf.HSubdivs);
			var vSubdivs = Mathf.Max(1, pf.VSubdivs);
			var totalTexLen = texDists[texDists.Length - 1] - texDists[0];
			var numSlices = Mathf.Max(1, Mathf.RoundToInt(totalTexLen * atlasRegion.height / atlasRegion.width));
			var subdivWidth = totalTexLen / (numSlices * hSubdivs);
			var iv = subEdge.InnerVerts[0];
			var ov = subEdge.OuterVerts[0];
			var curPt = 0;
			for (var i = 0; i < numSlices; i++) {
				var baseVert = verts.Count;
				for (var j = 0; j < hSubdivs; j++) {
					var subdivStart = (i * hSubdivs + j) * subdivWidth;
					var subdivEnd = subdivStart + subdivWidth;
					var subdivUVMin = new Vector2(uvRect.xMin + j * uvRect.width / hSubdivs, uvRect.yMin);
					var subdivUVMax = new Vector2(uvRect.xMin + (j + 1) * uvRect.width / hSubdivs, uvRect.yMax);
					var iuv = new Vector2(subdivUVMin.x, subdivUVMin.y);
					var ouv = new Vector2(subdivUVMin.x, subdivUVMax.y);
					EmitSubEdgeVerts(iv, ov, iuv, ouv, vSubdivs, verts, uvs);
					while (curPt < subEdge.OuterVerts.Count - 2 && subdivEnd > texDists[curPt + 1]) {
						curPt++;
						iv = subEdge.InnerVerts[curPt];
						ov = subEdge.OuterVerts[curPt];
						var ut = (texDists[curPt] - subdivStart) / subdivWidth;
						var u = (1.0f - ut) * subdivUVMin.x + ut * subdivUVMax.x;
						iuv = new Vector2(u, subdivUVMin.y);
						ouv = new Vector2(u, subdivUVMax.y);
						EmitSubEdgeVerts(iv, ov, iuv, ouv, vSubdivs, verts, uvs);
					}
					var vt = (subdivEnd - texDists[curPt]) / (dists[curPt + 1] - dists[curPt]);
					vt = TextureToSegment(vt, subEdge.Scales[curPt], subEdge.Scales[curPt + 1]);
					iv = (1.0f - vt) * subEdge.InnerVerts[curPt] + vt * subEdge.InnerVerts[curPt + 1];
					ov = (1.0f - vt) * subEdge.OuterVerts[curPt] + vt * subEdge.OuterVerts[curPt + 1];
					iuv = new Vector2(subdivUVMax.x, subdivUVMin.y);
					ouv = new Vector2(subdivUVMax.x, subdivUVMax.y);
					EmitSubEdgeVerts(iv, ov, iuv, ouv, vSubdivs, verts, uvs);
				}
				EmitSubEdgeIndices(baseVert, verts.Count, vSubdivs, indices);
			}
		}

		private void EmitSubEdgeVerts(Vector3 iv, Vector3 ov, Vector2 iuv, Vector2 ouv, int vSubdivs, List<Vector3> verts, List<Vector2> uvs)
		{
			var tDelta = 1.0f / vSubdivs;
			for (var i = 0; i <= vSubdivs; i++) {
				var t = i * tDelta;
				verts.Add((1.0f - t) * iv + t * ov);
				uvs.Add((1.0f - t) * iuv + t * ouv);
			}
		}

		private void EmitSubEdgeIndices(int startVert, int endVert, int vSubdivs, List<int> indices)
		{
			for (var i = startVert; i < endVert - 2 * vSubdivs - 1; i++) {
				for (var j = 0; j < vSubdivs; j++) {
					indices.Add(i);
					indices.Add(i + 1);
					indices.Add(i + vSubdivs + 1);
					indices.Add(i + 1);
					indices.Add(i + vSubdivs + 2);
					indices.Add(i + vSubdivs + 1);
					i++;
				}
			}
		}

		private float SegmentToTexture(float t, float scale1, float scale2)
		{
			// df(t) = (1 / ((1 - t) scale1 + t scale2)) dt
			// f(t) = ln((1 - t) scale1 + t scale2) / (scale2 - scale1)

			// f(t) - f(0) = (ln((1 - t) scale1 + t scale2) - ln(scale1)) / (scale2 - scale1)
			// f(t) - f(0) = ln(((1 - t) scale1 + t scale2) / scale1) / (scale2 - scale1)
			// f(t) - f(0) = ln((scale1 + (scale2 - scale1) t) / scale1) / (scale2 - scale1)

			var scaleDelta = scale2 - scale1;
			if (Mathf.Abs(scaleDelta) < Utility.ZeroTolerance)
				return t / scale1;
			return Mathf.Log((scale1 + scaleDelta * t) / scale1) / scaleDelta;
		}

		private float TextureToSegment(float t, float scale1, float scale2)
		{
			// f(t) = ln((scale1 + (scale2 - scale1) t) / scale1) / (scale2 - scale1)
			// f(t) (scale2 - scale1) = ln((scale1 + (scale2 - scale1) t) / scale1)
			// e^(f(t) (scale2 - scale1)) = (scale1 + (scale2 - scale1) t) / scale1
			// e^(f(t) (scale2 - scale1)) = 1 + (scale2 - scale1) t / scale1
			// e^(f(t) (scale2 - scale1)) - 1 = (scale2 - scale1) t / scale1
			// t = scale1 (e^f(t) (scale2 - scale1)] - 1) / (scale2 - scale1)

			var scaleDelta = scale2 - scale1;
			if (Mathf.Abs(scaleDelta) < Utility.ZeroTolerance)
				return t * scale1;
			return scale1 * (Mathf.Exp(t * scaleDelta) - 1) / scaleDelta;
		}

		private void BuildCorner(Platform pf, Corner corner)
		{
			var atlas = pf.Material.EdgeMaterial.mainTexture;
			var atlasRegion = pf.Material.GetCorner(corner.SideFrom, corner.SideTo);
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
					norm *= cur.Scale * 0.5f;
					subEdge.OuterVerts.Add(cur.Pos + norm);
					subEdge.InnerVerts.Add(cur.Pos - norm);
					subEdge.Scales.Add(cur.Scale);
				} else {
					var dirIn = (cur.Pos - prev.Pos).normalized;
					var dirOut = (next.Pos - cur.Pos).normalized;
					var miter = Utility.Miter(dirIn, dirOut) * cur.Scale * 0.5f;
					var side = GetSide(dirOut);
					if (side == subEdge.Side) {
						subEdge.OuterVerts.Add(cur.Pos + miter);
						subEdge.InnerVerts.Add(cur.Pos - miter);
						subEdge.Scales.Add(cur.Scale);
					} else {
						var subEdgeNext = new SubEdge { Side = side };
						var normalIn = Utility.Orthogonal(dirIn) * cur.Scale * 0.5f;
						var normalOut = Utility.Orthogonal(dirOut) * cur.Scale * 0.5f;
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
						subEdge.Scales.Add(cur.Scale);
						ivNext.Add(a);
						ovNext.Add(d);
						subEdges.Add(subEdge);
						subEdge = subEdgeNext;
						subEdge.Scales.Add(cur.Scale);
					}
				}
			}
			if (pf.Closed && subEdges.Count > 0) {
				var firstSubEdge = subEdges[0];
				firstSubEdge.InnerVerts.RemoveAt(0);
				firstSubEdge.OuterVerts.RemoveAt(0);
				firstSubEdge.Scales.RemoveAt(0);
				firstSubEdge.InnerVerts.InsertRange(0, subEdge.InnerVerts);
				firstSubEdge.OuterVerts.InsertRange(0, subEdge.OuterVerts);
				firstSubEdge.Scales.InsertRange(0, subEdge.Scales);
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
			public List<float> Scales = new List<float>();
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