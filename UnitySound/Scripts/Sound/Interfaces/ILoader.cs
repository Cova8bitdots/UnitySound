using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CovaTech.UnitySound
{
    public interface ILoader<T> : System.IDisposable
    {
        /// <summary>
        /// AssetLoad処理
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_token"></param>
        /// <returns></returns>
        UniTask<T> LoadAsset( string _key, CancellationToken _token);
    }
}