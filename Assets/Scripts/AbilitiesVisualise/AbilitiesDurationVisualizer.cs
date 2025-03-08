/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

Please view README file for detailed information

*/
﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

/*
    Visual feedback for the boost that a player has picked up
 */

public class AbilitiesDurationVisualizer : MonoBehaviour
{
    private const float UpdateInterval = 0.1f;

    [SerializeField]
    private TextMeshProUGUI _textMesh;

    [SerializeField]
    private Tag _effectTag;

    private AbilitySystemComponent _asc;

    private float _toNextUpdate;

    #region Unity Messages

    private void Awake()
    {
    }

    private void Start()
    {
        var player = GameController.ActiveInstance.Player;

        _asc = player.GetComponent<AbilitySystemComponent>();
    }

    private void LateUpdate()
    {
        _toNextUpdate -= Time.deltaTime;

        if (_toNextUpdate > 0)
            return;

        _toNextUpdate += UpdateInterval;

        var effectInfos = _asc.GetActiveEffectsInfoByTag(_effectTag);

        _textMesh.text = GenerateDisplayText(effectInfos);
    }

    #endregion Unity Messages

    private static string GenerateDisplayText(IEnumerable<EffectInfo> effectInfos)
    {
        var attributeDataMap = GameData.Instance.AttributeDataMap;
        var effectInfoArray = effectInfos.ToArray();

        var sb = new StringBuilder(256);

        foreach (var effectInfo in effectInfoArray)
        {
            var effectName = string.IsNullOrWhiteSpace(effectInfo.Effect.DisplayName)
                ? effectInfo.Effect.name
                : effectInfo.Effect.DisplayName;

            sb.Append("<b>");
            sb.Append(effectName);
            sb.Append(" - ");
            sb.Append(effectInfo.TimeLeft.ToString("F2", CultureInfo.InvariantCulture));
            sb.Append(" ");
            sb.Append("[");
            sb.Append(effectInfo.Stacks);
            sb.Append("]");
            sb.Append("</b>");
            sb.AppendLine();

            foreach (var attributeModifierInfo in effectInfo.AttributeModifiers)
            {
                var attribute = attributeModifierInfo.Attribute;
                var delta = attributeModifierInfo.Value;

                sb.Append("\t");

                if (delta < 0f)
                    sb.Append("-");
                else if (delta > 0f)
                    sb.Append("+");

                if (attribute.IsScaleAttribute())
                {
                    sb.Append(Mathf.Abs(delta * 100f).ToString("0.##", CultureInfo.InvariantCulture));
                    sb.Append("%");
                }
                else
                {
                    sb.Append(Mathf.Abs(delta).ToString("0.##", CultureInfo.InvariantCulture));
                }

                var attributeDisplayName = attributeDataMap.TryGetValue(attribute, out var attributeData) ? attributeData.DisplayName : attribute.GetName();

                sb.Append(" ");
                sb.Append(attributeDisplayName);
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }
}