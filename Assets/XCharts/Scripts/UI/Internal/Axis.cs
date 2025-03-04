﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace XCharts
{
    [System.Serializable]
    public class Axis : JsonDataSupport, IEquatable<Axis>
    {
        public enum AxisType
        {
            Value,
            Category,
            //Time,
            //Log
        }

        public enum AxisMinMaxType
        {
            Default,
            MinMax,
            Custom
        }

        public enum SplitLineType
        {
            None,
            Solid,
            Dashed,
            Dotted
        }

        [System.Serializable]
        public class AxisTick
        {
            [SerializeField] private bool m_Show;
            [SerializeField] private bool m_AlignWithLabel;
            [SerializeField] private bool m_Inside;
            [SerializeField] private float m_Length;

            public bool show { get { return m_Show; } set { m_Show = value; } }
            public bool alignWithLabel { get { return m_AlignWithLabel; } set { m_AlignWithLabel = value; } }
            public bool inside { get { return m_Inside; } set { m_Inside = value; } }
            public float length { get { return m_Length; } set { m_Length = value; } }

            public static AxisTick defaultTick
            {
                get
                {
                    var tick = new AxisTick
                    {
                        m_Show = true,
                        m_AlignWithLabel = false,
                        m_Inside = false,
                        m_Length = 5f
                    };
                    return tick;
                }
            }
        }

        [SerializeField] protected bool m_Show = true;
        [SerializeField] protected AxisType m_Type;
        [SerializeField] protected AxisMinMaxType m_MinMaxType;
        [SerializeField] protected int m_Min;
        [SerializeField] protected int m_Max;
        [SerializeField] protected int m_SplitNumber = 5;
        [SerializeField] protected int m_TextRotation = 0;
        [SerializeField] protected bool m_ShowSplitLine = false;
        [SerializeField] protected SplitLineType m_SplitLineType = SplitLineType.Dashed;
        [SerializeField] protected bool m_BoundaryGap = true;
        [SerializeField] protected List<string> m_Data = new List<string>();
        [SerializeField] protected AxisTick m_AxisTick = AxisTick.defaultTick;

        public bool show { get { return m_Show; } set { m_Show = value; } }
        public AxisType type { get { return m_Type; } set { m_Type = value; } }
        public AxisMinMaxType minMaxType { get { return m_MinMaxType; } set { m_MinMaxType = value; } }
        public int min { get { return m_Min; } set { m_Min = value; } }
        public int max { get { return m_Max; } set { m_Max = value; } }
        public int splitNumber { get { return m_SplitNumber; } set { m_SplitNumber = value; } }
        public int textRotation { get { return m_TextRotation; } set { m_TextRotation = value; } }
        public bool showSplitLine { get { return m_ShowSplitLine; } set { m_ShowSplitLine = value; } }
        public SplitLineType splitLineType { get { return m_SplitLineType; } set { m_SplitLineType = value; } }
        public bool boundaryGap { get { return m_BoundaryGap; } set { m_BoundaryGap = value; } }
        public List<string> data { get { return m_Data; } }
        public AxisTick axisTick { get { return m_AxisTick; } set { m_AxisTick = value; } }

        public int filterStart { get; set; }
        public int filterEnd { get; set; }
        public List<string> filterData { get; set; }

        public void Copy(Axis other)
        {
            m_Show = other.show;
            m_Type = other.type;
            m_Min = other.min;
            m_Max = other.max;
            m_SplitNumber = other.splitNumber;
            m_TextRotation = other.textRotation;
            m_ShowSplitLine = other.showSplitLine;
            m_SplitLineType = other.splitLineType;
            m_BoundaryGap = other.boundaryGap;
            m_Data.Clear();
            foreach (var d in other.data) m_Data.Add(d);
        }

        public void ClearData()
        {
            m_Data.Clear();
        }

        public void AddData(string category, int maxDataNumber)
        {
            if (maxDataNumber > 0)
            {
                while (m_Data.Count > maxDataNumber) m_Data.RemoveAt(0);
            }
            m_Data.Add(category);
        }

        public string GetData(int index,DataZoom dataZoom)
        {
            var showData = GetData(dataZoom);
            if (index >= 0 && index < showData.Count)
                return showData[index];
            else
                return "";
        }

        public List<string> GetData(DataZoom dataZoom)
        {
            if (dataZoom != null && dataZoom.show)
            {
                var startIndex = (int)((data.Count-1) * dataZoom.start / 100);
                var endIndex = (int)((data.Count - 1) * dataZoom.end / 100);
                var count = endIndex == startIndex ? 1 : endIndex - startIndex + 1;
                if(filterData == null || filterData.Count != count)
                {
                    UpdateFilterData(dataZoom);
                }
                return filterData;
            }
            else
            {
                return m_Data;
            }
        }

        public void UpdateFilterData(DataZoom dataZoom)
        {
            if (dataZoom != null && dataZoom.show)
            {
                var startIndex = (int)((data.Count - 1) * dataZoom.start / 100);
                var endIndex = (int)((data.Count - 1) * dataZoom.end / 100);
                if(startIndex != filterStart || endIndex != filterEnd)
                {
                    filterStart = startIndex;
                    filterEnd = endIndex;
                    if (m_Data.Count > 0)
                    {
                        var count = endIndex == startIndex ? 1 : endIndex - startIndex + 1;
                        filterData = m_Data.GetRange(startIndex, count);
                    }
                    else
                    {
                        filterData = m_Data;
                    }
                }
                else if(endIndex == 0)
                {
                    filterData = new List<string>();
                }
            }
        }

        public int GetSplitNumber(DataZoom dataZoom)
        {
            if (type == AxisType.Value) return m_SplitNumber;
            int dataCount = GetData(dataZoom).Count;
            if (dataCount > 2 * m_SplitNumber || dataCount <= 0)
                return m_SplitNumber;
            else
                return dataCount;
        }

        public float GetSplitWidth(float coordinateWidth,DataZoom dataZoom)
        {
            return coordinateWidth / (m_BoundaryGap ? GetSplitNumber(dataZoom) : GetSplitNumber(dataZoom) - 1);
        }

        public int GetDataNumber(DataZoom dataZoom)
        {
            return GetData(dataZoom).Count;
        }

        public float GetDataWidth(float coordinateWidth, DataZoom dataZoom)
        {
            var dataCount = GetDataNumber(dataZoom);
            return coordinateWidth / (m_BoundaryGap ? dataCount : dataCount - 1);
        }

        public string GetScaleName(int index, float minValue, float maxValue, DataZoom dataZoom)
        {
            if (m_Type == AxisType.Value)
            {
                float value = (minValue + (maxValue - minValue) * index / (GetSplitNumber(dataZoom) - 1));
                if (value - (int)value == 0)
                    return (value).ToString();
                else
                    return (value).ToString("f1");
            }
            var showData = GetData(dataZoom);
            int dataCount = showData.Count;
            if (dataCount <= 0) return "";

            if (index == GetSplitNumber(dataZoom) - 1 && !m_BoundaryGap)
            {
                return showData[dataCount - 1];
            }
            else
            {
                float rate = dataCount / GetSplitNumber(dataZoom);
                if (rate < 1) rate = 1;
                int offset = m_BoundaryGap ? (int)(rate / 2) : 0;
                int newIndex = (int)(index * rate >= dataCount - 1 ?
                    dataCount - 1 : offset + index * rate);
                return showData[newIndex];
            }
        }

        public int GetScaleNumber(DataZoom dataZoom)
        {
            if (type == AxisType.Value)
            {
                return m_BoundaryGap ? m_SplitNumber + 1 : m_SplitNumber;
            }
            else
            {
                var showData = GetData(dataZoom);
                int dataCount = showData.Count;
                if (dataCount > 2 * splitNumber || dataCount <= 0)
                    return m_BoundaryGap ? m_SplitNumber + 1 : m_SplitNumber;
                else
                    return m_BoundaryGap ? dataCount + 1 : dataCount;
            }
        }

        public float GetScaleWidth(float coordinateWidth,DataZoom dataZoom)
        {
            int num = GetScaleNumber(dataZoom) - 1;
            if (num <= 0) num = 1;
            return coordinateWidth / num;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Axis)) return false;
            return Equals((Axis)obj);
        }

        public bool Equals(Axis other)
        {
            return show == other.show &&
                type == other.type &&
                min == other.min &&
                max == other.max &&
                splitNumber == other.splitNumber &&
                showSplitLine == other.showSplitLine &&
                textRotation == other.textRotation &&
                splitLineType == other.splitLineType &&
                boundaryGap == other.boundaryGap &&
                ChartHelper.IsValueEqualsList<string>(m_Data, other.data);
        }

        public static bool operator ==(Axis point1, Axis point2)
        {
            return point1.Equals(point2);
        }

        public static bool operator !=(Axis point1, Axis point2)
        {
            return !point1.Equals(point2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void ParseJsonData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData) || !m_DataFromJson) return;
            m_Data = ChartHelper.ParseStringFromString(jsonData);
        }
    }

    [System.Serializable]
    public class XAxis : Axis
    {
        public static XAxis defaultXAxis
        {
            get
            {
                var axis = new XAxis
                {
                    m_Show = true,
                    m_Type = AxisType.Category,
                    m_Min = 0,
                    m_Max = 0,
                    m_SplitNumber = 5,
                    m_TextRotation = 0,
                    m_ShowSplitLine = false,
                    m_SplitLineType = SplitLineType.Dashed,
                    m_BoundaryGap = true,
                    m_Data = new List<string>()
                    {
                        "x1","x2","x3","x4","x5"
                    }
                };
                return axis;
            }
        }
    }

    [System.Serializable]
    public class YAxis : Axis
    {
        public static YAxis defaultYAxis
        {
            get
            {
                var axis = new YAxis
                {
                    m_Show = true,
                    m_Type = AxisType.Value,
                    m_Min = 0,
                    m_Max = 0,
                    m_SplitNumber = 5,
                    m_TextRotation = 0,
                    m_ShowSplitLine = false,
                    m_SplitLineType = SplitLineType.Dashed,
                    m_BoundaryGap = false,
                    m_Data = new List<string>(5),
                };
                return axis;
            }
        }
    }
}