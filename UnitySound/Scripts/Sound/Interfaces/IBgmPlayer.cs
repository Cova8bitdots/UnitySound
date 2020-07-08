using System.Threading;
using Cysharp.Threading.Tasks;

namespace CovaTech.UnitySound
{
    public interface IBgmPlayer
    {
        /// <summary>
        /// BGM 再生
        /// </summary>
        /// <param name="_bgmId">BGM ID</param>
        /// <param name="_volume">volume [0, 1]</param>
        /// <param name="_isLoop">Is LoopSound?</param>
        /// <param name="_token">キャンセル用トークン</param>
        /// <returns>ハンドラ</returns>
        UniTask<int> PlayBgm( int _bgmId, float _volume, bool _isLoop, CancellationToken _token);

        /// <summary>
        /// 指定BGM を停止
        /// </summary>
        /// <param name="_handler">ハンドラID</param>
        /// <param name="_isForceStop">強制停止フラグ</param>
        /// <param name="_token">キャンセル用トークン</param>
        /// <returns></returns>
        UniTask StopBGM ( int _handler, bool _isForceStop, CancellationToken _token);

        /// <summary>
        /// 再生準備フェーズまでの事前準備を行う
        /// </summary>
        /// <param name="_bgmId">BGM ID</param>
        /// <param name="_isLoop">Is LoopSound?</param>
        /// <param name="_token">キャンセル用トークン</param>
        /// <returns>ハンドラ</returns>
        UniTask<int> PrewarmBGM( int _bgmId, bool _isLoop, CancellationToken _token);
        
    }
}
