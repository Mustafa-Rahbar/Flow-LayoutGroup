using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class WrapLayoutGroup : LayoutGroup
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
                float childWidth = GetChildSize(child, 0, m_StartAxis == Axis.Horizontal);
                float childHeight = GetChildSize(child, 1, m_StartAxis == Axis.Vertical);

                float nextX = currentX + (rowItemCount > 0 ? spacing.x : 0f) + childWidth;
                if (rowItemCount > 0 && nextX > availableWidth)
                {
                    m_RowDataList.Add(new RowData(rowTotalWidth, rowMaxHeight, rowItemCount));

                    currentY += rowMaxHeight + spacing.y;
                    currentX = 0f;
                    rowMaxHeight = 0f;
                    rowTotalWidth = 0f;
                    rowItemCount = 0;

                    x = 0;
                    y++;
                }

                m_CellDataList.Add(new CellData(x, y));

                currentX += (rowItemCount > 0 ? spacing.x : 0f) + childWidth;
                rowTotalWidth = currentX;
                rowMaxHeight = Mathf.Max(rowMaxHeight, childHeight);
                rowItemCount++;

                maxWidth = Mathf.Max(maxWidth, rowTotalWidth);
                maxHeight = Mathf.Max(maxHeight, currentY + rowMaxHeight);

                x++;
            }

            // Add last row
            if (rowItemCount > 0)
            {
                m_RowDataList.Add(new RowData(currentX, rowMaxHeight, rowItemCount));
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

                    rectTransform.anchorMin = Vector2.up;
                    rectTransform.anchorMax = Vector2.up;
                    rectTransform.sizeDelta = m_StartAxis switch
                    {
                        Axis.Horizontal => new Vector2(rectTransform.sizeDelta[0], cellSize.y),
                        _ => new Vector2(cellSize.x, rectTransform.sizeDelta[1])
                    };
                }

                return;
            }

            float startX = 0f;
            float startY = GetStartOffset(1, maxHeight);

            int lastRow = -1;
            float currentX = 0f;
            for (int i = 0; i < count; i++)
            {
                RectTransform child = rectChildren[i];
                float childWidth = GetChildSize(child, 0, m_ChildControlWidth);
                float childHeight = child.sizeDelta[1];

                CellData cell = m_CellDataList[i];

                int positionX = cell.x;
                int positionY = cell.y;

                RowData row = m_RowDataList[positionY];

                if (lastRow != positionY)
                {
                    lastRow = positionY;
                    startX = GetStartOffset(0, row.width);
                    currentX = ((int)startCorner % 2 == 1) ? row.width : 0f;
                }

                if ((int)startCorner % 2 == 1) currentX -= childWidth;
                if ((int)startCorner / 2 == 1)
                    positionY = m_RowDataList.Count - 1 - positionY;

                if (m_ChildControlWidth) SetChildAlongAxisWithScale(child, 0, startX + currentX, childWidth, child.localScale[0]);
                else SetChildAlongAxisWithScale(child, 0, startX + currentX, child.localScale[0]);
                SetChildAlongAxisWithScale(child, 1, startY + (childHeight + spacing.y) * positionY, childHeight, child.localScale[1]);

                currentX += ((int)startCorner % 2 == 1) ? spacing.x * -1 : childWidth + spacing.x;
            }
        }

        private float GetChildSize(RectTransform child, int axis, bool controlSize)
        {
            return (controlSize) switch
            {
                true => LayoutUtility.GetPreferredSize(child, axis),
                _ => child.sizeDelta[axis],
            };
        }
    }
}