using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    public class MainViewModel
    {
        public ReactiveList<DiagramEditorViewModel> DiagramEditors { get; } = new ReactiveArrayList<DiagramEditorViewModel>();

        public void EditNewDiagram()
        {
            DiagramEditors.Add(new DiagramEditorViewModel(new Diagram()));
        }

        public void EditDiagram(Diagram diagram)
        {
            DiagramEditors.Add(new DiagramEditorViewModel(diagram));
        }

        public void CloseEditor(DiagramEditorViewModel editor)
        {
            DiagramEditors.Remove(editor);
        }
    }
}