using System.Collections.Generic;

namespace UnityEngine.UI {

    public class FlowLayoutGroup : LayoutGroup {

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

        private struct CellData {
            public int x;
            public int y;

            public CellData(int X, int Y) {
                x = X;
                y = Y;
            }
        }

        private struct RowData {
            public float width;
            public float height;
            public int itemCount;

            public RowData(float _width, float _height, int _itemCount) {
                width = _width;
                height = _height;
                itemCount = _itemCount;
            }
        }

        private List<CellData> m_CellDataList = new List<CellData>();
        private List<RowData> m_RowDataList = new List<RowData>();

        public override void CalculateLayoutInputHorizontal() {
            base.CalculateLayoutInputHorizontal();
            CalculateLayoutInput((int) m_StartAxis);
            SetLayoutInputForAxis(preferredWidth, preferredWidth, -1, 0);
        }

        public override void CalculateLayoutInputVertical() {
            CalculateLayoutInput((int) m_StartAxis);
            SetLayoutInputForAxis(preferredHeight, preferredHeight, -1, 1);
        }

        public override void SetLayoutHorizontal() {
            SetCellsAlongAxis(0);
        }

        public override void SetLayoutVertical() {
            SetCellsAlongAxis(1);
        }

        private void CalculateLayoutInput(int axis) {
            m_CellDataList.Clear();
            m_RowDataList.Clear();

            float size = rectTransform.rect.size[axis];
            float padding = (axis == 1 ? base.padding.vertical : base.padding.horizontal);
            float availableSize = size - padding;

            int itemCount = 0;
            Vector2 maxSize = Vector2.zero;
            Vector2 totalPreferred = Vector2.zero;
            Vector2Int row = Vector2Int.zero;

            int count = rectChildren.Count;
            for (int i = 0; i < count; i++) {
                RectTransform child = rectChildren[i];
                GetChildSizes(child, 0, m_ChildControlWidth, out var xMin, out var xPreferred, out var xFlexible);
                GetChildSizes(child, 1, m_ChildControlHeight, out var yMin, out var yPreferred, out var yFlexible);
                Vector2 cellSize = new Vector2(xPreferred, yPreferred);

                // Handle the spacing properly for the first item in the row
                float space = (itemCount > 0 ? spacing[axis] : 0f);  // No space before the first item
                float maxPosition = totalPreferred[axis] + space + cellSize[axis];

                // Check if wrapping to a new row is needed
                if (itemCount > 0 && maxPosition > availableSize) {
                    m_RowDataList.Add(new RowData(totalPreferred[0], totalPreferred[1], itemCount));

                    maxSize[axis] = Mathf.Max(maxSize[axis], totalPreferred[axis]);
                    maxSize[1 - axis] += totalPreferred[1 - axis] + spacing[1 - axis];

                    // Reset for new row
                    row[axis] = 0;
                    row[1 - axis]++;
                    totalPreferred[axis] = 0f;
                    totalPreferred[1 - axis] = 0f;
                    itemCount = 0;
                    space = 0f; // Reset spacing for the first item of the new row
                }

                // Add the current item to the row data
                m_CellDataList.Add(new CellData(row[0], row[1]));

                // Update the total preferred size
                totalPreferred[axis] += space + cellSize[axis];
                totalPreferred[1 - axis] = Mathf.Max(totalPreferred[1 - axis], cellSize[1 - axis]);
                itemCount++;
                row[axis]++;
            }

            // Final row (in case it ended without wrap)
            if (itemCount > 0) {
                m_RowDataList.Add(new RowData(totalPreferred[0], totalPreferred[1], itemCount));

                maxSize[axis] = Mathf.Max(maxSize[axis], totalPreferred[axis]);
                maxSize[1 - axis] += totalPreferred[1 - axis];
            }

            float totalPreferedX = maxSize[0] + padding;
            float totalPreferedY = maxSize[1] + padding;

            SetLayoutInputForAxis(totalPreferedX, totalPreferedX, 0f, 0);
            SetLayoutInputForAxis(totalPreferedY, totalPreferedY, 0f, 1);
        }

