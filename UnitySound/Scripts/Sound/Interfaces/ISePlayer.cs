using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace CovaTech.UnitySound
{
    public interface ISePlayer
    {
        /// <summary>
        /// SE再生
        /// </summary>
        /// <param name="_param">再生に必要なパラメータ</param>
        /// <param name="_token">キャンセル用トークン</param>
        /// <returns></returns>
        UniTask<int> PlaySe( SeParam _param,  CancellationToken _token);

        /// <summary>
        /// Prewarm 完了後に呼び出すSE 再生API
        /// </summary>
        /// <param name="_handler">Item のHandler</param>
        /// <param name="_volume">音量 [0 1]</param>
        /// <returns>ハンドラ</returns>
        int PlaySe( int _handler, float _volume);

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
        /// <param name="_param">再生に必要なパラメータ</param>
        /// <param name="_token">キャンセル用トークン</param>
        /// <returns></returns>
        UniTask<int> PrewarmSE( SeParam _param, CancellationToken _token);
    }


    /// <summary>
    /// SE 再生に必要なパラメータ
    /// </summary>
    public class SeParam
    {
        /// <summary>
        /// 再生するBGMID
        /// </summary>
        public int SeId => m_seId;
        private int m_seId = 0;
        public void SetId( int _id){ m_seId = _id;}

        /// <summary>
        /// 再生する音量
        /// </summary>
        public float Volume => m_volume;
        private float m_volume = 1.0f;
        public void SetVolume( float _volume){ m_volume = Mathf.Clamp01(m_volume);}


        /// <summary>
        /// ループ再生かどうか
        /// </summary>
        public bool IsLoop => m_isLoop;
        private bool m_isLoop = false;
        public void SetLoopFlag( bool _isLoop){m_isLoop = _isLoop;}


        /// <summary>
        /// 再生位置
        /// </summary>
        public Vector3 Position => m_position;
        private Vector3 m_position = Vector3.zero;
        public void SetPosition( Vector3 _pos){ m_position = _pos; }
    }

    /// <summary>
    /// 拡張メソッド定義用
    /// </summary>
    public static class SeParamExtention
    {
        /// <summary>
        /// パラメータのValidation
        /// 必要に応じて追記
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsValid( this SeParam self)
        {
            return self != null
            ;
        }
    }
}
