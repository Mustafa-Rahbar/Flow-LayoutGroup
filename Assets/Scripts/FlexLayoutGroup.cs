using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class FlexLayoutGroup : LayoutGroup
    {
        [SerializeField] private Vector2 spacing = Vector2.zero;
        [SerializeField] private bool reverseArrangement = false;

        private List<List<RectTransform>> rows = new List<List<RectTransform>>();
        private List<float> rowHeights = new List<float>();
        private List<float> rowWidths = new List<float>();

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            rows.Clear();
            rowHeights.Clear();
            rowWidths.Clear();

            float width = rectTransform.rect.width;
            float maxWidth = width - padding.horizontal;

            List<RectTransform> currentRow = new List<RectTransform>();
            float currentX = 0f;
            float rowHeight = 0f;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                Vector2 cellSize = child.sizeDelta;

                if (currentRow.Count > 0 && currentX + cellSize.x > maxWidth)
                {
                    rows.Add(currentRow);
                    rowHeights.Add(rowHeight);
                    rowWidths.Add(currentX - spacing.x);

                    currentRow = new List<RectTransform>();
                    currentX = 0f;
                    rowHeight = 0f;
                }

                currentRow.Add(child);
                currentX += cellSize.x + spacing.x;
                rowHeight = Mathf.Max(rowHeight, cellSize.y);
            }

            if (currentRow.Count > 0)
            {
                rows.Add(currentRow);
                rowHeights.Add(rowHeight);
                rowWidths.Add(currentX - spacing.x);
            }

            SetLayoutInputForAxis(width, width, -1, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            float height = padding.vertical;
            foreach (var rowHeight in rowHeights)
                height += rowHeight + spacing.y;

            if (rowHeights.Count > 0)
                height -= spacing.y; // remove last spacing

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

        private void SetCellsAlongAxis(int axis)
        {
            int count = base.rectChildren.Count;
            if (axis == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    RectTransform rectTransform = base.rectChildren[i];
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition);
                    rectTransform.anchorMin = Vector2.up;
                    rectTransform.anchorMax = Vector2.up;
                }

                return;
            }

            float contentHeight = 0f;
            for (int i = 0; i < rowHeights.Count; i++)
                contentHeight += rowHeights[i] + spacing.y;
            if (rowHeights.Count > 0)
                contentHeight -= spacing.y;

            float availableHeight = rectTransform.rect.height - padding.vertical;
            float startY = GetStartOffsetForColoumn(availableHeight, contentHeight);
            float y = startY;

            float layoutWidth = rectTransform.rect.width - padding.horizontal;
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];
                float rowHeight = rowHeights[rowIndex];
                float rowWidth = rowWidths[rowIndex];

                float xStart = GetStartOffsetForRow(layoutWidth, rowWidth);
                float x = xStart;

                if (reverseArrangement)
                    row.Reverse();

                for (int i = 0; i < row.Count; i++)
                {
                    RectTransform child = row[i];
                    Vector2 cellSize = child.sizeDelta;

                    float posX = reverseArrangement ? xStart + (rowWidth - (x - xStart)) - cellSize.x : x;
                    SetChildAlongAxis(child, 0, posX);
                    SetChildAlongAxis(child, 1, y);

                    x += cellSize.x + spacing.x;
                }

                y += rowHeight + spacing.y;
            }
        }

        private float GetStartOffsetForRow(float totalWidth, float rowWidth)
        {
            switch (childAlignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.LowerLeft:
                    return padding.left;

                case TextAnchor.UpperCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.LowerCenter:
                    return padding.left + (totalWidth - rowWidth) / 2f;

                case TextAnchor.UpperRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.LowerRight:
                    return padding.left + (totalWidth - rowWidth);

                default:
                    return padding.left;
            }
        }

        private float GetStartOffsetForColoumn(float totalHeight, float contentHeight)
        {
            switch (childAlignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperCenter:
                case TextAnchor.UpperRight:
                    return padding.top;

                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    return padding.top + (totalHeight - contentHeight) / 2f;

                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    return padding.top + (totalHeight - contentHeight);

                default:
                    return padding.top;
            }
        }
    }
}