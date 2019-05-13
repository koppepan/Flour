using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Flour.UI
{
	public class TiltComponent : BaseMeshEffect
	{
		[SerializeField, Range(-75, 75)]
		public int angle = 0;

		public override void ModifyMesh(Mesh in_mesh)
		{
			if (!isActiveAndEnabled)
			{
				return;
			}

			List<UIVertex> list = new List<UIVertex>();
			using (VertexHelper vertexHelper = new VertexHelper(in_mesh))
			{
				vertexHelper.GetUIVertexStream(list);
			}

			this.ModifyVertices(list);

			using (VertexHelper vertexHelper2 = new VertexHelper())
			{
				vertexHelper2.AddUIVertexTriangleStream(list);
				vertexHelper2.FillMesh(in_mesh);
			}

		}

		public override void ModifyMesh(VertexHelper in_vh)
		{
			if (!isActiveAndEnabled)
			{
				return;
			}

			List<UIVertex> vertexList = new List<UIVertex>();
			in_vh.GetUIVertexStream(vertexList);

			ModifyVertices(vertexList);

			in_vh.Clear();
			in_vh.AddUIVertexTriangleStream(vertexList);
		}

		private void ModifyVertices(List<UIVertex> in_vList)
		{
			if (in_vList == null || in_vList.Count == 0)
			{
				return;
			}

			var vec = in_vList[1].position - in_vList[0].position;
			float fac = vec.y * Mathf.Tan(Mathf.Deg2Rad * this.angle);

			List<Vector3> v = new List<Vector3>
			{
				Vector3.zero,
				Vector3.right * fac,
				Vector3.right * fac,
				Vector3.right * fac,
				Vector3.zero,
				Vector3.zero
			};

			for (int i = 0; i < in_vList.Count; i++)
			{
				UIVertex tmpV = in_vList[i];
				tmpV.position += v[i];
				in_vList[i] = tmpV;
			}
		}
	}
}