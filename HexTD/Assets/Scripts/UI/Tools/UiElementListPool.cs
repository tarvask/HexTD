using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Tools
{
    public class UiElementListPool<T> : ElementListPool<T> where T : MonoBehaviour
    {
        private readonly Transform _parent;
        private readonly T _prefab;

        public UiElementListPool(T prefab, Transform rootTransform = null)
        {
            if (rootTransform == null)
            {
                rootTransform = new GameObject(prefab.name + "s").transform;
                rootTransform.position = Vector3.zero;
            }
            else
            {
                foreach (Transform child in rootTransform)
                {
                    Object.Destroy(child.gameObject);
                }
            }

            _parent = rootTransform;
            _prefab = prefab;
        }

        protected override T CreateNewElement()
        {
            return Object.Instantiate(_prefab, _parent);
        }

        protected override void InitElement(T element)
        {
            element.gameObject.SetActive(true);
        }

        protected override void CacheElement(T element)
        {
            element.gameObject.SetActive(false);
            element.transform.SetAsLastSibling();
        }

        protected override void DisposeElement(T element)
        {
            Object.Destroy(element.gameObject);
        }
    }
}