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
        }

        private List<CellData> cellDataList = new List<CellData>();
        float totalWidth;
        float totalHeight;

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

            totalWidth = 0f;
            totalHeight = 0f;

            float width = rectTransform.rect.width;
            float availableWidth = width - padding.horizontal;

            float currentX = 0f;
            float currentY = 0f;

            int count = rectChildren.Count;
            for (int i = 0; i < count; i++)
            {
                RectTransform child = rectChildren[i];
                float childWidth = child.rect.width;
                float childHeight = child.rect.height;

                if (currentX + childWidth > availableWidth)
                {
                    currentY += totalHeight + spacing.y;
                    currentX = 0f;
                }

                currentX += childWidth + spacing.x;
                totalWidth = Mathf.Max(childWidth, currentX - spacing.x);
                totalHeight = Mathf.Max(childHeight, currentY - spacing.y);

                cellDataList.Add(new CellData());
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

            for (int i = 0; i < count; i++)
            {
                RectTransform child = rectChildren[i];
                float childWidth = child.rect.width;
                float childHeight = child.rect.height;

                CellData data = cellDataList[i];

                float startX = GetStartOffset(0, totalWidth);
                float startY = GetStartOffset(1, totalHeight);

                float posX = startX + childWidth / 2f;
                float posY = startY;

                SetChildAlongAxis(child, 0, posX);
                SetChildAlongAxis(child, 1, posY);
            }
        }
    }
}