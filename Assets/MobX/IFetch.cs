using Cysharp.Threading.Tasks;

public interface IFetch
{
    UniTask<T> Fetch<T>(string url);
}
