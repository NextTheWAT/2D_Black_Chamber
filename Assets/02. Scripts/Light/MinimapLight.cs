using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapLight : MonoBehaviour
{
    public RawImage fogImage;   // 미니맵위에 덮을 이미지
    public Transform player;
    public int mapWidth;
    public int mapHeight;
    public int lightRadius; // 플레이어 주변 빛 범위

    private Texture2D fogTexture;   // 픽셀 단위로 fog조절할 Texture2D

    private void Start()
    {
        fogTexture = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, false);        // fogTexture 만들기 RGBA32로 알파 지원
        Color[] colors = new Color[mapWidth * mapHeight];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.black;

        fogTexture.SetPixels(colors);   // 픽셀단위로 colors 색상 적용
        fogTexture.Apply();

        fogImage.texture = fogTexture;
        fogImage.color = Color.white;
    }

    private void Update()
    {
        Vector2 texPos = TexturePosition(player.position);

        for (int x = -lightRadius; x <= lightRadius; x++)   // 플레이어 주변 영역
        {
            for (int y = -lightRadius; y <= lightRadius; y++)
            {
                int px = (int)texPos.x + x;
                int py = (int)texPos.y + y;
                if (px >= 0 && px < mapWidth && py >= 0 && py < mapHeight)  // 텍스쳐 밖 픽셀 건드리지 않게
                    fogTexture.SetPixel(px, py, Color.clear);           // 투명처리

            }
        }

        fogTexture.Apply(); // 투명 적용된 텍스쳐 반영
    }

    Vector2 TexturePosition(Vector3 worldPosition)
    {
        // 월드 좌표를 텍스쳐 좌표로 변환
        float tx = Mathf.InverseLerp(-25, 25, worldPosition.x) * mapWidth;  // worldPosition.x가 -50 ~ 50 사이에서 0~1로 매핑, 맵길이 곱해서 픽셀좌표로 변환
        float ty = Mathf.InverseLerp(-25, 25, worldPosition.y) * mapHeight;
        return new Vector2 (tx, ty);
    }
}
