using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponHoverData
{
    public string weaponName;

    [Header("������ �̸�")]
    public string scopeName;        // ������
    public string flashlightName;   // �÷��ö���Ʈ
    public string laserName;        // ����������Ʈ
    public string gripName;         // ������
    public string magazineName;     // źâ
    public string compensatorName;  // ������

    [Header("���� ����")]
    public string category;         // �з�
    public float damage;            // ������
    public float fireRate;          // �߻�ӵ�
    public int ammoCapacity;        // ��ź ��
    public float accuracy;          // ��Ȯ��
    public float noise;             // ���� ��ġ
    public float spread;            // ź ����
    public float recoilControl;     // �ݵ�����
    public float aimRange;          // ���� ��Ÿ�
    public float accuracyRecovery;  // ���� ������
    public float bulletSpeed;       // ź��
    public float reloadSpeed;       // �����ӵ�
    public float mobilityReduction; // �̵� ����
}
