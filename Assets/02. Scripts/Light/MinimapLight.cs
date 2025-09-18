using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapLight : MonoBehaviour
{
    public RawImage fogImage;   // �̴ϸ����� ���� �̹���
    public Transform player;
    public int mapWidth;
    public int mapHeight;
    public int lightRadius; // �÷��̾� �ֺ� �� ����

    private Texture2D fogTexture;   // �ȼ� ������ fog������ Texture2D

    private void Start()
    {
        fogTexture = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, false);        // fogTexture ����� RGBA32�� ���� ����
        Color[] colors = new Color[mapWidth * mapHeight];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.black;

        fogTexture.SetPixels(colors);   // �ȼ������� colors ���� ����
        fogTexture.Apply();

        fogImage.texture = fogTexture;
        fogImage.color = Color.white;
    }

    private void Update()
    {
        Vector2 texPos = TexturePosition(player.position);

        for (int x = -lightRadius; x <= lightRadius; x++)   // �÷��̾� �ֺ� ����
        {
            for (int y = -lightRadius; y <= lightRadius; y++)
            {
                int px = (int)texPos.x + x;
                int py = (int)texPos.y + y;
                if (px >= 0 && px < mapWidth && py >= 0 && py < mapHeight)  // �ؽ��� �� �ȼ� �ǵ帮�� �ʰ�
                    fogTexture.SetPixel(px, py, Color.clear);           // ����ó��

            }
        }

        fogTexture.Apply(); // ���� ����� �ؽ��� �ݿ�
    }

    Vector2 TexturePosition(Vector3 worldPosition)
    {
        // ���� ��ǥ�� �ؽ��� ��ǥ�� ��ȯ
        float tx = Mathf.InverseLerp(-25, 25, worldPosition.x) * mapWidth;  // worldPosition.x�� -50 ~ 50 ���̿��� 0~1�� ����, �ʱ��� ���ؼ� �ȼ���ǥ�� ��ȯ
        float ty = Mathf.InverseLerp(-25, 25, worldPosition.y) * mapHeight;
        return new Vector2 (tx, ty);
    }
}
