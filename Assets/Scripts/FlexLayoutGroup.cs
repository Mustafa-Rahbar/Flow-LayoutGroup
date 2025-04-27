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

        private struct CellData
        {
            public int x;
            public int y;

            public CellData(int _x, int _y)
            {
                x = _x;
                y = _y;
            }
        }

        private List<CellData> cellDataList = new List<CellData>();
        private float totalWidth;
        private float totalHeight;
        private int actualRowCount;
        private int actualColumnCount;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcLayout();
            float width = totalWidth + padding.horizontal;
            SetLayoutInputForAxis(width, width, -1, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            float height = totalHeight + padding.vertical;
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
            cellDataList.Clear();
            totalWidth = totalHeight = 0f;
            actualRowCount = actualColumnCount = 0;

            float width = rectTransform.rect.width;
            float availableWidth = width - padding.horizontal;

            float currentX = 0f;
            float currentY = 0f;
            float maxRowHeight = 0f;

            int x = 0, y = 0;
            int count = rectChildren.Count;
            for (int i = 0; i < count; i++)
            {
                RectTransform child = rectChildren[i];
                float childWidth = child.rect.width;
                float childHeight = child.rect.height;

                if (currentX > 0f && currentX + childWidth > availableWidth)
                {
                    currentY += maxRowHeight + spacing.y;
                    currentX = 0f;
                    maxRowHeight = 0f;
                    x = 0;
                    y++;
                }

                cellDataList.Add(new CellData(x, y));

                currentX += childWidth + spacing.x;
                maxRowHeight = Mathf.Max(maxRowHeight, childHeight);

                totalWidth = Mathf.Max(totalWidth, currentX - spacing.x);
                totalHeight = currentY + maxRowHeight;

                actualRowCount = Mathf.Max(actualRowCount, y + 1);
                actualColumnCount = Mathf.Max(actualColumnCount, x + 1);

                x++;
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
                    rectTransform.anchorMin = rectTransform.anchorMax = Vector2.up; // top-left
                }

                return;
            }

            float startX = GetStartOffset(0, totalWidth);
            float startY = GetStartOffset(1, totalHeight);

            for (int i = 0; i < count; i++)
            {
                RectTransform child = rectChildren[i];
                CellData cell = cellDataList[i];

                int positionX = cell.x;
                int positionY = cell.y;

                if ((int)startCorner % 2 == 1)
                    positionX = actualColumnCount - 1 - positionX;
                if ((int)startCorner / 2 == 1)
                    positionY = actualRowCount - 1 - positionY;

                Vector2 size = child.rect.size;

                SetChildAlongAxis(rectChildren[i], 0, startX + (spacing.x + size.x) * positionX);
                SetChildAlongAxis(rectChildren[i], 1, startY + (spacing.y + size.y) * positionY);
            }
        }
    }
}