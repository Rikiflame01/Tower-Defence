using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Quick Duplicator")]
public class QuickDuplicatorOverlay : Overlay
{
    private GameObject selectedObject;
    private GameObject previewObject;
    private float offset = 1.0f;
    private bool selectNewObject = false;
    private bool moveCameraToNewObject = false;
    private float cameraZoom = 1.0f;

    public override void OnCreated()
    {
        Selection.selectionChanged += OnSelectionChanged;
        OnSelectionChanged();
    }

    public override void OnWillBeDestroyed()
    {
        Selection.selectionChanged -= OnSelectionChanged;
        ClearPreview();
    }

    private void OnSelectionChanged()
    {
        selectedObject = Selection.activeGameObject;
        ClearPreview();
    }

    public override VisualElement CreatePanelContent()
    {
        var container = new VisualElement();

        var title = new Label("Quick Duplicator") { style = { unityFontStyleAndWeight = FontStyle.Bold } };
        container.Add(title);

        var selectedObjectLabel = new Label("Selected Object: ");
        container.Add(selectedObjectLabel);

        var selectedObjectField = new TextField { value = selectedObject != null ? selectedObject.name : "None" };
        selectedObjectField.isReadOnly = true;
        container.Add(selectedObjectField);

        var offsetField = new FloatField("Offset") { value = offset };
        offsetField.RegisterValueChangedCallback(evt => offset = evt.newValue);
        container.Add(offsetField);

        var selectNewObjectToggle = new Toggle("Select New Object") { value = selectNewObject };
        selectNewObjectToggle.RegisterValueChangedCallback(evt => selectNewObject = evt.newValue);
        container.Add(selectNewObjectToggle);

        var moveCameraToNewObjectToggle = new Toggle("Move Camera to New Object") { value = moveCameraToNewObject };
        moveCameraToNewObjectToggle.RegisterValueChangedCallback(evt => moveCameraToNewObject = evt.newValue);
        container.Add(moveCameraToNewObjectToggle);

        if (moveCameraToNewObject)
        {
            var cameraZoomSlider = new Slider("Camera Zoom", 0.1f, 10.0f) { value = cameraZoom };
            cameraZoomSlider.RegisterValueChangedCallback(evt => cameraZoom = evt.newValue);
            container.Add(cameraZoomSlider);
        }

        container.Add(CreateColoredButton("Duplicate Left", Vector3.left, Color.red));
        container.Add(CreateColoredButton("Duplicate Right", Vector3.right, Color.red));
        container.Add(CreateColoredButton("Duplicate Up", Vector3.up, Color.green));
        container.Add(CreateColoredButton("Duplicate Down", Vector3.down, Color.green));
        container.Add(CreateColoredButton("Duplicate Forward", Vector3.forward, Color.blue));
        container.Add(CreateColoredButton("Duplicate Backward", Vector3.back, Color.blue));

        var clearPreviewButton = new Button(ClearPreview) { text = "Clear Preview" };
        container.Add(clearPreviewButton);

        return container;
    }

    private Button CreateColoredButton(string label, Vector3 direction, Color color)
    {
        var button = new Button(() => DuplicateObject(direction)) { text = label };
        button.style.backgroundColor = new StyleColor(color);
        button.RegisterCallback<MouseEnterEvent>(evt => PreviewDuplicate(direction));
        button.RegisterCallback<MouseLeaveEvent>(evt => ClearPreview());
        return button;
    }

    private void DuplicateObject(Vector3 direction)
    {
        if (selectedObject != null)
        {
            GameObject newObject = Object.Instantiate(selectedObject);
            newObject.transform.position = selectedObject.transform.position + direction * offset;
            newObject.name = selectedObject.name + " (Duplicate)";
            Undo.RegisterCreatedObjectUndo(newObject, "Duplicate Object");

            if (selectNewObject)
            {
                Selection.activeGameObject = newObject;
            }

            if (moveCameraToNewObject)
            {
                SceneView.lastActiveSceneView.FrameSelected();
                AdjustCameraZoom(cameraZoom);
            }

            ClearPreview();
        }
        else
        {
            Debug.LogWarning("No selected object to duplicate.");
        }
    }

    private void PreviewDuplicate(Vector3 direction)
    {
        if (selectedObject != null)
        {
            if (previewObject == null)
            {
                previewObject = Object.Instantiate(selectedObject);
                previewObject.name = selectedObject.name + " (Preview)";
                previewObject.hideFlags = HideFlags.HideAndDontSave;

                var renderer = previewObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material tempMaterial = new Material(renderer.sharedMaterial)
                    {
                        color = new Color(0, 1, 0, 0.5f)
                    };
                    renderer.sharedMaterial = tempMaterial;
                }
            }

            previewObject.transform.position = selectedObject.transform.position + direction * offset;
        }
        else
        {
            Debug.LogWarning("No selected object to preview duplication.");
        }
    }

    private void ClearPreview()
    {
        if (previewObject != null)
        {
            Object.DestroyImmediate(previewObject);
        }
    }

    private void AdjustCameraZoom(float zoomLevel)
    {
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.size = zoomLevel;
        }
    }
}
