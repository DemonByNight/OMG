using UnityEngine;
using UnityEngine.EventSystems;

namespace OMG
{
    public class GameFieldRawImageBehavior : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Camera raycastCamera;
        [SerializeField] private RectTransform clickCatcherRectTransform;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameField gameField;

        private bool isPointerDownSuccess;
        private Vector2 _pointerDownViewportCache;

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDownSuccess = GetViewportClickInsideRect(clickCatcherRectTransform, eventData.position, raycastCamera, out _pointerDownViewportCache);
        }

        public async void OnPointerUp(PointerEventData eventData)
        {
            if (!isPointerDownSuccess)
                return;

            if (GetViewportClickInsideRect(clickCatcherRectTransform, eventData.position, raycastCamera, out var pointerUpViewportCache))
            {
                canvasGroup.interactable = false;
                //await gameField.HandleCommand(new MoveCommand(_pointerDownViewportCache, pointerUpViewportCache));
            }

            canvasGroup.interactable = true;
            isPointerDownSuccess = false;
        }

        private bool GetViewportClickInsideRect(RectTransform rect, Vector2 clickPosition, Camera camera, out Vector2 viewport)
        {
            bool result = false;
            viewport = new(-1, -1);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, clickPosition, camera, out Vector2 localPoint))
            {
                result = true;
                var localXCorrectedByPivot = (rect.rect.width * rect.pivot.x) + localPoint.x;
                var localYCorrectedByPivot = (rect.rect.height * rect.pivot.y) + localPoint.y;
                viewport = new(localXCorrectedByPivot / rect.rect.width, localYCorrectedByPivot / rect.rect.height);
            }

            return result;
        }
    }
}
