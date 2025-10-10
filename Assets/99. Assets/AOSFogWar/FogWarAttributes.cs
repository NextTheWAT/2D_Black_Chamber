using UnityEngine;

namespace FischlWorks_FogWar
{
    /// <summary>
    /// 인스펙터에서 특정 bool 필드가 true일 때만 해당 프로퍼티를 노출합니다.
    /// 사용법: [ShowIf("_someBoolField")]
    /// </summary>
    public sealed class ShowIfAttribute : PropertyAttribute
    {
        public readonly string _BaseCondition;
        public ShowIfAttribute(string baseCondition) { _BaseCondition = baseCondition; }
    }

    /// <summary>
    /// 인스펙터에 큰 헤더(섹션 타이틀)를 표시합니다.
    /// 사용법: [BigHeader("Section Title")]
    /// </summary>
    public sealed class BigHeaderAttribute : PropertyAttribute
    {
        public readonly string _Text;
        public BigHeaderAttribute(string text) { _Text = text; }
    }
}
