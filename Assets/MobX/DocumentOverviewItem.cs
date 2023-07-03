using System;
using UnityEngine.UIElements;

class DocumentOverviewItem : VisualElement
{
    public DocumentOverviewItem(Action<int> onClicked)
    {
        var btn = new Button();
        btn.clicked += () => onClicked(Id);
        Add(btn);
    }

    public int Id { get; set; }
}
