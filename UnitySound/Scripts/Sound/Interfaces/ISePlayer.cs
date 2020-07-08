using System.Threading;
using Cysharp.Threading.Tasks;

namespace CovaTech.UnitySound
{
    public interface ISePlayer
    {
        /// <summary>
        /// SE再生
        /// </summary>
        /// <param name="_seId">SEID</param>
        /// <param name="_volume">volume [0, 1]</param>
        /// <param name="_isLoop">Is LoopSound?</param>
        /// <param name="_token">キャンセル用トークン</param>
        /// <returns></returns>
        UniTask<int> PlaySe( int _seId, float _volume, bool _isLoop, CancellationToken _token);

        /// <summary>
        /// SE再生(OneShot)
        /// 
        /// </summary>
        /// <param name="_seId">SEID</param>
        /// <param name="_volume">volume [0, 1]</param>
        /// <returns></returns>
        void PlayOneShotSE( int _seId, float _volume);

        /// <summary>
        /// 指定SEを停止
        /// </summary>
        /// <param name="_handler">ハンドラID</param>
        /// <param name="_isForceStop">強制停止フラグ</param>
        /// <param name="_token">キャンセル用トークン</param>
        /// <returns></returns>
        UniTask StopSE( int _handler, bool _isForceStop, CancellationToken _token);


        /// <summary>
        /// SoundItem で再生準備フェーズまでの事前準備を行う
        /// </summary>
        /// <param name="_seId"></param>
        /// <param name="_isLoop"></param>
        /// <param name="_token"></param>
        /// <returns></returns>
        UniTask<int> PrewarmSE( int _seId, bool _isLoop, CancellationToken _token);
    }
}
