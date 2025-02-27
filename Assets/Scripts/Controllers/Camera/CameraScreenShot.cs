﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Controllers.Camera
{
    /// <summary>
    ///     截屏
    /// </summary>
    public class CameraScreenShot : MonoBehaviour
    {
        /// <summary>
        ///     顶层纹理
        /// </summary>
        public Texture mixTexture;

        /// <summary>
        ///     混合Shader
        /// </summary>
        public Shader mixShader;

        /// <summary>
        ///     基础附加颜色
        /// </summary>
        public Color baseColor;

        /// <summary>
        ///     顶层附加颜色
        /// </summary>
        public Color mixColor;

        /// <summary>
        ///     不想被看到的UI
        /// </summary>
        public List<GameObject> unusedUis;

        /// <summary>
        ///     截屏
        /// </summary>
        public Texture2D screenShot;

        /// <summary>
        ///     截屏合成结果
        /// </summary>
        public Texture2D renderResultTex;

        /// <summary>
        ///     截屏是否完成
        /// </summary>
        public bool isShotDone;

        private UnityEngine.Camera _camera;
        private Material           _mixMaterial;

        private Material MixMaterial
        {
            get
            {
                if (_mixMaterial == null) _mixMaterial = GenerateMaterial(mixShader, ref _mixMaterial);
                ;

                return _mixMaterial;
            }
        }

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
        }

        private Material GenerateMaterial(Shader shader, ref Material targetMaterial)
        {
            Material nullMat = null;
            if (shader != null)
                if (shader.isSupported)
                {
                    if (targetMaterial == null) targetMaterial = new Material(shader);
                    return targetMaterial;
                }

            return nullMat;
        }


        /// <summary>
        ///     捕捉相机
        /// </summary>
        /// <param name="rect">捕捉范围</param>
        /// <returns></returns>
        public Texture2D CaptureCamera(Rect rect)
        {
            // 创建一个RenderTexture对象  
            // 先创建一个的空纹理，大小可根据实现需要来设置  
            var screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);

            // 读取屏幕像素信息并存储为纹理数据，  
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            // 然后将这些纹理数据，成一个png图片文件  
            var bytes    = screenShot.EncodeToPNG();
            var filename = Application.streamingAssetsPath + "/Screenshot.png";
            File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("截屏了一张图片: {0}", filename));

            // 最后，我返回这个Texture2d对象，这样我们直接，所这个截图图示在游戏中，当然这个根据自己的需求的。  
            return screenShot;
        }


        /// <summary>
        ///     异步获取截屏
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetCaptureScreen()
        {
            unusedUis.ForEach(g => g.SetActive(false));
            //等待所有的摄像机和GUI被渲染完成。
            yield return new WaitForEndOfFrame();

            //创建一个空纹理（图片大小为屏幕的宽高）
            var tex = new Texture2D(Screen.width, Screen.height);
            //只能在帧渲染完毕之后调用（从屏幕左下角开始绘制，绘制大小为屏幕的宽高，宽高的偏移量都为0）
            tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            //图片应用（此时图片已经绘制完成）
            tex.Apply();
            screenShot = tex;
            var renderResult = RenderTexture.GetTemporary(Screen.width, Screen.height);
            if (MixMaterial != null)
            {
                // Debug.Log(renderTexOuter.GetRenderResult());
                MixMaterial.SetTexture("_BaseMap", screenShot);
                MixMaterial.SetTexture("_MixMap",  mixTexture);
                // MixMaterial.SetTexture("_MixTex1", layerCamera.GetRenderResult());
                MixMaterial.SetColor("_BaseColor", baseColor);
                MixMaterial.SetColor("_MixColor",  mixColor);
                // MixMaterial.SetTexture("MixTex", renderTexture);

                Graphics.Blit(screenShot, renderResult, MixMaterial);
            }
            else
            {
                Graphics.Blit(screenShot, renderResult);
            }

            isShotDone = true;
            //将图片装换成jpg的二进制格式，保存在byte数组中（计算机是以二进制的方式存储数据）

            var renderResultTex2D = new Texture2D(renderResult.width, renderResult.height, TextureFormat.ARGB32, false);
            RenderTexture.active = renderResult;
            renderResultTex2D.ReadPixels(new Rect(0, 0, renderResult.width, renderResult.height), 0, 0);
            renderResultTex2D.Apply();
            renderResultTex = renderResultTex2D;
            var result = renderResultTex2D.EncodeToJPG();
            //文件保存，创建一个新文件，在其中写入指定的字节数组（要写入的文件的路径，要写入文件的字节。）
            File.WriteAllBytes(Application.streamingAssetsPath + "/ScreenShot.JPG", result);
            unusedUis.ForEach(g => g.SetActive(true));
        }
    }
}