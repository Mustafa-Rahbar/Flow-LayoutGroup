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
        private List<RowData> m_ColumnDataList = new List<RowData>();

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
            m_ColumnDataList.Clear();
            maxWidth = maxHeight = 0f;

            float width = rectTransform.rect.width;
            float availableWidth = width - padding.horizontal;

            float currentX = 0f;
            float currentY = 0f;

            float rowMaxHeight = 0f;
            float rowTotalWidth = 0f;
            int rowItemCount = 0;


            int x = 0, y = 0;
            int count = rectChildren.Count;
            for (int i = 0; i < count; i++)
            {
                RectTransform child = rectChildren[i];
                float childWidth = child.rect.width;
                float childHeight = child.rect.height;

                if (rowItemCount > 0 && currentX + childWidth > availableWidth)
                {
                    m_RowDataList.Add(new RowData(rowTotalWidth - spacing.x, rowMaxHeight, rowItemCount));

                    currentY += rowMaxHeight + spacing.y;
                    currentX = 0f;
                    rowMaxHeight = 0f;
                    rowTotalWidth = 0f;
                    rowItemCount = 0;

                    x = 0;
                    y++;
                }

                currentX += childWidth + spacing.x;
                rowTotalWidth += childWidth + spacing.x;
                rowMaxHeight = Mathf.Max(rowMaxHeight, childHeight);
                rowItemCount++;

                maxWidth = Mathf.Max(maxWidth, rowTotalWidth - spacing.x);
                maxHeight = Mathf.Max(maxHeight, currentY + rowMaxHeight);
                m_CellDataList.Add(new CellData(x, y));

                x++;
            }

            // add last row if not added
            if (rowItemCount > 0)
            {
                m_RowDataList.Add(new RowData(rowTotalWidth - spacing.x, rowMaxHeight, rowItemCount));
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

            float startX = 0f;
            float startY = GetStartOffset(1, maxHeight);

            int lastVisualRow = -1;
            for (int i = 0; i < count; i++)
            {
                CellData cell = m_CellDataList[i];

                int positionX = cell.x;
                int positionY = cell.y;

                RowData row = m_RowDataList[positionY];

                if ((int)startCorner % 2 == 1)
                    positionX = row.itemCount - 1 - positionX;
                if ((int)startCorner / 2 == 1)
                    positionY = m_RowDataList.Count - 1 - positionY;

                if (lastVisualRow != positionY)
                {
                    lastVisualRow = positionY;
                    startX = GetStartOffset(0, row.width);
                }

                Vector2 size = rectChildren[i].rect.size;
                SetChildAlongAxis(rectChildren[i], 0, startX + (spacing.x + size.x) * positionX);
                SetChildAlongAxis(rectChildren[i], 1, startY + (spacing.y + size.y) * positionY);
            }
        }
    }
}