using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScreenShot : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = this.GetComponent<Camera>();
    }
    
    public Texture2D CaptureCamera(Rect rect)   
    {  
        // 创建一个RenderTexture对象  
        // 先创建一个的空纹理，大小可根据实现需要来设置  
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);  
  
        // 读取屏幕像素信息并存储为纹理数据，  
        screenShot.ReadPixels(rect, 0, 0);  
        screenShot.Apply();  
  
        // 然后将这些纹理数据，成一个png图片文件  
        byte[] bytes    = screenShot.EncodeToPNG();  
        string filename = Application.streamingAssetsPath + "/Screenshot.png";  
        System.IO.File.WriteAllBytes(filename, bytes);  
        Debug.Log(string.Format("截屏了一张图片: {0}", filename));  
  
        // 最后，我返回这个Texture2d对象，这样我们直接，所这个截图图示在游戏中，当然这个根据自己的需求的。  
        return screenShot;      
    }

    public Texture2D tex        = new Texture2D (Screen.width, Screen.height);
    public bool      isShotDone = false;
    public IEnumerator MyCaptureScreen()
    {
        //等待所有的摄像机和GUI被渲染完成。
        yield return new WaitForEndOfFrame ();
        //创建一个空纹理（图片大小为屏幕的宽高）
        Texture2D tex = new Texture2D (Screen.width, Screen.height);
        //只能在帧渲染完毕之后调用（从屏幕左下角开始绘制，绘制大小为屏幕的宽高，宽高的偏移量都为0）
        tex.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0);
        //图片应用（此时图片已经绘制完成）
        tex.Apply ();
        this.tex   = tex;
        isShotDone = true;
        //将图片装换成jpg的二进制格式，保存在byte数组中（计算机是以二进制的方式存储数据）
        byte[] result = tex.EncodeToJPG ();
        //文件保存，创建一个新文件，在其中写入指定的字节数组（要写入的文件的路径，要写入文件的字节。）
        System.IO.File.WriteAllBytes (Application.streamingAssetsPath +"/ScreenShot.JPG", result);
    }
}
