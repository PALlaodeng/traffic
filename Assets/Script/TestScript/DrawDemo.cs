using System.Collections.Generic;
using UnityEngine;

public class DrawDemo : MonoBehaviour
{
    /// <summary>
    /// 路线绘制粗细，数值越大则越粗
    /// </summary>
    [Range(1, 10)]
    public int painterWide = 1;
    /// <summary>
    /// 路线绘制线条颜色
    /// </summary>
    public Color painterColor = Color.black;

    /// <summary>
    /// 鼠标单次绘制轨迹点列表
    /// </summary>
    private List<Vector2> mouseTrackPoints;
    /// <summary>
    /// 绘制的轨迹集合
    /// </summary>
    private Dictionary<int, List<Vector2>> trackDic;

    /// <summary>
    /// 绘制显示区域的UITexture组件
    /// </summary>
    public UITexture DrawAreaTexture;
    /// <summary>
    /// 绘制显示区域的material组件
    /// </summary>
    private Material drawAreaMaterial;

    /// <summary>
    /// 中间生成的纹理
    /// </summary>
    private Texture2D tempTex;
    /// <summary>
    /// 中间生成的材质
    /// </summary>
    private Material tempMaterial;

    /// <summary>
    /// 显示区域的宽度
    /// </summary>
    private int showAreaWidth = 1024;
    /// <summary>
    /// 显示区域的高度
    /// </summary>
    private int showAreaHeight = 766;


    void Start()
    {
        mouseTrackPoints = new List<Vector2>();
        trackDic = new Dictionary<int, List<Vector2>>();
        drawAreaMaterial = DrawAreaTexture.material;
    }

    void CreateLineMaterial()
    {
        if (!tempMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            tempMaterial = new Material(shader);
            tempMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            tempMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            tempMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            tempMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            tempMaterial.SetInt("_ZWrite", 0);
        }
    }

    /// <summary>
    /// 绘制方法
    /// </summary>
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        tempMaterial.SetPass(0);
        GL.PushMatrix();
        // 更改为正交投影
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(painterColor);
        // 连线

        Vector2 frontPoint;
        Vector2 backPoint;
        foreach (var line in trackDic.Values)
        {
            for (int i = 0; i < line.Count-1; i++)
            {
                frontPoint = line[i];
                backPoint = line[i + 1];
                GL.Vertex3(frontPoint.x, frontPoint.y, 0);
                GL.Vertex3(backPoint.x, backPoint.y, 0);
            }
        }
        GL.End();
        GL.PopMatrix();
    }

    /// <summary>
    /// 生成中间纹理
    /// </summary>
    void CreatTexture()
    {
        tempTex = new Texture2D(showAreaWidth, showAreaHeight);

        // 设置每个点的像素
        Vector2 frontPoint;
        Vector2 backPoint;
        foreach (var line in trackDic.Values)
        {
            for (int i = 0; i < line.Count - 1; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    frontPoint = line[i];
                    backPoint = line[i + 1];
                    float scaleX = Mathf.Lerp(frontPoint.x, backPoint.x, j / 100f);
                    float scaleY = Mathf.Lerp(frontPoint.y, backPoint.y, j / 100f);
                    int textureX = (int)(scaleX * showAreaWidth);
                    int textureY = (int)(scaleY * showAreaHeight);
                    // 线条加粗
                    for (int a = textureX - painterWide; a < textureX + painterWide; a++)
                    {
                        for (int b = textureY - painterWide; b < textureY + painterWide; b++)
                        {
                            tempTex.SetPixel(a, b, painterColor);
                        }
                    }
                }
            }
        }
        
        tempTex.Apply();
        drawAreaMaterial.SetTexture("_MainTex", tempTex);
    }

    void Update()
    {
        // 按下鼠标记录鼠标位置
        if (Input.GetMouseButton(0))
        {
            Vector2 addPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            // 限制输入区域
            if (addPoint.x < 0.025f ||
                addPoint.x > 0.975f ||
                addPoint.y < 0.03f ||
                addPoint.y > 0.42f)
            {
                mouseTrackPoints.Clear();
            }
            else
            {
                mouseTrackPoints.Add(addPoint);
            }
        }
        // 抬起鼠标清空屏幕，并投影到对象上
        if (Input.GetMouseButtonUp(0))
        {
            trackDic.Add(trackDic.Keys.Count + 1, mouseTrackPoints);
            mouseTrackPoints = new List<Vector2>();
            CreatTexture();
        }

    }
}