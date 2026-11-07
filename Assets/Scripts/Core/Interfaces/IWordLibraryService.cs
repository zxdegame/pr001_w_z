using System.Threading.Tasks;

/// <summary>
/// 词库服务抽象（支持 Phase 0-1 Resources，Phase 2+ Addressables）。
/// 真源：Architecture §4.3。
/// </summary>
public interface IWordLibraryService
{
    /// <summary>列出可用教材（仅元数据）。</summary>
    Task<TextbookMetadata[]> ListAvailable();

    /// <summary>按 key 按需加载词库。</summary>
    Task<WordDatabase> LoadLibrary(string key);

    /// <summary>不再使用时释放词库。</summary>
    void ReleaseLibrary(string key);
}
