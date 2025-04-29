using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class DynamicFlowLayoutGroup : LayoutGroup
    {
        public enum Corner { UpperLeft, UpperRight, LowerLeft, LowerRight }
        public enum Axis { Horizontal, Vertical }

        [SerializeField] protected Corner m_StartCorner;
        [SerializeField] protected Axis m_StartAxis;
        [SerializeField] protected Vector2 m_CellSize = new Vector2(100f, 100f);
        [SerializeField] protected Vector2 m_Spacing = Vector2.zero;
        [SerializeField] protected bool m_ChildControlWidth = true;
        [SerializeField] protected bool m_ChildControlHeight = true;

        public Corner startCorner { get => m_StartCorner; set => SetProperty(ref m_StartCorner, value); }
        public Axis startAxis { get => m_StartAxis; set => SetProperty(ref m_StartAxis, value); }
        public Vector2 cellSize { get => m_CellSize; set => SetProperty(ref m_CellSize, value); }
        public Vector2 spacing { get => m_Spacing; set => SetProperty(ref m_Spacing, value); }

        private struct CellData
        {
            public int x;
            public int y;

            public CellData(int X, int Y)
            {
                x = X;
                y = Y;
            }
        }

        private struct RowData
        {
            public float width;
            public float height;
            public int itemCount;

            public RowData(float _width, float _height, int _itemCount)
            {
                width = _width;
                height = _height;
                itemCount = _itemCount;
            }
        }

        private List<CellData> m_CellDataList = new List<CellData>();
        private List<RowData> m_RowDataList = new List<RowData>();

        private float maxWidth;
        private float maxHeight;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcLayout();
            float width = maxWidth + padding.horizontal;
            SetLayoutInputForAxis(width, width, -1, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            CalcLayout();
            float height = maxHeight + padding.vertical;
            SetLayoutInputForAxis(height, height, -1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            SetCellsAlongAxis(0);
        }

        public override void SetLayoutVertical()
        {
            SetCellsAlongAxis(1);
        }

        private void CalcLayout()
        {
            m_CellDataList.Clear();
            m_RowDataList.Clear();
            maxWidth = maxHeight = 0f;

            bool isVertical = m_StartAxis == Axis.Vertical;

            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;
            float availableWidth = width - padding.horizontal;
            float availableHeight = height - padding.vertical;

            float currentMain = 0f; // X for Horizontal, Y for Vertical
            float currentCross = 0f; // Y for Horizontal, X for Vertical

            float rowMaxCross = 0f;
            float rowTotalMain = 0f;
            int rowItemCount = 0;

            int x = 0, y = 0;
            int count = rectChildren.Count;

            for (int i = 0; i < count; i++)
            {
                RectTransform child = rectChildren[i];

                GetChildSizes(child, 0, !isVertical && m_ChildControlWidth, false, out var xMin, out var xPreferred, out var xFlexible);
                GetChildSizes(child, 1, isVertical && m_ChildControlHeight, false, out var yMin, out var yPreferred, out var yFlexible);

                float mainSize = isVertical ? yPreferred : xPreferred;
                float crossSize = isVertical ? xPreferred : yPreferred;
                float availableMain = isVertical ? availableHeight : availableWidth;
                float spacingMain = isVertical ? spacing.y : spacing.x;
                float spacingCross = isVertical ? spacing.x : spacing.y;

                float nextMain = currentMain + (rowItemCount > 0 ? spacingMain : 0f) + mainSize;
                if (rowItemCount > 0 && nextMain > availableMain)
                {
                    m_RowDataList.Add(new RowData(rowTotalMain, rowMaxCross, rowItemCount));

                    currentCross += rowMaxCross + spacingCross;
                    currentMain = 0f;
                    rowMaxCross = 0f;
                    rowTotalMain = 0f;
                    rowItemCount = 0;

                    if (isVertical)
                    {
                        y = 0;
                        x++;
                    }
                    else
                    {
                        x = 0;
                        y++;
                    }
                }

                m_CellDataList.Add(new CellData(x, y));

                currentMain += (rowItemCount > 0 ? spacingMain : 0f) + mainSize;
                rowTotalMain = currentMain;
                rowMaxCross = Mathf.Max(rowMaxCross, crossSize);
                rowItemCount++;

                if (isVertical) y++; else x++;

                maxWidth = Mathf.Max(maxWidth, isVertical ? currentCross + rowMaxCross : rowTotalMain);
                maxHeight = Mathf.Max(maxHeight, isVertical ? rowTotalMain : currentCross + rowMaxCross);
            }

            if (rowItemCount > 0)
            {
                m_RowDataList.Add(new RowData(currentMain, rowMaxCross, rowItemCount));
            }
        }

        private void SetCellsAlongAxis(int axis)
        {
            int count = rectChildren.Count;
            if (axis == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    RectTransform rectTransform = rectChildren[i];
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition);
                    rectTransform.anchorMin = Vector2.up;
                    rectTransform.anchorMax = Vector2.up;
                    rectTransform.sizeDelta = m_StartAxis switch
                    {
                        Axis.Horizontal => new Vector2(rectTransform.sizeDelta.x, cellSize.y),
                        _ => new Vector2(cellSize.x, rectTransform.sizeDelta.y),
                    };
                }
                return;
            }

            bool isVertical = m_StartAxis == Axis.Vertical;
            bool reverseCross = (startCorner is Corner.UpperRight or Corner.LowerRight); // right side
            bool reverseMain = (startCorner is Corner.LowerLeft or Corner.LowerRight);   // bottom side

            float crossStartOffset = GetStartOffset(isVertical ? 1 : 0, isVertical ? maxHeight : maxWidth);
            float mainStartOffset = GetStartOffset(isVertical ? 0 : 1, isVertical ? maxWidth : maxHeight);

            int lastRowIndex = -1;
            float currentCrossPos = 0f;

            for (int i = 0; i < count; i++)
            {
                RectTransform child = rectChildren[i];
                GetChildSizes(child, 0, !isVertical && m_ChildControlWidth, false, out var xMin, out var xPreferred, out var xFlexible);
                GetChildSizes(child, 1, isVertical && m_ChildControlHeight, false, out var yMin, out var yPreferred, out var yFlexible);

                CellData cell = m_CellDataList[i];
                int posX = cell.x;
                int posY = cell.y;

                int rowIndex = isVertical ? posX : posY;
                RowData row = m_RowDataList[rowIndex];

                if (lastRowIndex != rowIndex)
                {
                    lastRowIndex = rowIndex;
                    currentCrossPos = (reverseCross ? row.width : 0f);
                }

                if (isVertical)
                {
                    if (reverseCross) currentCrossPos -= yPreferred;

                    float posXFinal = mainStartOffset + (xPreferred + spacing.x) * (reverseMain ? (m_RowDataList.Count - 1 - posX) : posX);
                    float posYFinal = crossStartOffset + (reverseCross ? currentCrossPos : currentCrossPos);

                    SetChildAlongAxisWithScale(child, 0, posXFinal, xPreferred, child.localScale.x);

                    if (m_ChildControlHeight) SetChildAlongAxisWithScale(child, 1, posYFinal, yPreferred, child.localScale.y);
                    else SetChildAlongAxisWithScale(child, 1, posYFinal, child.localScale.y);

                    currentCrossPos += (reverseCross ? -(spacing.y) : (yPreferred + spacing.y));
                }
                else // Horizontal
                {
                    if (reverseCross) currentCrossPos -= xPreferred;

                    float posXFinal = crossStartOffset + (reverseCross ? currentCrossPos : currentCrossPos);
                    float posYFinal = mainStartOffset + (yPreferred + spacing.y) * (reverseMain ? (m_RowDataList.Count - 1 - posY) : posY);

                    if (m_ChildControlWidth) SetChildAlongAxisWithScale(child, 0, posXFinal, xPreferred, child.localScale.x);
                    else SetChildAlongAxisWithScale(child, 0, posXFinal, child.localScale.x);

                    SetChildAlongAxisWithScale(child, 1, posYFinal, yPreferred, child.localScale.y);

                    currentCrossPos += (reverseCross ? -(spacing.x) : (xPreferred + spacing.x));
                }
            }
        }

        private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand, out float min, out float preferred, out float flexible)
        {
            if (!controlSize)
            {
                min = child.sizeDelta[axis];
                preferred = min;
                flexible = 0f;
            }
            else
            {
                min = LayoutUtility.GetMinSize(child, axis);
                preferred = LayoutUtility.GetPreferredSize(child, axis);
                flexible = LayoutUtility.GetFlexibleSize(child, axis);
            }

            if (childForceExpand)
            {
                flexible = Mathf.Max(flexible, 1f);
            }
        }
    }
}