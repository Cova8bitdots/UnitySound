using UnityEngine;
using UnityEngine.Audio;
using Cysharp.Threading.Tasks;

namespace CovaTech.UnitySound
{
    /// <summary>
    /// サウンド管理クラス
    /// </summary>
    public class SoundManager : MonoBehaviour, IAudioPlayer, IVolumeController
    {
        //------------------------------------------------------------------
        // メンバ変数
        //------------------------------------------------------------------
        #region  ===== MEMBER_VARIABLES =====

        [SerializeField]
        private AudioMixer m_mixer = null;
        [SerializeField]
        private AudioMixerGroup[] m_mixerGroups = null;

        private IVolumeController m_volumeCtrl = null;
        #endregion //) ===== MEMBER_VARIABLES =====
        



        //-----------------------------------------------------
        // IVolumeController 実装
        //-----------------------------------------------------
        #region ===== IVolumeController =====

        /// <summary>
        /// 指定カテゴリの音量を更新
        /// </summary>
        /// <param name="_category"></param>
        /// <param name="_volume"></param>
        void IVolumeController.UpdateVolume( SOUND_CATEGORY _category, float _volume)
        {
            m_volumeCtrl?.UpdateVolume( _category, _volume );
        }

        /// <summary>
        /// 指定オーディオの音量を返す
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        float IVolumeController.GetVolume( SOUND_CATEGORY _category)
        {
            if( m_volumeCtrl == null )
            {
                return SoundConsts.DEFAULT_VOLUME;
            }
            return m_volumeCtrl.GetVolume( _category );
        }

        #endregion //) ===== IVolumeController =====


        //------------------------------------------------------------------
        // IBgmPlayer 
        //------------------------------------------------------------------
        #region  ===== IBgmPlayer =====
        /// <summary>
        /// BGM 再生
        /// </summary>
        /// <param name="_bgmId">BGM ID</param>
        /// <param name="_volume">volume [0, 1]</param>
        /// <param name="_isLoop">Is LoopSound?</param>
        /// <returns></returns>
        async UniTask<int> IBgmPlayer.PlayBgm( int _bgmId, float _volume, bool _isLoop )
        {
            return SoundConsts.INVALID_HANDLER;
        }

        /// <summary>
        /// 指定BGM を停止
        /// </summary>
        /// <param name="_handler">ハンドラID</param>
        /// <param name="_isForceStop">強制停止フラグ</param>
        /// <returns></returns>
        async UniTask IBgmPlayer.StopBGM ( int _handler, bool _isForceStop)
        {

        }
        #endregion //) ===== IBgmPlayer =====


        //------------------------------------------------------------------
        // ISePlayer
        //------------------------------------------------------------------
        #region  ===== ISePlayer =====
        /// <summary>
        /// SE再生
        /// </summary>
        /// <param name="_seId">SEID</param>
        /// <param name="_volume">volume [0, 1]</param>
        /// <param name="_isLoop">Is LoopSound?</param>
        /// <returns></returns>
        async UniTask<int> ISePlayer.PlaySe( int _seId, float _volume, bool _isLoop )
        {
            return SoundConsts.INVALID_HANDLER;
        }

        /// <summary>
        /// SE再生(OneShot)
        /// 
        /// </summary>
        /// <param name="_seId">SEID</param>
        /// <param name="_volume">volume [0, 1]</param>
        /// <returns></returns>
        int ISePlayer.PlayOneShotSE( int _seId, float _volume)
        {
            return SoundConsts.INVALID_HANDLER;

        }

        /// <summary>
        /// 指定SEを停止
        /// </summary>
        /// <param name="_handler">ハンドラID</param>
        /// <param name="_isForceStop">強制停止フラグ</param>
        /// <returns></returns>
        async UniTask ISePlayer.StopSE( int _handler, bool _isForceStop)
        {
        }
        #endregion //) ===== ISePlayer =====
    }
}