        private void SetCellsAlongAxis(int axis) {
            int count = rectChildren.Count;
            if (axis == 0) {
                for (int i = 0; i < count; i++) {
                    RectTransform rectTransform = rectChildren[i];
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition);
                    rectTransform.anchorMin = Vector2.up;
                    rectTransform.anchorMax = Vector2.up;
                    rectTransform.sizeDelta = m_StartAxis switch {
                        Axis.Horizontal => new Vector2(rectTransform.sizeDelta.x, cellSize.y),
                        _ => new Vector2(cellSize.x, rectTransform.sizeDelta.y),
                    };
                }
                return;
            }

            bool isVertical = m_StartAxis == Axis.Vertical;
            bool reverseX = ((int) startCorner % 2) == 1;
            bool reverseY = ((int) startCorner / 2) == 1;

            int lastRowIndex = -1;
            float currentPos = 0f;
            float startOffsetX = 0f;
            float startOffsetY = 0f;

            bool num = isVertical ? reverseY : reverseX;
            int num2 = num ? count - 1 : 0;
            int num3 = num ? 0 : count;
            int num4 = num ? -1 : 1;
            for (int i = num2; num ? i >= num3 : i < num3; i += num4) {
                RectTransform child = rectChildren[i];
                CellData cell = m_CellDataList[i];

                GetChildSizes(child, 0, !isVertical && m_ChildControlWidth, out var xMin, out var xPreferred, out var xFlexible);
                GetChildSizes(child, 1, isVertical && m_ChildControlHeight, out var yMin, out var yPreferred, out var yFlexible);

                int positionX = cell.x;
                int positionY = cell.y;

                int rowIndex = isVertical ? positionX : positionY;
                RowData row = m_RowDataList[rowIndex];

                if (isVertical) {
                    if (lastRowIndex != rowIndex) {
                        lastRowIndex = rowIndex;
                        float rowHeight = row.height;
                        startOffsetX = GetStartOffset(0, preferredWidth);
                        startOffsetY = GetStartOffset(1, rowHeight);
                        currentPos = 0;
                    }

                    float posXFinal = startOffsetX + (xPreferred + spacing.x) * (reverseX ? (m_RowDataList.Count - 1 - positionX) : positionX);
                    float posYFinal = startOffsetY + currentPos;

                    SetChildAlongAxisWithScale(child, 0, posXFinal, xPreferred, child.localScale.x);

                    if (m_ChildControlHeight) SetChildAlongAxisWithScale(child, 1, posYFinal, yPreferred, child.localScale.y);
                    else SetChildAlongAxisWithScale(child, 1, posYFinal, child.localScale.y);

                    currentPos += yPreferred + spacing.y;
                }
                else // Horizontal
                {
                    if (lastRowIndex != rowIndex) {
                        lastRowIndex = rowIndex;
                        float rowWidth = row.width;
                        startOffsetX = GetStartOffset(0, rowWidth);
                        startOffsetY = GetStartOffset(1, preferredHeight);
                        currentPos = 0;
                    }

                    float posXFinal = startOffsetX + currentPos;
                    float posYFinal = startOffsetY + (yPreferred + spacing.y) * (reverseY ? (m_RowDataList.Count - 1 - positionY) : positionY);

                    if (m_ChildControlWidth) SetChildAlongAxisWithScale(child, 0, posXFinal, xPreferred, child.localScale.x);
                    else SetChildAlongAxisWithScale(child, 0, posXFinal, child.localScale.x);

                    SetChildAlongAxisWithScale(child, 1, posYFinal, yPreferred, child.localScale.y);

                    currentPos += xPreferred + spacing.x;
                }
            }
        }

        private void GetChildSizes(RectTransform child, int axis, bool controlSize, out float min, out float preferred, out float flexible) {
            min = !controlSize ?
                child.sizeDelta[axis] :
                (m_StartAxis, axis) switch {
                    (Axis.Horizontal, 1) or
                    (Axis.Vertical, 0) => cellSize[axis],
                    _ => LayoutUtility.GetPreferredSize(child, axis)
                };
            preferred = min;
            flexible = 0f;
        }
    }
}