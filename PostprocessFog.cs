using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostprocessFog : PostEffectBase {
	[SerializeField] Color fogColor;
	[SerializeField] float fogStart;
	[SerializeField, Range(0, 10)] float fogEnd;
	[SerializeField, Range(0, 1)] float fogDensity;

	private void OnEnable() {
		Camera.main.depthTextureMode |= DepthTextureMode.Depth;
	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
        {
			Matrix4x4 frustumCorners = Matrix4x4.identity;
			float fov = Camera.main.fieldOfView;
			float near = Camera.main.nearClipPlane;
			float far = Camera.main.farClipPlane;
			float aspect = Camera.main.aspect;

			float halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
			Vector3 toRight = Camera.main.transform.right * halfHeight * aspect;
			Vector3 toTop = Camera.main.transform.up * halfHeight;

			Vector3 topLeft = Camera.main.transform.forward * near + toTop - toRight;
			float scale = topLeft.magnitude / near;

			topLeft.Normalize();
			topLeft *= scale;

			Vector3 topRight = Camera.main.transform.forward * near + toRight + toTop;
			topRight.Normalize();
			topRight *= scale;

			Vector3 bottomLeft = Camera.main.transform.forward * near - toTop - toRight;
			bottomLeft.Normalize();
			bottomLeft *= scale;

			Vector3 bottomRight = Camera.main.transform.forward * near + toRight - toTop;
			bottomRight.Normalize();
			bottomRight *= scale;

			frustumCorners.SetRow(0, bottomLeft);
			frustumCorners.SetRow(1, bottomRight);
			frustumCorners.SetRow(2, topRight);
			frustumCorners.SetRow(3, topLeft);

			material.SetMatrix("_FrustumCornersRay", frustumCorners);
			material.SetMatrix("_ViewProjectionInverseMatrix", (Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix).inverse);

			material.SetColor("_FogColor", fogColor);
			material.SetFloat("_FogStart", fogStart);
			material.SetFloat("_FogEnd", fogEnd);
			material.SetFloat("_FogDensity", fogDensity);

            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private void Update()
    {
        // blendOpacity = Mathf.Clamp(blendOpacity, 0.0f, 1.0f);
    }

}
