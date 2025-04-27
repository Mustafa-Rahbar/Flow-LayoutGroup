using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class FlexLayoutGroup : LayoutGroup
    {
        public enum Corner { UpperLeft, UpperRight, LowerLeft, LowerRight }
        public enum Axis { Horizontal, Vertical }

        [SerializeField] protected Corner m_StartCorner;
        [SerializeField] protected Axis m_StartAxis;
        [SerializeField] protected Vector2 m_CellSize = new Vector2(100f, 100f);
        [SerializeField] protected Vector2 m_Spacing = Vector2.zero;

        public Corner startCorner { get => m_StartCorner; set => SetProperty(ref m_StartCorner, value); }
        public Axis startAxis { get => m_StartAxis; set => SetProperty(ref m_StartAxis, value); }
        public Vector2 cellSize { get => m_CellSize; set => SetProperty(ref m_CellSize, value); }
        public Vector2 spacing { get => m_Spacing; set => SetProperty(ref m_Spacing, value); }

        private List<List<RectTransform>> rows = new List<List<RectTransform>>();
        private List<float> m_RowHeights = new List<float>();
        private List<float> m_RowWidths = new List<float>();

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcLayout();
            float width = rectTransform.rect.width;
            SetLayoutInputForAxis(width, width, -1, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            float totalHeight = padding.vertical;
            foreach (var h in m_RowHeights)
                totalHeight += h + spacing.y;
            if (m_RowHeights.Count > 0)
                totalHeight -= spacing.y;
            SetLayoutInputForAxis(totalHeight, totalHeight, -1, 1);
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
            rows.Clear();
            m_RowHeights.Clear();
            m_RowWidths.Clear();

            float width = rectTransform.rect.width;
            float availableWidth = width - padding.horizontal;
            List<RectTransform> currentRow = new List<RectTransform>();

            float currentX = 0f;
            float rowHeight = 0f;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                var child = rectChildren[i];
                float childWidth = child.rect.width;
                float childHeight = child.rect.height;

                // Debug log to check if child fits in the current row or if it wraps
                Debug.Log($"Child {i}: Width={childWidth}, CurrentX={currentX}, AvailableWidth={availableWidth}");

                if (currentRow.Count > 0 && currentX + childWidth > availableWidth)
                {
                    rows.Add(currentRow);
                    m_RowHeights.Add(rowHeight);
                    m_RowWidths.Add(currentX - spacing.x);

                    currentRow = new List<RectTransform>();
                    currentX = 0f;
                    rowHeight = 0f;
                }

                currentRow.Add(child);
                currentX += childWidth + spacing.x;
                rowHeight = Mathf.Max(rowHeight, childHeight);
            }

            if (currentRow.Count > 0)
            {
                rows.Add(currentRow);
                m_RowHeights.Add(rowHeight);
                m_RowWidths.Add(currentX - spacing.x);
            }

            // Debug logs to check rows, row widths, and row heights
            Debug.Log($"Rows: {rows.Count}");
            for (int i = 0; i < rows.Count; i++)
            {
                Debug.Log($"Row {i}: Width={m_RowWidths[i]}, Height={m_RowHeights[i]}");
            }
        }

        private void SetCellsAlongAxis(int axis)
        {
            int count = base.rectChildren.Count;
            if (axis == 0)
            {
                // Handle horizontal layout
                for (int i = 0; i < count; i++)
                {
                    RectTransform rectTransform = base.rectChildren[i];
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition);
                    rectTransform.anchorMin = Vector2.up;
                    rectTransform.anchorMax = Vector2.up;
                }

                return;
            }

            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            bool flipX = startCorner == Corner.UpperRight || startCorner == Corner.LowerRight;
            bool flipY = startCorner == Corner.LowerLeft || startCorner == Corner.LowerRight;

            // Calculate total vertical size
            float requiredHeight = 0f;
            count = m_RowHeights.Count;
            for (int i = 0; i < count; i++)
                requiredHeight += m_RowHeights[i] + spacing.y;
            if (count > 0) requiredHeight -= spacing.y;

            // Proper starting Y
            float yStart = GetStartOffset(1, requiredHeight);
            if (flipY)
                yStart = parentHeight - yStart - requiredHeight;

            float y = yStart;

            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];
                float rowHeight = m_RowHeights[rowIndex];
                float rowWidth = m_RowWidths[rowIndex];

                // Proper starting X
                float xStart = GetStartOffset(0, rowWidth);
                if (flipX)
                    xStart = parentWidth - xStart - rowWidth;

                float x = xStart;

                for (int i = 0; i < row.Count; i++)
                {
                    RectTransform child = row[i];
                    Vector2 childSize = child.rect.size;

                    // Correct position for current row
                    float posX = x;
                    float posY = y;

                    SetChildAlongAxis(child, 0, posX);
                    SetChildAlongAxis(child, 1, posY);

                    // Move x position correctly after setting
                    if (flipX)
                        x -= childSize.x + spacing.x;  // For right-side flip, move x left
                    else
                        x += childSize.x + spacing.x;  // For left-side, move x right
                }

                // Adjust y position based on flipY
                if (flipY)
                    y -= rowHeight + spacing.y;  // For bottom flip, move y upwards
                else
                    y += rowHeight + spacing.y;  // For top-side, move y downwards
            }
        }
    }
}