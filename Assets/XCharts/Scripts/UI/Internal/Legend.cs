﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XCharts
{
    [System.Serializable]
    public class Legend : JsonDataSupport, IPropertyChanged, IEquatable<Legend>
    {
        [SerializeField] private bool m_Show = true;
        [SerializeField] private Orient m_Orient = Orient.Horizonal;
        [SerializeField] private Location m_Location = Location.defaultRight;
        [SerializeField] private float m_ItemWidth = 50.0f;
        [SerializeField] private float m_ItemHeight = 20.0f;
        [SerializeField] private float m_ItemGap = 5;
        [SerializeField] private int m_ItemFontSize = 18;
        [SerializeField] private List<string> m_Data = new List<string>();

        [NonSerialized] private List<bool> m_DataActiveList = new List<bool>();
        [NonSerialized] private List<Button> m_DataBtnList = new List<Button>();

        public bool show { get { return m_Show; } set { m_Show = value; } }

        public Orient orient { get { return m_Orient; } set { m_Orient = value; } }

        public Location location { get { return m_Location; } set { m_Location = value; } }

        public float itemWidth { get { return m_ItemWidth; } set { m_ItemWidth = value; } }

        public float itemHeight { get { return m_ItemHeight; } set { m_ItemHeight = value; } }

        public float itemGap { get { return m_ItemGap; } set { m_ItemGap = value; } }

        public int itemFontSize { get { return m_ItemFontSize; } set { m_ItemFontSize = value; } }

        public List<string> data { get { return m_Data; } }

        public static Legend defaultLegend
        {
            get
            {
                var legend = new Legend
                {
                    m_Show = true,
                    m_Orient = Orient.Horizonal,
                    m_Location = Location.defaultRight,
                    m_ItemWidth = 60.0f,
                    m_ItemHeight = 20.0f,
                    m_ItemGap = 5,
                    m_ItemFontSize = 16,
                    m_Data = new List<string>()
                    {
                        "Legend"
                    }
                };
                return legend;
            }
        }
        public void Copy(Legend legend)
        {
            m_Show = legend.show;
            m_Orient = legend.orient;
            m_Location.Copy(legend.location);
            m_ItemWidth = legend.itemWidth;
            m_ItemHeight = legend.itemHeight;
            m_ItemGap = legend.itemGap;
            m_ItemFontSize = legend.itemFontSize;
            m_Data.Clear();
            foreach (var d in legend.data) m_Data.Add(d);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Legend)) return false;
            return Equals((Legend)obj);
        }

        public bool Equals(Legend other)
        {
            return show == other.show &&
                orient == other.orient &&
                location == other.location &&
                itemWidth == other.itemWidth &&
                itemHeight == other.itemHeight &&
                itemGap == other.itemGap &&
                itemFontSize == other.itemFontSize &&
                ChartHelper.IsValueEqualsList<string>(m_Data, other.data);
        }

        public static bool operator ==(Legend point1, Legend point2)
        {
            return point1.Equals(point2);
        }

        public static bool operator !=(Legend point1, Legend point2)
        {
            return !point1.Equals(point2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool IsActive(string name)
        {
            if (string.IsNullOrEmpty(name)) return true;
            for(int i = 0; i < data.Count; i++)
            {
                if (data[i].Equals(name)) return m_DataActiveList[i];
            }
            return false;
        }

        public bool IsActive(int seriesIndex)
        {
            if (seriesIndex < 0 || seriesIndex > data.Count - 1) seriesIndex = 0;
            if (seriesIndex >= data.Count) return true;
            if (seriesIndex < 0 || seriesIndex > m_DataActiveList.Count - 1)
                return true;
            else
                return m_DataActiveList[seriesIndex];
        }

        public void ClearData()
        {
            m_Data.Clear();
        }

        public bool ContainsData(string name)
        {
            return m_Data.Contains(name);
        }

        public void RemoveData(string name)
        {
            if (m_Data.Contains(name))
            {
                m_Data.Remove(name);
            }
        }

        public void AddData(string name)
        {
            if (!m_Data.Contains(name))
            {
                m_Data.Add(name);
            }
        }

        public void SetActive(int index, bool flag)
        {
            m_DataActiveList[index] = flag;
        }

        public void SetActive(string name, bool flag)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Equals(name))
                {
                    m_DataActiveList[i] = flag;
                    break;
                }
            }
        }

        public void SetButton(int index, Button btn)
        {
            btn.transform.localPosition = GetButtonLocationPosition(index);
            if (index < 0 || index > m_DataBtnList.Count - 1)
            {
                m_DataBtnList.Add(btn);
                m_DataActiveList.Add(true);
            }
            else
            {
                m_DataBtnList[index] = btn;
            }
            btn.gameObject.SetActive(show);
            btn.GetComponentInChildren<Text>().text = data[index];
        }

        public void UpdateButtonColor(int index,Color ableColor,Color unableColor)
        {
            if (IsActive(index))
            {
                m_DataBtnList[index].GetComponent<Image>().color = ableColor;
            }
            else
            {
                m_DataBtnList[index].GetComponent<Image>().color = unableColor;
            }
        }

        public void OnChanged()
        {
            m_Location.OnChanged();
        }

        private Vector2 GetButtonLocationPosition(int index)
        {
            int size = m_Data.Count;
            switch (m_Orient)
            {
                case Orient.Vertical:
                    switch (m_Location.align)
                    {
                        case Location.Align.TopCenter:
                        case Location.Align.TopLeft:
                        case Location.Align.TopRight:
                            return new Vector2(0, -index * (itemHeight + itemGap));

                        case Location.Align.Center:
                        case Location.Align.CenterLeft:
                        case Location.Align.CenterRight:
                            float totalHeight = size * itemHeight + (size - 1) * itemGap;
                            float startY = totalHeight / 2;
                            return new Vector2(0, startY - index * (itemHeight + itemGap));

                        case Location.Align.BottomCenter:
                        case Location.Align.BottomLeft:
                        case Location.Align.BottomRight:
                            return new Vector2(0, (size - index - 1) * (itemHeight + itemGap));
                    }
                    return Vector2.zero;

                case Orient.Horizonal:
                    switch (m_Location.align)
                    {
                        case Location.Align.TopLeft:
                        case Location.Align.CenterLeft:
                        case Location.Align.BottomLeft:
                            return new Vector2(index * (itemWidth + itemGap), 0);

                        case Location.Align.TopCenter:
                        case Location.Align.Center:
                        case Location.Align.BottomCenter:
                            float totalWidth = size * itemWidth + (size - 1) * itemGap;
                            float startX = totalWidth / 2;
                            return new Vector2(-startX + itemWidth / 2 + index * (itemWidth + itemGap), 0);
                        case Location.Align.TopRight:
                        case Location.Align.CenterRight:
                        case Location.Align.BottomRight:
                            return new Vector2(-(size - index - 1) * (itemWidth + itemGap), 0);
                    }
                    return Vector2.zero;
            }
            return Vector2.zero;
        }

        public override void ParseJsonData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData) || !m_DataFromJson) return;
            m_Data = ChartHelper.ParseStringFromString(jsonData);
        }
    }
}
