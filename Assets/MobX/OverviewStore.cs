using Cysharp.Threading.Tasks;
using System;
using UniMob;

public class OverviewStore : Store
{
    private readonly UniTask<DocumentInfo[]> _documentTask;

    public OverviewStore(UniTask<DocumentInfo[]> documentsTask)
    {
        _documentTask = documentsTask;

        Observe(documentsTask).Forget();
    }

    public OverviewStore()
    {
    }

    private async UniTask Observe(UniTask<DocumentInfo[]> task)
    {
        State = DState.Pending;

        try
        {
            Documents = await task;
            State = DState.Fulfilled;
        }
        catch (Exception ex)
        {
            State = DState.Rejected;
        }
    }


    [Atom]
    public DocumentInfo[] Documents { get; private set; }

    [Atom]
    public DState State { get; private set; }
}
