using UnityEngine;

namespace FischlWorks_FogWar
{
    /// <summary>
    /// �ν����Ϳ��� Ư�� bool �ʵ尡 true�� ���� �ش� ������Ƽ�� �����մϴ�.
    /// ����: [ShowIf("_someBoolField")]
    /// </summary>
    public sealed class ShowIfAttribute : PropertyAttribute
    {
        public readonly string _BaseCondition;
        public ShowIfAttribute(string baseCondition) { _BaseCondition = baseCondition; }
    }

    /// <summary>
    /// �ν����Ϳ� ū ���(���� Ÿ��Ʋ)�� ǥ���մϴ�.
    /// ����: [BigHeader("Section Title")]
    /// </summary>
    public sealed class BigHeaderAttribute : PropertyAttribute
    {
        public readonly string _Text;
        public BigHeaderAttribute(string text) { _Text = text; }
    }
}
