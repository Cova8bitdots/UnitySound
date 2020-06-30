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
        /// <returns></returns>
        UniTask<int> PlaySe( int _seId, float _volume, bool _isLoop );

        /// <summary>
        /// SE再生(OneShot)
        /// 
        /// </summary>
        /// <param name="_seId">SEID</param>
        /// <param name="_volume">volume [0, 1]</param>
        /// <returns></returns>
        int PlayOneShotSE( int _seId, float _volume);

        /// <summary>
        /// 指定SEを停止
        /// </summary>
        /// <param name="_handler">ハンドラID</param>
        /// <param name="_isForceStop">強制停止フラグ</param>
        /// <returns></returns>
        UniTask StopSE( int _handler, bool _isForceStop);
    }
}
