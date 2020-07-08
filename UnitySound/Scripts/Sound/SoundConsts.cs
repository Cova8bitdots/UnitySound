using UnityEngine;
using UnityEngine.Audio;
namespace CovaTech.UnitySound
{
    /// <summary>
    /// サウンド関連の定数管理クラス
    /// </summary>
    public static class SoundConsts
    {
        //------------------------------------------------------------------
        // 全般
        //------------------------------------------------------------------
        #region  ===== GENERAL =====
        public const int INVALID_HANDLER = -1;

        // デフォルトの音量[deg]
        public const float DEFAULT_VOLUME = 1.0f;

        #endregion //) ===== GENERAL =====
        
        //------------------------------------------------------------------
        // ObjectPool
        //------------------------------------------------------------------
        #region  ===== OBJECT_POOL =====

        /// <summary>
        /// BGM のプールサイズ
        /// 単純にBGMのみであればクロスフェード分も合わせて2
        /// マップ上に特定のBGM再生音源がある場合はその数に応じて増やす形
        /// (出来れば4つ程度をうまいこと使い回して表現した方がいい)
        /// </summary>
        public const int DEFAULT_BGM_POOL_SIZE = 4;

        /// <summary>
        /// SEのプールサイズ
        /// 環境音なども含めるとある程度同時再生する可能性があるため8~32あたりで設定したいところ
        /// </summary>
        public const int DEFAULT_SE_POOL_SIZE = 8;

        /// <summary>
        /// Voiceのプールサイズ
        /// ボイスパートとかで「声」に限定してエフェクトかける場合などで使用
        /// 最悪SE側に落とし込んでも良い
        /// 3Dアバターで動く場合はPC数分用意しても良いかもしれない
        /// </summary>
        public const int DEFAULT_VOICE_POOL_SIZE = 2;

        #endregion //) ===== OBJECT_POOL =====

        //------------------------------------------------------------------
        // Cache
        //------------------------------------------------------------------
        #region  ===== CACHE =====

        /// <summary>
        /// ロードしたアセットのキャッシュサイズ
        /// </summary>
        public const int DEFAULT_CACHE_SIZE = 16;


        #endregion //) ===== CACHE =====
    }

    /// <summary>
    /// オーディオのカテゴリ
    /// </summary>
    public enum SOUND_CATEGORY
    {
        UNDEFINED = -1,

        /* 最終的な出力*/
        MASTER,

        /* BGM 系列 */
        BGM_MASTER,
        SYSTEM_BGM,
        DIEGETIC_BGM,


        /* SE 系列 */
        SE_MASTER,
        SYSTEM_SE,
        JINGLE,
        ENVIROMENT_SE,
        DIEGETIC_SE,

        /* VOICE 系列 */
        VOICE_MASTER,
        SYSTEM_VOICE,
    }

    /// <summary>
    /// サウンドアイテムのの状態
    /// </summary>
    public enum SOUND_STATE
    {
        UNDEFINED,          // 未定義State
        INIT,               // 初期化中
        IDLE,               // 待機中
        PRE_PLAY,           // 再生準備中
        PLAYING,            // 再生中
        PRE_STOP,           // 停止準備中
        STOP,               // 停止中
    }
}
