using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CovaTech.UnitySound
{
    public interface IBgmPlayer
    {
        /// <summary>
        /// BGM 再生
        /// </summary>
        /// <param name="_param">BGM 再生に必要なパラメータ</param>
        /// <param name="_token">キャンセル用トークン</param>
        /// <returns>ハンドラ</returns>
        UniTask<int> PlayBgm( BgmParam _param, CancellationToken _token);

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
        /// <param name="_param">BGM 再生に必要なパラメータ</param>
        /// <param name="_token">キャンセル用トークン</param>
        /// <returns>ハンドラ</returns>
        UniTask<int> PrewarmBGM( BgmParam _param, CancellationToken _token);
        
    }


    /// <summary>
    /// BGM再生に必要なパラメータ
    /// </summary>
    public class BgmParam
    {
        /// <summary>
        /// 再生するBGMID
        /// </summary>
        public int BgmId => m_bgmId;
        private int m_bgmId = 0;
        public void SetId( int _id){ m_bgmId = _id;}

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
    public static class BgmParamExtention
    {
        /// <summary>
        /// パラメータのValidation
        /// 必要に応じて追記
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsValid( this BgmParam self)
        {
            return self != null
            ;
        }
    }
}
